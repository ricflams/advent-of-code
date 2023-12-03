using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day11
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Dumbo Octopus";
		public override int Year => 2021;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").Part1(1656).Part2(195);
			Run("input").Part1(1735).Part2(400);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var flashes = 0;
			for (var i = 0; i < 100; i++)
			{
				flashes += Step(map);
			}

			return flashes;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var N = map.Width() * map.Height();

			var step = 1;
			while (Step(map) != N)
			{
				step++;
			}

			return step;
		}

		private static int Step(char[,] map)
		{
			// Increase energy level by 1
			map.Map(ch => (char)(ch + 1));

			// Flash all points now above 9, but only once
			var doFlash = new HashSet<Point>(map.AllPoints(ch => ch > '9'));
			var hasFlashed = new HashSet<Point>();
			while (doFlash.Count > 0)
			{
				var p = doFlash.First();
				hasFlashed.Add(p);
				foreach (var n in p.LookDiagonallyAround().Within(map).Where(x => !hasFlashed.Contains(x)))
				{
					if (++map[n.X, n.Y] > '9')
					{
						doFlash.Add(n);
					}
				}
				doFlash.Remove(p);
			}

			// Reset all points that have flashed
			foreach (var p in hasFlashed)
			{
				map[p.X, p.Y] = '0';
			}

			return hasFlashed.Count;
		}
	}
}
