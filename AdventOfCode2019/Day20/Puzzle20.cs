using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;

namespace AdventOfCode2019.Day20
{
	internal static class Puzzle20
	{
		const int Infinite = 10000000;

		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var maze = ReadMaze("Day20/input.txt");

			var shortestPath = ShortestPath(maze, StepOnto);
			Console.WriteLine($"Day 20 Puzzle 1: {shortestPath}");
			Debug.Assert(shortestPath == 608);
		}

		private static void Puzzle2()
		{
			var maze = ReadMaze("Day20/input.txt");
			//maze.Render(maze.Entry);

			var shortestPath = MaxDepths().Select(d => FindMinimumDistanceBfsPlutonian(maze, d)).First(x => x != Infinite);
			Console.WriteLine($"Day 20 Puzzle 2: {shortestPath}");
			Debug.Assert(shortestPath == 6706);
		}

		private static IEnumerable<int> MaxDepths()
		{
			for (var depth = 25; ; depth += 5)
			{
				yield return depth;
			}
		}

		private static Point StepOnto(Maze maze, Point p)
		{
			var portal = maze.Portals[p];
			if (portal != null)
			{
				////Console.WriteLine($"Seeing a portal at {maze.Portals[portal]}");
			}
			return portal?.Pos ?? p;
		}

		private static List<Maze> _mazes = new List<Maze>();

		private static Point StepOntoPlutonian(Maze maze, Point p)
		{
			//var portal = maze.Portals[p];
			//if (portal == null)
			//	return p;

			////Console.WriteLine($"Seeing a portal at {maze.Portals[portal]}");

			//if (!portal.IsDownward)
			//{

			//}

			//return portal?.Pos ?? p;
			return p;
		}

		//private static int dummyYpos = 0;
		//private static void InflateGraph(List<Vertex> vertices)
		//{
		//	var dummy = Point.From(1000000, dummyYpos++); // oh dear
		//	while (true)
		//	{
		//		var node = vertices.FirstOrDefault(v => v.Edges.Values.Any(d => d > 1));
		//		if (node == null)
		//			break;
		//		foreach (var edge in node.Edges.Where(e => e.Value > 1).ToList())
		//		{
		//			node.Edges.Remove(edge.Key);
		//			edge.Key.Edges.Remove(node);
		//			var distance = edge.Value;
		//			var p = edge.Key;
		//			for (var i = 0; i < distance - 1; i++)
		//			{
		//				var insert = new Vertex(dummy);
		//				vertices.Add(insert);
		//				insert.Edges.Add(p, 1);
		//				p.Edges.Add(insert, 1);
		//				p = insert;
		//				dummy = dummy.Right;
		//			}
		//			node.Edges.Add(p, 1);
		//			p.Edges.Add(node, 1);
		//		}
		//	}
		//}


		private static void PrintGraph(Graph graph)
		{
			Console.WriteLine("digraph {");
			foreach (var v in graph.Vertices)
			{
				foreach (var e in v.Edges)
				{
					var portalName = v.PortalName ?? e.Key.PortalName;
					var label = e.Value == 0 ? "" : e.Value == -1 ? $"portal {portalName} down to {e.Key.Pos}" : $"portal {portalName} up to {e.Key.Pos}";
					Console.WriteLine($"  \"{v.Pos}\" -> \"{e.Key.Pos}\" [label=\"{label}\"]");
				}
			}
			Console.WriteLine("}");
		}

