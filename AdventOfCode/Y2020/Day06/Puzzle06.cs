using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day06
{
	internal class Puzzle06
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var groups = File.ReadLines("Y2020/Day06/input.txt")
				.GroupByEmptyLine();

			var result1 =
				groups.Select(g => string.Concat(g).ToCharArray().Distinct().Count())
				.Sum();

			var result2 =
				groups.Select(g => string.Concat(g).ToCharArray().GroupBy(x => x).Count(x => x.Count() == g.Length))
				.Sum();

			Console.WriteLine($"Day 06 Puzzle 1: {result1}");
			Console.WriteLine($"Day 06 Puzzle 2: {result2}");
			Debug.Assert(result1 == 6683);
			Debug.Assert(result2 == 3122);
		}
	}
}
