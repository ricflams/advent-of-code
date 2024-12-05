using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day04
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Ceres Search";
		public override int Year => 2024;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(18).Part2(9);
			Run("input").Part1(2644).Part2(1952);
			Run("extra").Part1(2578).Part2(1972);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input).ExpandBy(1, '.');

			var n = map.AllPoints(ch => ch == 'X').Sum(CountFromX);
			return n;

			int CountFromX(Point p0)
			{
				return Point.VectorDiagonallyAround().Count(IsLookingAtMas);

				bool IsLookingAtMas(Func<Point, Point> move)
				{
					var p = p0;
					foreach (var letter in "MAS")
					{
						p = move(p);
						if (map.Get(p) != letter) return false;
					}
					return true;
				}			
			}			
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input).ExpandBy(1, '.');

			var n = map.AllPoints(ch => ch == 'A').Count(IsInXmas);
			return n;

			bool IsInXmas(Point p)
			{
				var n = 0;
				if (map.Get(p.NW) == 'M' && map.Get(p.SE) == 'S') n++;
				if (map.Get(p.NE) == 'M' && map.Get(p.SW) == 'S') n++;
				if (map.Get(p.SW) == 'M' && map.Get(p.NE) == 'S') n++;
				if (map.Get(p.SE) == 'M' && map.Get(p.NW) == 'S') n++;
				return n == 2;
			}
		}
	}
}
