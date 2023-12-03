using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day24.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Immune System Simulator 20XX";
		public override int Year => 2018;
		public override int Day => 24;

		public override void Run()
		{
			Run("test1").Part1(5216).Part2(51);
			//Run("test2").Part1(7);
			//Run("test9").Part1(906).Part2(121493971);
			Run("input").Part1(10538).Part2(9174);
			// 6302 too low
		}

		protected override long Part1(string[] input)
		{
			var (a1, a2) = Army.Parse(input, 0);
			return Army.Fight(a1, a2);
		}

		protected override long Part2(string[] input)
		{
			//var units = 0;
			//Guess.FindLowest(1, guess =>
			//{
			//	var (a1, a2) = Army.Parse(input, guess);
			//	Army.Fight(a1, a2);
			//	var u = a1.TotalUnits;
			//	if (u > 0)
			//		units = u;
			//	Console.WriteLine($"Boost={guess}: {units} units left");
			//	return u > 0;
			//});

			for (var boost = 1; boost<50; boost++)
			{
				var (a1, a2) = Army.Parse(input, boost);
				if (Army.Fight(a1, a2) < 0)
					continue;
				Console.WriteLine($"Boost={boost}: {a1.TotalUnits} units left");
				//if (a1.TotalUnits > 0)
				//	return a1.TotalUnits;
			}
			return 0;
		}

		private class Army
		{
			public enum AttackType { Bludgeoning, Cold, Fire, Radiation, Slashing };

			public string Name { get; init; }
			public List<Group> Groups { get; init; }

			public static (Army, Army) Parse(string[] input, int boost)
			{
				var armies = input
					.GroupByEmptyLine()
					.Select(x => new Army(x))
					.ToArray();
				foreach (var g in armies.Single(a => a.Name == "Immune System").Groups)
				{
					g.AttackDamage += boost;
				}
				return (armies[0], armies[1]);
			}

			internal Army(string[] input)
			{
				Name = input.First().TrimEnd(':');
				Groups = input.Skip(1).Select((s, idx) => Group.Parse(this, idx+1, s)).ToList();
			}

			public Dictionary<Group, Group> SelectTargets(Army enemy)
			{
				var targets = new Dictionary<Group, Group>();
				foreach (var me in Groups.OrderByDescending(g => g.EffectivePower).ThenByDescending(g => g.Initiative))
				{
					var (you, damage) = enemy.Groups.Select(you => (You: you, Damage: me.CanDamage(you)))
						.OrderByDescending(x => x.Damage)
						.ThenByDescending(x => x.You.EffectivePower)
						.ThenByDescending(x => x.You.Initiative)
						.First();
					if (damage > 0 && !targets.ContainsValue(you))
					{
						targets[me] = you;
					}
				}
				return targets;
			}

			public void RemoveEmptyGroups()
			{
				foreach (var g in Groups.ToArray())
				{
					if (g.Units == 0)
						Groups.Remove(g);
				}
			}

			public void WriteStatus()
			{
				Console.WriteLine($"{Name}:");
				foreach (var g in Groups)
					Console.WriteLine($"Group {g.Number} contains {g.Units} units");
			}

			public int TotalUnits => Groups.Sum(g => g.Units);

			public static int Fight(Army a1, Army a2)
			{
				while (a1.TotalUnits > 0 && a2.TotalUnits > 0)
				{
					//	Console.WriteLine();
					//	a1.WriteStatus();
					//	a2.WriteStatus();

					// Phase 1: target selection
					var attackFrom = new Dictionary<Army.Group, Army.Group>();
					foreach (var me in a1.Groups.Concat(a2.Groups).OrderByDescending(g => g.EffectivePower).ThenByDescending(g => g.Initiative))
					{
						var enemy = me.Army == a1 ? a2 : a1;
						var (you, damage) = enemy.Groups.Select(you => (You: you, Damage: me.CanDamage(you)))
							.OrderByDescending(x => x.Damage)
							.ThenByDescending(x => x.You.EffectivePower)
							.ThenByDescending(x => x.You.Initiative)
							.FirstOrDefault(t => !attackFrom.ContainsValue(t.You));
						if (damage > 0)
						{
							attackFrom[me] = you;
						}
					}

					//var t1 = a1.SelectTargets(a2);
					//var t2 = a2.SelectTargets(a1);
					//var targets = t1.Concat(t2).ToDictionary(x => x.Key, x => x.Value);

					//	Console.WriteLine();
					foreach (var (me, you) in attackFrom)
					{
						//		Console.WriteLine($"{me.Army.Name} group {me.Number} would deal defending group {you.Number} {me.CanDamage(you)} damage");
					}

					// Phase 2: attack
					//	Console.WriteLine();
					var totalkills = 0;
					foreach (var (me, you) in attackFrom.OrderByDescending(x => x.Key.Initiative))
					{
						if (me.Units == 0)
							continue;
						var damage = me.CanDamage(you);
						var kills = Math.Min(you.Units, damage / you.HitPoints); // round down
						you.Units -= kills;
						totalkills += kills;
						//		Console.WriteLine($"{me.Army.Name} group {me.Number} attacks defending group {you.Number}, killing {kills} units");
					}

					// Carry out the dead
					a1.RemoveEmptyGroups();
					a2.RemoveEmptyGroups();

					if (totalkills == 0)
						return -1;
				}

				var remains = a1.TotalUnits + a2.TotalUnits;
				return remains;
			}

			public class Group
			{
				public int Units;
				public int AttackDamage;
				public int HitPoints { get; init; }
				public AttackType AttackType { get; init; }
				public int Initiative { get; init; }
				public HashSet<AttackType> Weaknesses { get; internal set; }
				public HashSet<AttackType> Immunities { get; internal set; }

				public Army Army { get; init; }
				public int Number { get; init; }

				public int EffectivePower => Units * AttackDamage;

				public int CanDamage(Group you)
				{
					var damage = EffectivePower;
					if (you.Immunities.Contains(AttackType))
						damage = 0;
					else if (you.Weaknesses.Contains(AttackType))
						damage *= 2;
					return damage;
				}

				public static Group Parse(Army army, int number, string line)
				{
					// 1140 units each with 17741 hit points (weak to bludgeoning; immune to fire, slashing) with an attack that does 25 fire damage at initiative 2
					var (units, hitpoints, features, damage, type, initiative) = line
						.RxMatch("%d units each with %d hit points%*with an attack that does %d %s damage at initiative %d")
						.Get<int, int, string, int, string, int>();
					var group = new Group
					{
						Army= army,
						Number = number,
						Units = units,
						HitPoints = hitpoints,
						Weaknesses = new(),
						Immunities = new(),
						AttackDamage = damage,
						AttackType = (AttackType)Enum.Parse(typeof(AttackType), type, true),
						Initiative = initiative
					};
					foreach (var f in features.Trim(new[] { ' ', '(', ')' }).Split(';', StringSplitOptions.RemoveEmptyEntries))
					{
						var (effect, what) = f.Trim().RxMatch("%s to %*").Get<string, string>();
						var types = what.Split(',').Select(s => (AttackType)Enum.Parse(typeof(AttackType), s, true));
						if (effect == "weak")
							group.Weaknesses = new HashSet<AttackType>(types);
						else if (effect == "immune")
							group.Immunities = new HashSet<AttackType>(types);
						else
							throw new Exception($"Unknown effect {effect}");

					}
					return group;
				}

				public override string ToString()
				{
					var weaknesses = Weaknesses.Any() ? $"weak to {string.Join(", ", Weaknesses)}" : "";
					var immunities = Immunities.Any() ? $"immune to {string.Join(", ", Immunities)}" : "";
					var features = weaknesses.Any() || immunities.Any()
						? $" ({string.Join("; ", new[] { weaknesses, immunities })})"
						: "";
					return $"{Army.Name} group {Number}: {Units} units each with {HitPoints} hit points{features} with an attack that does {AttackDamage} {AttackType} damage at initiative {Initiative}";
				}
			}
		}

	}
}
