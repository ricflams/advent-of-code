using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(94).Part2(154);
		//	Run("test2").Part1(0).Part2(0);
			Run("input").Part1(2306).Part2(6718);

			// 1346 too low
			// 2022 too low

			//	Run("extra").Part1(0).Part2(0);
		}

		private class DagNode
		{
			public Point Pos;
			public List<(DagNode, int)> Edges = new();
		}

		private void BuildGraph(CharMap map, List<DagNode> nodes, HashSet<Point> seen, DagNode from, Point p, Point dest)
		{
			var w = 1;

			seen.Add(p);
			while (true)
			{
				var nb = p.LookAround().FirstOrDefault(p => !seen.Contains(p) && map[p] != '#');
				if (nb == dest)
				{
					var edge = new DagNode
					{
						Pos = nb,
						//Length = w
					};
					from.Edges.Add((edge, w));
					if (!nodes.Contains(edge))
						nodes.Add(edge);
					return;
				}
				if (nb == null)
					break;
				w++;
				p = nb;
				seen.Add(p);
				if (map[p] != '.')
				{
					var pcomefrom = p;
					p = p.LookAround().FirstOrDefault(p => !seen.Contains(p) && map[p] == '.'); 
					if (p == null)
						return;
					seen.Add(p);
					w++;
					var edge = new DagNode
					{
						Pos = p,
						//Length = w
					};
					from.Edges.Add((edge, w));
					if (!nodes.Contains(edge))
						nodes.Add(edge);
					if (map[p.Left] == '<')
					{
						var n = nodes.FirstOrDefault(x => x.Pos == p.Left);
						if (n == null)
							 BuildGraph(map, nodes, seen, edge, p.Left, dest);
					}
					if (map[p.Right] == '>')
					{
						var n = nodes.FirstOrDefault(x => x.Pos == p.Right);
						if (n == null)
							 BuildGraph(map, nodes, seen, edge, p.Right, dest);
					}
					if (map[p.Up] == '^')
					{
						var n = nodes.FirstOrDefault(x => x.Pos == p.Up);
						if (n == null)
							 BuildGraph(map, nodes, seen, edge, p.Up, dest);
					}
					if (map[p.Down] == 'v')
					{
						var n = nodes.FirstOrDefault(x => x.Pos == p.Down);
						if (p.Down == Point.From(21, 12))
							;
						if (n == null)
							 BuildGraph(map, nodes, seen, edge, p.Down, dest);
					}
				}
			}

		}


		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (w, h) = map.Size();

			var start = new DagNode { Pos = Point.From(1, -1) };
			var dest = new DagNode { Pos = Point.From(w-2, h-1) };
			var nodes = map.AllPoints()
				.Where(p => p.LookAround().Count(x => "<>^v".Contains(map[x])) >= 3)
				.Select(p => new DagNode { Pos = p })
				.Append(start)
				.Append(dest)
				.ToDictionary(x => x.Pos, x => x);

			BuildEdge(start, start.Pos, start.Pos.Down);
			foreach (var n in nodes.Values)
			{
				var p = n.Pos;

				if (map[p.Left] == '<')
				{
					BuildEdge(n, p, p.Left);
				}
				if (map[p.Right] == '>')
				{
					BuildEdge(n, p, p.Right);
				}
				if (map[p.Up] == '^')
				{
					BuildEdge(n, p, p.Up);
				}
				if (map[p.Down] == 'v')
				{
					BuildEdge(n, p, p.Down);
				}
			}

			void BuildEdge(DagNode from, Point prev, Point p)
			{
				int len = 1;
				while (!nodes.ContainsKey(p))
				{
					var next = p.LookAround().First(p => p != prev && map[p] != '#');
					prev = p;
					p = next;
					len++;
				}
				from.Edges.Add((nodes[p], len));
			}

			foreach (var line in map.Render((p, ch) => nodes.ContainsKey(p) ? 'X' : ch))
			{
				//Console.WriteLine(line);
			}

			var stack = new Stack<Point>();
			var visited = new HashSet<Point>();

		    void TopologicalSort(Point p)
			{
				// Mark the current node as visited
				visited.Add(p);

				// Recur for all the vertices adjacent to this
				// vertex
				for (int i = 0; i < nodes[p].Edges.Count; i++) {
					var node = nodes[p].Edges[i];
					if (!visited.Contains(node.Item1.Pos))
						TopologicalSort(node.Item1.Pos);
				}
				// Push current vertex to stack which stores
				// topological sort
				stack.Push(p);
			}
	
			// Call the recursive helper function to store
			// Topological Sort starting from all vertices one
			// by one
			foreach (var p in nodes.Keys)
			{
				if (!visited.Contains(p))
					TopologicalSort(p);
			}


			var dist = nodes.Values.ToDictionary(n => n.Pos, _ => int.MinValue);
			dist[start.Pos] = 0;

        while (stack.Count > 0) {
 
            // Get the next vertex from topological order
            var u = stack.Pop();
 
            // Update distances of all adjacent vertices ;
            if (dist[u] != int.MinValue)
			{
                for (int i = 0; i < nodes[u].Edges.Count; i++) {
                    var node = nodes[u].Edges[i];
                    if (dist[node.Item1.Pos] < dist[u] + node.Item2)
                        dist[node.Item1.Pos] = dist[u] + node.Item2;
                }
            }
        }

		var maxDist = dist.Values.Max() - 1;
		return maxDist;


