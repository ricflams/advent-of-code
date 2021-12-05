using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2021.Day05
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Hydrothermal Venture";
		public override int Year => 2021;
		public override int Day => 5;

		public void Run()
		{
			Run("test1").Part1(5).Part2(12);
			Run("input").Part1(5442).Part2(19571);
		}

		protected override int Part1(string[] input)
		{
			var map = new SparseMap<int>();

			foreach (var s in input)
			{
				var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();

				if (x1 != x2 && y1 != y2)
					continue;

				map.WalkLine(x1, y1, x2, y2, p => map[p]++);
			}

			var overlaps = map.AllValues(v => v > 1).Count();
			return overlaps;
		}

		protected override int Part2(string[] input)
		{
			var map = new SparseMap<int>();

			foreach (var s in input)
			{
				var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();
				map.WalkLine(x1, y1, x2, y2, p => map[p]++);
			}

			var overlaps = map.AllValues(v => v > 1).Count();
			return overlaps;
		}
	}
}
