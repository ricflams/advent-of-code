using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day14
{
	internal class Puzzle : PuzzleWithParameter<(int, int), long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Restroom Redoubt";
		public override int Year => 2024;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").WithParameter((11, 7)).Part1(12);
			Run("input").WithParameter((101, 103)).Part1(218619324).Part2(6446);
			Run("extra").Part1(229632480).Part2(7051);
		}

		protected override long Part1(string[] input)
		{
			var (width, height) = PuzzleParameter;
			var robots = input.Select(Robot.Parse).ToArray();

			var sec = 100;
			foreach (var r in robots)
			{
				r.AdjustForSize(width, height);
				r.X = (r.X + sec * r.VX)  % width;
				r.Y = (r.Y + sec * r.VY)  % height;
				
			}

			var halfw = width / 2;
			var halfh = height/ 2;
			var n1 = robots.Count(r => r.X < halfw && r.Y < halfh);
			var n2 = robots.Count(r => r.X > halfw && r.Y < halfh);
			var n3 = robots.Count(r => r.X < halfw && r.Y > halfh);
			var n4 = robots.Count(r => r.X > halfw && r.Y > halfh);
			var factor = n1 * n2 * n3 * n4;

			return factor;
		}

		protected override long Part2(string[] input)
		{
			var (width, height) = PuzzleParameter;
			var robots = input.Select(Robot.Parse).ToArray();

			var map = new int[width, height];
			var inRow = new int[height];
			foreach (var r in robots)
			{
				r.AdjustForSize(width, height);
				map[r.X, r.Y] += 1;
				inRow[r.Y] += 1;
			}

			for (var sec = 1;; sec++)
			{
				// Move the robots and keep track of how many are in each row
				foreach (var r in robots)
				{
					map[r.X, r.Y] -= 1;
					inRow[r.Y] -= 1;
					r.X = (r.X + r.VX) % width;
					r.Y = (r.Y + r.VY) % height;
					map[r.X, r.Y] += 1;
					inRow[r.Y] += 1;
				}

				// Examine each row for a streak of at least 10 robots
				for (var y = 0; y < height; y++)
				{
					if (inRow[y] < 10)
						continue;
					var streak = 0;
					for (var x = 0; x < width; x++)
					{
						if (map[x, y] == 0)
							streak = 0;
						else if (++streak == 10)
							return sec;
					}
				}
			}
		}

		internal class Robot 
		{
			public int X;
			public int Y;
			public int VX;
			public int VY;

			public void AdjustForSize(int width, int height)
			{
				// Modify velocities to be positive for easier modulus
				VX = (VX + width) % width;
				VY = (VY + height) % height;
			}

			public static Robot Parse(string s)
			{
				// p=39,7 v=77,-20
				var (x, y, vx, vy) = s.RxMatch("p=%d,%d v=%d,%d").Get<int, int, int, int>();
				return new Robot
				{
					X = x,
					Y = y,
					VX = vx,
					VY = vy
				};
			}
		}		
	}
}
