using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Y2019.Day18
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Many-Worlds Interpretation";
		public override int Year => 2019;
		public override int Day => 18;

		public void Run()
		{
			RunPart1For("test1", 8);
			RunPart1For("test2", 86);
			RunPart1For("test3", 132);
			RunPart1For("test4", 136);
			RunPart1For("test5", 81);

			RunPart1For("input", 3216);
			// TODO, doesn't work anymore: RunFor("input", 3216, 1538);
		}

		protected override int Part1(string[] input)
		{
			var steps = ShortestPath(ReadMap(input));
			return steps;
		}

		protected override int Part2(string[] input)
		{
			var graphs = BuildFourSplitGraph(ReadMap(input));

			//foreach (var g in graphs)
			//{
			//	InflateGraph(g);
			//}
			//var distance = FindMinimumDistanceBfs(graphs);
			foreach (var g in graphs)
			{
				PruneGraph(g);
			}
			var distance = FindMinimumDistanceDfs4(graphs);
			//var vertices = graphs.SelectMany(x => x).ToList();
			//var distance = FindMinimumDistanceDfs4(graphs);
			////PrintGraph(vertices);
			return distance;
		}


		[DebuggerDisplay("{ToString()}")]
		internal class Vertex
		{
			public Vertex(Point pos, char value)
			{
				Pos = pos;
				Value = value;
				IsDoor = char.IsUpper(Value);
				IsKey = char.IsLower(Value);
				Key = 1U << (Convert.ToInt32(char.ToLower(Value)) - Convert.ToInt32('a'));
				IsKeyKey = IsKey ? Key : 0;
			}
			public Point Pos { get; }
			public char Value { get; }
			public bool IsDoor { get; }
			public bool IsKey { get; }
			public uint Key { get; }
			public uint IsKeyKey { get; }
			public bool IsPassableWith(uint keys) => !IsDoor || (Key & keys) != 0;
			public bool Visited { get; set; }
			public bool Skip { get; set; }
			public int Distance { get; set; }
			public int ExtraDistance { get; set; }
			public uint PickedUpKeys { get; set; }
			public HashSet<uint> VisitedBy = new HashSet<uint>();
			public Dictionary<Vertex, int> Edges { get; set; } = new Dictionary<Vertex, int>();
			public override string ToString() => $"{Pos} '{Value}'";
			public SafeDictionary<uint, int> DistanceWithKeys = new SafeDictionary<uint, int>(10000000);

			public Dictionary<uint, int> ShortestPathToNearestKeys = new Dictionary<uint, int>();
		}

		internal class VaultGraph : Graph<VaultVertex>
		{
		}

		internal class VaultMaze : Maze
		{
			public VaultMaze(string[] lines)
				: base(CharMap.FromArray(lines))
			{
				Entry = Map.FirstOrDefault(ch => ch == '@');
			}
		}

		internal class VaultVertex
		{
			public VaultVertex(char value)
			{
				ChValue = value;
				IsDoor = char.IsUpper(ChValue);
				IsKey = char.IsLower(ChValue);
				Key = 1U << (Convert.ToInt32(char.ToLower(ChValue)) - Convert.ToInt32('a'));
			}
			public char ChValue { get; }
			public bool IsDoor { get; }
			public bool IsKey { get; }
			public uint Key { get; }
			public bool IsPassableWith(uint keys) => !IsDoor || (Key & keys) != 0;
			public override string ToString() => $"'{ChValue}'";
		}

		private static int ShortestPath(CharMap map)
		{
			var vertices = BuildGraph(map);
			//map.ConsoleWrite(false);

			PruneGraph(vertices);
			//PrintGraph(vertices);
			var sw = Stopwatch.StartNew();
			var distance = FindMinimumDistanceDfs(vertices);
			//Console.WriteLine($"Elapsed1: {sw.Elapsed}");

			//InflateGraph(vertices);
			//var distance = FindMinimumDistanceBfs(vertices);

			return distance;
		}

		private static int FindMinimumDistanceBfs(List<Vertex> vertices)
		{
			const int Infinite = 10000000;
			const uint NoKey = 1U << 31;
			var allKeyMask = ((1U << vertices.Count(v => v.IsKey)) - 1) | NoKey;

			var root = vertices.First();
			var visited = vertices.ToDictionary(x => x.Pos, x => new HashSet<uint>());
			visited[root.Pos].Add(NoKey);

			var queue = new Queue<(Vertex, uint, int)>();
			queue.Enqueue((root, NoKey, 0));
			while (queue.Any())
			{
				var (node, initialKeys, distance) = queue.Dequeue();

				var keys = initialKeys | node.IsKeyKey;
				if (keys == allKeyMask)
				{
					return distance;
				}

				var nodes = node.Edges.Keys
					.Where(n => !visited[n.Pos].Contains(keys) && n.IsPassableWith(keys))
					.ToList();
				foreach (var n in nodes)
				{
					visited[n.Pos].Add(keys);
					queue.Enqueue((n, keys, distance + 1));
				}
			}
			return Infinite;
		}

		private static int FindMinimumDistanceBfs(List<List<Vertex>> graphs)
		{
			const int Infinite = 10000000;
			const uint NoKey = 1U << 31;
			var allKeyMask = ((1U << graphs.Sum(g => g.Count(v => v.IsKey))) - 1) | NoKey;

			var roots = graphs.Select(g => g.First()).ToList();
			var visited = graphs.SelectMany(x => x).ToDictionary(x => x.Pos, x => new HashSet<uint>());
			foreach (var root in roots)
			{
				visited[root.Pos].Add(NoKey);
			}

			var queue = new Queue<(List<Vertex>, uint, int)>();
			queue.Enqueue((roots, NoKey, 0));
			while (queue.Any())
			{
				var (walks, initialKeys, distance) = queue.Dequeue();

				var keys = initialKeys;
				foreach (var w in walks.Where(n => n.IsKey))
				{
					keys |= w.Key;
				}
				//var keys = initialKeys | (uint)(walks.Where(n => n.IsKey).Sum(n => n.Key)); // why no Sum(uint) ?;

				if (keys == allKeyMask)
				{
					return distance;
				}

				foreach (var walk in walks)
				{
					var nodes = walk.Edges.Keys
						.Where(n => !visited[n.Pos].Contains(keys) && n.IsPassableWith(keys))
						.ToList();
					foreach (var n in nodes)
					{
						visited[n.Pos].Add(keys);
						var walks2 = walks.Select(v => v == walk ? n : v).ToList();
						queue.Enqueue((walks2, keys, distance + 1));
					}
				}
			}
			return Infinite;
		}

		private static void PrintGraph(List<Vertex> graph)
		{
			Console.WriteLine("digraph {");
			foreach (var v in graph)
			{
				foreach (var e in v.Edges)
				{
					Console.WriteLine($"  \"{v.Value} {v.Pos}\" -> \"{e.Key.Value} {e.Key.Pos}\" [label=\"{e.Value}\"]");
				}
			}
			Console.WriteLine("}");
		}

		private static void PruneGraph(List<Vertex> vertices)
		{
			while (true)
			{
				var deadend = vertices.FirstOrDefault(v => (v.IsDoor || v.Value == '.') && v.Edges.Count == 1);
				if (deadend == null)
					break;
				vertices.Remove(deadend);
				deadend.Edges.First().Key.Edges.Remove(deadend);
			}

			while (true)
			{
				var node = vertices.FirstOrDefault(v => v.Value == '.');
				if (node == null)
					break;
				vertices.Remove(node);
				foreach (var n1 in node.Edges)
				{
					foreach (var n2 in node.Edges.Where(n => n.Key != n1.Key))
					{
						if (!n1.Key.Edges.ContainsKey(n2.Key))
						{
							n1.Key.Edges.Add(n2.Key, n1.Value + n2.Value);
						}
						else
						{
							n1.Key.Edges[n2.Key] = Math.Min(n1.Key.Edges[n2.Key], n1.Value + n2.Value);
						}
					}
					n1.Key.Edges.Remove(node);
				}
			}
		}

		private static int dummyYpos = 0;
		private static void InflateGraph(List<Vertex> vertices)
		{
			var dummy = Point.From(1000000, dummyYpos++); // oh dear
			while (true)
			{
				var node = vertices.FirstOrDefault(v => v.Edges.Values.Any(d => d > 1));
				if (node == null)
					break;
				foreach (var edge in node.Edges.Where(e => e.Value > 1).ToList())
				{
					node.Edges.Remove(edge.Key);
					edge.Key.Edges.Remove(node);
					var distance = edge.Value;
					var p = edge.Key;
					for (var i = 0; i < distance - 1; i++)
					{
						var insert = new Vertex(dummy, '.');
						vertices.Add(insert);
						insert.Edges.Add(p, 1);
						p.Edges.Add(insert, 1);
						p = insert;
						dummy = dummy.Right;
					}
					node.Edges.Add(p, 1);
					p.Edges.Add(node, 1);
				}
			}
		}


		private static List<List<Vertex>> BuildFourSplitGraph(CharMap map)
		{
			var pos0 = map.AllPoints(ch => ch == '@').First();

			map[pos0] = '#';
			foreach (var p in pos0.LookAround())
			{
				map[p] = '#';
			}
			return pos0.SpiralFrom().Take(9)
				.Where(p => map[p] == '.')
				.Select(p =>
				{
					map[p] = '@';
					var graph = BuildGraph(map);
					PruneGraph(graph);
					map[p] = '.';
					return graph;
				})
				.ToList();
		}

		private static List<Vertex> BuildGraph(CharMap map)
		{
			var walked = new SparseMap<bool>();

			var pos0 = map.AllPoints(ch => ch == '@').First();
			var root = new Vertex(pos0, '@');
			List<Vertex> vertices = new List<Vertex>();
			vertices.Add(root);
			walked[pos0] = true;

			foreach (var p in pos0.LookAround().Where(p => map[p] != '#'))
			{
				BuildGraph(root, p);
			}
			return vertices;

			void BuildGraph(Vertex vertex, Point pos)
			{
				var weight = 1;
				if (walked[pos])
				{
					var node = vertices.FirstOrDefault(v => v != vertex && v.Pos == pos);
					if (node != null)
					{
						vertex.Edges[node] = weight;
						node.Edges[vertex] = weight;
					}
					return;
				}
				while (true)
				{
					var oldwalked = walked[pos];
					walked[pos] = true;
					var routes = pos.LookAround()
						.Where(p => vertices.Any(v => v != vertex && v.Pos == p) || !walked[p] && map[p] != '#')
						.ToList();
					var fork = vertices.FirstOrDefault(v => v.Pos == pos);
					if (fork != null)
					{
						if (vertex.Edges.ContainsKey(fork))
						{
							// Already exists - if this route is longer then just bail
							if (vertex.Edges[fork] <= weight)
							{
								return;
							}
							vertex.Edges[fork] = weight;
							fork.Edges[vertex] = weight;
						}
						else
						{
							vertex.Edges.Add(fork, weight);
							fork.Edges.Add(vertex, weight);
						}
						return;
					}

					if (routes.Count() > 1 || char.IsLetter(map[pos]))
					{
						// On key or door - make a node
						var node = new Vertex(pos, map[pos]);
						vertices.Add(node);
						vertex.Edges.Add(node, weight);
						node.Edges.Add(vertex, weight);
						foreach (var p in routes)
						{
							BuildGraph(node, p);
						}
						return;
					}
					else if (routes.Count() == 0)
					{
						// Dead end - no edge here
						return;
					}

					// Only one way, so move forward
					pos = routes.First();
					weight++;
				}
			}
		}

		private static CharMap ReadMap(string[] lines)
		{
			var map = new CharMap();
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					map[x][y] = line[x];
				}
			}
			return map;
		}

		private static string KeysToString(uint keys) => new string(Enumerable.Range(0, 26).Where(i => (1U << i & keys) != 0).Select(i => Convert.ToChar(97 + i)).ToArray());

		private class DfsMemo : Dictionary<char, int> {}

		public static int FindMinimumDistanceDfs(List<Vertex> vertices)
		{
			const int Infinite = 10000000;
			var allKeyMask = (1U << vertices.Count(v => v.IsKey)) - 1;
			var keyvertices = vertices.Where(v => v.IsKey).ToList();

			var minimumDistanceMemoHits = 0;
			var minimumDistanceHits = 0;
			var allKeysFoundHits = 0;
			var shortestPathToKeysHits = 0;
			//var skipLongerHits = 0;

			//foreach (var kv in keyvertices)
			//{
			//	var d = ShortestPathToNearestKey(kv, allKeyMask);
			//	kv.ShortestPathToNearestKeys = d.ToDictionary(x => x.Item1.Key, x => x.Item2);
			//}
			//shortestPathToKeysHits = 0;

			//var keymemo = new SafeDictionary<uint, DfsMemo>(() => new DfsMemo());
			var memo = new Dictionary<ulong, int>();
			var minDistanceGuess = new Dictionary<ulong, int>();
			var missingKeyBits = new Dictionary<uint, List<uint>>();

			//var distanceDfs = MinimumDistanceDijkstra(vertices.First());//, 0, 0);
			var distanceDfs = MinimumDistance(vertices.First(), 0);

			//Console.WriteLine("Stats:");
			//Console.WriteLine($"minimumDistanceMemoHits= {minimumDistanceMemoHits}");
			//Console.WriteLine($"minimumDistanceHits= {minimumDistanceHits}");
			//Console.WriteLine($"allKeysFoundHits= {allKeysFoundHits}");
			////Console.WriteLine($"skipLongerHits= {skipLongerHits}");
			//Console.WriteLine($"shortestPathToKeysHits= {shortestPathToKeysHits}");

			return distanceDfs;

			int MinimumDistance(Vertex node, uint keys)
			{
				var id = ((ulong)node.Value << 32) + keys;
				if (memo.TryGetValue(id, out var dist))
				{
					minimumDistanceMemoHits++;
					return dist;
				}
				minimumDistanceHits++;

				keys |= node.IsKeyKey;
				if (keys == allKeyMask)
				{
					allKeysFoundHits++;
					memo[id] = 0;
					return 0;
				}

				memo[id] = Infinite;
				foreach (var (n, distance, pathkeys) in ShortestPathToNearestKey(node, keys))
				{
					var remaining = distance + MinimumDistance(n, keys | pathkeys);
					if (remaining < memo[id])
					{
						memo[id] = remaining;
					}
				}
				return memo[id];
			}

			List<(Vertex, int, uint)> ShortestPathToNearestKey(Vertex from, uint initialkeys)
			{
				shortestPathToKeysHits++;

				foreach (var v in vertices)
				{
					v.Distance = Infinite;
					v.Visited = false;
					v.PickedUpKeys = 0;
				}
				from.Distance = 0;

				var queue = new Queue<Vertex>();
				queue.Enqueue(from);
				while (queue.Any())
				{
					var node = queue.Dequeue();
					var keys = node.PickedUpKeys;
					foreach (var (neighbor, distance) in node.Edges)
					{
						if (neighbor.Visited || !neighbor.IsPassableWith(keys | initialkeys))
						{
							continue;
						}
						var newdistance = node.Distance + distance;
						if (newdistance > neighbor.Distance)
						{
							continue;
						}
						neighbor.Distance = newdistance;
						neighbor.PickedUpKeys = keys | neighbor.IsKeyKey;
						queue.Enqueue(neighbor);
						neighbor.Visited = true;
					}
				}

				var reachables = keyvertices
					.Where(v => v.Visited)// && v != from)// && v.Distance < Infinite)// && v.Edges.Any(vv => !vv.Key.Visited))
					//.OrderByDescending(v => v.PickedUpKeys.NumberOfSetBits()).ThenBy(v => v.Distance + v.Pos.ManhattanDistanceTo(from.Pos))
					//.OrderBy(v => v.Distance).ThenByDescending(v => v.PickedUpKeys.NumberOfSetBits())// + v.v.Pos.ManhattanDistanceTo(from.Pos))
					.OrderBy(v => v.Distance)
					//.OrderBy(v => v.Distance / (v.PickedUpKeys.NumberOfSetBits() + 1))
					//.OrderBy(v => v.Distance + v.Pos.ManhattanDistanceTo(from.Pos))
					//.OrderBy(v => v.Distance / (v.PickedUpKeys.NumberOfSetBits() + 1))// + v.v.Pos.ManhattanDistanceTo(from.Pos))
					//.OrderBy(v => v.Distance / (v.PickedUpKeys.NumberOfSetBits() + 1))// + v.v.Pos.ManhattanDistanceTo(from.Pos))
					.Select(v => (v, v.Distance, /*initialkeys|*/v.PickedUpKeys | initialkeys/* + v.ExtraDistance*/))
					.ToList();

				return reachables;
			}
		}

		private static int FindMinimumDistanceDfs4(List<List<Vertex>> graphs)
		{
			const int Infinite = 10000000;
			//const uint NoKey = 1U << 31;
			var allKeyMask = ((1U << graphs.Sum(g => g.Count(v => v.IsKey))) - 1);// | NoKey;

			var roots = graphs.Select(g => g.First()).ToList();
			//var visited = graphs.SelectMany(x => x).ToDictionary(x => x.Pos, x => new HashSet<uint>());
			var visitCount = 0;

			foreach (var root in roots)
			{
				root.DistanceWithKeys[0] = 0;
			}
			var queue = new Queue<(List<Vertex>, uint)>();
			queue.Enqueue((roots, 0));

			var minDistance = Infinite;

			while (queue.Any())
			{
				var (nodes, keys) = queue.Dequeue();
				visitCount++;
				//Console.WriteLine($"Examine {string.Join(" ", nodes.Select(n => n.ToString()))} with keys {KeysToString(keys)}");

				//var initialKeys = keys;
				//foreach (var n in nodes)
				//{
				//	keys |= n.IsKeyKey;

				//	if (n.DistanceWithKeys[initialKeys] < n.DistanceWithKeys[keys])
				//	{
				//		n.DistanceWithKeys[keys] = n.DistanceWithKeys[initialKeys];
				//	}

				//}

				//if (keys == allKeyMask)
				//{
				//	//var distance = nodes.Sum(n => n.DistanceWithKeys[keys]);
				//	var distance = nodes.Sum(n => n.DistanceWithKeys[keys]);
				//	if (distance < minDistance)
				//	{
				//		minDistance = distance;

				//		Console.WriteLine();
				//		foreach (var n in nodes)
				//		{
				//			Console.Write($"Node {n} {n.Value} keys={KeysToString(keys)} distance={n.DistanceWithKeys[keys]}: ");
				//			foreach (var e in n.Edges.Keys)
				//			{
				//				var visited = e.VisitedBy.Contains(keys);
				//				var passable = e.IsPassableWith(keys);
				//				var deadend = visited || !passable;
				//				Console.Write($"{e} {e.Value} vis/pas={visited}/{passable}:{(deadend ? "--" : "OK")}  | ");
				//			}
				//			Console.WriteLine();
				//		}
				//	}
				//}

				//Console.WriteLine();
				//foreach (var n in nodes)
				//{
				//	Console.Write($"Node {n} {n.Value} keys={KeysToString(keys)} distance={n.DistanceWithKeys[keys]}: ");
				//	foreach (var e in n.Edges.Keys)
				//	{
				//		var visited = e.VisitedBy.Contains(keys);
				//		var passable = e.IsPassableWith(keys);
				//		var deadend = visited || !passable;
				//		Console.Write($"{e} {e.Value} vis/pas={visited}/{passable}:{(deadend ? "--" : "OK")}  | ");
				//	}
				//	Console.WriteLine();
				//}


				//if (nodes.Any(n => n.Edges.Keys.All(e =>
				//{
				//	var visited = e.VisitedBy.Contains(keys);
				//	var passable = e.IsPassableWith(keys);
				//	var deadend = visited || !passable;
				//	return deadend;
				//})))
				//	continue;

				foreach (var node in nodes)
				{
					var nodeDistance = node.DistanceWithKeys[keys];
					foreach (var (neighbor, distance) in node.Edges)
					{
						if (neighbor.VisitedBy.Contains(keys) || !neighbor.IsPassableWith(keys))
						{
							continue;
						}
						var newdistance = nodeDistance + distance;
						if (newdistance > neighbor.DistanceWithKeys[keys])
						{
							continue;
						}
						neighbor.DistanceWithKeys[keys] = newdistance;
						var newkeys = keys;

						if (neighbor.IsKey && (keys & neighbor.Key) == 0)
						{
							newkeys |= neighbor.IsKeyKey;
							foreach (var nn in nodes)
							{
								if (nn.DistanceWithKeys[newkeys] > nn.DistanceWithKeys[keys])
								{
									nn.DistanceWithKeys[newkeys] = nn.DistanceWithKeys[keys];
								}
							}
							if (neighbor.DistanceWithKeys[newkeys] > neighbor.DistanceWithKeys[keys])
							{
								neighbor.DistanceWithKeys[newkeys] = neighbor.DistanceWithKeys[keys];
							}

							if (newkeys == allKeyMask)
							{
								//var distance = nodes.Sum(n => n.DistanceWithKeys[keys]);
								var thisDistance = nodes.Sum(n => n.DistanceWithKeys[keys]);
								if (thisDistance < minDistance)
								{
									minDistance = thisDistance;

									//Console.WriteLine();
									//Console.WriteLine($"minDistance={minDistance}");
									//foreach (var n in nodes)
									//{
									//	Console.Write($"Node {n} {n.Value} keys={KeysToString(keys)} distance={n.DistanceWithKeys[keys]}: ");
									//	foreach (var e in n.Edges.Keys)
									//	{
									//		var visited = e.VisitedBy.Contains(keys);
									//		var passable = e.IsPassableWith(keys);
									//		var deadend = visited || !passable;
									//		Console.Write($"{e} {e.Value} vis/pas={visited}/{passable}:{(deadend ? "--" : "OK")}  | ");
									//	}
									//	Console.WriteLine();
									//}
								}
							}


						}

						var newnodes = nodes.Select(n => n == node ? neighbor : n).ToList();

						//neighbor.DistanceWithKeys[newkeys] = newdistance;
						//var newkeys = keys | neighbor.IsKeyKey;
						queue.Enqueue((newnodes, newkeys));

						//Console.Write($"  Explore Node {neighbor} {neighbor.Value} keys={KeysToString(newkeys)} distance={neighbor.DistanceWithKeys[newkeys]}: ");
						//foreach (var e in neighbor.Edges.Keys)
						//{
						//	var visited = e.VisitedBy.Contains(keys);
						//	var passable = e.IsPassableWith(keys);
						//	var deadend = visited || !passable;
						//	Console.Write($"{e} {e.Value} vis/pas={visited}/{passable}:{(deadend ? "--" : "OK")}  | ");
						//}
						//Console.WriteLine();

						neighbor.VisitedBy.Add(keys);
					}
					//if (node.Edges.Keys.All(e => e.IsPassableWith(keys)))
					//{
					//}
				}
			}

			//Console.WriteLine($"Visits: {visitCount}");

			return minDistance;
		}


		//foreach (var kv in keyvertices)
		//{
		//	var d = ShortestPathToNearestKey(kv, allKeyMask);
		//	kv.ShortestPathToNearestKeys = d.ToDictionary(x => x.Item1.Key, x => x.Item2);
		//}
		//.....
		//int MinimumDistanceGuess(Vertex node, uint foundKeys)
		//{
		//	var missingKeys = allKeyMask & ~foundKeys;
		//	if (missingKeys == 0)
		//		return 0;

		//	var id = ((ulong)node.Value << 32) + foundKeys;
		//	if (!minDistanceGuess.TryGetValue(id, out var distance))
		//	{
		//		distance = missingKeys.Bits().Select(key => node.ShortestPathToNearestKeys[key]).Max();
		//		minDistanceGuess[id] = distance;
		//	}
		//	return distance;
		//}


		//int MinimumDistance(Vertex node, uint keys)
		//{
		//	var id = ((ulong)node.Value << 32) + keys;
		//	if (memo.TryGetValue(id, out var dist))
		//	{
		//		minimumDistanceMemoHits++;
		//		return dist;
		//	}
		//	minimumDistanceHits++;

		//	keys |= node.IsKeyKey;
		//	if (keys == allKeyMask)
		//	{
		//		allKeysFoundHits++;
		//		memo[id] = 0;
		//		return 0;
		//	}

		//	memo[id] = Infinite;
		//	var doGuess = keys.NumberOfSetBits() > 22;
		//	foreach (var (n, distance, pathkeys) in ShortestPathToNearestKey(node, keys))
		//	{
		//		var id2 = ((ulong)n.Value << 32) + keys;
		//		if (!memo.TryGetValue(id2, out var dexact))
		//		{
		//			var dguess = 0;
		//			if (doGuess)
		//			{
		//				dguess = MinimumDistanceGuess(n, id2, keys | pathkeys);
		//				if (distance + dguess >= memo[id])
		//				{
		//					skipLongerHits++;
		//					continue;
		//				}
		//			}
		//			dexact = MinimumDistance(n, keys | pathkeys);
		//			if (dguess > dexact)
		//			{
		//				// BUG
		//			}
		//			memo[id2] = dexact;
		//		}

		//		var remaining = distance + dexact;
		//		if (remaining < memo[id])
		//		{
		//			memo[id] = remaining;
		//		}
		//	}
		//	return memo[id];
		//}

		//int MinimumDistanceGuess(Vertex node, ulong id, uint foundKeys)
		//{
		//	var missingKeys = allKeyMask & ~foundKeys;
		//	if (missingKeys == 0)
		//		return 0;

		//	if (!minDistanceGuess.TryGetValue(id, out var distance))
		//	{
		//		if (!missingKeyBits.TryGetValue(missingKeys, out var bits))
		//		{
		//			bits = missingKeys.Bits().ToList();
		//			missingKeyBits[missingKeys] = bits;
		//		}
		//		distance = bits.Select(key => node.ShortestPathToNearestKeys[key]).Max();
		//		minDistanceGuess[id] = distance;
		//	}
		//	return distance;

		//	//var span = keyvertices
		//	//	.Where(v => (v.Key & keys) == 0)
		//	//	.Select(v => (Math.Abs(v.Pos.X - node.Pos.X), Math.Abs(v.Pos.Y - node.Pos.Y)))
		//	//	.ToList();
		//	//return span.Any()
		//	//	? span.Max(s => s.Item1) + span.Max(s => s.Item2)
		//	//	: 0;
		//}



		public static int FindMinimumDistanceDfs(Graph<VaultVertex> graph)
		{
			const int Infinite = 10000000;
			var vertices = graph.Vertices.Values;
			var allKeyMask = (1U << vertices.Count(v => v.Value.IsKey)) - 1;

			var _reachableNodes = new Dictionary<string, List<VaultGraph.Vertex>>();
			var shortestPath = new Dictionary<string, int>();
			var _memo = new Dictionary<string, int>();

			return MinimumDistance(graph.Root, 0);

			int MinimumDistance(VaultGraph.Vertex node, uint initialKeys)
			{
				var id = $"{initialKeys}-{node.Value}";
				if (_memo.TryGetValue(id, out var distance))
				{
					return distance;
				}

				var keys = initialKeys | (node.Value.IsKey ? node.Value.Key : 0);
				if (keys == allKeyMask)
				{
					return 0;
				}

				var remainingDistance = ReachableKeyNodes(node, keys)
					.Select(n =>
					{
						var nextDistance = MinimumDistance(n, keys);
						return nextDistance == Infinite
							? Infinite
							: ShortestPathBetween(node, n, keys) + nextDistance;
					})
					.Append(Infinite)
					.Min();

				_memo[id] = remainingDistance;
				return remainingDistance;
			}

			List<VaultGraph.Vertex> ReachableKeyNodes(VaultGraph.Vertex from, uint keys)
			{
				var id = $"{keys}-{from.Value.ChValue}";
				if (_reachableNodes.TryGetValue(id, out var cachedNodes))
				{
					return cachedNodes;
				}

				foreach (var v in vertices)
				{
					v.Visited = false;
				}

				var nodes = ReachableKeyNodesFrom(from).Where(n => n != from).Distinct().ToList();
				_reachableNodes[id] = nodes;
				return nodes;

				IEnumerable<VaultGraph.Vertex> ReachableKeyNodesFrom(VaultGraph.Vertex node)
				{
					if (!node.Visited)
					{
						node.Visited = true;
						if (node.Value.IsKey && (keys & node.Value.Key) == 0)
						{
							yield return node;
						}
						else
						{
							foreach (var n in node.Edges.Keys.Where(e => !e.Value.IsDoor || (keys & e.Value.Key) != 0))
							{
								foreach (var vn in ReachableKeyNodesFrom(n))
								{
									yield return vn;
								}
							}
						}
					}
				}
			}

			int ShortestPathBetween(VaultGraph.Vertex from, VaultGraph.Vertex goal, uint keys)
			{
				var id1 = $"{keys}:{from.Value}{goal.Value.ChValue}";
				if (shortestPath.TryGetValue(id1, out var cachedDist))
				{
					return cachedDist;
				}

				foreach (var v in vertices)
				{
					v.Distance = int.MaxValue;
					v.Visited = false;
				}
				from.Distance = 0;

				var node = from;
				while (true)
				{
					if (node == null)
						break;
					foreach (var edge in node.Edges)
					{
						var neighbour = edge.Key;
						if (neighbour.Value.IsDoor && (keys & neighbour.Value.Key) == 0)
						{
							continue;
						}
						var weight = edge.Value;
						var dist = node.Distance + weight;
						if (dist < neighbour.Distance)
						{
							neighbour.Distance = dist;
						}
					}
					node.Visited = true;
					if (node == goal)
						break;
					node = vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
				}

				var distance = goal.Distance;
				shortestPath[$"{keys}:{from.Value}{goal.Value.ChValue}"] = distance;
				shortestPath[$"{keys}:{goal.Value}{from.Value.ChValue}"] = distance;
				return distance;
			}
		}



		//}


		//internal class DistanceFinder4
		//{
		//	private readonly uint _allKeys;
		//	private readonly List<List<Vertex>> _graphs;
		//	private Dictionary<string, List<List<Vertex>>> _reachableNodes = new Dictionary<string, List<List<Vertex>>>();
		//	private Dictionary<string, int> shortestPath = new Dictionary<string, int>();

		//	class Walker
		//	{
		//		public Vertex[] Nodes;
		//	};

		//	public DistanceFinder4(List<List<Vertex>> graphs)
		//	{
		//		_graphs = graphs;
		//		_allKeys = (1U << _graphs.Sum(g => g.Count(v => v.IsKey))) - 1;
		//	}

		//	public int FindMinimumDistance()
		//	{
		//		const int Infinite = 10000000;
		//		var _memo = new Dictionary<string, int>();

		//		var roots = new Walker() { Nodes = _graphs.Select(g => g.First()).ToArray() };
		//		return MinimumDistance(roots, 0);

		//		int MinimumDistance(Walker position, uint initialKeys)
		//		{
		//			var id = $"{initialKeys}-{new string(position.Nodes.Select(n => n.Value).ToArray())}";
		//			if (_memo.TryGetValue(id, out var distance))
		//			{
		//				return distance;
		//			}

		//			var keys = initialKeys | (uint)(position.Nodes.Where(n => n.IsKey).Sum(n => n.Key)); // why no Sum(uint) ?
		//			if (keys == _allKeys)
		//			{
		//				return 0;
		//			}

		//			var remainingDistance = ReachableKeyNodes(position, keys)
		//				.Select(n =>
		//				{
		//					var nextDistance = MinimumDistance(n, keys);
		//					return nextDistance == Infinite
		//						? Infinite
		//						: ShortestPathBetween(node, n, keys) + nextDistance;
		//				})
		//				.Append(Infinite)
		//				.Min();

		//			_memo[id] = remainingDistance;
		//			return remainingDistance;
		//		}
		//	}

		//	private List<List<Vertex>> ReachableKeyNodes(Walker walker, uint keys)
		//	{
		//		var id = $"{keys}-{new string(walker.Nodes.Select(n => n.Value).ToArray())}";
		//		if (_reachableNodes.TryGetValue(id, out var cachedNodes))
		//		{
		//			return cachedNodes;
		//		}

		//		foreach (var v in _graphs.SelectMany(x => x))
		//		{
		//			v.Visited = false;
		//		}

		//		var nodes = walker.Nodes.Select(from => ReachableKeyNodes(from).Where(n => n != from).Distinct().ToList()).ToList();
		//		_reachableNodes[id] = nodes;
		//		return nodes;

		//		IEnumerable<Vertex> ReachableKeyNodes(Vertex node)
		//		{
		//			if (!node.Visited)
		//			{
		//				node.Visited = true;
		//				if (node.IsKey && (keys & node.Key) == 0)
		//				{
		//					yield return node;
		//				}
		//				else
		//				{
		//					foreach (var n in node.Edges.Keys.Where(e => !e.IsDoor || (keys & e.Key) != 0))
		//					{
		//						foreach (var vn in ReachableKeyNodes(n))
		//						{
		//							yield return vn;
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	private int ShortestPathBetween(Vertex from, Vertex goal, uint keys)
		//	{
		//		var id1 = $"{keys}:{from.Value}{goal.Value}";
		//		if (shortestPath.TryGetValue(id1, out var cachedDist))
		//		{
		//			return cachedDist;
		//		}

		//		foreach (var v in _vertices)
		//		{
		//			v.Distance = int.MaxValue;
		//			v.Visited = false;
		//		}
		//		from.Distance = 0;

		//		var node = from;
		//		while (true)
		//		{
		//			if (node == null)
		//				break;
		//			foreach (var edge in node.Edges)
		//			{
		//				var neighbour = edge.Key;
		//				if (neighbour.IsDoor && (keys & neighbour.Key) == 0)
		//				{
		//					continue;
		//				}
		//				var weight = edge.Value;
		//				var dist = node.Distance + weight;
		//				if (dist < neighbour.Distance)
		//				{
		//					neighbour.Distance = dist;
		//				}
		//			}
		//			node.Visited = true;
		//			if (node == goal)
		//				break;
		//			node = _vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
		//		}

		//		var distance = goal.Distance;
		//		shortestPath[$"{keys}:{from.Value}{goal.Value}"] = distance;
		//		shortestPath[$"{keys}:{goal.Value}{from.Value}"] = distance;
		//		return distance;
		//	}
		//}

		//private void OldPruneGraph(List<Vertex> vertices)
		//{
		//	while (true)
		//	{
		//		var deadend = vertices.FirstOrDefault(v => (v.IsDoor || v.Value == '.') && v.Edges.Count == 1);
		//		if (deadend == null)
		//			break;
		//		vertices.Remove(deadend);
		//		deadend.Edges.First().Key.Edges.Remove(deadend);
		//	}

		//	while (true)
		//	{
		//		var node = vertices.FirstOrDefault(v => v.Value == '.' && v.Edges.Count == 2/* && v.Edges.Any(e => e.Key.Value == '.')*/);
		//		if (node == null)
		//			break;
		//		vertices.Remove(node);
		//		var n1 = node.Edges.First();
		//		var n2 = node.Edges.Last();
		//		n1.Key.Edges.Add(n2.Key, n1.Value + n2.Value);
		//		n2.Key.Edges.Add(n1.Key, n1.Value + n2.Value);
		//		n1.Key.Edges.Remove(node);
		//		n2.Key.Edges.Remove(node);
		//	}

		//	while (true)
		//	{
		//		var node = vertices.FirstOrDefault(v => v.IsDoor && v.Edges.Count == 1);
		//		if (node == null)
		//			break;
		//		vertices.Remove(node);
		//		node.Edges.First().Key.Edges.Remove(node);
		//	}

		//}

		//private Vertex FindDoorDependencies(List<Vertex> vertices)
		//{
		//	foreach (var v in vertices)
		//	{
		//		v.Visited = false;
		//	}

		//	var deps = new List<Vertex>();
		//	var root = new Vertex(null, '@');
		//	deps.Add(root);
		//	AddDoorDependencies(root, vertices.First());

		//	Console.WriteLine();
		//	foreach (var v in deps)
		//	{
		//		foreach (var e in v.Edges)
		//		{
		//			Console.WriteLine($"\"{v.Value} {v.Pos}\" -> \"{e.Key.Value} {e.Key.Pos}\" [label=\"{e.Value}\"]");
		//		}
		//	}

		//	foreach (var v in deps)
		//	{
		//		v.Visited = false;
		//	}
		//	CollapseDoors(root);
		//	Console.WriteLine();
		//	foreach (var v in deps)
		//	{
		//		foreach (var e in v.Edges)
		//		{
		//			Console.WriteLine($"\"{v.Value} {v.Pos}\" -> \"{e.Key.Value} {e.Key.Pos}\" [label=\"{e.Value}\"]");
		//		}
		//	}
		//	return root;

		//	void CollapseDoors(Vertex dep)
		//	{
		//		if (dep.Visited)
		//		{
		//			return;
		//		}
		//		dep.Visited = true;
		//		var subs = dep.Edges.Keys.Where(n => !n.Visited).ToList();
		//		foreach (var n in subs)
		//		{
		//			if (n.IsDoor)
		//			{
		//				var keynode = deps.First(x => x.IsKey && x.Key == n.Key);
		//				foreach (var sub in n.Edges.Keys.Where(x => !x.Visited).ToList())
		//				{
		//					n.Edges.Remove(sub);
		//					sub.Edges.Remove(n);
		//					if (!keynode.Edges.Keys.Contains(sub))
		//					{
		//						keynode.Edges.Add(sub, 0);
		//						sub.Edges.Add(keynode, 0);
		//					}
		//				}
		//			}
		//			CollapseDoors(n);
		//		}
		//	}

		//	void AddDoorDependencies(Vertex dep, Vertex node)
		//	{
		//		if (node.Visited)
		//		{
		//			return;
		//		}
		//		node.Visited = true;
		//		if (node.IsDoor || node.IsKey)
		//		{
		//			var n = new Vertex(null, node.Value);
		//			dep.Edges.Add(n, 0);
		//			n.Edges.Add(dep, 0);
		//			deps.Add(n);
		//			dep = n;
		//		}
		//		foreach (var n in node.Edges.Keys)
		//		{
		//			AddDoorDependencies(dep, n);
		//		}
		//	}
		//}

		//private List<Vertex> BuildBfsGraph(CharMap map)
		//{
		//	var walked = new SparseMap<Vertex>();

		//	var pos0 = map.AllPoints(ch => ch == '@').First();
		//	List<Vertex> vertices = new List<Vertex>();
		//	BuildGraph(pos0);
		//	return vertices;

		//	void BuildGraph(Point pos)
		//	{
		//		var node = new Vertex(pos, map[pos]);
		//		walked[pos] = node;
		//		vertices.Add(node);
		//		foreach (var p in pos.LookAround().Where(p => map[p] != '#'))
		//		{
		//			var existing = walked[p];
		//			if (existing != null)
		//			{
		//				node.Edges[existing] = 1;
		//				existing.Edges[node] = 1;
		//			}
		//			else
		//			{
		//				BuildGraph(p);
		//			}
		//		}
		//	}
		//}



		//class DistanceFinder
		//{
		//	//private int MinDistance;
		//	////private int NumberOfKeys;
		//	private readonly uint AllKeys;
		//	private readonly List<Vertex> _vertices;
		//	private readonly Dictionary<uint, Vertex> _keys;
		//	private readonly Dictionary<Vertex, Dictionary<Vertex, int>> _distances = new Dictionary<Vertex, Dictionary<Vertex, int>>();

		//	public DistanceFinder(List<Vertex> vertices)
		//	{
		//		_vertices = vertices;

		//		AllKeys = (1U << _vertices.Count(v => v.IsKey)) - 1;
		//		var keys = _vertices.Where(v => v.IsKey).ToList();
		//		foreach (var k1 in keys)
		//		{
		//			_distances[k1] = new Dictionary<Vertex, int>();
		//			foreach (var k2 in keys)
		//			{
		//				// FindShortestPath will return memoed result, if any
		//				_distances[k1][k2] = ShortestPathBetween(k1, k2, 0xffffffff);
		//			}
		//		}

		//		_keys = keys.ToDictionary(x => x.KeyBit, x => x);
		//	}

		//	//public int FindMinDistance(uint keys = 0)
		//	//{
		//	//	var sw = Stopwatch.StartNew();
		//	//	MinDistance = int.MaxValue;
		//	//	//NumberOfKeys = vertices.Count(v => v.IsKey);
		//	//	//_distanceCache.Clear();
		//	//	//_visibleNodesCache.Clear();

		//	//	//var root = FindDoorDependencies(vertices);

		//	//	//FindMinDistanceDfs(_vertices.First(), keys, 0);
		//	//	//FindMinDistanceBfs(_vertices.First(), keys, 0);
		//	//	//ShortestPathPlainDijkstra();
		//	//	//MinDistance = FindMinDistanceDfs2(_vertices.First(), keys);
		//	//	MinDistance = FindMinDistanceDfs4(_vertices.First(), keys);
		//	//	//MinDistance = dist;
		//	//	//FindMinDistanceBfs3(_vertices.First());

		//	//	Console.WriteLine($"[dfs={StatMinDistanceDfs},bfs={StatMinDistanceBfs},shortest={StatFindShortestPath},unvisited={StatVisibleUnvisitedKeyNodes} elapsed={sw.Elapsed}]");

		//	//	return MinDistance;
		//	//}

		//	//public int FindDistanceByOrder(string order) // maybe useful
		//	//{
		//	//	var ordering = order.ToCharArray();
		//	//	var dist = 0;
		//	//	for (var i = 0; i < ordering.Length - 1; i++)
		//	//	{
		//	//		var from = _vertices.FirstOrDefault(v => v.IsKey && v.Key == ordering[i]);
		//	//		var to = _vertices.FirstOrDefault(v => v.IsKey && v.Key == ordering[i+1]);
		//	//		if (from == null || to == null)
		//	//			continue;
		//	//		dist += ShortestPathBetween(from, to, 0xffffffff);
		//	//	}
		//	//	return dist;
		//	//}

		//	////private void FindMinDistance(Vertex from, List<Vertex> vertices, string keys, int distance)
		//	//private void FindMinDistanceDfs(Vertex from, uint keys, int distance)
		//	//{
		//	//	//var id = $"{keys}-{from.Value}";
		//	//	//if (_minDistanceDfsCache.ContainsKey(id))
		//	//	//	return;

		//	//	////Console.WriteLine(keys);
		//	//	StatMinDistanceDfs++;

		//	//	//if (keys.Length == NumberOfKeys)
		//	//	if (keys == AllKeys)
		//	//	{
		//	//		if (distance < MinDistance)
		//	//		{
		//	//			MinDistance = distance;
		//	//			Console.Write($"[{MinDistance}]");
		//	//		}
		//	//		return;
		//	//	}
		//	//	var keydistances = 0;// MinRemainingKeyDistances(keys);
		//	//	var nodes = ReachableKeyNodes(from, keys)
		//	//		.Select(n =>
		//	//		{
		//	//			//var newKeys = new string(keys.ToCharArray().Append(n.Value).OrderBy(c => c).ToArray());
		//	//			var newKeys = keys | n.KeyBit;
		//	//			var dist = distance + ShortestPathBetween(from, n, newKeys);
		//	//			return dist + keydistances < MinDistance
		//	//				? new { Node = n, Dist = dist, Keys = newKeys }
		//	//				: null;
		//	//		})
		//	//		.Where(x => x != null)
		//	//		.OrderBy(x => x.Dist)
		//	//		//.OrderByDescending(x => x.Dist)
		//	//		.ToList();
		//	//	//	.OrderByDescending(x => x.Dist);

		//	//	////if (nodes.Count > 6)
		//	//	////{
		//	//	////	foreach (var n in nodes)
		//	//	////	{
		//	//	////		//var newKeys = keys + n.Value;
		//	//	////		FindMinDistanceBfs(n.Node, n.Keys, n.Dist);
		//	//	////	}
		//	//	////}
		//	//	////else
		//	//	{
		//	//		foreach (var n in nodes)
		//	//		{
		//	//			//var newKeys = keys + n.Value;
		//	//			FindMinDistanceDfs(n.Node, n.Keys, n.Dist);
		//	//		}

		//	//	}
		//	//	//_minDistanceDfsCache[id] = true;
		//	//}


		//	//private HashSet<string> _minDistanceDfs3Cache = new HashSet<string>();

		//	//private void FindMinDistanceDfs3(Vertex from, uint keys, int distance)
		//	//{
		//	//	var id = $"{keys}-{from.Value}";
		//	//	if (_minDistanceDfs3Cache.Contains(id))
		//	//		return;

		//	//	if (keys == AllKeys)
		//	//	{
		//	//		if (distance < MinDistance)
		//	//		{
		//	//			MinDistance = distance;
		//	//		}
		//	//		_minDistanceDfs3Cache.Add(id);
		//	//		return;
		//	//	}

		//	//	var newKeys = keys | (from.IsKey ? from.KeyBit : 0);
		//	//	foreach (var n in ReachableKeyNodes(from, newKeys))
		//	//	{
		//	//		//var newKeys = new string(keys.ToCharArray().Append(n.Value).OrderBy(c => c).ToArray());
		//	//		var d1 = ShortestPathBetween(from, n, newKeys);
		//	//		FindMinDistanceDfs3(n, newKeys, distance + d1);
		//	//	}

		//	//	_minDistanceDfs3Cache.Add(id);
		//	//}


		//	//private Dictionary<string, int> _minDistanceDfs2Cache = new Dictionary<string, int>();
		//	//private int FindMinDistanceDfs2(Vertex from, uint keys)
		//	//{
		//	//	var id = $"{keys}-{from.Value}";
		//	//	if (_minDistanceDfs2Cache.TryGetValue(id, out var distance))
		//	//		return distance;

		//	//	////Console.WriteLine(keys);
		//	//	StatMinDistanceDfs++;

		//	//	//if (keys.Length == NumberOfKeys)
		//	//	var newKeys = keys | (from.IsKey ? from.KeyBit : 0);
		//	//	if (newKeys == AllKeys)
		//	//	{
		//	//		return 0;
		//	//	}

		//	//	var remainingDistance = 10000000;
		//	//	foreach (var n in ReachableKeyNodes(from, newKeys))
		//	//	{
		//	//		//var newKeys = new string(keys.ToCharArray().Append(n.Value).OrderBy(c => c).ToArray());
		//	//		var d1 = ShortestPathBetween(from, n, newKeys);
		//	//		var d2 = FindMinDistanceDfs2(n, newKeys);
		//	//		var dist = d1 + d2;
		//	//		if (dist < remainingDistance)
		//	//		{
		//	//			remainingDistance = dist;
		//	//		}
		//	//	}

		//	//	_minDistanceDfs2Cache[id] = remainingDistance;
		//	//	return remainingDistance;
		//	//}

		//	public int FindMinimumDistance()
		//	{
		//		const int Infinite = 10000000;
		//		var _memo = new Dictionary<string, int>();

		//		return MinimumDistance(_vertices.First(), 0);

		//		int MinimumDistance(Vertex node, uint initialKeys)
		//		{
		//			var id = $"{initialKeys}-{node.Value}";
		//			if (_memo.TryGetValue(id, out var distance))
		//			{
		//				return distance;
		//			}

		//			var keys = initialKeys | (node.IsKey ? node.KeyBit : 0);
		//			if (keys == AllKeys)
		//			{
		//				return 0;
		//			}

		//			var remainingDistance = ReachableKeyNodes(node, keys)
		//				.Select(n =>
		//				{
		//					var nextDistance = MinimumDistance(n, keys);
		//					return nextDistance == Infinite
		//						? Infinite
		//						: ShortestPathBetween(node, n, keys) + nextDistance;
		//				})
		//				.Append(Infinite)
		//				.Min();

		//			_memo[id] = remainingDistance;
		//			return remainingDistance;
		//		}
		//	}




		//	//private Dictionary<uint, int> _minRemainingKeyDistances = new Dictionary<uint, int>();
		//	//private int MinRemainingKeyDistances(uint keys)
		//	//{
		//	//	if (MinDistance == int.MaxValue)
		//	//		return 0;

		//	//	if (_minRemainingKeyDistances.TryGetValue(keys, out var distance))
		//	//	{
		//	//		//Console.Write("£");
		//	//		return distance;
		//	//	}

		//	//	// we've found a minimum
		//	//	var keydistances = 0;
		//	//	var left = _keys.Where(v => (v.Key & keys) == 0).Select(x => x.Value).ToArray();

		//	//	//if (left.Length > 8)
		//	//	//	return 0;
		//	//	if (left.Length < 2)
		//	//		return 0;

		//	//	if (left.Length  < 6)
		//	//	{
		//	//		keydistances = int.MaxValue;
		//	//		foreach (var perm in MathHelper.AllPermutations(left.Length))
		//	//		{
		//	//			var order = perm.ToArray();
		//	//			var dist = 0;
		//	//			for (var i = 0; i < order.Length - 1; i++)
		//	//			{
		//	//				dist += _distances[left[i]][left[i + 1]];
		//	//			}
		//	//			if (dist < keydistances)
		//	//			{
		//	//				keydistances = dist;
		//	//			}
		//	//		}
		//	//	}
		//	//	else
		//	//	{
		//	//		keydistances = left.Sum(node => left.Where(v => v != node).Min(v => _distances[node][v]));
		//	//	}

		//	//	//foreach (var perm in MathHelper.AllPermutations(left.Count))
		//	//	//{
		//	//	//	var order = perm.ToArray();
		//	//	//	var dist = 0;
		//	//	//	for (var i = 0; i < order.Length - 1; i++)
		//	//	//	{
		//	//	//		dist += _distances[left[i]][left[i + 1]];
		//	//	//	}
		//	//	//	if (dist < keydistances)
		//	//	//	{
		//	//	//		keydistances = dist;
		//	//	//	}
		//	//	//}

		//	//	_minRemainingKeyDistances[keys] = keydistances;
		//	//	return keydistances;
		//	//}
		//	//internal class NodeBfsItem
		//	//{
		//	//	public Vertex Node;
		//	//	public uint Keys;
		//	//	public int Distance;
		//	//	public string Visits;
		//	//}
		//	//private void FindMinDistanceBfs(Vertex from, uint keys, int distance)
		//	//{
		//	//	var queue = new Queue<NodeBfsItem>();
		//	//	queue.Enqueue(new NodeBfsItem() { Node = from, Keys = keys, Distance = distance, Visits = "" });
		//	//	while (queue.Any())
		//	//	{
		//	//		StatMinDistanceBfs++;
		//	//		var item = queue.Dequeue();
		//	//		if (item.Distance >= MinDistance)
		//	//		{
		//	//			//Console.Write($"[{MinDistance} at {item.Visits + n.Node.Value.ToString()}]");
		//	//			continue;
		//	//		}

		//	//		var keydistances = MinRemainingKeyDistances(item.Keys);
		//	//		var nodes = ReachableKeyNodes(item.Node, item.Keys)
		//	//			.Select(n =>
		//	//			{
		//	//			//var newKeys = new string(keys.ToCharArray().Append(n.Value).OrderBy(c => c).ToArray());
		//	//			var newKeys = item.Keys | n.KeyBit;
		//	//				var dist = item.Distance + ShortestPathBetween(item.Node, n, newKeys);
		//	//				return dist + keydistances < MinDistance
		//	//					? new { Node = n, Dist = dist, Keys = newKeys }
		//	//					: null;
		//	//			})
		//	//			.Where(x => x != null)
		//	//			.OrderBy(x => x.Dist)
		//	//			.ToArray();
		//	//		foreach (var n in nodes)
		//	//		{
		//	//			//Console.Write($"({item.Visits}: {n.Node.Value} d{n.Dist}) ");
		//	//			if (n.Keys == AllKeys)
		//	//			{
		//	//				if (n.Dist < MinDistance)
		//	//				{
		//	//					MinDistance = n.Dist;
		//	//					Console.Write($"[{MinDistance} at {item.Visits + n.Node.Value.ToString()}]");
		//	//					continue;
		//	//				}
		//	//			}
		//	//			if (item.Visits.Length > 16 || queue.Count() > 10000)
		//	//				FindMinDistanceDfs(n.Node, n.Keys, n.Dist);
		//	//			else
		//	//				queue.Enqueue(new NodeBfsItem() { Node = n.Node, Keys = n.Keys, Distance = n.Dist, Visits = item.Visits + n.Node.Value.ToString() });
		//	//		}
		//	//	}
		//	//}

		//	private string KeysToString(uint keys) => new string(Enumerable.Range(0, 26).Where(i => (1U << i & keys) != 0).Select(i => Convert.ToChar(97 + i)).ToArray());

		//	private Dictionary<string, List<Vertex>> _visibleNodesCache = new Dictionary<string, List<Vertex>>();

		//	//private List<Vertex> VisibleUnvisitedKeyNodes(Vertex from, List<Vertex> vertices, string keys)
		//	private List<Vertex> ReachableKeyNodes(Vertex from, uint keys)
		//	{
		//		var id = $"{keys}-{from.Value}";
		//		if (_visibleNodesCache.TryGetValue(id, out var cachedNodes))
		//		{
		//			//Console.Write("£");
		//			return cachedNodes;
		//		}
		//		//Console.Write(",");
		//		//StatVisibleUnvisitedKeyNodes++;

		//		foreach (var v in _vertices)
		//		{
		//			v.Visited = false;
		//		}

		//		var nodes = VisibleKeyNodes(from).Where(n => n != from).Distinct().ToList();
		//		_visibleNodesCache[id] = nodes;
		//		return nodes;

		//		IEnumerable<Vertex> VisibleKeyNodes(Vertex node)
		//		{
		//			if (!node.Visited)
		//			{
		//				node.Visited = true;
		//				//if (node.IsKey && !keys.Contains(node.Value))
		//				if (node.IsKey && (keys & node.KeyBit) == 0)
		//				{
		//					yield return node;
		//				}
		//				else
		//				{
		//					foreach (var n in node.Edges.Keys.Where(e => !e.IsDoor || (keys & e.KeyBit) != 0))
		//					{
		//						foreach (var vn in VisibleKeyNodes(n))
		//						{
		//							yield return vn;
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	private Dictionary<string, int> _distanceCache = new Dictionary<string, int>();

		//	//private int FindShortestPath(List<Vertex> vertices, Vertex from, Vertex goal, string keys)
		//	private int ShortestPathBetween(Vertex from, Vertex goal, uint keys)
		//	{
		//		var id1 = $"{keys}:{from.Value}{goal.Value}";
		//		if (_distanceCache.TryGetValue(id1, out var cachedDist))
		//		{
		//			//Console.Write("$");
		//			return cachedDist;
		//		}
		//		//Console.Write(".");
		//		//StatFindShortestPath++;

		//		foreach (var v in _vertices)
		//		{
		//			v.Distance = int.MaxValue;
		//			v.Visited = false;
		//		}
		//		from.Distance = 0;

		//		var node = from;
		//		while (true)
		//		{
		//			if (node == null)
		//				break;
		//			foreach (var edge in node.Edges)
		//			{
		//				var neighbour = edge.Key;
		//				//if (neighbour.IsDoor && !keys.Contains(neighbour.NeededKey))
		//				if (neighbour.IsDoor && (keys & neighbour.KeyBit) == 0)
		//				{
		//					continue;
		//				}
		//				var weight = edge.Value;
		//				var dist = node.Distance + weight;
		//				if (dist < neighbour.Distance)
		//				{
		//					neighbour.Distance = dist;
		//				}
		//			}
		//			node.Visited = true;
		//			if (node == goal)
		//				break;
		//			node = _vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
		//		}

		//		var distance = goal.Distance;
		//		_distanceCache[$"{keys}:{from.Value}{goal.Value}"] = distance;
		//		_distanceCache[$"{keys}:{goal.Value}{from.Value}"] = distance;
		//		return distance;
		//	}

		//	//private int ShortestPathPlainDijkstra()
		//	//{
		//	//	foreach (var v in _vertices)
		//	//	{
		//	//		//v.Distance = int.MaxValue;
		//	//		//v.Distances = new Dictionary<uint, int>();
		//	//	}
		//	//	var node = _vertices.First();
		//	//	var keys = 0u;
		//	//	//node.KeyInfo[keys] = new KeyInfo { TheDistance = 0, Edges = VisibleUnvisitedKeyNodes(node, keys).ToDictionary(x => x, x => int.MaxValue) };
		//	//	//node.Distance = 0;
		//	//	MinDistance = int.MaxValue;
		//	//	ShortestPathPlainDijkstra(node, keys);
		//	//	return MinDistance;
		//	//}

		//	//	private void ShortestPathPlainDijkstra(Vertex v, uint keys)
		//	//	{
		//	//		var newkeys = keys | (v.IsKey ? v.KeyBit : 0);

		//	//		if (!v.KeyInfo.ContainsKey(newkeys))
		//	//		{
		//	//			v.KeyInfo[keys] = new KeyInfo
		//	//			{
		//	//				TheDistance = int.MaxValue,
		//	//				Edges = VisibleUnvisitedKeyNodes(v, keys).ToDictionary(x => x, x => FindShortestPath(v, x, newkeys))
		//	//			};
		//	//		}
		//	//		var node = v.KeyInfo[keys];

		//	//		if (node.Visited)
		//	//		{
		//	//			return;
		//	//		}
		//	//		node.Visited = true;

		//	//		if (node.TheDistance >= MinDistance)
		//	//		{
		//	//			return;
		//	//		}


		//	//		if (newkeys == AllKeys)
		//	//		{
		//	//			MinDistance = node.TheDistance;
		//	//			Console.Write($"[{MinDistance}]");
		//	//			return;
		//	//		}

		//	//		foreach (var edge in node.Edges)
		//	//		{
		//	//			var neighbour = edge.Key;
		//	//			var weight = edge.Value;
		//	//			var dist = node.TheDistance + weight;
		//	//			if (!neighbour.KeyInfo.ContainsKey(keys))
		//	//			{
		//	//				neighbour.KeyInfo[keys] = new KeyInfo
		//	//				{
		//	//					TheDistance = int.MaxValue,
		//	//					Edges = VisibleUnvisitedKeyNodes(v, newkeys).ToDictionary(x => x, x => FindShortestPath(v, x, newkeys))
		//	//				};
		//	//			}
		//	//			if (dist < neighbour.KeyInfo[keys].TheDistance)
		//	//			{
		//	//				neighbour.KeyInfo[keys].TheDistance = dist;
		//	//			}
		//	//		}
		//	//		node.Visited = true;
		//	//		foreach (var n in node.Edges.Keys.Where(x => !x.KeyInfo[keys].Visited).OrderBy(x => x.KeyInfo[keys].TheDistance).ToList())
		//	//		{
		//	//			ShortestPathPlainDijkstra(n, newkeys);
		//	//		}
		//	//	}


		//}

		//internal class DepNode
		//{
		//	public char Key { get; set; }
		//	public uint KeyBit { get; set; }
		//	public bool IsDoor { get; set; }
		//	public List<DepNode> Dependents { get; set; } = new List<DepNode>();

		//	public void Print()
		//	{
		//		foreach (var dep in Dependents)
		//		{
		//			Console.WriteLine($"\"{Key}\" -> \"{dep.Key}\" [label=\"{(IsDoor ? "door" : "key")}\"]");
		//			dep.Print();
		//		}
		//	}
		//}

		//DepNode FindDependencies(List<Vertex> graph)
		//{
		//	foreach (var v in graph)
		//	{
		//		v.Visited = false;
		//	}

		//	var alldeps = new List<DepNode>();
		//	var handledkeys = 0U;

		//	alldeps.AddRange(graph.Where(x => x.IsKey).Select(x => new DepNode { Key = x.Key, KeyBit = x.KeyBit }));

		//	FindDoorDependencies(null, graph.First());
		//	var deproot = new DepNode() { Key = '@', KeyBit = 0 };
		//	var unhandled = alldeps.Where(d => (d.KeyBit & handledkeys) == 0).ToList();
		//	deproot.Dependents.AddRange(unhandled);
		//	return deproot;

		//	void FindDoorDependencies(DepNode dep, Vertex node)
		//	{
		//		if (node.Visited)
		//			return;
		//		node.Visited = true;
		//		if (node.IsKey || node.IsDoor)
		//		{
		//			//var key = alldeps.First(d => d.KeyBit == node.KeyBit);

		//			var key = alldeps.FirstOrDefault(d => d.KeyBit == node.KeyBit);
		//			if (key == null)
		//			{
		//				// For foursquare puzzle
		//				key = new DepNode { Key = node.Key, KeyBit = node.KeyBit };
		//				alldeps.Add(key);
		//			}

		//			if (dep != key && (!dep?.Dependents.Contains(key) ?? false))
		//			{
		//				dep.Dependents.Add(key);
		//				handledkeys |= key.KeyBit;
		//			}
		//			dep = key;
		//		}
		//		foreach (var n in node.Edges.Keys)
		//		{
		//			FindDoorDependencies(dep, n);
		//		}
		//	}
		//}

		//		internal class Vertex
		//		{
		//			public Vertex(Point pos, string keys, bool isFork) { Pos = pos; Keys = keys; IsFork = isFork; }
		//			public Point Pos { get; private set; }
		//			public string Keys { get; private set; }
		//			public bool IsFork { get; private set; }
		//			public override int GetHashCode() => (Pos.GetHashCode() * 397) ^ Keys.GetHashCode();
		//			public bool Visited { get; set; }
		//			public int Distance { get; set; }
		//			public Dictionary<Vertex, int> Edges { get; set; } = new Dictionary<Vertex, int>();
		//		}

		//		private int FindShortestPath(List<Vertex> vertices, Vertex root, Vertex goal)
		//		{
		//			foreach (var v in vertices)
		//			{
		//				v.Distance = int.MaxValue;
		//				v.Visited = false;
		//			}
		//			root.Distance = 0;

		//			var node = root;
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
		//				if (node == goal)
		//					break;
		//				node = vertices.Where(v => !v.Visited).OrderBy(x => x.Distance).FirstOrDefault();
		//			}

		//			return goal.Distance;
		//		}

		//		private int ShortestPath(CharMap map)
		//		{
		//			var allkeys = new string(map.AllPoints(char.IsLower).Select(p => map[p]).OrderBy(c => c).ToArray());
		//			var numberOfKeys = allkeys.Length;
		//			var pos0 = map.AllPoints(ch => ch == '@').First();
		//			var posN = map.AllPoints(ch => ch == allkeys.Last()).First();
		//			map[pos0] = '.'; // a bit bad

		//			var walked = new SparseMap<HashSet<string>>();
		//			List<Vertex> vertices = new List<Vertex>();

		//			var root = new Vertex(pos0, "", false);
		//			var goal = new Vertex(posN, allkeys, false); // todo otherwise
		//			vertices.Add(root);
		//			vertices.Add(goal);

		//			walked[pos0] = new HashSet<string>{ "" };

		//			var step = 0;
		//			var MinimumStep = int.MaxValue;

		//			Silent = true;

		//			//walked[maze.Entry] = true;
		//			foreach (var p in pos0.LookAround().Where(p => map[p] != '#'))
		//			{
		//				BuildGraph(root, p, "", 0);
		//			}

		//			//BuildGraph(graph.Vertices.First(), maze.Entry);

		//			//maze.Render(null, p =>
		//			//{
		//			//	return
		//			//		graph.Vertices.Any(v => v.Pos.Is(p)) ? 'X' :
		//			//		walked[p] ? 'o' :
		//			//		(char?)null;
		//			//});
		//			//graph.Render();
		//			////Console.ReadKey();

		//			map.ConsoleWrite(false);
		//			Console.WriteLine();
		//			foreach (var v in vertices)
		//			{
		//				foreach (var e in v.Edges)
		//				{
		//					Console.WriteLine($"\"{v.Keys}{v.Pos}\" -> \"{e.Key.Keys}{e.Key.Pos}\" [label=\"{e.Value}:{e.Key.Keys}\"]");
		//				}
		//			}
		//			return FindShortestPath(vertices, root, goal);


		//			void BuildGraph(Vertex vertex, Point pos, string keys, int steps)
		//			{
		//				if (walked[pos]?.Contains(keys) ?? false)
		//				{
		//					return;
		//				}

		//				var weight = 1;
		////				Point prevpos = null;
		//				while (true)
		//				{
		//					Console.Write(".");
		//					if (steps+weight >= MinimumStep)
		//					{
		//						// Will never be better
		//						return;
		//					}

		//					if (walked[pos] == null)
		//					{
		//						walked[pos] = new HashSet<string>();
		//					}
		//					walked[pos].Add(keys);

		//					var routes = pos.LookAround()
		//						.Where(p => map[p] != '#')
		//						//.Where(p => vertices.Any(v => v != vertex && v.Pos.Is(p)) || map[p] != '#' && !(walked[p]?.Contains(keys) ?? false))
		//						//.Where(p => vertices.Any(v => v != vertex && !v.Pos.Is(prevpos) && v.Pos.Is(p) && v.Keys == keys) || walked[p] == null || !walked[p].Contains(keys))
		//						.Where(p =>
		//							(!char.IsUpper(map[p]) || keys.Contains(char.ToLowerInvariant(map[p]))) &&
		//							(vertices.Any(v => v != vertex && v.IsFork && v.Pos.Is(p) && v.Keys == keys) || walked[p] == null || !walked[p].Contains(keys))
		//						)
		//						.ToList();

		//					//if (char.IsUpper(map[pos]))
		//					//{
		//					//	// Can't pass this door
		//					//	if (!keys.Contains(char.ToLowerInvariant(map[pos])))
		//					//	{
		//					//		return;
		//					//	}
		//					//}



		//					////Console.Clear();
		//					//if (step == 298)
		//					//{
		//					//	Silent = false;
		//					//}

		//					WriteLine();
		//					var lines = map.Render((p, ch) =>
		//					{
		//						return
		//							pos.Is(p) ? '@' :
		//							//vertices.Any(v => v.Pos.Is(p)) ? 'X' :
		//							walked[p] != null && walked[p].Contains(keys) ? 'o' :
		//							ch;
		//					});
		//					foreach (var line in lines)
		//					{
		//						WriteLine(line);
		//					}
		//					WriteLine($"Step: {step++}");
		//					WriteLine($"Steps: {steps} Min:{MinimumStep}");
		//					WriteLine($"Pos: {pos}");
		//					WriteLine($"Keys: {keys}");
		//					WriteLine($"Routes: {string.Join(" ", routes)}");
		//					WriteLine($"Vertices:{vertices.Count}");
		//					foreach (var v in vertices)
		//					{
		//						foreach (var e in v.Edges)
		//						{
		//							WriteLine($"\"{v.Keys}{v.Pos}\" -> \"{e.Key.Keys}{e.Key.Pos}\" [label=\"{e.Value}:{e.Key.Keys}\"]");
		//						}
		//					}
		//					//ReadKey();


		//					var fork = vertices.FirstOrDefault(v => v != root && v.Pos.Is(pos) && v.Keys == keys);

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

		//							////DAG  fork.Edges[vertex] = weight;


		//						}
		//						else
		//						{
		//							vertex.Edges.Add(fork, weight);

		//							////DAG  fork.Edges.Add(vertex, weight);
		//						}

		//						////Console.ReadKey();
		//						return;
		//					}

		//					////Console.ReadKey();
		//					if (char.IsLower(map[pos])) // found key
		//					{
		//						if (!keys.Contains(map[pos])) // key not found; explore from here as for forks
		//						{

		//							// Add char to keys
		//							//keys = new string(keys.ToCharArray().Append(map[pos]).OrderBy(c => c).ToArray());
		//							keys += map[pos];

		//							walked[pos].Add(keys);

		//							if (keys.Length == numberOfKeys)
		//							{
		//								vertex.Edges.Add(goal, weight);

		//								var shortest = steps + weight;// FindShortestPath(vertices, root, goal);
		//								if (shortest < MinimumStep)
		//								{
		//									MinimumStep = shortest;
		//								}

		//								// goal.Edges.Add(vertex, weight);
		//								return;
		//							}


		//							var fork2 = new Vertex(pos, keys, false);
		//							vertices.Add(fork2);

		//							//var edge = new Edge(vertex, fork, weight);
		//							vertex.Edges.Add(fork2, weight);

		//							/////DAG  fork2.Edges.Add(vertex, weight);

		//							//BuildGraph(fork2, prevpos ?? vertex.Pos, keys); // go back


		//							routes = pos.LookAround()
		//								.Where(p => map[p] != '#')
		//								//.Where(p => vertices.Any(v => v != vertex && v.Pos.Is(p)) || map[p] != '#' && !(walked[p]?.Contains(keys) ?? false))
		//								//.Where(p => vertices.Any(v => v != vertex && !v.Pos.Is(prevpos) && v.Pos.Is(p) && v.Keys == keys) || walked[p] == null || !walked[p].Contains(keys))
		//								.Where(p => vertices.Any(v => v != vertex && v.Pos.Is(p) && v.Keys == keys) || walked[p] == null || !walked[p].Contains(keys))
		//								.ToList();
		//							foreach (var p in routes)
		//							{
		//								BuildGraph(fork2, p, keys, steps + weight + 1);
		//							}

		//							return;
		//						}
		//					}





		//					switch (routes.Count())
		//					{
		//						case 0: // Dead end - no edge here
		//							return;
		//						case 1: // Only one way, so move forward if possible
		//							{
		//					//			prevpos = pos;
		//								pos = routes.First();
		//								weight++;
		//							}
		//							break;
		//						default: // Forks, so place vertex here and take each road
		//							if (fork == null)
		//							{
		//								fork = new Vertex(pos, keys, true);
		//								vertices.Add(fork);
		//							}
		//							//var edge = new Edge(vertex, fork, weight);
		//							vertex.Edges.Add(fork, weight);

		//							//////DAG fork.Edges.Add(vertex, weight);


		//							foreach (var p in routes)
		//							{
		//								BuildGraph(fork, p, keys, steps + weight + 1);
		//							}
		//							return;
		//					}
		//				}


		//			}
		//		}








		//class BestPositionFinding
		//{
		//	public Dictionary<string, int> Findings { get; set; }
		//}

		//class Pending
		//{
		//	public int Step;
		//	public Point Prev;
		//	public Point Pos;
		//	public Keys Keys;
		//}

		//private static int ShortestPath(CharMap map)
		//{
		//	var debug = false;
		//	var TooBigStep = int.MaxValue;

		//	var numberOfKeys = map.AllPoints(ch => char.IsLower(ch)).Count();
		//	var pos0 = map.AllPoints(ch => ch == '@').First();
		//	map[pos0] = '.'; // not that nice
		//	var keys0 = new Keys();

		//	foreach (var line in map.Render((p, ch) => pos0.Is(p) ? '@' : ch))
		//	{
		//		Console.WriteLine(line);
		//	}

		//	var ongoing = new Dictionary<string, BestPositionFinding>();
		//	foreach (var p in map.AllPoints(ch => ch != '#'))
		//	{
		//		if (!ongoing.TryGetValue(p.ToString(), out var posfindings))
		//		{
		//			posfindings = ongoing[p.ToString()] = new BestPositionFinding { Findings = new Dictionary<string, int>() };
		//		}
		//	}

		//	var shortest = int.MaxValue;
		//	var totalsteps = 0;

		//	void DebugClear() { if (debug) Console.Clear(); }
		//	void DebugWriteLine(string s) { if (debug) Console.WriteLine(s); }
		//	void DebugReadKey() { if (debug) Console.ReadKey(); }


		//	foreach (var p in pos0.LookAround().Where(p => IsVacant(map[p], keys0)))
		//	{
		//		ShortestPath(pos0.ToString(), 1, pos0, p, keys0);
		//	}
		//	Console.WriteLine($"Total steps: {totalsteps}");

		//	return shortest;

		//	void ShortestPath(string fullpath, int step, Point prev, Point pos, Keys startkeys)
		//	{
		//		fullpath = $"[{fullpath.Length}]";
		//		DebugWriteLine("");
		//		DebugWriteLine($"[{step}/{shortest}] {fullpath} Start exploring");

		//		var keys = startkeys.Copy();

		//		var pending = new List<Pending>();


		//		var path = new Dictionary<string, int>();
		//		while (step < shortest - 1)
		//		{
		//			DebugClear();
		//			DebugWriteLine($"Position {pos}");

		//			if (totalsteps % 1000 == 0)
		//			{
		//				Console.Write($".");
		//			}

		//			if (keys.FoundNewKey(map[pos]))
		//			{
		//				DebugWriteLine($"Found new key {map[pos]} - keys: {keys.AllKeys}");
		//				prev = null;
		//			}
		//			if (debug)
		//			{
		//				foreach (var line in map.Render((p, ch) => pos.Is(p) ? '@' : ch))
		//				{
		//					DebugWriteLine($"[{step}/{shortest}] {fullpath} {line}");
		//				}
		//				DebugReadKey();
		//			}

		//			if (!ongoing.TryGetValue(pos.ToString(), out var posfindings))
		//			{
		//				posfindings = ongoing[pos.ToString()] = new BestPositionFinding { Findings = new Dictionary<string, int>() };
		//			}
		//			var better = posfindings.Findings.FirstOrDefault(k => (keys.AreEqual(k.Key) || keys.IsSubsetOf(k.Key)) && k.Value < step);
		//			if (better.Key != null)
		//			{
		//				// Others have done better than us at finding these keys; abort, abort
		//				DebugWriteLine($"[{step}/{shortest}] {fullpath} Previously found in fewer steps: {better.Value}");
		//				//Console.Write("-");
		//				break;
		//			}

		//			// This is the best effort registered on this position
		//			if (!posfindings.Findings.ContainsKey(keys.AllKeys))
		//			{
		//				posfindings.Findings[keys.AllKeys] = int.MaxValue;
		//			}
		//			posfindings.Findings[keys.AllKeys] = Math.Min(posfindings.Findings[keys.AllKeys], step);

		//			if (keys.Count == numberOfKeys)
		//			{
		//				DebugWriteLine($"[{step}/{shortest}] {fullpath} Found all keys in {step}");
		//				shortest = step;
		//				Console.Write($"({shortest})");
		//				break;
		//			}






		//			var directions = pos
		//				.LookAround()
		//				.Where(p => !p.Is(prev) && IsVacant(map[p], keys))
		//				.ToList();
		//			if (directions.Count() == 0)
		//			{
		//				if (!Keys.IsKey(map[pos]))
		//				{
		//					DebugWriteLine($"[{step}/{shortest}] {fullpath} Dead-end without a key; abort");
		//					break;
		//				}
		//				var temp = pos;
		//				pos = prev;
		//				prev = temp;
		//				step++;
		//				totalsteps++;
		//			}
		//			else if (directions.Count() == 1)
		//			{
		//				prev = pos;
		//				pos = directions.First();
		//				step++;
		//				totalsteps++;
		//			}
		//			else
		//			{
		//				DebugWriteLine($"[{step}/{shortest}] {fullpath} About to explore {string.Join(" ", directions)}");
		//				foreach (var p in directions.Skip(1))
		//				{
		//					pending.Add(new Pending
		//					{
		//						Step = step + 1,
		//						Prev = pos,
		//						Pos = p,
		//						Keys = keys.Copy()
		//					});
		//				}


		//				// Follow the first route here in this function
		//				prev = pos;
		//				pos = directions.First();
		//				step++;
		//				totalsteps++;
		//			}

		//			if (path.TryGetValue(pos.ToString(), out var keycount) && keys.Count == keycount)
		//			{
		//				// Going in circles, not adding any keys - dead end
		//				DebugWriteLine($"[{step}/{shortest}] {fullpath} Going in circles after {step}");
		//				step = TooBigStep;
		//			}
		//			path[pos.ToString()] = keys.Count;

		//		}

		//		// This is the best effort registered on this position
		//		var posfindings2 = ongoing[pos.ToString()];
		//		if (!posfindings2.Findings.ContainsKey(keys.AllKeys))
		//		{
		//			posfindings2.Findings[keys.AllKeys] = int.MaxValue;
		//		}
		//		posfindings2.Findings[keys.AllKeys] = Math.Min(posfindings2.Findings[keys.AllKeys], step);

		//		foreach (var p in pending)
		//		{
		//			ShortestPath($"{fullpath} {p.Pos}", p.Step, p.Prev, p.Pos, p.Keys);
		//		}

		//	}

		//	bool IsVacant(char ch, Keys keys) => ch == '.' || Keys.IsKey(ch) || (keys?.IsDoorOpen(ch) ?? false);
		//}

		//private class Keys
		//{
		//	public string AllKeys { get; private set; }
		//	public Keys(string keys = "")
		//	{
		//		AllKeys = keys.ToString();
		//	}
		//	public bool FoundNewKey(char key)
		//	{
		//		if (IsKey(key) && !AllKeys.Contains(key))
		//		{
		//			// Keep keys sorted for faster lookups and comparisons
		//			AllKeys = new string((AllKeys + key).ToCharArray().OrderBy(c => c).ToArray());
		//			return true;
		//		}
		//		return false;
		//	}
		//	public static bool IsKey(char key) => char.IsLower(key);
		//	public int Count => AllKeys.Length;
		//	public bool IsDoorOpen(char door) => AllKeys.Contains(char.ToLower(door));
		//	public Keys Copy() => new Keys(AllKeys);
		//	public bool AreEqual(Keys other) => AreEqual(other.AllKeys);
		//	public bool AreEqual(string rawkeys) => AllKeys == rawkeys;
		//	public bool IsSubsetOf(Keys other) => IsSubsetOf(other.AllKeys);
		//	public bool IsSubsetOf(string rawkeys) => rawkeys.Length > Count && AllKeys.ToCharArray().Except(rawkeys.ToCharArray()).Count() == 0;
		//}
	}
}

