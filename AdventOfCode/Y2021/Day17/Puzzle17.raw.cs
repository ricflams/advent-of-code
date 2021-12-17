using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day17.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 17";
		public override int Year => 2021;
		public override int Day => 17;

		public void Run()
		{
			Run("test1").Part1(45).Part2(112);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(0).Part2(0);
			// 496, 528, 683 not right
		}

		protected override long Part1(string[] input)
		{
			var (x1, x2, y1, y2) = input[0].RxMatch("target area: x=%d..%d, y=%d..%d").Get<int, int, int, int>();

			var maxxxy = int.MinValue;
			for (var dx = 0; dx < 2000; dx++)
			{
				for (var dy = 0; dy < 2000; dy++)
				{
					var yy = HighestYPos(dx, dy);
					if (yy > maxxxy)
						maxxxy = yy.Value;
				}
			}
			return maxxxy;

			int? HighestYPos(int vx, int vy)
			{
				var x = 0;
				var y = 0;
				var maxy = int.MinValue;
				//while (true)
				for (var i = 0; i < 1000; i++)
				{
					x += vx;
					y += vy;

					if (x > 0) vx--;
					else if (x < 0) vx++;
					vy--;

					//	Console.WriteLine($"{x}, {y}");

					if (y > maxy)
						maxy = y;

					if (x >= x1 && x <= x2 && y >= y1 && y <= y2) // inside target
					{
						return maxy;
					}
					if (y < y1)
						break;

				}
				return null;
			}
		}

		protected override long Part2(string[] input)
		{
			var (x1, x2, y1, y2) = input[0].RxMatch("target area: x=%d..%d, y=%d..%d").Get<int, int, int, int>();


var solution = @"20,-8
10,-1
10,-2
11,-1
11,-2
11,-3
11,-4
12,-2
12,-3
12,-4
13,-2
13,-3
13,-4
14,-2
14,-3
14,-4
15,-2
15,-3
15,-4
20,-10
20,-5
20,-6
20,-7
20,-9
21,-10
21,-5
21,-6
21,-7
21,-8
21,-9
22,-10
22,-5
22,-6
22,-7
22,-8
22,-9
23,-10
23,-5
23,-6
23,-7
23,-8
23,-9
24,-10
24,-5
24,-6
24,-7
24,-8
24,-9
25,-10
25,-5
25,-6
25,-7
25,-8
25,-9
26,-10
26,-5
26,-6
26,-7
26,-8
26,-9
27,-10
27,-5
27,-6
27,-7
27,-8
27,-9
28,-10
28,-5
28,-6
28,-7
28,-8
28,-9
29,-10
29,-5
29,-6
29,-7
29,-8
29,-9
30,-10
30,-5
30,-6
30,-7
30,-8
30,-9
6,0
6,1
6,2
6,3
6,4
6,5
6,6
6,7
6,8
6,9
7,0
7,1
7,-1
7,2
7,3
7,4
7,5
7,6
7,7
7,8
7,9
8,0
8,1
8,-1
8,-2
9,0
9,-1
9,-2
".Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Split(',')).Select(s => Point.From(int.Parse(s[0]), int.Parse(s[1])))
		.ToList();


			//var maxxxy = int.MinValue;
			var n = 0;
			var N = 1000;
			for (var dx = 0; dx < N; dx++)
			{
				for (var dy = -N; dy < N; dy++)
				{
					var yy = HighestYPos(dx, dy);
					if (yy)
					{
						n++;
						//var match = solution.First(p => p.X == dx && p.Y == dy);
						//solution.Remove(match);
					}
				}
			}
			return n;

			bool HighestYPos(int vx, int vy)
			{
				var x = 0;
				var y = 0;
				//var maxy = int.MinValue;
				//while (true)
				for (var i = 0; i < 1000; i++)
				{
					x += vx;
					y += vy;

					if (vx > 0) vx--;
					else if (vx < 0) vx++;
					vy--;

					//	Console.WriteLine($"{x}, {y}");

					//if (y > maxy)
					//	maxy = y;

					if (x >= x1 && x <= x2 && y >= y1 && y <= y2) // inside target
					{
						return true;
					}
					if (y < y1)
						break;

				}
				return false;
			}
		}
	}
}
