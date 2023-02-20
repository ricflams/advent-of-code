using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day20
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Donut Maze";
		public override int Year => 2019;
		public override int Day => 20;

		public void Run()
		{
			Run("test1").Part1(23);
			Run("test2").Part1(58);
			Run("test3").Part2(396);
			Run("input").Part1(608).Part2(6706);
		}

		const int Infinite = 10000000;

		protected override int Part1(string[] input)
		{
			var maze = new PortalMaze(input);
			var graph = PortalGraph.BuildWeightedGraphFromMaze(maze);
			var shortestPath = graph.ShortestPathDijkstra(maze.Entry, maze.Exit);
			return shortestPath;
		}

		protected override int Part2(string[] input)
		{
			var maze = new PortalMaze(input);
			var shortestPath = MaxDepths().Select(d => FindMinimumDistanceBfsPlutonian(maze, d)).First(x => x != Infinite);
			return shortestPath;
		}

		private static IEnumerable<int> MaxDepths()
		{
			for (var depth = 25; ; depth += 5)
			{
				yield return depth;
			}
		}

		private static int FindMinimumDistanceBfsPlutonian(PortalMaze maze, int maxDepth)
		{
			var graph = BuildSimpleGraph(maze);
			var toplevel = new MazeLevel()
			{
				Level = 0
			};
			var entry = graph.Vertices[maze.Entry];
			var exit = graph.Vertices[maze.Exit];

			toplevel.Visited[entry.Pos] = true;

			var queue = new Queue<(MazeLevel, PortalGraph.Vertex, int)>();
			queue.Enqueue((toplevel, entry, 0));
			while (queue.Any())
			{
				var (level, node, distance) = queue.Dequeue();
				if (node.Pos == exit.Pos && level.Level == 0)
				{
					return distance;
				}
				foreach (var e in node.Edges)
				{
					var nextnode = e.Key;
					var nextlevel = level;
					if (e.Value != 0)
					{
						if (e.Value == -1)
						{
							if (level.Level > maxDepth)
							{
								continue;
							}
							if (level.Inner == null)
							{
								level.Inner = new MazeLevel()
								{
									Level = level.Level + 1,
									Outer = level
								};
							}
							nextlevel = level.Inner;
						}
						else
						{
							nextlevel = level.Outer;
							if (nextlevel == null)
							{
								continue;
							}
						}
						nextnode = graph.Vertices[nextnode.Pos];
					}

					if (nextlevel.Visited[nextnode.Pos])
					{
						continue;
					}
					nextlevel.Visited[nextnode.Pos] = true;

					queue.Enqueue((nextlevel, nextnode, distance + 1));
				}
			}
			return Infinite;
		}

		internal class PortalGraph : Graphx<PortalMaze.Portal> { }

		internal class MazeLevel
		{
			public int Level { get; set; }
			public SparseMap<bool> Visited { get; } = new SparseMap<bool>();
			public MazeLevel Inner { get; set; }
			public MazeLevel Outer { get; set; }
		}

		private static PortalGraph BuildSimpleGraph(PortalMaze maze)
		{
			var walked = new SparseMap<PortalGraph.Node>();
			var graph = new PortalGraph();

			var start = new PortalMaze.Portal
			{
				Name = "?",
				Pos = maze.Entry,
				IsDownward = true
			};
			graph.AddNode(start);
			BuildSimpleGraph(start);

			return graph;

			void BuildSimpleGraph(PortalGraph.Node node)
			{
				var portal = node.Data;
				while (walked[portal.Pos] == null)
				{
					walked[portal.Pos] = node;
					var positions = portal.Pos
						.LookAround()
						.Select(p => new { Pos = p, Dest = maze.Transform(p) })
						.Where(x => maze.Map[x.Dest] == '.')
						.Where(x => walked[x.Dest] == null || !walked[x.Dest].Neighbors.Any(c => x.Pos == portal.Pos))
						.ToList();

					foreach (var p in positions.Where(x => walked[x.Dest] != null).ToList())
					{
						var existing = walked[p.Dest];
						var portal = maze.Portals[p.Pos];
						var portalValue = portal == null ? 0 : portal.IsDownward ? -1 : 1;
						var portalName = portal == null ? null : portal.Name;
						node.Value = portal;
						existing.Value = portal;
						node.Edges[existing] = portalValue;
						existing.Edges[node] = -portalValue;
						positions.Remove(p);
					}

					switch (positions.Count())
					{
						case 0:
							return;
						case 1:
							var p = positions.First();
							var next = graph.AddVertex(p.Dest);
							//graph.Vertices.Add(next);
							var portal = maze.Portals[p.Pos];
							var portalValue = portal == null ? 0 : portal.IsDownward ? -1 : 1;
							node.Value = portal;
							next.Value = portal;
							node.Edges[next] = portalValue;
							next.Edges[node] = -portalValue;
							node = next;
							break;
						default:
							var forks = positions.Select(x => graph.AddVertex(x.Dest)).ToList();
							foreach (var fork in forks)
							{
								BuildSimpleGraph(fork);
							}
							return;
					}
				}
			}
		}
	}
}
