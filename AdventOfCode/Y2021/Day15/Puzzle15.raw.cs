using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day15.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 15";
		public override int Year => 2021;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(40).Part2(315);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(613).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var seen = new HashSet<Point>();
			var end = map.MinMax().Item2;
			var risk = LowestRisk2(map, Point.Origin, end);

			//var graph = new Graph<int>();
			//foreach (var p in map.AllPoints())
			//{
			//	graph.AddVertex(p);
			//}
			//foreach (var p in map.AllPoints())
			//{
			//	var nn = p.LookAround().Within(map);
			//	foreach (var n in nn)
			//	{
			//		var v1 = graph.Vertices[p];
			//		var v2 = graph.Vertices[n];
			//		Graph<int>.SetEdge(v1, v2, map[n.X, n.Y] - '0');
			//	}
			//}

			//var endp = Point.From(map.Dim()).DiagonalUpLeft;
			//var startp = Point.Origin;
			//var start = graph.Vertices[startp];
			//var end = graph.Vertices[endp];
			//var risk = graph.ShortestPathDijkstra(startp, endp);

			return risk;
		}

		private int LowestRisk2(CharMap map, Point start, Point end)
		{
			var (min, max) = map.Range();
			var frontier = new PriorityQueue<Point, int>();
			frontier.Enqueue(start, 0);
			var cameFrom = new Dictionary<Point, Point>();
			var costSoFar= new Dictionary<Point, int>();
			cameFrom[start] = null;
			costSoFar[start] = 0;

			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();
				if (current == null)
					break;
				foreach (var next in current.LookAround().Within(max))
				{
					var newCost = costSoFar[current] + map[next] - '0';
					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						costSoFar[next] = newCost;
						var priority = newCost + Heuristic(end, next);
						frontier.Enqueue(next, priority);
						cameFrom[next] = current;
					}
				}
			}

			var risk = costSoFar[end];
			return risk;
		}

		int Heuristic(Point goal, Point p)
		{
			return goal.ManhattanDistanceTo(p);
		}

		private int LowestRisk(CharMap map, HashSet<Point> seen, int risk, Point p, Point end)
		{
			if (p == end)
				return risk + map[p] - '0';
//			seen.Add(p);
			var nn = p.LookAround().Within(end.DiagonalDownRight).Where(x => !seen.Contains(x));
			var risky = nn.Select(n =>
				{
					var seen2 = new HashSet<Point>(seen);
					seen2.Add(n);
					return LowestRisk(map, seen2, risk + map[n] - '0', n, end);
				})
				.ToArray();
			return risky.Any() ? risky.Min() : 10000000;
		}


		//private static int FindMinimumDistanceBfs(CharMap map, Point start, Point end)
		//{
		//	const int Infinite = 10000000;
		//	var vertices = Vertices.Values;
		//	var start = vertices.First(v => v.Pos == startPos);
		//	var destination = vertices.First(v => v.Pos == destinationPos);

		//	foreach (var v in vertices)
		//	{
		//		v.Distance = int.MaxValue;
		//	}
		//	start.Distance = 0;

		//	var node = start;
		//	while (node != null)
		//	{
		//		if (node == destination)
		//		{
		//			return destination.Distance;
		//		}
		//		foreach (var edge in node.Edges)
		//		{
		//			var neighbour = edge.Key;
		//			var weight = edge.Value;
		//			var dist = node.Distance + weight;
		//			if (dist < neighbour.Distance)
		//			{
		//				neighbour.Distance = dist;
		//			}
		//		}
		//		node.Visited = true;
		//		node = vertices
		//			.Where(v => !v.Visited)
		//			.OrderBy(x => x.Distance)
		//			.FirstOrDefault();
		//	}

		//	return Infinite;
		//}



		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (xxx, yyy) = map.Range();
			var (w, h) = (yyy.X, yyy.Y);

			var seen = new HashSet<Point>();

			for (var xf = 0; xf < 5; xf++)
			{
				for (var yf = 0; yf < 5; yf++)
				{
					if (xf == 0 && yf == 0)
						continue;
					for (var x = 0; x < w; x++)
					{
						for (var y = 0; y < h; y++)
						{
							//if (xf == 4 && yf == 0 && y == 0 && x == 7)
							//	;
							var xx = x + xf * w;
							var yy = y + yf * h;
							var v0 = map[x][y] - '0';
							var v = v0 + xf + yf;
							while (v > 9)
								v -= 9;
							map[xx][yy] = (char)('0' + v);
						}
					}
				}
			}

			//map.ConsoleWrite();

			var end = map.MinMax().Item2;
			var risk = LowestRisk2(map, Point.Origin, end);


			return risk;
		}
	}
}
