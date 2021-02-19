using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class BaseUnitGraph<T>
	{
		public VertexByValue Vertices { get; } = new VertexByValue();
		public Vertex Root => Vertices.Values.FirstOrDefault();

		public class VertexByValue : Dictionary<T, Vertex>
		{
			public new Vertex this[T p]
			{
				get => TryGetValue(p, out var vertex) ? vertex : null;
				set => base[p] = value;
			}
		}
		[DebuggerDisplay("{ToString()}")]
		public class Vertex
		{
			public Vertex(T value)
			{
				Value = value;
			}
			public T Value { get; set; }
			public HashSet<Vertex> Edges { get; } = new HashSet<Vertex>();

			// // Always present, for ease-of-use in graph-searches
			// public int Distance { get; set; }
			// public bool Visited { get; set; }

			public override string ToString() => Value?.ToString() ?? "";
		}

		public void WriteAsGraphwiz()
		{
			Console.WriteLine("digraph {");
			foreach (var v in Vertices.Values)
			{
				foreach (var e in v.Edges)
				{
					Console.WriteLine($"  \"{v.Value}\" -> \"{e.Value}\"");
				}
			}
			Console.WriteLine("}");
		}


		public void AddEdge(T v1, T v2)
		{
			AddEdge(GetOrCreateVertex(v1), GetOrCreateVertex(v2));
		}

		public void AddEdge(Vertex v1, Vertex v2)
		{
			v1.Edges.Add(v2);
			v2.Edges.Add(v1);
		}

		public void AddDirectedEdge(T v1, T v2)
		{
			AddDirectedEdge(GetOrCreateVertex(v1), GetOrCreateVertex(v2));
		}

		public void AddDirectedEdge(Vertex v1, Vertex v2)
		{
			v1.Edges.Add(v2);
		}

		private Vertex GetOrCreateVertex(T value)
		{
			if (Vertices[value] == null)
			{
				Vertices[value] = new Vertex(value);
			}
			return Vertices[value];
		}

		public Vertex[] VerticesReachableFrom(Vertex v0)
		{
			var visited = new Dictionary<T, Vertex>();
			var queue = new Queue<Vertex>();
			queue.Enqueue(v0);
			while (queue.Any())
			{
				var v = queue.Dequeue();
				visited[v.Value] = v;
				foreach (var e in v.Edges.Where(e => !visited.ContainsKey(e.Value)))
				{
					queue.Enqueue(e);
				}
			}
			return visited.Values.ToArray();
		}

	}


	public class Graph<T>
    {
		public VertexByPoint Vertices { get; } = new VertexByPoint();
		public Vertex Root => Vertices.Values.FirstOrDefault();

		public class VertexByPoint : Dictionary<Point, Vertex>
		{
			public new Vertex this[Point p]
			{
				get => TryGetValue(p, out var vertex) ? vertex : null;
				set => base[p] = value;
			}
		}

		[DebuggerDisplay("{ToString()}")]
		public class Vertex
		{
			public Vertex(Point pos)
			{
				Pos = pos;
			}

			public Point Pos { get; }
			public T Value { get; set; }
			public IDictionary<Vertex, int> Edges { get; } = new Dictionary<Vertex, int>();

			// Always present, for ease-of-use in graph-searches
			public int Distance { get; set; }
			public bool Visited { get; set; }

			public override string ToString() => $"{Pos} {Value?.ToString() ?? ""}";
		}

		public void WriteAsGraphwiz()
		{
			Console.WriteLine("digraph {");
			foreach (var v in Vertices.Values)
			{
				foreach (var e in v.Edges)
				{
					Console.WriteLine($"  \"{v.Pos}\" -> \"{e.Key.Pos}\" [label=\"{e.Value}\"]");
				}
			}
			Console.WriteLine("}");
		}

		public int ShortestPathDijkstra(Point startPos, Point destinationPos)
		{
			const int Infinite = 10000000;
			var vertices = Vertices.Values;
			var start = vertices.First(v => v.Pos == startPos);
			var destination = vertices.First(v => v.Pos == destinationPos);

			foreach (var v in vertices)
			{
				v.Distance = int.MaxValue;
			}
			start.Distance = 0;

			var node = start;
			while (node != null)
			{
				if (node == destination)
				{
					return destination.Distance;
				}
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
				node = vertices
					.Where(v => !v.Visited)
					.OrderBy(x => x.Distance)
					.FirstOrDefault();
			}

			return Infinite;
		}

		public Vertex AddVertex(Point pos)
		{
			var vertex = new Vertex(pos);
			Vertices[pos] = vertex;
			return vertex;
		}

		public static void AddEdge(Vertex v1, Vertex v2, int weight)
		{
			v1.Edges.Add(v2, weight);
			v2.Edges.Add(v1, weight);
		}

		public static void SetEdge(Vertex v1, Vertex v2, int weight)
		{
			if (!v1.Edges.Keys.Contains(v2))
			{
				v1.Edges.Add(v2, weight);
				v2.Edges.Add(v1, weight);
			}
			else if (weight < v1.Edges[v2])
			{
				v1.Edges[v2] = weight;
				v2.Edges[v1] = weight;
			}
		}

		public static Graph<T> BuildWeightedGraphFromMaze(Maze maze)
		{
			var graph = new Graph<T>();
			var root = graph.AddVertex(maze.Entry);
			foreach (var p in maze.ExternalMapPoints)
			{
				graph.AddVertex(p);
			}

			var walked = new SparseMap<bool>();
			walked[root.Pos] = true;
			foreach (var p in root.Pos.LookAround().Where(maze.IsWalkable))
			{
				BuildGraph(root, p);
			}
			return graph;

			void BuildGraph(Vertex origin, Point pos)
			{
				if (walked[pos])
				{
					return;
				}
				var weight = 1;
				while (true)
				{
					walked[pos] = true;

					var v = graph.Vertices[pos];
					if (v != null)
					{
						AddEdge(origin, v, weight);
						return;
					}

					var routes = pos.LookAround()
						.Select(maze.Transform)
						.Where(p => !walked[p] && maze.IsWalkable(p) || graph.Vertices[p] != null && graph.Vertices[p] != origin)
						.ToArray();

					switch (routes.Length)
					{
						case 0: // Dead end - no edge here
							return;
						case 1: // Only one way, so move forward
							pos = routes[0];
							weight++;
							break;
						default: // Forks, so place vertex here and take each road
							var fork = graph.AddVertex(pos);
							AddEdge(origin, fork, weight);
							foreach (var p in routes)
							{
								BuildGraph(fork, p);
							}
							return;
					}
				}
			}
		}

		public static Graph<T> BuildUnitGraphFromMaze(Maze maze)
		{
			var graph = new Graph<T>();
			var root = graph.AddVertex(maze.Entry);
			foreach (var p in maze.ExternalMapPoints)
			{
				graph.AddVertex(p);
			}

			BuildGraph(root);
			return graph;

			void BuildGraph(Vertex origin)
			{
				var routes = origin.Pos.LookAround()
						.Select(maze.Transform)
						.Where(maze.IsWalkable)
						.ToArray();

				foreach (var p in routes)
				{
					var v = graph.Vertices[p];
					if (v != null)
					{
						SetEdge(origin, v, 1);
					}
					else
					{
						var next = graph.AddVertex(p);
						BuildGraph(next);
					}
				}
			}
		}

	}
}