		private static int FindMinimumDistanceBfsPlutonian(Maze maze, int maxDepth)
		{
			//Console.WriteLine($"Find min dist for depth={maxDepth}");

			var topgraph = BuildSimpleGraph(maze, StepOnto);
			//PrintGraph(topgraph);

			//InflateGraph(topgraph.Vertices);

			topgraph.Outer = null;
			topgraph.Inner = null;

			var entry = topgraph.Vertices.First(v => v.Pos.Is(maze.Entry));
			var exit = topgraph.Vertices.First(v => v.Pos.Is(maze.Exit));


			var root = topgraph.Vertices.First();
			root.Visited = true;
			//root.VisitedBy.Add("");

			var queue = new Queue<(Graph, int, string, Vertex, int)>();
			queue.Enqueue((topgraph, 0, "", root, 0));
			while (queue.Any())
			{
				var (graph, level, visitedBy, node, distance) = queue.Dequeue();

				//Console.WriteLine($"[{level} {node.Pos} {visitedBy} {distance}]");

				if (node.Pos.Is(exit.Pos) && level == 0)
				{
					return distance;
				}

				var edges = node.Edges
					//.Where(n => !n.Key.VisitedBy.Contains(visitedBy))
					//.Where(n => !n.Key.Visited)
					.ToList();
				foreach (var e in edges)
				{
					var nextnode = e.Key;
					var nextlevel = level;
					var nextgraph = graph;
					//var nextVisitedBy = visitedBy;
					if (e.Value != 0)
					{
						if (e.Value == -1)
						{
							if (level > maxDepth)// || nextVisitedBy.Length > 15)
								continue;
							if (graph.Inner == null)
							{
								graph.Inner = BuildSimpleGraph(maze, StepOnto);
								graph.Inner.Outer = graph;
							}
							nextgraph = graph.Inner;
							//nextVisitedBy += "-";
						}
						else
						{
							nextgraph = graph.Outer;
							if (nextgraph == null)
								continue;
							//nextVisitedBy += "+";
						}
						nextlevel -= e.Value;
						nextnode = nextgraph.Vertices.First(v => v.Pos.Is(nextnode.Pos));
					}
					if (nextnode.Visited)//By.Contains(nextVisitedBy))
						continue;
					nextnode.Visited = true;// By.Add(nextVisitedBy);
					queue.Enqueue((nextgraph, nextlevel, /*nextVisitedBy*/visitedBy, nextnode, distance + 1));
				}
			}
			return Infinite;
		}


			//entry.Distance = 0;
			//foreach (var v in graph.Vertices.Skip(1))
			//{
			//	v.Distance = int.MaxValue;
			//}

			//var node = entry;
			//while (true)
			//{
			//	if (node == null)
			//		break;
			//	foreach (var edge in node.Edges)
			//	{
			//		var neighbour = edge.Key;
			//		var weight = edge.Value;
			//		var dist = node.Distance + weight;
			//		if (dist < neighbour.Distance)
			//		{
			//			neighbour.Distance = dist;
			//		}
			//	}
			//	node.Visited = true;
			//	if (node == exit)
			//		break;
			//	node = graph.Vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
			//}

			//return exit.Distance;

		private static int ShortestPath(Maze maze, Func<Maze, Point, Point> stepOnto)
		{
			var graph = BuildGraph(maze, stepOnto);

			var entry = graph.Vertices.First(v => v.Pos.Is(maze.Entry));
			var exit = graph.Vertices.First(v => v.Pos.Is(maze.Exit));
			entry.Distance = 0;
			foreach (var v in graph.Vertices.Skip(1))
			{
				v.Distance = int.MaxValue;
			}

			var node = entry;
			while (true)
			{
				if (node == null)
					break;
				foreach (var edge in node.Edges)
				{
					var neighbour = edge.Key;
					var weight = edge.Value;
					var dist = node.Distance + weight;
					if (dist < neighbour.Distance)
					{
						neighbour.Distance = dist;
					}
				}
				node.Visited = true;
				if (node == exit)
					break;
				node = graph.Vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
			}

			return exit.Distance;
		}

		internal class Graph
		{
			public List<Vertex> Vertices { get; } = new List<Vertex>();
			public void Render()
			{
				foreach (var v in Vertices)
				{
					Console.WriteLine($"At {v.Pos} with edges {string.Join(" ", v.Edges.Select(e => $"{e.Value}->{e.Key.Pos}"))}");
				}
			}
			public Graph Inner { get; set; }
			public Graph Outer { get; set; }
		}

