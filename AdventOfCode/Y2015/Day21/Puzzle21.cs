using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2015.Day21
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "RPG Simulator 20XX";
		public override int Year => 2015;
		public override int Day => 21;

		public void Run()
		{
			RunFor("input", 111, 188);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var boss = ReadStats(input);

			var minCostForWinning = int.MaxValue;
			var maxCostForLosing = 0;
			foreach (var weapon in Weapons)
			{
				foreach (var armor in Armors)
				{
					foreach (var ring1 in Rings)
					{
						foreach (var ring2 in Rings.Where(r => r != ring1))
						{
							var cost = weapon.Cost + armor.Cost + ring1.Cost + ring2.Cost;
							var you = new Stats
							{
								Hitpoints = 100,
								Damage = weapon.Damage + armor.Damage + ring1.Damage + ring2.Damage,
								Armor = weapon.Armor + armor.Armor + ring1.Armor + ring2.Armor
							};
							if (YouWinFight(boss, you))
							{
								if (cost < minCostForWinning)
								{
									minCostForWinning = cost;
								}
							}
							else
							{
								if (cost > maxCostForLosing)
								{
									maxCostForLosing = cost;
								}
							}
						}
					}
				}
			}
			return (minCostForWinning, maxCostForLosing);
		}

		internal static bool YouWinFight(Stats boss, Stats you)
		{
			while (true)
			{
				boss.Hitpoints -= Math.Max(1, you.Damage - boss.Armor);
				if (boss.Hitpoints <= 0)
				{
					return true;
				}
				you.Hitpoints -= Math.Max(1, boss.Damage - you.Armor);
				if (you.Hitpoints <= 0)
				{
					return false;
				}
			}
		}

		internal static Stats ReadStats(string[] input)
		{
			//Hit Points: 109
			//Damage: 8
			//Armor: 2
			var info = string.Join(Environment.NewLine, input);
			return new Stats
			{
				Hitpoints = SimpleRegex.MatchInt(info, "Hit Points: %d"),
				Damage = SimpleRegex.MatchInt(info, "Damage: %d"),
				Armor = SimpleRegex.MatchInt(info, "Armor: %d")
			};
		}

		internal struct Stats
		{
			public int Hitpoints { get; set; }
			public int Damage { get; set; }
			public int Armor { get; set; }
		}

		// Weapons:    Cost  Damage  Armor
		// Dagger        8     4       0
		// Shortsword   10     5       0
		// Warhammer    25     6       0
		// Longsword    40     7       0
		// Greataxe     74     8       0

		// Armor:      Cost  Damage  Armor
		// Leather      13     0       1
		// Chainmail    31     0       2
		// Splintmail   53     0       3
		// Bandedmail   75     0       4
		// Platemail   102     0       5

		// Rings:      Cost  Damage  Armor
		// Damage +1    25     1       0
		// Damage +2    50     2       0
		// Damage +3   100     3       0
		// Defense +1   20     0       1
		// Defense +2   40     0       2
		// Defense +3   80     0       3
		internal class Item
		{
			public Item(string name, int cost, int damage, int armor)
			{
				Name = name; Cost = cost; Damage = damage; Armor = armor;
			}
			public string Name { get; set; }
			public int Cost { get; set; }
			public int Damage { get; set; }
			public int Armor { get; set; }
		}

		private static readonly Item[] Weapons =
		{
			new Item("Dagger", 8, 4, 0),
			new Item("Shortsword", 10, 5, 0),
			new Item("Warhammer", 25, 6, 0),
			new Item("Longsword", 40, 7, 0),
			new Item("Greataxe", 74, 8, 0)
		};

		private static readonly Item[] Armors =
		{
			new Item("(none)", 0, 0, 0),
			new Item("Leather", 13, 0, 1),
			new Item("Chainmail", 31, 0, 2),
			new Item("Splintmail", 53, 0, 3),
			new Item("Bandedmail", 75, 0, 4),
			new Item("Platemail", 102, 0, 5)
		};

		private static readonly Item[] Rings =
		{
			new Item("(none)", 0, 0, 0),
			new Item("Damage +1", 25, 1, 0),
			new Item("Damage +2", 50, 2, 0),
			new Item("Damage +3", 100, 3, 0),
			new Item("Defense +1", 20, 0, 1),
			new Item("Defense +2", 40, 0, 2),
			new Item("Defense +3", 80, 0, 3)
		};
	}
}
