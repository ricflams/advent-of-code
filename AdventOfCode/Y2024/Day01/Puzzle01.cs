using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Collections.Immutable;

namespace AdventOfCode.Y2024.Day01
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Historian Hysteria";
		public override int Year => 2024;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(11).Part2(31);
			Run("input").Part1(1341714).Part2(27384707);
			Run("extra").Part1(1197984).Part2(23387399);
		}

		protected override long Part1(string[] input)
		{
			var pairs = input
				.Select(x => x.SplitSpace())
				.Select(x => (int.Parse(x[0]), int.Parse(x[1])))
				.ToArray();

			var p1 = pairs.Select(x => x.Item1).OrderBy(x => x).ToArray();
			var p2 = pairs.Select(x => x.Item2).OrderBy(x => x).ToArray();
			
			var total = 0;
			for (var i = 0; i < p1.Length; i++)
			{
				total += Math.Abs(p1[i] - p2[i]);
			}

			return total;
		}

		protected override long Part2(string[] input)
		{
			var pairs = input
				.Select(x => x.SplitSpace())
				.Select(x => (int.Parse(x[0]), int.Parse(x[1])))
				.ToArray();

			var ids = pairs.Select(x => x.Item1);
			var occurrences = pairs.Select(x => x.Item2)
				.GroupBy(x => x)
				.ToImmutableDictionary(x => x.Key, x => x.Count());

			var score = ids.Sum(p => p * occurrences.GetValueOrDefault(p));

			return score;
		}
	}
}
