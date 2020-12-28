using System;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day05
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Binary Boarding";
		protected override int Year => 2020;
		protected override int Day => 5;

		public void Run()
		{
			RunFor("input", 855, 552);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var ids = input
				.Select(x => x.Replace("F", "0").Replace("B", "1").Replace("L", "0").Replace("R", "1"))
				.Select(x => Convert.ToInt32(x, 2))
				.ToArray();

			var result1 = ids.Max();

			ids = ids.OrderBy(x => x).ToArray();
			var result2 = ids
				.SkipWhile((id, i) => id + 1 == ids[i + 1])
				.First() + 1;

			return (result1, result2);
		}
	}
}
