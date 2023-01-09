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
		//	Run("test1").Part1(1651).Part2(1707);
			Run("test9").Part1(1850).Part2(2306);
			Run("input").Part1(1915).Part2(2772);
		}


		protected override long Part1(string[] input)
		{
			var network = Network.Parse(input);
			var graph2 = network.Graph2;
			graph2.Reduce(v => v.Name != "AA" && v.Flow == 0);
			var starts2 = graph2["AA"].Edges.Select(x => (x.Node as Network.Valve, x.Weight)).ToList();
			graph2.Reduce(v => v.Flow == 0);

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
			var network = Network.Parse(input);

			var graph2 = network.Graph2;
			graph2.Reduce(v => v.Name != "AA" && v.Flow == 0);
			var starts2 = graph2["AA"].Edges.Select(x => (x.Node as Network.Valve, x.Weight)).ToList();
			graph2.Reduce(v => v.Flow == 0);

			var bit = 1u;
			foreach (var n in graph2.Nodes)
			{
				n.Bit = bit;
				bit <<= 1;
			}

			var maxv = 0;
			var cache = new ResultCache(graph2.Nodes.Count);

			for (var i = bit; --i > 0; )
			{
				cache.GetOrCreate(i, m => network.FindMaxReleased(starts2, 26, m));
			}
			Console.WriteLine($"hits={cache.Hits} inserts={cache.Inserts} size={cache.Size} filled={cache.Filled}");

			for (var i = 0u; i < bit/2; i++)
			{
				var mask = i;
				var maskv = ~i;
				var m1 = cache.GetOrCreate(mask, m => network.FindMaxReleased(starts2, 26, m));
				var m2 = cache.GetOrCreate(maskv, m => network.FindMaxReleased(starts2, 26, m));
				if (m1+m2 > maxv)
					maxv = m1+m2;
			}
			Console.WriteLine($"hits={cache.Hits} inserts={cache.Inserts} size={cache.Size} filled={cache.Filled}");
			return maxv;
		}

		private class ResultCache
		{
			private readonly int _bits;
			private readonly uint _mask;
			private record Cached(uint Mask, uint Found, int Result);
			private readonly List<Cached> _cache = new();
			private readonly int[] _cache2;

			public int Hits = 0;
			public int Inserts = 0;
			public int Size => _cache2.Length;
			public int Filled => _cache2.Count(x => x != 0);

			public ResultCache(int bits)
			{
				_bits = bits;
				_mask = (1u<<bits) - 1;
				_cache2 = new int[(1u<<bits)];
			}

			public int GetOrCreate(uint allow, Func<uint, (int, uint)> calculator)
			{
				allow &= _mask;

				// var cached = _cache.FirstOrDefault(c => (c.Mask & allow) == allow && (allow & c.Found) == c.Found);
				// if (cached != null)
				// {
				// 	Hits++;
				// 	return cached.Result;
				// }
				// var (result, found) = calculator(allow);
				// if ((allow & ~found) != 0)
				// {
				// 	_cache.Add(new Cached(allow, found, result));
				// 	//Console.Write("+");
				// }
				// //else Console.Write("-");

				// return result;
				if (_cache2[allow] == 0)
				{
					var (result, found) = calculator(allow);
					_cache2[allow] = result;
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
						//Console.Write($"[{n}]");
						//var diffs = new List<int>();
						for (var i = 0u; i < n; i++)
						{
							var vx = allow;
							for (var j = 0; j < bits.Length; j++)
							{
								if ((i & (1u<<j)) != 0)
									vx ^= bits[j];
							}
							_cache2[vx] = result;
							//diffs.Add((int)vx - (int)allow);
							Inserts++;
						}
						//Console.WriteLine(string.Join(" ", diffs.OrderBy(x=>x)));
						// foreach (var v in MathHelper.Combinations(bits, bits.Length))
						// {
						// 	var vx = allow | v.Sum();
						// 	try
						// 	{
						// 		_cache[vx] = result;
						// 	}
						// 	catch
						// 	{
						// 		Console.WriteLine($"allow={Convert.ToString(allow, 2),20}");
						// 		Console.WriteLine($"found={Convert.ToString(found, 2),20}");
						// 		Console.WriteLine($"unusd={Convert.ToString(unused, 2),20}");
						// 		Console.WriteLine($"  sum={Convert.ToString(v.Sum(), 2),20}");
						// 		Console.WriteLine($"   vx={Convert.ToString(vx, 2),20}");
						// 		throw;
						// 	}
						// }						
					}
				}
				else
					Hits++;
				return _cache2[allow];
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
			public Graphx<Valve> Graph2 = new();

			public static Network Parse(string[] input)
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
						var a = network.Graph2.AddEdge(v.Name, t.Name, 1);
						a.Flow = v.Flow;
					}
				}
				return network;
			}

			public void Draw()
			{
				Graph2.WriteAsGraphwiz();
			}

			public (int Released, uint Opened) FindMaxReleased(List<(Network.Valve Node, int Dist)> starts, int minutes, uint mask)
			{
				foreach (var n in Graph2.Nodes)
				{
					n.MayOpen = (mask & n.Bit) != 0;
				}

				var allSet = Graph2.Nodes.Where(n => n.MayOpen).Sum(n => n.Bit);

				//var queue = new PriorityQueue<(Network.Valvex, int, int, int, uint), int>();
				var stack = new Stack<(Network.Valve, int, int, int, uint)>();

				foreach (var s in starts)
				{
					// queue.Enqueue((s.Node, 1+s.Dist, 0, 0, 0), 0);
					stack.Push((s.Node, 1+s.Dist, 0, 0, 0));
				}

				var maxflowrate = Graph2.Nodes.Where(x => x.MayOpen).Sum(x => x.Flow);
				var seen = new Dictionary<uint, (int Time, int Released, int Flowrate)>();
				var max = 0;
				var openatmax = 0u;

				//while (queue.TryDequeue(out var item, out var _))
				while (stack.TryPop(out var item))
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
						stack.Push((v, time+1, flowrate2, released+flowrate2, opened2));
					}
					foreach (var t in v.Edges)
					{
						stack.Push((t.Node as Network.Valve, time+t.Weight, flowrate, released+flowrate*t.Weight, opened)); // go there
					}
				}
				return (max, openatmax);
			}

		}

	}
}