// 			map[1, -1] = '#';
// //			map[w - 2, h] = '#';

			//var maze = new Maze(map)
			//	.WithEntry(Point.From(1, 0));



			//var dest = Point.From(w-2, h-1);
			//maze.ExternalMapPoints = [maze.Entry, dest];
			//var graph = Graph<char>.BuildUnitGraphFromMaze(maze);

			// var seenall = new Dictionary<string, int>();
			// var queue = Quack<(Point, int, HashSet<Point>)>.Create(QuackType.PriorityQueue);
			// var start = Point.From(1, 0);
			// var dest = Point.From(w-2, h-1);
			// var maxSteps = 0;

			// queue.Put((start, 0, new HashSet<Point>([start])), 0);

			// while (queue.TryGet(out var item))
			// {
			// 	var (pos, steps, seen) = item;

			// 	if (pos == dest)
			// 	{
			// 		if (steps > maxSteps)
			// 			maxSteps = steps;
			// 		continue;
			// 	}

			// 	//var key = $"{pos}-{slide}";
			// 	//if (seenall.TryGetValue(key, out var s))
			// 	//{
			// 	//	if (s < steps)
			// 	//		continue;
			// 	//}
			// 	//seenall[key] = steps;

			// 	var ch = map[pos];
			// 	if ("<>^v".Contains(ch))
			// 	{
			// 		var next = ch switch
			// 		{
			// 			'<' => pos.Left,
			// 			'>' => pos.Right,
			// 			'^' => pos.Up,
			// 			'v' => pos.Down,
			// 			_ => throw new Exception()
			// 		};
			// 		Debug.Assert(map[next] != '#');
			// 		if (seen.Contains(next))
			// 			continue;
			// 		var seen2 = new HashSet<Point>(seen)
			// 		{
			// 			next
			// 		};
			// 		queue.Put((next, steps + 1, seen2), next.ManhattanDistanceTo(dest));
			// 	}

			// 	foreach (var n in pos.LookAround().Where(p => map[p] != '#'))
			// 	{
			// 		if (seen.Contains(n))
			// 			continue;
			// 		var seen2 = new HashSet<Point>(seen)
			// 		{
			// 			n
			// 		};
			// 		queue.Put((n, steps + 1, seen2), n.ManhattanDistanceTo(dest));
			// 	}

			// 	//if (map[pos.Left] != '#' && map[pos.Left] != '>')
			// 	//	queue.Put((pos.Left, steps + 1, slide), pos.Left.ManhattanDistanceTo(dest));
			// 	//if (map[pos.Right] != '#' && map[pos.Right] != '<')
			// 	//	queue.Put((pos.Right, steps + 1, slide), pos.Right.ManhattanDistanceTo(dest));
			// 	//if (map[pos.Up] != '#' && map[pos.Up] != 'v')
			// 	//	queue.Put((pos.Up, steps + 1, slide), pos.Up.ManhattanDistanceTo(dest));
			// 	//if (map[pos.Down] != '#' && map[pos.Down] != '^')
			// 	//	queue.Put((pos.Down, steps + 1, slide), pos.Down.ManhattanDistanceTo(dest));
			// }

			// return maxSteps;
		}


		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (w, h) = map.Size();

			var start = new DagNode { Pos = Point.From(1, -1) };
			var dest = new DagNode { Pos = Point.From(w-2, h-1) };
			var nodes = map.AllPoints()
				.Where(p => p.LookAround().Count(x => "<>^v".Contains(map[x])) >= 3)
				.Select(p => new DagNode { Pos = p })
				.Append(start)
				.Append(dest)
				.ToDictionary(x => x.Pos, x => x);

			BuildEdge(start, start.Pos, start.Pos.Down);
			foreach (var n in nodes.Values)
			{
				var p = n.Pos;

				if (map[p.Left] == '<')
				{
					BuildEdge(n, p, p.Left);
				}
				if (map[p.Right] == '>')
				{
					BuildEdge(n, p, p.Right);
				}
				if (map[p.Up] == '^')
				{
					BuildEdge(n, p, p.Up);
				}
				if (map[p.Down] == 'v')
				{
					BuildEdge(n, p, p.Down);
				}
			}

			void BuildEdge(DagNode from, Point prev, Point p)
			{
				int len = 1;
				while (!nodes.ContainsKey(p))
				{
					var next = p.LookAround().First(p => p != prev && map[p] != '#');
					prev = p;
					p = next;
					len++;
				}
				from.Edges.Add((nodes[p], len));
				nodes[p].Edges.Add((from, len));
			}


			int LongestPath(DagNode node, int path, HashSet<Point> seen)
			{
				if (node == dest)
					return path;
				var nexts = node.Edges.Where(e => !seen.Contains(e.Item1.Pos)).ToArray();
				if (nexts.Length == 0)
					return 0;
				return nexts.Max(n =>
				{
					var seen2 = new HashSet<Point>(seen.Union([n.Item1.Pos]));
					return LongestPath(n.Item1, path + n.Item2, seen2);
				});
			}

			var maxPath = LongestPath(start, 0, new HashSet<Point>([start.Pos]));

			return maxPath - 1;
		}
	}
}
