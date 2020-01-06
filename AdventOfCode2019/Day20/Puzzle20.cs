using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day20
{
	internal static class Puzzle20
	{
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

		}

		private static void Puzzle2()
		{
			var maze = ReadMaze("Day20/input-1.txt");
			_mazes.Add(maze);


			var shortestPath = ShortestPath(maze, StepOnto);
			Console.WriteLine($"Day 20 Puzzle 2: {shortestPath}");
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
		private static int MaxDepth = 20;

		private static Point StepOntoPlutonian(Maze maze, Point p)
		{
			var portal = maze.Portals[p];
			if (portal == null)
				return p;

			//Console.WriteLine($"Seeing a portal at {maze.Portals[portal]}");
			
			if (!portal.Outer)
			{

			}

			return portal?.Pos ?? p;
		}



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
		}

		internal class Vertex
		{
			public Vertex(Point pos) { Pos = pos; }
			public Point Pos { get; private set; }
			public bool Visited { get; set; }
			public int Distance { get; set; }
			public Dictionary<Vertex,int> Edges { get; set;  } = new Dictionary<Vertex, int>();
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

			maze.Render(null, p =>
			{
				return
					graph.Vertices.Any(v => v.Pos.Is(p)) ? 'X' :
					walked[p] ? 'o' :
					(char?)null;
			});
			graph.Render();
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
						Portals[p] != null ? 'x' :
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
			public Point Pos { get; set; }
			public bool Outer { get; set; }
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
					Pos = p2.Item2,
					Outer = IsOuterPortal(p2.Item2)
				};
				portals[p2.Item1] = new Portal
				{
					Pos = p1.Item2,
					Outer = IsOuterPortal(p1.Item2)
				};
			}

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

