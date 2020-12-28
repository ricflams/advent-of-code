using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day14
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Space Stoichiometry";
		protected override int Year => 2019;
		protected override int Day => 14;

		public void Run()
		{
			RunPart1For("test1", 31);
			RunPart1For("test2", 165);
			RunFor("test3", 13312, 82892753);
			RunFor("test4", 180697, 5586022);
			RunFor("test5", 2210736, 460664);
			RunFor("input", 397771, 3126714);
		}

		protected override long Part1(string[] input)
		{
			var oreNeeded = new NanoFactory(input).ReduceFuelToOre(1);
			return oreNeeded;
		}

		protected override long Part2(string[] input)
		{
			var target = 1000000000000;
			var maxfuel = Guess.Find(Guess.ValueIs.ExactOrLowerThan, target,
				fuel => new NanoFactory(input).ReduceFuelToOre(fuel));
			return maxfuel;
		}


		internal class NanoFactory
		{
			private readonly Dictionary<Chemical, Reaction> _reactions;

			public NanoFactory(string[] input)
			{
				_reactions = input
					.Select(Reaction.Parse)
					.ToDictionary(x => x.Output, x => x);
			}

			public long ReduceFuelToOre(long fuel)
			{
				var materials = _reactions.Keys.ToDictionary(x => x.Name, x => 0L);
				materials["FUEL"] = fuel;
				materials["ORE"] = 0;
				Reduce(materials);
				return materials["ORE"];
			}

			public void Reduce(Dictionary<string, long> materials)
			{
				while (!materials.All(x => x.Key == "ORE" || x.Value <= 0))
				{
					// Just reduce one at a time
					var reduction = materials.First(x => x.Key != "ORE" && x.Value > 0);
					var chemical = reduction.Key;
					var quantity = reduction.Value;
					var reaction = _reactions.First(x => x.Key.Name == chemical).Value;
					var n = (long)Math.Ceiling((double)quantity / reaction.Output.Quantity);
					materials[chemical] -= n * reaction.Output.Quantity;
					foreach (var ic in reaction.Inputs)
					{
						materials[ic.Name] += n * ic.Quantity;
					}
					//Console.WriteLine($"{n} x {reaction}");
				}
			}

			internal class Chemical
			{
				public string Name { get; set; }
				public int Quantity { get; set; }

				public bool IsOre => Name == "ORE";
				public override string ToString() => $"{Quantity} {Name}";

				public static Chemical Parse(string s)
				{
					var parts = s.Trim().Split(" ");
					return new Chemical
					{
						Name = parts[1],
						Quantity = int.Parse(parts[0])
					};
				}
			}

			internal class Reaction
			{
				public Chemical Output { get; set; }
				public IReadOnlyCollection<Chemical> Inputs { get; set; }

				public override string ToString() => $"{string.Join(",", Inputs)} => {Output}";

				public static Reaction Parse(string s)
				{
					// FOrmat is eg: 5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV
					// Except ORE:
					// 157 ORE => 5 NZVS
					// 165 ORE => 6 DCFZ
					var react = s.Split("=>", StringSplitOptions.RemoveEmptyEntries);
					return new Reaction
					{
						Output = Chemical.Parse(react[1]),
						Inputs = react[0].Split(',').Select(Chemical.Parse).ToArray()
					};
				}
			}
		}
	}
}
