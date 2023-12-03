using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day04
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Camp Cleanup";
		public override int Year => 2022;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(2).Part2(4);
			Run("test9").Part1(500).Part2(815);
			Run("input").Part1(453).Part2(919);
		}

		protected override long Part1(string[] input)
		{
			var overlaps = input
				.Count(line => 
				{
					var (a1, a2, b1, b2) = line.RxMatch("%d-%d,%d-%d").Get<int, int, int, int>();
					return a1 <= b1 && a2 >= b2 || b1 <= a1 && b2 >= a2;
				}) ;
			return overlaps;
		}

		protected override long Part2(string[] input)
		{
			var overlaps = input
				.Count(line => 
				{
					var (a1, a2, b1, b2) = line.RxMatch("%d-%d,%d-%d").Get<int, int, int, int>();
					return a1 <= b2 && b1 <= a2;
				});
			return overlaps;
		}
	}
}
