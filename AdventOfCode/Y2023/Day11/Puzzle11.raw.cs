using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using AdventOfCode.Y2021.Day23.Raw;

namespace AdventOfCode.Y2023.Day11.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").Part1(374).Part2(0);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
			// not right 36337370229
			// not right 363293870229
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var galaxies = map.AllPoints(c => c == '#').ToArray();
			var (w, h) = map.Size();

			var emptyx = Enumerable.Range(0, w).Where(x => !galaxies.Any(p => p.X == x)).ToArray();
			var emptyy = Enumerable.Range(0, h).Where(y => !galaxies.Any(p => p.Y == y)).ToArray();

			var shifted = galaxies
				.Select(p => {
					var (x, y) = p;
					x += emptyx.Count(p => p < x);
					y += emptyy.Count(p => p < y);
					return Point.From(x, y);
				}).ToArray();

			var sum = 0;
			for (var i = 0; i < shifted.Length-1; i++)
			{
				var g1 = shifted[i];
				//var minstep = galaxies[(i+1)..].Min(g => CalcDist(g1, g));
				var steps = shifted[(i+1)..].Sum(g => CalcDist(g1, g));
				sum += steps;
				
			}

			int CalcDist(Point p1, Point p2)
			{
				return p1.ManhattanDistanceTo(p2);
			}


			return sum;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var galaxies = map.AllPoints(c => c == '#').ToArray();
			var (w, h) = map.Size();

			var emptyx = Enumerable.Range(0, w).Where(x => !galaxies.Any(p => p.X == x)).ToArray();
			var emptyy = Enumerable.Range(0, h).Where(y => !galaxies.Any(p => p.Y == y)).ToArray();

			var expandBy = 1000000L;
			var shifted = galaxies
				.Select(p => {
					long x = p.X;
					long y = p.Y;
					x += emptyx.Count(p => p < x)*(expandBy-1);
					y += emptyy.Count(p => p < y)*(expandBy-1);
					return (X:x, Y:y);
				}).ToArray();

			var sum = 0L;
			for (var i = 0; i < shifted.Length-1; i++)
			{
				var g1 = shifted[i];
				//var minstep = galaxies[(i+1)..].Min(g => CalcDist(g1, g));
				var steps = shifted[(i+1)..].Sum(g => 
					Math.Abs(g.X - g1.X) + Math.Abs(g.Y - g1.Y));
				sum += steps;
				
			}

			// int CalcDist(Point p1, Point p2)
			// {
			// 	return Math.Abs(X - pos.X) + Math.Abs(Y - pos.Y);
			// }


			return sum;
		}
	}
}
