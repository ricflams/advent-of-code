using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day16
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Aunt Sue";
		public override int Year => 2015;
		public override int Day => 16;

		public void Run()
		{
			RunFor("input", 40, 241);
		}

		protected override int Part1(string[] input)
		{
			var (possessions, measures) = GetPossessionsAndMeasures(input);

			var sue = possessions.First(AllMustMatch).Number;
			return sue;

			bool AllMustMatch(Possessions p) => p.All(i => measures.ContainsKey(i.Key) && i.Value == measures[i.Key]);
		}

		protected override int Part2(string[] input)
		{
			var (possessions, measures) = GetPossessionsAndMeasures(input);

			var realSue = possessions.First(MatchOutdatedRetroencabulator).Number;
			return realSue;

			bool MatchOutdatedRetroencabulator(Possessions p) => p.All(i =>
			{
				var name = i.Key;
				var value = i.Value;
				// cats and trees readings indicates that there are greater than that many
				// pomeranians and goldfish readings indicate that there are fewer than that many
				if (name == "cats" || name == "trees")
				{
					return measures.ContainsKey(i.Key) && i.Value > measures[i.Key];
				}
				if (name == "pomeranians" || name == "goldfish")
				{
					return !measures.ContainsKey(i.Key) || i.Value < measures[i.Key];
				}
				return measures.ContainsKey(i.Key) && i.Value == measures[i.Key];
			});
		}

		private (List<Possessions>, Dictionary<string, int>) GetPossessionsAndMeasures(string[] input)
		{
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

			var posessions = input.Select(x => new Possessions(x)).ToList();
			var measured = message
				.Select(x => x.Split(':'))
				.ToDictionary(x => x[0], x => int.Parse(x[1]));
			return (posessions, measured);
		}

		private class Possessions : Dictionary<string, int>
		{
			public Possessions(string line)
			{
				// Example:
				// Sue 16: vizslas: 6, cats: 6, pomeranians: 10
				var (number, possessions) = line.RxMatch("Sue %d:%*").Get<int, string>();
				Number = number;
				foreach (var p in possessions.Split(','))
				{
					var (name, n) = p.Trim().RxMatch("%s: %d").Get<string, int>();
					this[name] = n;
				}
			}
			public int Number { get; private set; }
		}
	}
}
