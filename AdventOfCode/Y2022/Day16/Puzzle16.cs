using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using AdventOfCode.Helpers.Byte;

namespace AdventOfCode.Y2022.Day16
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 16";
		public override int Year => 2022;
		public override int Day => 16;

		public void Run()
		{
			Run("test1").Part1(1651).Part2(1707);
		 	Run("test9").Part1(1850).Part2(2306);
		 	Run("input").Part1(1915).Part2(2772);
		}


		protected override long Part1(string[] input)
		{
			var (network, starts2) = Network.Parse(input);
			var graph2 = network.Graph;
			// graph2.Reduce(v => v.Name != "AA" && v.Flow == 0);
			// var starts2 = graph2["AA"].Edges.Select(x => (x.Node as Network.Valve, x.Weight)).ToList();
			// graph2.Reduce(v => v.Flow == 0);
			// network.FindDists();

			var bit = 1u;
			foreach (var n in graph2.Nodes)
			{
				n.Bit = bit;
				bit <<= 1;
			}

			return network.FindMaxReleased(starts2, 30, 0xffffffffu).Released;
		}

		protected override long Part2(string[] input)
		{
			var (network, starts2) = Network.Parse(input);

			var graph2 = network.Graph;


			var bit = 1u;
			foreach (var n in graph2.Nodes)
			{
				n.Bit = bit;
				bit <<= 1;
			}

			var cache = new ResultCache(graph2.Nodes.Count);

			var masks = Enumerable.Range(0, (int)bit)
				.Select(val => ((uint)val, Bits: val.NumberOfSetBits()))
				.OrderByDescending(x => x.Bits)
				.ToArray();
			foreach (var mask in masks)
			{
				cache.GetOrCreate(mask.Item1, m => network.FindMaxReleased(starts2, 26, m));
			}

			//Console.WriteLine($"hits={cache.Hits} creates={cache.Creates} inserts={cache.Inserts} size={cache.Size} filled={cache.Filled}");

			var maxv = 0;
			for (var i = 0u; i < bit/2; i++)
			{
				var mask = i;
				var maskv = ~i;
				var m1 = cache.GetOrCreate(mask, m => network.FindMaxReleased(starts2, 26, m));
				var m2 = cache.GetOrCreate(maskv, m => network.FindMaxReleased(starts2, 26, m));
				if (m1+m2 > maxv)
					maxv = m1+m2;
			}

			//Console.WriteLine($"hits={cache.Hits} creates={cache.Creates} inserts={cache.Inserts} size={cache.Size} filled={cache.Filled}");
			return maxv;
		}

		private class ResultCache
		{
			private readonly int _bits;
			private readonly uint _mask;
			private readonly int[] _cache;

			public int Hits = 0;
			public int Creates = 0;
			public int Inserts = 0;
			public int Size => _cache.Length;
			public int Filled => _cache.Count(x => x != 0);

			public ResultCache(int bits)
			{
				_bits = bits;
				_mask = (1u<<bits) - 1;
				_cache = new int[(1u<<bits)];
			}

			public int GetOrCreate(uint allow, Func<uint, (int, uint)> calculator)
			{
				allow &= _mask;
				if (_cache[allow] == 0)
				{
					Creates++;
					var (result, found) = calculator(allow);
					_cache[allow] = result;
					var unused = allow & ~found;
					// Console.WriteLine($"allow={Convert.ToString(allow, 2),20}");
					// Console.WriteLine($"found={Convert.ToString(found, 2),20}");
					// Console.WriteLine($"unusd={Convert.ToString(unused, 2),20}");
					if (unused > 0)
					{
						var bits = Enumerable.Range(0, _bits)
							.Select(b => 1u<<b)
							.Where(b => (unused & b) != 0)
							.ToArray();
						var n = 1u<<bits.Length;
						for (var i = 0u; i < n; i++)
						{
							var vx = allow;
							for (var j = 0; j < bits.Length; j++)
							{
								if ((i & (1u<<j)) != 0)
									vx ^= bits[j];
							}
							_cache[vx] = result;
							Inserts++;
						}
					}
				}
				else
					Hits++;
				return _cache[allow];
			}
		}


		private class Network
		{
			public class Valve : GraphxNode
			{
				public int Flow;
				public uint Bit;
				public bool MayOpen;
				public override string ToString() => $"{Name} Index={Index} Bit={Bit} Flow={Flow}";
			}
			public Graphx<Valve> Graph = new();

			public static (Network, List<(Valve,int)>) Parse(string[] input)
			{
				var valves = input
					.Select(s =>
					{
						var (name, flow, _, __, ___, dir) = s.RxMatch("Valve %s has flow rate=%d; %s %s to %s %*").Get<string, int, string, string, string, string>();
						return new
						{
							Name = name,
							Flow = flow,
							TunnelNames = dir.Split(", ").ToArray()				
						};
					})
					.ToDictionary(x => x.Name, x => x);

				var network = new Network();
				foreach (var v in valves.Values)
				{
					foreach (var tn in v.TunnelNames)
					{
						var t = valves[tn];
						var a = network.Graph.AddEdge(v.Name, t.Name, 1);
						a.Flow = v.Flow;
					}
				}

				network.Graph.Reduce(v => v.Name != "AA" && v.Flow == 0);
				var starts2 = network.Graph["AA"].Edges.Select(x => (x.Node as Network.Valve, x.Weight)).ToList();
				network.DistInfo = network.Graph.Nodes
					.Select(v => (Vertex:v, Dists:network.Graph.ShortestPathToAllDijkstra(v)))
					.ToDictionary(x => x.Vertex, x => x.Dists);
				network.DistInfoFromA = network.DistInfo[network.Graph["AA"]];
				network.DistInfoFromA.Remove(network.Graph["AA"]);
				network.Graph.Reduce(v => v.Flow == 0);

				network.DistInfo2 = new int[network.Graph.Nodes.Count, network.Graph.Nodes.Count];
				foreach (var n1 in network.Graph.Nodes)
				{
					foreach (var n2 in network.Graph.Nodes.Where(n => n.Index != n1.Index))
					{
						network.DistInfo2[n1.Index, n2.Index] = network.DistInfo[n1][n2];
					}
				}
				network.DistInfoFromA2 = new int[network.Graph.Nodes.Count];
				foreach (var n in network.DistInfoFromA)
				{
					network.DistInfoFromA2[n.Key.Index] = n.Value;
				}

				return (network, starts2);
			}

			public Dictionary<Valve, Dictionary<Valve, int>> DistInfo;
			public int[,] DistInfo2;
			public int[] DistInfoFromA2;
			public Dictionary<Valve, int> DistInfoFromA;

			public void Draw()
			{
				Graph.WriteAsGraphwiz();
			}


			public (int Released, uint Opened) FindMaxReleased2(List<(Network.Valve Node, int Dist)> starts, int minutes, uint mask)
			{
				foreach (var n in Graph.Nodes)
				{
					n.MayOpen = (mask & n.Bit) != 0;
				}

				var allSet = Graph.Nodes.Where(n => n.MayOpen).Sum(n => n.Bit);

				//var queue = new PriorityQueue<(Network.Valve, int, int, int, uint), int>();
				//var stack = new Stack<(Network.Valve, int, int, int, uint)>();
				var queue = Quack<(Network.Valve, int, int, int, uint)>.Create(QuackType.Queue);

				foreach (var s in starts)
				{
					//queue.Enqueue((s.Node, 1+s.Dist, 0, 0, 0), 0);
					//stack.Push((s.Node, 1+s.Dist, 0, 0, 0));
					queue.Put((s.Node, 1+s.Dist, 0, 0, 0), 0);
				}

				var maxflowrate = Graph.Nodes.Where(x => x.MayOpen).Sum(x => x.Flow);
				var seen = new Dictionary<uint, (int Time, int Released, int Flowrate)>();
				var max = 0;
				var openatmax = 0u;

				//while (queue.TryDequeue(out var item, out var _))
				//while (stack.TryPop(out var item))
				while (queue.TryGet(out var item))
				{
					var (v, time, flowrate, released, opened) = item;

					var key = (uint)v.Index << 24 | opened;
					if (seen.TryGetValue(key, out var last))
					{
						if (time >= last.Time && released <= last.Released + (time - last.Time) * last.Flowrate)
							continue;
						// if (time >= last.Time && flowrate <= last.Flowrate)
						// 	continue;						
					}
					seen[key] = (time, released, flowrate);

					if (opened == allSet)
					{
						var total = released + flowrate * (minutes - time);
						if (total > max)
							//max = total;
							(max, openatmax) = (total, opened);
						continue;
					}
					if (time >= minutes)
					{
						var toomuch = flowrate*(time-minutes);
						var total = released - toomuch;
						if (total > max)
							(max, openatmax) = (total, opened);
						continue;
					}

					if (released + maxflowrate * (minutes - time) <= max)
					{
						continue;
					}				

					if (v.MayOpen && (opened & v.Bit) == 0)
					{
						// Open it
						var opened2 = opened | v.Bit;
						var flowrate2 = flowrate + v.Flow;
						//stack.Push((v, time+1, flowrate2, released+flowrate2, opened2));
						//queue.Enqueue((v, time+1, flowrate2, released+flowrate2, opened2), -(released+flowrate2));
						var pot = released + flowrate2 * (minutes - time);
						if (pot <= max)
						{
							continue;
						}	
						queue.Put((v, time+1, flowrate2, released+flowrate2, opened2), -pot);
					}
					foreach (var t in v.Edges)
					{
						var pot = released + flowrate * (minutes - time);
						if (pot <= max)
						{
							continue;
						}	

						queue.Put((t.Node as Network.Valve, time+t.Weight, flowrate, released+flowrate*t.Weight, opened), -pot); // go there
					}
				}
				return (max, openatmax);
			}

			public (int Released, uint Opened) FindMaxReleased(List<(Network.Valve Node, int Dist)> starts, int minutes, uint mask)
			{
				var nodes = Graph.Nodes;

				foreach (var n in nodes)
				{
					n.MayOpen = (mask & n.Bit) != 0;
				}
				var mayOpen = nodes.Where(x => x.MayOpen).OrderByDescending(x => x.Flow * (minutes - DistInfoFromA2[x.Index])).ToArray();

				var allSet = mayOpen.Sum(n => n.Bit);
				var maxflowrate = mayOpen.Sum(x => x.Flow);				

				var max = 0;
				var openatmax = 0u;

				foreach (var n in mayOpen)
				{
					var dist = DistInfoFromA2[n.Index];
					if (dist+1 >= minutes)
						continue;
					FindMax(n, 1 + dist+1, n.Flow, n.Flow, n.Bit);
				}
				return (max, openatmax);

				void FindMax(Network.Valve v, int time, int flowrate, int released, uint opened)
				{
					if (released + maxflowrate * (minutes - time) <= max)
						return;
					if (opened == allSet)
					{
						var total = released + flowrate * (minutes - time);
						if (total > max)
							(max, openatmax) = (total, opened);
						return;
					}

					foreach (var t in mayOpen)
					{
						if ((opened & t.Bit) != 0)
							continue;
						var dist = DistInfo2[v.Index, t.Index];
						if (time+dist+1 < minutes)
						{
							var opened2 = opened | t.Bit;
							var flowrate2 = flowrate + t.Flow;
							FindMax(t, time+dist+1, flowrate2, released + flowrate*dist + flowrate2, opened2); // go there
						}
						else
						{
							var total = released + flowrate * (minutes - time);
							if (total > max)
								(max, openatmax) = (total, opened);
						}
					}					
				}



				// var queue = Quack<(Network.Valve, int, int, int, uint)>.Create(QuackType.Stack);

				// foreach (var n in mayOpen)
				// {
				// 	var dist = DistInfoFromA[n];
				// 	if (1+dist+1 >= minutes)
				// 		continue;
				// 	queue.Put((n, 1+dist+1, n.Flow, n.Flow, n.Bit));
				// }

				// // var max = 0;
				// // var openatmax = 0u;
				// var rounds = 0;

				// while (queue.TryGet(out var item))
				// {
				// 	var (v, time, flowrate, released, opened) = item;

				// 	if (released + maxflowrate * (minutes - time) <= max)
				// 	{
				// 		continue;
				// 	}

				// 	rounds++;

				// 	if (opened == allSet)
				// 	{
				// 		var total = released + flowrate * (minutes - time);
				// 		if (total > max)
				// 			(max, openatmax) = (total, opened);
				// 		continue;
				// 	}

				// 	Debug.Assert(v.MayOpen);
				// 	Debug.Assert((opened & v.Bit) != 0);

				// 	foreach (var t in mayOpen.Where(v => (opened & v.Bit) == 0))
				// 	{
				// 		var dist = DistInfo[v][t];
				// 		if (time+dist+1 < minutes)
				// 		{
				// 			var opened2 = opened | t.Bit;
				// 			var flowrate2 = flowrate + t.Flow;
				// 			queue.Put((t, time+dist+1, flowrate2, released + flowrate*dist + flowrate2, opened2)); // go there
				// 		}
				// 		else
				// 		{
				// 			var total = released + flowrate * (minutes - time);
				// 			if (total > max)
				// 				(max, openatmax) = (total, opened);
				// 		}
				// 	}
				// }
				// // Console.Write(rounds);
				// // Console.Write(" ");
				// return (max, openatmax);
			}

		}
	}
}