		[DebuggerDisplay("{ToString()}")]
		internal class Vertex
		{
			public Vertex(Point pos) { Pos = pos; }
			public Point Pos { get; private set; }
			public bool Visited { get; set; }
			public string PortalName { get; set; }
			public int Distance { get; set; }
			public Dictionary<Vertex,int> Edges { get; set;  } = new Dictionary<Vertex, int>();
			public override string ToString() => $"{Pos} visited={Visited} edges={string.Join(" ", Edges.Keys.Select(e => $"{e.Pos}/{e.Visited}"))}";
		}

		////internal class Edge
		////{
		////	public Edge(Vertex v1, Vertex v2, int weight)
		////	{
		////		V1 = v1;
		////		V2 = v2;
		////		Weight = weight;
		////	}
		////	public Vertex V1 { get; private set; }
		////	public Vertex V2 { get; private set; }
		////	public int Weight { get; private set; }
		////}

		private static Graph BuildGraph(Maze maze, Func<Maze, Point, Point> stepOnto)
		{
			var walked = new SparseMap<bool>();
			var map = maze.Map;

			var graph = new Graph();
			graph.Vertices.Add(new Vertex(maze.Entry));
			graph.Vertices.Add(new Vertex(maze.Exit));
			var start = graph.Vertices.First();
			walked[start.Pos] = true;

			//walked[maze.Entry] = true;
			BuildGraph(start, maze.Entry.LookAround().First(p => map[p] == '.'));
			//BuildGraph(graph.Vertices.First(), maze.Entry);

			//maze.Render(null, p =>
			//{
			//	return
			//		graph.Vertices.Any(v => v.Pos.Is(p)) ? 'X' :
			//		walked[p] ? 'o' :
			//		(char?)null;
			//});
			// graph.Render();
			//Console.ReadKey();

			return graph;


			void BuildGraph(Vertex vertex, Point pos)
			{
				if (walked[pos])
				{
					return;
				}
				var weight = 1;
				while (true)
				{
					walked[pos] = true;

					var routes = pos.LookAround()
						.Select(p => stepOnto(maze, p))
						.Where(p => graph.Vertices.Any(v => v != vertex && v.Pos.Is(p)) || !walked[p] && map[p] != '#')
						.ToList();

					////Console.Clear();
					////maze.Render(pos, p =>
					////{
					////	return
					////		routes.Count() > 1 && routes.Any(r => r.Is(p)) ? '?' :
					////		graph.Vertices.Any(v => v.Pos.Is(p)) ? 'X' :
					////		walked[p] ? 'o' :
					////		(char?)null;
					////});
					////Console.WriteLine($"Graph: vertices:{graph.Vertices.Count}");
					////graph.Render();

					var fork = graph.Vertices.FirstOrDefault(v => v.Pos.Is(pos));
					if (fork != null)
					{
						//var edge = new Edge(vertex, fork, weight);
						if (vertex.Edges.ContainsKey(fork))
						{
							// Already exists - if this route is longer then just bail
							if (vertex.Edges[fork] <= weight)
							{
								////Console.WriteLine($"Abandoning longer route between {vertex.Pos} and {fork.Pos} - press any key");
								////Console.ReadKey();
								return;
							}
							else
							{
								////Console.WriteLine($"Found shorter route between {vertex.Pos} and {fork.Pos} - press any key");
							}
							vertex.Edges[fork] = weight;
							fork.Edges[vertex] = weight;
						}
						else
						{
							vertex.Edges.Add(fork, weight);
							fork.Edges.Add(vertex, weight);
						}

						////Console.ReadKey();
						return;
					}

					////Console.ReadKey();

					switch (routes.Count())
					{
						case 0: // Dead end - no edge here
							return;
						case 1: // Only one way, so move forward
							pos = routes.First();
							weight++;
							break;
						default: // Forks, so place vertex here and take each road
							if (fork == null)
							{
								fork = new Vertex(pos);
								graph.Vertices.Add(fork);
							}
							//var edge = new Edge(vertex, fork, weight);
							vertex.Edges.Add(fork, weight);
							fork.Edges.Add(vertex, weight);
							foreach (var p in routes)
							{
								BuildGraph(fork, p);
							}
							return;
					}
				}


			}
		}

