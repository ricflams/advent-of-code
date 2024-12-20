using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class Graph<TId>
		: Graph<TId, bool>
	{
		public Node AddNode(TId id) => base.AddNode(id, false);
		public new Node AddNode(TId id, bool _) => throw new Exception("Don't call this directly");

		public (Node A, Node B) AddNodes(TId id1, TId id2, int weight) => base.AddNodes(id1, false, id2, false, weight);
		public new (Node A, Node B) AddNodes(TId id1, bool _, TId id2, bool __, int weight) => throw new Exception("Don't call this directly");
	}

	public class Graph<TId, TData>
	{
		const int Infinite = 1000000000;
		protected int NodeIndex = 0;

		public class Node(TId id, TData data, int index)
		{
			public TId Id { get; init; } = id;
			public TData Data { get; set; } = data;
			public int Index { get; set; } = index;

			public Dictionary<Node, int> Neighbors { get; internal set; } = [];

			public override string ToString() => Data?.ToString() ?? Id.ToString();
			public override int GetHashCode() => Id.GetHashCode();
			public override bool Equals(object obj) => obj is Node n && n.Index == Index;

			public void AddEdge(Node node, int weight)
			{
				if (Neighbors.TryGetValue(node, out var w))
				{
					Debug.Assert(w == weight);
					return;
				}
				Neighbors[node] = weight;
			}

			public void UpdateEdge(Node node, int weight)
			{
				Neighbors[node] = weight;
			}

			public void AddOrUpdateEdge(Node node, int weight)
			{
				if (Neighbors.TryGetValue(node, out var w))
				{
					if (weight < w)
						Neighbors[node] = weight;
				}
				else
				{
					AddEdge(node, weight);
				}
			}

			public void RemoveEdge(Node node)
			{
				Neighbors.Remove(node);
			}
		}

		public List<Node> Nodes = [];
		private Dictionary<TId, Node> _nodeMap = [];

		public Node AddNode(TId id, TData v)
		{
			if (!_nodeMap.TryGetValue(id, out var node))
			{
				node = _nodeMap[id] = new(id, v, NodeIndex++);
				Nodes.Add(node);
			}
			return node;
		}

		public (Node A, Node B) AddNodes(TId id1, TData v1, TId id2, TData v2, int weight)
		{
			var (a, b) = (AddNode(id1, v1), AddNode(id2, v2));
			a.AddEdge(b, weight);
			b.AddEdge(a, weight);
			return (a, b);
		}

		public void SetWeight(Node n1, Node n2, int weight)
		{
			n1.AddOrUpdateEdge(n2, weight);
			n2.AddOrUpdateEdge(n1, weight);
		}

		public Node this[TId obj] => _nodeMap.TryGetValue(obj, out var n) ? n : null;

		public void Reduce(Func<Node, bool> removeCondition)
		{
			foreach (var node in Nodes.ToArray())
			{
				if (!removeCondition(node))
					continue;
				// Remove this vertex; connect all its edges directly
				Nodes.Remove(node);
				foreach (var (n1, w1) in node.Neighbors)
				{
					foreach (var (n2, w2) in node.Neighbors.Where(n => !n.Key.Equals(n1)))
					{
						if (n1.Neighbors.TryGetValue(n2, out int w12))
						{
							var weight = Math.Min(w12, w1 + w2);
							n1.UpdateEdge(n2, weight);
						}
						else
						{
							n1.AddEdge(n2, w1 + w2);
						}
					}
					n1.RemoveEdge(node);
				}
			}

			// Update node indexes; easiest to just re-index them all
			NodeIndex = 0;
			_nodeMap = [];
			foreach (var n in Nodes)
			{
				n.Index = NodeIndex++;
				_nodeMap[n.Id] = n;
			}
		}

		public void WriteAsGraphwiz()
		{
			Console.WriteLine("digraph {");
			foreach (var node in Nodes)
			{
				foreach (var (n, w) in node.Neighbors)
				{
					Console.WriteLine($"  \"{node}\" -> \"{n}\" [label=\"{w}\"]");
				}
			}
			Console.WriteLine("}");
		}

		public int ShortestPathDijkstra(TId start, TId dest)
		{
			return ShortestPathDijkstra(this[start], this[dest]);
		}

		public int ShortestPathDijkstra(Node start, Node dest)
		{
			var visited = new bool[Nodes.Count];
			var distance = new int[Nodes.Count];
			Array.Fill(distance, int.MaxValue);
			distance[start.Index] = 0;

			var node = start;
			while (node != null)
			{
				if (node == dest)
				{
					return distance[dest.Index];
				}
				foreach (var (n, w) in node.Neighbors)
				{
					var dist = distance[node.Index] + w;
					if (dist < distance[n.Index])
					{
						distance[n.Index] = dist;
					}
				}
				visited[node.Index] = true;
				node = Nodes
					.Where(n => !visited[n.Index])
					.OrderBy(n => distance[n.Index])
					.FirstOrDefault();
			}

			return Infinite;
		}

		public List<Node> AStarPath(Node start, Node goal, Func<Node, Node, int> heuristics)
		{
			var frontier = new PriorityQueue<Node, int>();
			// var start = "startNode"; // Replace with your start node
			// var goal = "goalNode";   // Replace with your goal node
			frontier.Enqueue(start, 0);

			var cameFrom = new Dictionary<Node, Node?>();
			var costSoFar = new Dictionary<Node, int>();

			cameFrom[start] = null;
			costSoFar[start] = 0;

			//var graph = new Graph(); // Replace with your graph instance

			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();

				if (current == goal)
				{
					break;
				}

				foreach (var (next, cost) in current.Neighbors)
				{
					int newCost = costSoFar[current] + cost;
					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						costSoFar[next] = newCost;
						int priority = newCost + heuristics(goal, next);
						frontier.Enqueue(next, priority);
						cameFrom[next] = current;
					}
				}
			}

			if (!cameFrom.ContainsKey(goal))
				return null;

			var path = new List<Node>();
			var curr = goal;
			while (curr != start)
			{
				path.Add(curr);
				curr = cameFrom[curr];
			}
			path.Add(start);

			return path;
		}

		public Dictionary<Node, int> ShortestPathToAllDijkstra(Node start)
		{
			var visited = new bool[Nodes.Count];
			var distance = new int[Nodes.Count];
			Array.Fill(distance, int.MaxValue);
			distance[start.Index] = 0;

			var node = start;
			while (node != null)
			{
				foreach (var (n, w) in node.Neighbors)
				{
					var dist = distance[node.Index] + w;
					if (dist < distance[n.Index])
					{
						distance[n.Index] = dist;
					}
				}
				visited[node.Index] = true;
				node = Nodes
					.Where(n => !visited[n.Index])
					.OrderBy(n => distance[n.Index])
					.FirstOrDefault();
			}

			return Nodes.ToDictionary(n => n, n => distance[n.Index]);
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
					dist[i, j] = int.MaxValue / 2;

			foreach (var node in Nodes)
			{
				foreach (var (n, w) in node.Neighbors)
				{
					dist[node.Index, n.Index] = w;
					next[node.Index, n.Index] = n.Index;
				}
				dist[node.Index, node.Index] = 0;
				next[node.Index, node.Index] = node.Index;
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

		public int TspShortestDistanceBruteForce()
		{
			var N = Nodes.Count;
			var mindistance = int.MaxValue;
			foreach (var perm in MathHelper.AllPermutations(N))
			{
				var visits = perm.Select(i => Nodes[i]).ToArray();
				var distance = 0;
				for (var i = 0; i < N - 1; i++)
				{
					distance += visits[i].Neighbors[visits[i + 1]];
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
			var N = Nodes.Count;
			var maxdistance = 0;
			foreach (var perm in MathHelper.AllPermutations(N))
			{
				var visits = perm.Select(i => Nodes[i]).ToArray();
				var distance = 0;
				for (var i = 0; i < N - 1; i++)
				{
					distance += visits[i].Neighbors[visits[i + 1]];
				}
				if (distance > maxdistance)
				{
					maxdistance = distance;
				}
			}
			return maxdistance;
		}

		public Node[] NodesReachableFrom(Node start)
		{
			var visited = new bool[Nodes.Count];

			var queue = new Queue<Node>();
			queue.Enqueue(start);
			while (queue.TryDequeue(out var node))
			{
				visited[node.Index] = true;
				foreach (var n in node.Neighbors.Keys.Where(x => !visited[x.Index]))
				{
					queue.Enqueue(n);
				}
			}

			return Nodes.Where(n => visited[n.Index]).ToArray();
		}











		public static Graph<Point> BuildWeightedGraphFromMaze(Maze maze)
		{
			var graph = new Graph<Point>();
			var root = graph.AddNode(maze.Entry);
			if (maze.Exit != null)
			{
				graph.AddNode(maze.Exit);
			}

			var walked = new SparseMap<bool>();
			walked[root.Id] = true;
			foreach (var p in root.Id.LookAround().Select(maze.Teleport).Where(maze.IsWalkable))
			{
				BuildGraph(root, p);
			}
			return graph;

			void BuildGraph(Graph<Point>.Node origin, Point pos)
			{
				if (walked[pos])
				{
					return;
				}
				var weight = 1;
				while (true)
				{
					walked[pos] = true;

					var v = graph[pos];
					if (v != null)
					{
						graph.SetWeight(origin, v, weight);
						return;
					}

					var routes = pos.LookAround()
						.Select(maze.Teleport)
						.Where(p => !walked[p] && maze.IsWalkable(p) || graph[p] != null && graph[p] != origin)
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
							var fork = graph.AddNode(pos);
							graph.AddNodes(origin.Id, fork.Id, weight);
							foreach (var p in routes)
							{
								BuildGraph(fork, p);
							}
							return;
					}
				}
			}
		}




		public static Graph<Point, TData> BuildUnitGraphFromMaze(Maze maze)
		{
			var graph = new Graph<Point, TData>();
			var root = graph.AddNode(maze.Entry, default);
			if (maze.Exit != null)
			{
				graph.AddNode(maze.Exit, default);
			}

			BuildGraph(root);
			return graph;

			void BuildGraph(Graph<Point, TData>.Node origin)
			{
				var routes = origin.Id.LookAround()
						.Select(maze.Teleport)
						.Where(maze.IsWalkable)
						.ToArray();

				foreach (var p in routes)
				{
					var v = graph[p];
					if (v != null)
					{
						graph.SetWeight(origin, v, 1);
					}
					else
					{
						var next = graph.AddNode(p, default);
						BuildGraph(next);
					}
				}
			}
		}



		public static Graph<Point, TData> BuildUnitGraphFromMazeByQueue(Maze maze)
		{
			var graph = new Graph<Point, TData>();
			var root = graph.AddNode(maze.Entry, default);
			if (maze.Exit != null)
			{
				graph.AddNode(maze.Exit, default);
			}

			var queue = new Queue<Graph<Point, TData>.Node>();
			queue.Enqueue(root);

			while (queue.Count > 0)
			{
				var origin = queue.Dequeue();
				var routes = origin.Id.LookAround()
						.Select(maze.Teleport)
						.Where(maze.IsWalkable)
						.ToArray();

				foreach (var p in routes)
				{
					var v = graph[p];
					if (v != null)
					{
						graph.SetWeight(origin, v, 1);
					}
					else
					{
						var next = graph.AddNode(p, default);
						queue.Enqueue(next);
					}
				}
			}
			return graph;
		}



		//public static Graph<Point, TData> FromMaze<Node>(CharMap map, Point start, Func<Point, char, TData> makeNode)
		//{
		//	TData MakeNode(Point p) => makeNode(p, map[p]);

		//	var graph = new Graph<Point, TData>();

		//	var data = MakeNode(start);
		//	var startNode = graph.AddNode(start, data);
		//	//foreach (var p in maze.ExternalMapPoints)
		//	//{
		//	//	graph.AddNode(p, MakeNode(p));
		//	//}

		//	BuildGraph(startNode);
		//	return graph;

		//	void BuildGraph(Graph<Point, TData>.Node node)
		//	{
		//		foreach (var p in node.Id.LookAround())
		//		{
		//			var n = graph[p];
		//			if (n != null)
		//			{
		//				graph.SetWeight(node, n, 1);
		//			}
		//			else
		//			{
		//				var data = makeNode(p, map[p]);
		//				if (data != null)
		//				{
		//					var next = graph.AddNode(p, MakeNode(p));
		//					BuildGraph(next);
		//				}
		//			}
		//		}
		//		//var neighbors = node.Id.LookAround()
		//		//		.Select(maze.Teleport)
		//		//		.Where(maze.IsWalkable)
		//		//		.ToArray();

		//		//foreach (var p in neighbors)
		//		//{
		//		//	var v = graph[p];
		//		//	if (v != null)
		//		//	{
		//		//		graph.SetEdge(node, v, 1);
		//		//	}
		//		//	else
		//		//	{
		//		//		var next = graph.AddNode(p, MakeNode(p));
		//		//		BuildGraph(next);
		//		//	}
		//		//}
		//	}
		//}
	}
}