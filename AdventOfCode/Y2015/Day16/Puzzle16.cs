using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day16
{
	internal class Puzzle16
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2015/Day16/input.txt");
			var message = new string[]
			{
				"children: 3",
				"cats: 7",
				"samoyeds: 2",
				"pomeranians: 3",
				"akitas: 0",
				"vizslas: 0",
				"goldfish: 5",
				"trees: 3",
				"cars: 2",
				"perfumes: 1"
			};

			var posessions = input.Select(x => new Posessions(x)).ToList();
			var measured = message
				.Select(x => x.Split(':'))
				.ToDictionary(x => x[0], x => int.Parse(x[1]));

			var sue = posessions.First(AllMustMatch).Number;
			Console.WriteLine($"Day 16 Puzzle 1: {sue}");
			Debug.Assert(sue == 40);

			var realSue = posessions.First(MatchOutdatedRetroencabulator).Number;
			Console.WriteLine($"Day 16 Puzzle 2: {realSue}");
			Debug.Assert(realSue == 241);

			bool AllMustMatch(Posessions p) => p.All(i => measured.ContainsKey(i.Key) && i.Value == measured[i.Key]);

			bool MatchOutdatedRetroencabulator(Posessions p) => p.All(i =>
			{
				var name = i.Key;
				var value = i.Value;
				// cats and trees readings indicates that there are greater than that many
				// pomeranians and goldfish readings indicate that there are fewer than that many
				if (name == "cats" || name == "trees")
				{
					return measured.ContainsKey(i.Key) && i.Value > measured[i.Key];
				}
				if (name == "pomeranians" || name == "goldfish")
				{
					return !measured.ContainsKey(i.Key) || i.Value < measured[i.Key];
				}
				return measured.ContainsKey(i.Key) && i.Value == measured[i.Key];
			});
		}

		private class Posessions : Dictionary<string, int>
		{
			public Posessions(string line)
			{
				// Example:
				// Sue 16: vizslas: 6, cats: 6, pomeranians: 10
				var val = SimpleRegex.Match(line, "Sue %d:%*");
				Number = int.Parse(val[0]);
				var posessions = val[1];
				foreach (var p in posessions.Split(','))
				{
					var item = SimpleRegex.Match(p.Trim(), "%s: %d");
					this[item[0]] = int.Parse(item[1]);
				}
			}
			public int Number { get; private set; }
		}
	}
}