		private static Graph BuildSimpleGraph(Maze maze, Func<Maze, Point, Point> stepOnto)
		{
			var walked = new SparseMap<Vertex>();
			var map = maze.Map;

			var graph = new Graph();
			graph.Vertices.Add(new Vertex(maze.Entry));
			//graph.Vertices.Add(new Vertex(maze.Exit));

			BuildSimpleGraph(graph.Vertices.First());

			return graph;

			void BuildSimpleGraph(Vertex node)
			{
				while (walked[node.Pos] == null)
				{
					walked[node.Pos] = node;
					var positions = node.Pos
						.LookAround()
						.Select(p => new { Pos = p, Dest = stepOnto(maze, p)})
						.Where(x => map[x.Dest] == '.')
						.Where(x => walked[x.Dest] == null || !walked[x.Dest].Edges.ContainsKey(node))
						.ToList();

					foreach (var p in positions.Where(x => walked[x.Dest] != null).ToList())
					{
						var existing = walked[p.Dest];
						var portal = maze.Portals[p.Pos];
						var portalValue = portal == null ? 0 : portal.IsDownward ? -1 : 1;
						var portalName = portal == null ? null : portal.Name;
						node.PortalName = portalName;
						existing.PortalName = portalName;
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
							var next = new Vertex(p.Dest);
							graph.Vertices.Add(next);
							var portal = maze.Portals[p.Pos];
							var portalValue = portal == null ? 0 : portal.IsDownward ? -1 : 1;
							var portalName = portal == null ? null : portal.Name;
							node.PortalName = portalName;
							next.PortalName = portalName;
							node.Edges[next] = portalValue;
							next.Edges[node] = -portalValue;
							node = next;
							break;
						default:
							var forks = positions.Select(x =>
							{
								var fork = new Vertex(x.Dest);
								graph.Vertices.Add(fork);

								//node.Edges[fork] = 1;
								//fork.Edges[node] = 1;
								return fork;
							}).ToList();
							foreach (var fork in forks)
							{
								BuildSimpleGraph(fork);
							}
							return;
					}
				}
			}
		}


		//internal class PlutonianMaze : Maze
		//{

		//}

		internal class Maze
		{
			public CharMap Map { get; set; }
			public SparseMap<Portal> Portals { get; set; }
			public Point Entry { get; set; }
			public Point Exit { get; set; } // Note: The exit is reached one space ahead of this point

			public Maze Inner { get; set; }
			public Maze Outer { get; set; }

			public void Render(Point position, Func<Point,char?> overlay = null)
			{
				var lines = Map.Render((p, ch) =>
				{
					return
						p.Is(position) ? '@' :
						p.Is(Entry) ?  '>' :
						p.Is(Exit) ? '<' :
						//Portals[p] != null ? 'x' :
						overlay?.Invoke(p) ?? ch;
				});
				for (var i = 0; i < lines.Length; i++)
				{
					//if (Math.Abs(position.Y - i) < 15)
						Console.WriteLine($"{lines[i]}");
				}
			}
		}

		internal class Portal
		{
			public string Name { get; set; }
			public Point Pos { get; set; }
			public bool IsDownward { get; set; }
		}

