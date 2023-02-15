using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class Graphx<T>
	{
		public class Node
		{
			internal static int NodeIndex = 0;

			public T Data { get; init; }
			public int Index { get; internal set; }
			public List<(Node Node, int Weight)> Neighbors { get; internal set; }
			public int Weight(Node node) => Neighbors.First(x => x.Node == node).Weight;

			public Node(T data)
			{
				Data = data;
				Index = NodeIndex++;
				Neighbors = new();
			}

			public override string ToString() => Data.ToString();
			public override int GetHashCode() => Data.GetHashCode();
			public override bool Equals(object obj) => obj is Node n && n.Index == Index;

			public void AddEdge(Node node, int weight)
			{
				var existing = Neighbors.FirstOrDefault(x => x.Node == node);
				if (existing != default)
				{
					Debug.Assert(existing.Weight == weight);
					return;
				}
				Neighbors.Add((node, weight));
			}

			public void UpdateEdge(Node node, int weight)
			{
				var existing = Neighbors.FirstOrDefault(x => x.Node == node);
				if (existing != default)
				{
					existing.Weight = weight;
				}
			}

			public void AddOrUpdateEdge(Node node, int weight)
			{
				var existing = Neighbors.FirstOrDefault(x => x.Node == node);
				if (existing == default)
				{
					AddEdge(node, weight);
				}
				else if (weight < existing.Weight)
				{
					existing.Weight = weight;
				}
			}

			public void RemoveEdge(Node node)
			{
				Neighbors.RemoveAll(x => x.Node == node);
			}
		}

		public List<Node> Nodes = new();
		private Dictionary<T,Node> _nodeMap = new();

		public Node AddNode(T v)
		{
			if (!_nodeMap.TryGetValue(v, out var node))
			{
				node = _nodeMap[v] = new Node(v);
				Nodes.Add(node);
			}
			return node;
		}

		public (Node A, Node B) Add(T v1, T v2, int weight)
		{
			var (a, b) = (AddNode(v1), AddNode(v2));
			a.AddEdge(b, weight);
			b.AddEdge(a, weight);
			return (a, b);
		}

		public void SetEdge(Node n1, Node n2, int weight)
		{
			n1.AddOrUpdateEdge(n2, weight);
			n2.AddOrUpdateEdge(n1, weight);
		}

		public Node this[T obj] => _nodeMap[obj];

		public void Reduce(Func<Node, bool> reduction)
		{
			foreach (var node in Nodes.ToArray())
			{
				if (!reduction(node))
					continue;
				// Remove this vertex; connect all its edges directly
				Nodes.Remove(node);
				foreach (var n1 in node.Neighbors)
				{
					foreach (var n2 in node.Neighbors.Where(n => !n.Equals(n1)))
					{
						if (n1.Node.Neighbors.Any(e => e.Node == n2.Node))
						{
							var weight = Math.Min(n1.Node.Weight(n2.Node), n1.Weight + n2.Weight);
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
			Node.NodeIndex = 0;
			_nodeMap = new();
			foreach (var n in Nodes)
			{
				n.Index = Node.NodeIndex++;
				_nodeMap[n.Data] = n;
			}
		}

		public void WriteAsGraphwiz()
		{
			Console.WriteLine("digraph {");
			foreach (var n in Nodes)
			{
				foreach (var e in n.Neighbors)
				{
					Console.WriteLine($"  \"{n}\" -> \"{e.Node}\" [label=\"{e.Weight}\"]");
				}
			}
			Console.WriteLine("}");
		}

		public Dictionary<Node, int> ShortestPathToAllDijkstra(Node from)
		{
			var vertices = Nodes;

			var visited = new HashSet<Node>();
			var distances = vertices.ToDictionary(x => x, _ => int.MaxValue);
			distances[from] = 0;

			var node = from;
			while (node != null)
			{
				foreach (var (next, weight) in node.Neighbors)
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
				foreach (var e in n.Neighbors)
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

		public Node[] NodesReachableFrom(Node start)
		{
			var visited = new Dictionary<T, Node>();
			var queue = new Queue<Node>();
			queue.Enqueue(start);
			while (queue.Any())
			{
				var v = queue.Dequeue();
				visited[v.Data] = v;
				foreach (var e in v.Neighbors.Where(e => !visited.ContainsKey(e.Node.Data)))
				{
					queue.Enqueue(e.Node);
				}
			}
			return visited.Values.ToArray();
		}

		//public int ShortestPathDijkstra(Node start, Node dest)
		//{
		//	const int Infinite = 10000000;
		//	var nodes = Nodes;

		//	var distances
		//	foreach (var v in Nodes)
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


		public static Graphx<Point> FromMaze(Maze maze)
		{
			var graph = new Graphx<Point>();

			var root = graph.AddNode(maze.Entry);
			foreach (var p in maze.ExternalMapPoints)
			{
				graph.AddNode(p);
			}

			BuildGraph(root);
			return graph;

			void BuildGraph(Graphx<Point>.Node origin)
			{
				var routes = origin.Data.LookAround()
						.Select(maze.Transform)
						.Where(maze.IsWalkable)
						.ToArray();

				foreach (var p in routes)
				{
					var v = graph[p];
					if (v != null)
					{
						graph.SetEdge(origin, v, 1);
					}
					else
					{
						var next = graph.AddNode(p);
						BuildGraph(next);
					}
				}
			}
		}
	}
}