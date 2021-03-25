using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2017.Day03
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Spiral Memory";
		public override int Year => 2017;
		public override int Day => 3;

		public void Run()
		{
			Run("test1").Part1(31).Part2(1968);
			Run("input").Part1(371).Part2(369601);
		}

		protected override int Part1(string[] input)
		{
			var n = int.Parse(input[0]);

			// Don't bother coming up with the formula; just use the spiral-from
			// helper to find the nth point's distance from (0,0)
			var p = Point.Origin;
			var dist = p.SpiralFrom().Skip(n-1).First().ManhattanDistanceTo(p);
			return dist;
		}

		protected override int Part2(string[] input)
		{
			var n = int.Parse(input[0]);
			var map = new SparseMap<int>(0);

			// Seed the map at (0,0) with 1
			// Then spiral out (skip 0,0) while building up map-values
			map[Point.Origin] = 1;
			foreach (var pos in Point.Origin.SpiralFrom().Skip(1))
			{
				var v = map[pos] = pos.LookDiagonallyAround().Sum(p => map[p]);
				if (v > n)
				{
					return v;
				}
			}
			throw new Exception("Not found");
		}
	}
}
