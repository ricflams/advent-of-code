using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day24
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
			Run("input").Part1(10538).Part2(9174);
			Run("extra").Part1(23701).Part2(779);
		}

		protected override long Part1(string[] input)
		{
			var (a1, a2) = Army.Parse(input);
			return Army.Fight(a1, a2);
		}

		protected override long Part2(string[] input)
		{
			var minUnits = int.MaxValue;
			Guess.FindLowest(1, guess =>
			{
				var (immune, infection) = Army.Parse(input);
				immune.Boost(guess);
				if (Army.Fight(immune, infection) < 0)
					return false;
				var units = immune.TotalUnits;
				if (units > 0 && units < minUnits)
					minUnits = units;
				return units > 0;
			});
			return minUnits;
		}

		private class Army
		{
			public enum AttackType { Bludgeoning, Cold, Fire, Radiation, Slashing };

			public string Name { get; init; }
			internal List<Group> Groups { get; init; }

			public static (Army, Army) Parse(string[] input)
			{
				var armies = input
					.GroupByEmptyLine()
					.Select(x => new Army(x))
					.ToArray();
				return (armies[0], armies[1]);
			}

			internal Army(string[] input)
			{
				Name = input.First().TrimEnd(':');
				Groups = input.Skip(1).Select((s, idx) => Group.Parse(this, idx+1, s)).ToList();
			}

			public int TotalUnits => Groups.Sum(g => g.Units);

			public void Boost(int boost)
			{
				foreach (var g in Groups)
				{
					g.AttackDamage += boost;
				}
			}

			private void RemoveEmptyGroups()
			{
				foreach (var g in Groups.ToArray())
				{
					if (g.Units == 0)
						Groups.Remove(g);
				}
			}

			public static int Fight(Army a1, Army a2)
			{
				while (a1.TotalUnits > 0 && a2.TotalUnits > 0)
				{
					// Phase 1: target selection
					var attackFrom = new Dictionary<Army.Group, Army.Group>();
					foreach (var me in a1.Groups.Concat(a2.Groups).OrderByDescending(g => g.EffectivePower).ThenByDescending(g => g.Initiative))
					{
						var enemy = me.Army == a1 ? a2 : a1;
						var target = enemy.Groups.Select(you => (You: you, Damage: me.CanDamage(you)))
							.OrderByDescending(x => x.Damage)
							.ThenByDescending(x => x.You.EffectivePower)
							.ThenByDescending(x => x.You.Initiative)
							.FirstOrDefault(t => !attackFrom.ContainsValue(t.You));
						if (target.Damage > 0)
						{
							attackFrom[me] = target.You;
						}
					}

					// Phase 2: attack
					var totalkills = 0;
					foreach (var (me, you) in attackFrom.OrderByDescending(x => x.Key.Initiative))
					{
						if (me.Units == 0)
							continue;
						var damage = me.CanDamage(you);
						var kills = Math.Min(you.Units, damage / you.HitPoints); // round down
						you.Units -= kills;
						totalkills += kills;
					}
					if (totalkills == 0) // No kills, it will be a tie from now on
						return -1;

					// Carry out the dead
					a1.RemoveEmptyGroups();
					a2.RemoveEmptyGroups();
				}

				var remains = a1.TotalUnits + a2.TotalUnits;
				return remains;
			}

			internal class Group
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
