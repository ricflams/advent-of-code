using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2021.Day05.Raw
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2021;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(5).Part2(12);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(5442).Part2(19571);
		}

		protected override int Part1(string[] input)
		{
			var map = new SparseMap<int>();

			var overlap = 0;
			foreach (var s in input)
			{
				var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();
				if (x1 == x2)
				{
					var yy1 = Math.Min(y1, y2);
					var yy2 = Math.Max(y1, y2);
					for (var y = yy1; y <= yy2; y++)
					{
						if (map[x1][y] == 1)
						{
							//Console.WriteLine($"{x1}, {y}");
							overlap++;
						}
						map[x1][y]++;
					}
				}
				if (y1 == y2)
				{
					var xx1 = Math.Min(x1, x2);
					var xx2 = Math.Max(x1, x2);
					for (var x = xx1; x <= xx2; x++)
					{
						if (map[x][y1] == 1)
						{
							//Console.WriteLine($"{x}, {y1}");
							overlap++;
						}
						map[x][y1]++;
					}
				}
			}


			return overlap;
		}

		protected override int Part2(string[] input)
		{
			var map = new SparseMap<int>();

			var overlap = 0;
			foreach (var s in input)
			{
				var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();

				var dx = x2 == x1 ? 0 : x2 > x1 ? 1 : -1;
				var dy = y2 == y1 ? 0 : y2 > y1 ? 1 : -1;
				var D = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));

				for (var d = 0; d <= D; d++)
				{
					var x = x1 + d * dx;
					var y = y1 + d * dy;
					if (map[x][y] == 1)
					{
						//Console.WriteLine($"{x}, {y1}");
						overlap++;
					}
					map[x][y]++;
				}

			}

			return overlap;


		}
	}
}