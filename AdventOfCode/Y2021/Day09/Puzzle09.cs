using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Smoke Basin";
		public override int Year => 2021;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(15).Part2(1134);
			Run("input").Part1(585).Part2(827904);
			Run("extra").Part1(500).Part2(970200);
		}

		protected override long Part1(string[] input)
		{
			var area = CharMatrix.FromArray(input);
			var (w, h) = area.Dim();

			// Count risks (height+1) of all low-points, ie points that are all
			// surrounded by higher heights
			var n = 0;
			foreach (var p in area.AllPoints())
			{
				var height = area[p.X, p.Y];
				if (p.LookAround().Within(w, h).All(x => area[x.X, x.Y] > height))
				{ 
					n += height - '0' + 1;
				}
			}

			return n;
		}

		protected override long Part2(string[] input)
		{
			var area = CharMatrix.FromArray(input);
			var (w, h) = area.Dim();

			var basinSizes = new List<int>();

			// Count basin-size of all low-points, ie points that are all
			// surrounded by higher heights. A basin is found by following
			// all neightbors to the low-point.
			foreach (var p in area.AllPoints())
			{
				var height = area[p.X, p.Y];
				if (p.LookAround().Within(w, h).All(x => area[x.X, x.Y] > height))
				{
					var basin = new HashSet<Point>() { p };
					CalcBasinSize(basin, p);
					basinSizes.Add(basin.Count);
				}
			}

			var n = basinSizes.OrderByDescending(x => x).Take(3).Prod();
			return n;

			void CalcBasinSize(HashSet<Point> basin, Point p)
			{
				var height = area[p.X, p.Y];
				foreach (var adj in p.LookAround().Within(w, h))
				{
					var neighborHeight = area[adj.X, adj.Y];
					if (neighborHeight < '9' && neighborHeight > height)
					{
						basin.Add(adj);
						CalcBasinSize(basin, adj);
					}
				}
			}
		}
	}
}
