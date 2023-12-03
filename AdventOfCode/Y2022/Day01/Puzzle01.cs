using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day01
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Calorie Counting";
		public override int Year => 2022;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(24000).Part2(45000);
			Run("test9").Part1(70720).Part2(207148);
			Run("input").Part1(66487).Part2(197301);
		}

		protected override long Part1(string[] input)
		{
			var max = input
				.GroupByEmptyLine()
				.Select(x => x.Sum(int.Parse))
				.Max();

			return max;
		}

		protected override long Part2(string[] input)
		{
			var max = input
				.GroupByEmptyLine()
				.Select(x => x.Sum(int.Parse))
				.OrderByDescending(x => x)
				.Take(3)
				.Sum();

			return max;
		}
	}
}
