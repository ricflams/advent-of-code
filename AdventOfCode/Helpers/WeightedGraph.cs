using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Helpers
{
    public class WeightedGraph<T>
    {
		public Dictionary<T, Vertex> Vertices { get; } = new Dictionary<T, Vertex>();
		public Vertex Root => Vertices.Values.FirstOrDefault();

		public class VertexByName : Dictionary<string, Vertex>
		{
			public new Vertex this[string name]
			{
				get => TryGetValue(name, out var vertex) ? vertex : null;
				set => base[name] = value;
			}
		}
		[DebuggerDisplay("{ToString()}")]
		public class Vertex
		{
			public T Value { get; set; }
			public IDictionary<Vertex, int> Edges { get; } = new Dictionary<Vertex, int>();

			//// Always present, for ease-of-use in graph-searches
			//public int Distance { get; set; }
			//public bool Visited { get; set; }

			public override string ToString() => $"{Value?.ToString() ?? ""}";
		}

		public void WriteAsGraphwiz()
		{
			Console.WriteLine("digraph {");
			foreach (var v in Vertices.Values)
			{
				foreach (var e in v.Edges)
				{
					Console.WriteLine($"  \"{v.Value}\" -> \"{e.Key.Value}\" [label=\"{e.Value}\"]");
				}
			}
			Console.WriteLine("}");
		}

		public void AddVertices(T val1, T val2, int distance)
		{
			var v1 = GetOrCreateVertex(val1);
			var v2 = GetOrCreateVertex(val2);
			AddEdge(v1, v2, distance);
		}

		private Vertex GetOrCreateVertex(T key)
		{
			if (!Vertices.TryGetValue(key, out var v))
			{
				v = new Vertex { Value = key };
				Vertices[key] = v;
			}
			return v;
		}

		public static void AddEdge(Vertex v1, Vertex v2, int weight)
		{
			if (!v1.Edges.ContainsKey(v2))
				v1.Edges.Add(v2, weight);
			if (!v2.Edges.ContainsKey(v1))
				v2.Edges.Add(v1, weight);
		}

		public void Reduce(Func<Vertex, bool> reduction)
		{
			var vertices = Vertices.ToArray();
			foreach (var (key,v) in vertices)
			{
				if (!reduction(v))
					continue;
				// Remove this vertex; connect all its edges directly
				Vertices.Remove(key);
				foreach (var n1 in v.Edges)
				{
					foreach (var n2 in v.Edges.Where(n => !n.Equals(n1)))
					{
						if (n1.Key.Edges.ContainsKey(n2.Key))
						{
							var dist = Math.Min(n1.Key.Edges[n2.Key], n1.Value + n2.Value);
							n1.Key.Edges[n2.Key] = dist;
						}
						else
						{
							n1.Key.Edges.Add(n2.Key, n1.Value + n2.Value);
						}
					}
					n1.Key.Edges.Remove(v);
				}
			}
		}

		public Dictionary<Vertex, int> ShortestPathToAllDijkstra(Vertex from)
		{
			var vertices = Vertices.Values;

			var visited = new HashSet<Vertex>();
			var distances = vertices.ToDictionary(x => x, _ => int.MaxValue);
			distances[from] = 0;

			var node = from;
			while (node != null)
			{
				foreach (var edge in node.Edges)
				{
					var neighbour = edge.Key;
					var weight = edge.Value;
					var dist = distances[node] + weight;
					if (dist < distances[neighbour])
					{
						distances[neighbour] = dist;
					}
				}
				visited.Add(node);
				node = vertices
					.Where(v => !visited.Contains(v))
					.OrderBy(x => distances[x])
					.FirstOrDefault();
			}

			return distances;
		}		

		public int TspShortestDistanceBruteForce()
		{
			var vertices = Vertices.Values.ToArray();
			var N = vertices.Length;
			var mindistance = int.MaxValue;
			foreach (var perm in MathHelper.AllPermutations(N))
			{
				var visits = perm.Select(i => vertices[i]).ToArray();
				var distance = 0;
				for (var i = 0; i < N - 1; i++)
				{
					distance += visits[i].Edges[visits[i + 1]];
				}
				if (distance < mindistance)
				{
					mindistance = distance;
				}
			}
			return mindistance;
		}

		public int TspLongestDistanceBruteForce()
		{
			var vertices = Vertices.Values.ToArray();
			var N = vertices.Length;
			var maxdistance = 0;
			foreach (var perm in MathHelper.AllPermutations(N))
			{
				var visits = perm.Select(i => vertices[i]).ToArray();
				var distance = 0;
				for (var i = 0; i < N - 1; i++)
				{
					distance += visits[i].Edges[visits[i + 1]];
				}
				if (distance > maxdistance)
				{
					maxdistance = distance;
				}
			}
			return maxdistance;
		}
	}
}