		private static Maze ReadMaze(string filename)
		{
			var map = new CharMap();

			var lines = File.ReadAllLines(filename);
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					map[x][y] = line[x];
				}
			}

			var portalinfo = map.AllPoints(char.IsUpper).OrderBy(p => p.X + p.Y*1000);
			var portalsByName = new Dictionary<string, List<Tuple<Point, Point>>>();

			// Map all entry-portals, identified initially as '?'
			foreach (var p in portalinfo)
			{

				if (map[p.Down] == '.')
				{
					//       X
					// p ->  Y  <- exit
					//       .  <- arrival
					AddPortal(p.Up, p, p, p.Down);
				}
				else if (map[p.Up] == '.')
				{
					//       .  <- arrival
					// p ->  X  <- exit
					//       Y
					AddPortal(p, p.Down, p, p.Up);
				}
			//};

			//// Next find the portal exits and match with their entry
			//foreach (var p in portalinfo)
			//{
				else if (map[p.Right] == '.')
				{
					// exit vv arrival 
					//     XY.
					//      ^ p
					AddPortal(p.Left, p, p, p.Right);
				}
				else if (map[p.Left] == '.')
				{
					// arrival vv exit
					//         .XY
					//          ^ p
					AddPortal(p, p.Right, p, p.Left);
				}
			};

			// Link up portals
			var area = map.Area();
			var entry = portalsByName["AA"].First().Item2;
			var exit = portalsByName["ZZ"].First().Item2;
			var portals = new SparseMap<Portal>();
			foreach (var pair in portalsByName.Where(p => p.Key != "AA" && p.Key != "ZZ"))
			{
				var p1 = pair.Value[0];
				var p2 = pair.Value[1];
				portals[p1.Item1] = new Portal
				{
					Name = pair.Key,
					Pos = p2.Item2,
					IsDownward = !IsOuterPortal(p1.Item1)
				};
				portals[p2.Item1] = new Portal
				{
					Name = pair.Key,
					Pos = p1.Item2,
					IsDownward = !IsOuterPortal(p2.Item1)
				};
			}

			map[portalsByName["AA"].First().Item1] = '#';
			map[portalsByName["ZZ"].First().Item1] = '#';

			return new Maze
			{
				Map = map,
				Portals = portals,
				Entry = entry,
				Exit = exit
			};

			bool IsOuterPortal(Point p) => p.X < 4 || p.X > area.Item2.X - 4 || p.Y < 4 || p.Y > area.Item2.Y - 4;


			void AddPortal(Point name1, Point name2, Point departure, Point arrival)
			{
				var name = new string(new char[] { map[name1], map[name2] });
				if (!portalsByName.TryGetValue(name, out var pn))
				{
					portalsByName[name] = new List<Tuple<Point, Point>>();
				}
				portalsByName[name].Add(new Tuple<Point, Point>(departure, arrival));
			}
		}
	}

}







//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using AdventOfCode2019.Helpers;
//using AdventOfCode2019.Intcode;

//namespace AdventOfCode2019.Day20
//{
//	internal static class Puzzle
//	{
//		public static void Run()
//		{
//			Puzzle1();
//			Puzzle2();
//		}

//		private static void Puzzle1()
//		{
//			var maze = ReadMaze("Day20/input.txt");

//			var shortestPath = ShortestPath(maze, StepOnto);
//			Console.WriteLine($"Day 20 Puzzle 1: {shortestPath}");

//		}

//		private static void Puzzle2()
//		{
//			var maze = ReadMaze("Day20/input-1.txt");

//			var shortestPath = ShortestPath(maze, StepOnto);
//			Console.WriteLine($"Day 20 Puzzle 2: {shortestPath}");
//		}

//		private static Point StepOnto(Maze maze, Point p)
//		{
//			var portal = maze.Portals[p];
//			if (portal != null)
//			{
//				////Console.WriteLine($"Seeing a portal at {maze.Portals[portal]}");
//			}
//			return portal ?? p;
//		}

//		private static int ShortestPath(Maze maze, Func<Maze, Point, Point> stepOnto)
//		{
//			var graph = BuildGraph(maze, stepOnto);

//			var entry = graph.Vertices.First(v => v.Pos.Is(maze.Entry));
//			var exit = graph.Vertices.First(v => v.Pos.Is(maze.Exit));
//			entry.Distance = 0;
//			foreach (var v in graph.Vertices.Skip(1))
//			{
//				v.Distance = int.MaxValue;
//			}

//			var node = entry;
//			while (true)
//			{
//				if (node == null)
//					break;
//				foreach (var edge in node.Edges)
//				{
//					var neighbour = edge.Key;
//					var weight = edge.Value;
//					var dist = node.Distance + weight;
//					if (dist < neighbour.Distance)
//					{
//						neighbour.Distance = dist;
//					}
//				}
//				node.Visited = true;
//				if (node == exit)
//					break;
//				node = graph.Vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
//			}

//			return exit.Distance;
//		}

