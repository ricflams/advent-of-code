using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day17
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Trick Shot";
		public override int Year => 2021;
		public override int Day => 17;

		public void Run()
		{
			Run("test1").Part1(45).Part2(112);

			// TODO clean
			//Run("input").Part1(17766).Part2(1733);
		}

		protected override long Part1(string[] input)
		{
			var (xMin, xMax, yMin, yMax) = input[0].RxMatch("target area: x=%d..%d, y=%d..%d").Get<int, int, int, int>();

			return MaxHeightsHittingTarget(xMin, xMax, yMin, yMax).Max();
		}

		protected override long Part2(string[] input)
		{
			var (xMin, xMax, yMin, yMax) = input[0].RxMatch("target area: x=%d..%d, y=%d..%d").Get<int, int, int, int>();

			return MaxHeightsHittingTarget(xMin, xMax, yMin, yMax).Count();
		}


		private static IEnumerable<int> MaxHeightsHittingTarget(int xMin, int xMax, int yMin, int yMax)
		{
			// Try all x-velocities that may hit within the target
			for (var vx0 = 0; vx0 <= xMax; vx0++)
			{
				// Skip those velocities that won't even reach the target
				if (!CanVelocityXReachTargetAtAll(vx0))
					continue;
				// Console.WriteLine($"{vx0}");

				// Try all y-velocities from pointing straight at the target's bottom
				// and to - well, just pick a high number (not very satisfying)
				for (var vy0 = yMin; vy0 < 200; vy0++)
				{
					var h = MaxHeightHittingTarget(vx0, vy0);
					if (h.HasValue)
						yield return h.Value;
				}
			}

			bool CanVelocityXReachTargetAtAll(int vx)
			{
				// Check if x will ever hit the target for this velocity
				for (var x = vx; vx-- > 0; x += vx)
				{
					if (x >= xMin && x <= xMax)
						return true;
				}
				return false;
			}

			int? MaxHeightHittingTarget(int vx, int vy)
			{
				var vx0 = vx;
				var vy0 = vy;

				var x = 0;
				var y = 0;
				var highest = 0;

				while (true)
				{
					x += vx;
					y += vy;
					//	Console.WriteLine($"{x},{y}");

					// If y goes below target or x goes beyond then it will never reach
					if (y < yMin || x > xMax)
						break;

					if (vx > 0) vx--;
					else if (vx < 0) vx++;
					vy--;

					// Register highest point
					if (y > highest)
						highest = y;

					// If x,y hits inside target then we're done
					if (x >= xMin && x <= xMax && y >= yMin && y <= yMax)
					{
						Console.WriteLine($"Hit: {vx0},{vy0}");
						return highest;
					}
				}

				// Velocities did not cause a target-hit
				return null;
			}
		}
	}
}
