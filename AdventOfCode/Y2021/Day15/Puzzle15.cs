using System.Collections.Generic;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day15
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Chiton";
		public override int Year => 2021;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(40).Part2(315);
			Run("input").Part1(613).Part2(2899);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var (start, end) = map.MinMax();
			var risk = LowestRisk(map, start, end);

			return risk;
		}

		protected override long Part2(string[] input)
		{
			var N = 5;

			var map0 = CharMatrix.FromArray(input);
			var (w, h) = map0.Dim();

			var map = new char[w * N, h * N];
			for (var xf = 0; xf < N; xf++)
			{
				for (var yf = 0; yf < N; yf++)
				{
					for (var x = 0; x < w; x++)
					{
						for (var y = 0; y < h; y++)
						{
							var v = map0[x, y] - '0' + xf + yf;
							while (v > 9)
								v -= 9;
							map[x + xf * w, y + yf * h] = (char)('0' + v);
						}
					}
				}
			}
			// map.ConsoleWrite();

			var (start, end) = map.MinMax();
			var risk = LowestRisk(map, start, end);

			return risk;
		}

		private static int LowestRisk(char[,] map, Point start, Point end)
		{
			var (w, h) = map.Dim();

			var frontier = new PriorityQueue<Point, int>();
			frontier.Enqueue(start, 0);

			var costSoFarMap = new int[w, h];
			costSoFarMap[start.X, start.Y] = 0;

			while (frontier.TryDequeue(out var current, out var _))
			{
				var costSoFar = costSoFarMap[current.X, current.Y];
				foreach (var next in current.LookAround().Within(w, h))
				{
					var (x, y) = (next.X, next.Y);
					var newCost = costSoFar + map[x, y] - '0';
					var costSoFarForNext = costSoFarMap[x, y];
					if (costSoFarForNext == 0 || newCost < costSoFarForNext)
					{
						costSoFarMap[x, y] = newCost;
						var priority = newCost + next.ManhattanDistanceTo(end);
						frontier.Enqueue(next, priority);
					}
				}
			}

			var risk = costSoFarMap[end.X, end.Y];
			return risk;
		}
	}
}