//		internal class Graph
//		{
//			public List<Vertex> Vertices { get; } = new List<Vertex>();
//			public void Render()
//			{
//				foreach (var v in Vertices)
//				{
//					Console.WriteLine($"At {v.Pos} with edges {string.Join(" ", v.Edges.Select(e => $"{e.Value}->{e.Key.Pos}"))}");
//				}
//			}
//		}

//		internal class Vertex
//		{
//			public Vertex(Point pos) { Pos = pos; }
//			public Point Pos { get; private set; }
//			public bool Visited { get; set; }
//			public int Distance { get; set; }
//			public Dictionary<Vertex, int> Edges { get; set; } = new Dictionary<Vertex, int>();
//		}

//		////internal class Edge
//		////{
//		////	public Edge(Vertex v1, Vertex v2, int weight)
//		////	{
//		////		V1 = v1;
//		////		V2 = v2;
//		////		Weight = weight;
//		////	}
//		////	public Vertex V1 { get; private set; }
//		////	public Vertex V2 { get; private set; }
//		////	public int Weight { get; private set; }
//		////}

//		private static Graph BuildGraph(Maze maze, Func<Maze, Point, Point> stepOnto)
//		{
//			var walked = new SparseMap<bool>();
//			var map = maze.Map;

//			var graph = new Graph();
//			graph.Vertices.Add(new Vertex(maze.Entry));
//			graph.Vertices.Add(new Vertex(maze.Exit));
//			var start = graph.Vertices.First();
//			walked[start.Pos] = true;

//			//walked[maze.Entry] = true;
//			BuildGraph(start, maze.Entry.LookAround().First(p => map[p] == '.'));
//			//BuildGraph(graph.Vertices.First(), maze.Entry);

//			maze.Render(null, p =>
//			{
//				return
//					graph.Vertices.Any(v => v.Pos.Is(p)) ? 'X' :
//					walked[p] ? 'o' :
//					(char?)null;
//			});
//			graph.Render();
//			//Console.ReadKey();

//			return graph;


//			void BuildGraph(Vertex vertex, Point pos)
//			{
//				if (walked[pos])
//				{
//					return;
//				}
//				var weight = 1;
//				while (true)
//				{
//					walked[pos] = true;

//					var routes = pos.LookAround()
//						.Select(p => stepOnto(maze, p))
//						.Where(p => graph.Vertices.Any(v => v != vertex && v.Pos.Is(p)) || !walked[p] && map[p] != '#')
//						.ToList();

//					////Console.Clear();
//					////maze.Render(pos, p =>
//					////{
//					////	return
//					////		routes.Count() > 1 && routes.Any(r => r.Is(p)) ? '?' :
//					////		graph.Vertices.Any(v => v.Pos.Is(p)) ? 'X' :
//					////		walked[p] ? 'o' :
//					////		(char?)null;
//					////});
//					////Console.WriteLine($"Graph: vertices:{graph.Vertices.Count}");
//					////graph.Render();

//					var fork = graph.Vertices.FirstOrDefault(v => v.Pos.Is(pos));
//					if (fork != null)
//					{
//						//var edge = new Edge(vertex, fork, weight);
//						if (vertex.Edges.ContainsKey(fork))
//						{
//							// Already exists - if this route is longer then just bail
//							if (vertex.Edges[fork] <= weight)
//							{
//								////Console.WriteLine($"Abandoning longer route between {vertex.Pos} and {fork.Pos} - press any key");
//								////Console.ReadKey();
//								return;
//							}
//							else
//							{
//								////Console.WriteLine($"Found shorter route between {vertex.Pos} and {fork.Pos} - press any key");
//							}
//							vertex.Edges[fork] = weight;
//							fork.Edges[vertex] = weight;
//						}
//						else
//						{
//							vertex.Edges.Add(fork, weight);
//							fork.Edges.Add(vertex, weight);
//						}

//						////Console.ReadKey();
//						return;
//					}

//					////Console.ReadKey();

