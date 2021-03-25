using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2016.Day24
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Air Duct Spelunking";
		public override int Year => 2016;
		public override int Day => 24;

		public void Run()
		{
			Run("test1").Part1(14);
			Run("input").Part1(456).Part2(704);
		}

		class DuctGraph : Graph<HashSet<uint>> {}

		protected override int Part1(string[] input)
		{
			return ShortestPath(input, false);
		}

		protected override int Part2(string[] input)
		{
			return ShortestPath(input, true);
		}

		private static int ShortestPath(string[] input, bool returnToZero)
		{
			var map = CharMap.FromArray(input);
			var maze = new Maze(map)
				.WithEntry(map.FirstOrDefault(c => c == '0'));
			var allNumbers = map
				.AllPoints(c => char.IsDigit(c)).ToArray()
				.Aggregate(0U, (mask, p) => mask |= (uint)(1U<<(map[p] - '0')));

			var graph = DuctGraph.BuildUnitGraphFromMaze(maze);
			foreach (var v in graph.Vertices.Values)
			{
				v.Value = new HashSet<uint>();
			}

			var queue = new Queue<(DuctGraph.Vertex, uint, int)>();
			queue.Enqueue((graph.Root, 0U, 0));
			while (queue.Any())
			{
				var (node, found, steps) = queue.Dequeue();

				if (node.Value.Contains(found))
					continue;
				node.Value.Add(found);

				var ch = map[node.Pos];
				if (char.IsDigit(ch))
				{
					found |= 1U<<(ch - '0');
					if (found == allNumbers && (!returnToZero || ch == '0'))
					{
						return steps;
					}
				}

				foreach (var n in node.Edges.Keys.Where(n => !n.Value.Contains(found)))
				{
					queue.Enqueue((n, found, steps + 1));
				}
			}

			throw new Exception("No path found");
		}
	}
}
