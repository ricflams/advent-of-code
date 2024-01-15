using System;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day05
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Binary Boarding";
		public override int Year => 2020;
		public override int Day => 5;

		public override void Run()
		{
			Run("input").Part1(855).Part2(552);
			Run("extra").Part1(901).Part2(661);
		}

		protected override int Part1(string[] input)
		{
			var ids = GetIds(input);
			var highestSeatId = ids.Max();
			return highestSeatId;
		}

		protected override int Part2(string[] input)
		{
			var ids = GetIds(input).OrderBy(x => x).ToArray();
			var seatId = ids
				.SkipWhile((id, i) => id + 1 == ids[i + 1])
				.First() + 1;
			return seatId;
		}

		private static int[] GetIds(string[] input)
		{
			var ids = input
				.Select(x => x.Replace("F", "0").Replace("B", "1").Replace("L", "0").Replace("R", "1"))
				.Select(x => Convert.ToInt32(x, 2))
				.ToArray();
			return ids;
		}
	}
}