//					switch (routes.Count())
//					{
//						case 0: // Dead end - no edge here
//							return;
//						case 1: // Only one way, so move forward
//							pos = routes.First();
//							weight++;
//							break;
//						default: // Forks, so place vertex here and take each road
//							if (fork == null)
//							{
//								fork = new Vertex(pos);
//								graph.Vertices.Add(fork);
//							}
//							//var edge = new Edge(vertex, fork, weight);
//							vertex.Edges.Add(fork, weight);
//							fork.Edges.Add(vertex, weight);
//							foreach (var p in routes)
//							{
//								BuildGraph(fork, p);
//							}
//							return;
//					}
//				}


//			}
//		}

//		internal class Maze
//		{
//			public CharMap Map { get; set; }
//			public SparseMap<Point> Portals { get; set; }
//			public Point Entry { get; set; }
//			public Point Exit { get; set; } // Note: The exit is reached one space ahead of this point

//			public void Render(Point position, Func<Point, char?> overlay = null)
//			{
//				var lines = Map.Render((p, ch) =>
//				{
//					return
//						p.Is(position) ? '@' :
//						p.Is(Entry) ? '>' :
//						p.Is(Exit) ? '<' :
//						Portals[p] != null ? 'x' :
//						overlay?.Invoke(p) ?? ch;
//				});
//				for (var i = 0; i < lines.Length; i++)
//				{
//					//if (Math.Abs(position.Y - i) < 15)
//					Console.WriteLine($"{lines[i]}");
//				}
//			}
//		}

//		private static Maze ReadMaze(string filename)
//		{
//			var map = new CharMap();

//			var lines = File.ReadAllLines(filename);
//			for (var y = 0; y < lines.Length; y++)
//			{
//				var line = lines[y];
//				for (var x = 0; x < line.Length; x++)
//				{
//					map[x][y] = line[x];
//				}
//			}

//			var portalinfo = map.AllPoints(char.IsUpper).OrderBy(p => p.X + p.Y * 1000);
//			var portalsByName = new Dictionary<string, List<Tuple<Point, Point>>>();

//			// Map all entry-portals, identified initially as '?'
//			foreach (var p in portalinfo)
//			{

//				if (map[p.Down] == '.')
//				{
//					//       X
//					// p ->  Y  <- exit
//					//       .  <- arrival
//					AddPortal(p.Up, p, p, p.Down);
//				}
//				else if (map[p.Up] == '.')
//				{
//					//       .  <- arrival
//					// p ->  X  <- exit
//					//       Y
//					AddPortal(p, p.Down, p, p.Up);
//				}
//				//};

//				//// Next find the portal exits and match with their entry
//				//foreach (var p in portalinfo)
//				//{
//				else if (map[p.Right] == '.')
//				{
//					// exit vv arrival 
//					//     XY.
//					//      ^ p
//					AddPortal(p.Left, p, p, p.Right);
//				}
//				else if (map[p.Left] == '.')
//				{
//					// arrival vv exit
//					//         .XY
//					//          ^ p
//					AddPortal(p, p.Right, p, p.Left);
//				}
//			};

//			// Link up portals
//			var entry = portalsByName["AA"].First().Item2;
//			var exit = portalsByName["ZZ"].First().Item2;
//			var portals = new SparseMap<Point>();
//			foreach (var pair in portalsByName.Where(p => p.Key != "AA" && p.Key != "ZZ"))
//			{
//				var p1 = pair.Value[0];
//				var p2 = pair.Value[1];
//				portals[p1.Item1] = p2.Item2;
//				portals[p2.Item1] = p1.Item2;
//			}

//			return new Maze
//			{
//				Map = map,
//				Portals = portals,
//				Entry = entry,
//				Exit = exit
//			};

//			void AddPortal(Point name1, Point name2, Point departure, Point arrival)
//			{
//				var name = new string(new char[] { map[name1], map[name2] });
//				if (!portalsByName.TryGetValue(name, out var pn))
//				{
//					portalsByName[name] = new List<Tuple<Point, Point>>();
//				}
//				portalsByName[name].Add(new Tuple<Point, Point>(departure, arrival));
//			}
//		}
//	}

//}

