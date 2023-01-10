using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class GraphxNode
	{
		public record Edge(GraphxNode Node, int Weight) {}
		public string Name { get; set; }
		public int Index { get; set; }
		public List<Edge> Edges = new();
		public Dictionary<string, Edge> EdgeByName = new();
		public void AddEdge(GraphxNode node, int weight)
		{
			var edge = new Edge(node, weight);
			Edges.Add(edge);
			EdgeByName[edge.Node.Name] = edge;
		}
		public void UpdateEdge(GraphxNode node, int weight)
		{
			var edge = new Edge(node, weight);
			Edges[Edges.IndexOf(e => e.Node == node)] = edge;
			EdgeByName[edge.Node.Name] = edge;
		}
		public void RemoveEdge(GraphxNode node)
		{
			Edges.RemoveAt(Edges.IndexOf(e => e.Node == node));
			EdgeByName.Remove(node.Name);
		}		
	}
	
	public class Graphx<T> where T:GraphxNode, new()
	{
		public List<T> Nodes = new();
		private Dictionary<string, int> _nodeIndex = new();

		public T AddEdge(string name1, string name2, int weight)
		{
			var a = GetOrCreate(name1);
			var b = GetOrCreate(name2);
			a.AddEdge(b, weight);
			return a;
		}

		public T this[string name] => Nodes[_nodeIndex[name]];


		private T GetOrCreate(string name)
		{
			if (!_nodeIndex.TryGetValue(name, out var nodeIndex))
			{
				var node = new T();
				node.Name = name;
				node.Index = Nodes.Count;
				Nodes.Add(node);
				nodeIndex = _nodeIndex[name] = node.Index;
			}
			return Nodes[nodeIndex];
		}

		public void Reduce(Func<T, bool> reduction)
		{
			foreach (var node in Nodes.ToArray())
			{
				if (!reduction(node))
					continue;
				// Remove this vertex; connect all its edges directly
				Nodes.Remove(node);
				foreach (var n1 in node.Edges)
				{
					foreach (var n2 in node.Edges.Where(n => !n.Equals(n1)))
					{
						if (n1.Node.Edges.Any(e => e.Node == n2.Node))
						{
							var weight = Math.Min(n1.Node.EdgeByName[n2.Node.Name].Weight, n1.Weight + n2.Weight);
							n1.Node.UpdateEdge(n2.Node, weight);
						}
						else
						{
							n1.Node.AddEdge(n2.Node, n1.Weight+ n2.Weight);
						}
					}
					n1.Node.RemoveEdge(node);
				}
			}
			_nodeIndex = Nodes.Select((n, idx) => (n, idx)).ToDictionary(x => x.n.Name, x => x.idx);
		}

		public void WriteAsGraphwiz()
		{
			Console.WriteLine("digraph {");
			foreach (var n in Nodes)
			{
				foreach (var e in n.Edges)
				{
					Console.WriteLine($"  \"{n}\" -> \"{e.Node}\" [label=\"{e.Weight}\"]");
				}
			}
			Console.WriteLine("}");
		}

		public Dictionary<T, int> ShortestPathToAllDijkstra(T from)
		{
			var vertices = Nodes;

			var visited = new HashSet<T>();
			var distances = vertices.ToDictionary(x => x, _ => int.MaxValue);
			distances[from] = 0;

			var node = from;
			while (node != null)
			{
				foreach (var (next, weight) in node.Edges)
				{
					var dist = distances[node] + weight;
					if (dist < distances[(T)next])
					{
						distances[(T)next] = dist;
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
	}



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

		public Dictionary<Vertex, int> ShortestPathToAllDijkstra2(Vertex from)
		{
			var vertices = Vertices.Values;

			var visited = new HashSet<Vertex>();
			var distances = vertices.ToDictionary(x => x, _ => int.MaxValue);
			distances[from] = 0;

			var node = from;
			while (node != null)
			{
				foreach (var (next, weight) in node.Edges)
				{
					var dist = distances[node] + weight;
					if (dist < distances[next])
					{
						distances[next] = dist;
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


		public Dictionary<Vertex, (int Distance, Vertex Direction)> ShortestPathToAllDijkstra(Vertex start)
		{
			var frontier = new PriorityQueue<Vertex, int>();
			frontier.Enqueue(start, 0);

			var paths = new Dictionary<Vertex, Vertex>();
			var costs = new Dictionary<Vertex, int>();
			paths[start] = null;
			costs[start] = 0;

			while (frontier.TryDequeue(out var current, out var _))
			{
				foreach (var (next, weight) in current.Edges)
				{
					var cost = costs[current] + weight;
					if (!costs.ContainsKey(next) || cost < costs[next])
					{
						costs[next] = cost;
						paths[next] = current;
						frontier.Enqueue(next, cost);
					}
				}
			}

			var results = Vertices.Values
				.Where(v => v != start)
				.Select(v =>
				{
					var dir = v;
					while (paths[dir] != start)
						dir = paths[dir];
					return (Dest: v, Distance: costs[v], Direction: dir);
				})
				.ToDictionary(x => x.Dest, x => (x.Distance, x.Direction));
			return results;
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
