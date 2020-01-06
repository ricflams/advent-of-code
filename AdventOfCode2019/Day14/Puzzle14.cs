using AdventOfCode2019.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Day14
{
    internal static class Puzzle14
    {
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			Debug.Assert(new NanoFactory("Day14/input-1.txt").ReduceFuelToOre(1) == 31);
			Debug.Assert(new NanoFactory("Day14/input-2.txt").ReduceFuelToOre(1) == 165);
			Debug.Assert(new NanoFactory("Day14/input-3.txt").ReduceFuelToOre(1) == 13312);
			Debug.Assert(new NanoFactory("Day14/input-4.txt").ReduceFuelToOre(1) == 180697);
			Debug.Assert(new NanoFactory("Day14/input-5.txt").ReduceFuelToOre(1) == 2210736);

			var oreNeeded = new NanoFactory("Day14/input.txt").ReduceFuelToOre(1);
			Console.WriteLine($"Day 14 Puzzle 1: {oreNeeded}");
		}

		private static void Puzzle2()
		{
			var maxfuel = NumberGuesser.Find(fuel =>
			{
				var target = 1000000000000;
				var actual = new NanoFactory("Day14/input.txt").ReduceFuelToOre(fuel);
				if (actual < target)
					return NumberGuesser.GuessIs.TooLow;
				if (actual > target)
					return NumberGuesser.GuessIs.TooHigh;
				return NumberGuesser.GuessIs.Correct;
			});
			Console.WriteLine($"Day 14 Puzzle 2: {maxfuel}");
			Debug.Assert(maxfuel == 3126714);
		}

		internal class NanoFactory
		{
			private readonly Dictionary<Chemical, Reaction> _reactions;
			private readonly Chemical _fuel;
			private readonly Chemical _ore;

			public NanoFactory(string filename)
			{
				_reactions = File.ReadAllLines(filename)
					.Select(Reaction.Parse)
					.ToDictionary(x => x.Output, x => x);
				_fuel = _reactions.First(x => x.Key.Name == "FUEL").Key;
				_ore = _reactions.SelectMany(x => x.Value.Inputs).FirstOrDefault(c => c.IsOre);
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
