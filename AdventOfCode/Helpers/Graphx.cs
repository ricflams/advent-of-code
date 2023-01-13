using System;
using System.Collections.Generic;
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
			for (var i = 0; i < Nodes.Count; i++)
				Nodes[i].Index = i;
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

		public int[,] FloydWarshallShortestPaths()
		{
			// let dist be a NxN array of minimum distances initialized to infinity
			// let next be a NxN array of vertex indices initialized to null			
			// procedure FloydWarshallWithPathReconstruction() is
			//     for each edge (u, v) do
			//         dist[u][v] ← w(u, v)  // The weight of the edge (u, v)
			//         next[u][v] ← v
			//     for each vertex v do
			//         dist[v][v] ← 0
			//         next[v][v] ← v
			//     for k from 1 to |V| do // standard Floyd-Warshall implementation
			//         for i from 1 to |V|
			//             for j from 1 to |V|
			//                 if dist[i][j] > dist[i][k] + dist[k][j] then
			//                     dist[i][j] ← dist[i][k] + dist[k][j]
			//                     next[i][j] ← next[i][k]			
			var N = Nodes.Count;
			var dist = new int[N, N];
			var next = new int[N, N];

			for (var i = 0; i < N; i++)
				for (var j = 0; j < N; j++)
					dist[i, j] = int.MaxValue/2;

			foreach (var n in Nodes)
			{
				foreach (var e in n.Edges)
				{
					dist[n.Index, e.Node.Index] = e.Weight;
					next[n.Index, e.Node.Index] = e.Node.Index;
				}
				dist[n.Index, n.Index] = 0;
				next[n.Index, n.Index] = n.Index;
			}

			for (var k = 0; k < N; k++)
			{
				for (var i = 0; i < N; i++)
				{
					for (var j = 0; j < N; j++)
					{
						if (dist[i, j] > dist[i, k] + dist[k, j])
						{
							dist[i, j] = dist[i, k] + dist[k, j];
							next[i, j] = next[i, k];
						}
					}
				}
			}

			return dist;
		}

		public IEnumerable<List<T>> Chunks()
		{
			var nodes = new HashSet<T>(Nodes);
			while (nodes.Any())
			{
				yield return Chunk(nodes.First());
			}

			List<T> Chunk(T node)
			{
				var chunk = new List<T>();
				var cq = new Queue<T>();
				cq.Enqueue(node);
				while (cq.TryDequeue(out var n))
				{
					nodes.Remove(n);
					chunk.Add(n);
					foreach (var e in n.Edges.Where(e => nodes.Contains(e.Node)))
						cq.Enqueue((T)e.Node);
				}
				return chunk;
			}
		}
	}
}