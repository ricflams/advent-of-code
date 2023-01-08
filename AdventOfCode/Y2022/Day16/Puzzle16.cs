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


		private class Network
		{
			public WeightedGraph<Valve> Graph = new();

			public class Valvex : GraphxNode
			{
				public int Flow;
				public uint Bit;
				public bool MayOpen;
				public override string ToString() => $"{Name} Index={Index} Bit={Bit} Flow={Flow}";
			}
			public Graphx<Valvex> Graph2 = new();

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
						network.Graph.AddVertices(new Valve(v.Name, v.Flow), new Valve(t.Name, t.Flow), 1);
						var a = network.Graph2.AddEdge(v.Name, t.Name, 1);
						a.Flow = v.Flow;
					}
				}
				return network;
			}

			public void Draw()
			{
				Graph.WriteAsGraphwiz();
				Graph2.WriteAsGraphwiz();
			}
		}

		private (int Released, uint Opened) FindMaxReleased(Network nw, List<(Network.Valvex Node, int Dist)> starts, int minutes, uint mask)
		{
			foreach (var n in nw.Graph2.Nodes)
			{
				n.MayOpen = (mask & n.Bit) != 0;
			}

			var allSet = nw.Graph2.Nodes.Where(n => n.MayOpen).Sum(n => n.Bit);

			//var queue = new PriorityQueue<(Network.Valvex, int, int, int, uint), int>();
			var stack = new Stack<(Network.Valvex, int, int, int, uint)>();

			foreach (var s in starts)
			{
				// queue.Enqueue((s.Node, 1+s.Dist, 0, 0, 0), 0);
				stack.Push((s.Node, 1+s.Dist, 0, 0, 0));
			}

			var maxflowrate = nw.Graph2.Nodes.Where(x => x.MayOpen).Sum(x => x.Flow);
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


				// var maxrelease = released + flowrate * (minutes - time);
				// var maxflow = nw.Graph2.Nodes.Where(x => (opened & x.Bit) == 0).Sum(x => x.Flow);
				// var maxmore = (minutes -1 /*- x.Value.Distance*/ - time)*maxflow;
				// if (maxrelease + maxmore <= max)
				// 	continue;
				if (released + maxflowrate * (minutes - time) <= max)
				{
					continue;
				}				

				//if (time < minutes)
				{
					if (v.MayOpen && (opened & v.Bit) == 0)
					{
						// Open it
						var opened2 = opened | v.Bit;
						var flowrate2 = flowrate + v.Flow;
						//var release2 = released+flowrate2 + (minutes-(time+1))*flowrate2;
						//queue.Enqueue((v, time+1, flowrate2, released+flowrate2, opened2), -release2);
						stack.Push((v, time+1, flowrate2, released+flowrate2, opened2));
					}
					foreach (var t in v.Edges)
					{
						//var release2 = released+flowrate*t.Weight + (minutes-(time+t.Weight))*flowrate;
						// if (valueOfDests.ContainsKey(t.Key))
						// 	release2 += valueOfDests[t.Key];
					 	//queue.Enqueue((t.Node as Network.Valvex, time+t.Weight, flowrate, released+flowrate*t.Weight, opened), -release2); // go there
						stack.Push((t.Node as Network.Valvex, time+t.Weight, flowrate, released+flowrate*t.Weight, opened)); // go there
					}
				}
			}
			return (max, openatmax);
		}

		protected override long Part1(string[] input)
		{
			var network = Network.Parse(input);
			var graph2 = network.Graph2;
			graph2.Reduce(v => v.Name != "AA" && v.Flow == 0);
			var starts2 = graph2["AA"].Edges.Select(x => (x.Node as Network.Valvex, x.Weight)).ToList();
			graph2.Reduce(v => v.Flow == 0);

			var bit = 1u;
			foreach (var n in graph2.Nodes)
			{
				n.Bit = bit;
				bit <<= 1;
			}

			var maxv = 0;

			return FindMaxReleased(network, starts2, 30, 0xffffffffu).Released;


			// // //network.Graph.WriteAsGraphwiz();
			// // //network.Draw();

			// // var graph = network.Graph;
			// // graph.Reduce(v => v.Value.Name != "AA" && v.Value.Flow == 0);
			// // var starts = graph.Vertices.Single(v => v.Key.Name == "AA").Value.Edges;
			// // graph.Reduce(v => v.Value.Flow == 0);

			// // //network.Draw();


			// // var queue = new PriorityQueue<(WeightedGraph<Valve>.Vertex, int, int, int, uint), int>();
			// // foreach (var s in starts)
			// // {
			// // 	queue.Enqueue((s.Key, 1+s.Value, 0, 0, 0), 0);
			// // }

			// // var index = 0;
			// // foreach (var v in graph.Vertices.Values)
			// // {
			// // 	v.Value.Id = index;
			// // 	v.Value.Bit = 1u<<index;
			// // 	index++;
			// // }
			// // var allSet = graph.Vertices.Values.Sum(v => v.Value.Bit);

			// // var seen = new Dictionary<uint, (int Time, long Released)>();
		
			// // var distinfo = graph.Vertices.Values
			// // 	.Select(v => (Vertex:v, Dists:graph.ShortestPathToAllDijkstra(v)))
			// // 	.ToDictionary(x => x.Vertex, x => x.Dists);


			// // const int minutes = 30;
			// // var max = 0L;
			// // while (queue.TryDequeue(out var item, out var _))
			// // {
			// // 	var (v, time, flowrate, released, opened) = item;

			// // 	if (opened == allSet)
			// // 	{
			// // 		var total = released + flowrate * (minutes - time);
			// // 		if (total > max)
			// // 			max = total;
			// // 		continue;
			// // 	}
			// // 	if (time == minutes)
			// // 	{
			// // 		if (released > max)
			// // 			max = released;
			// // 		continue;
			// // 	}

			// // 	var key = (uint)v.Value.Id << 24 | opened;
			// // 	if (seen.TryGetValue(key, out var last))
			// // 	{
			// // 		if (time >= last.Time && released <= last.Released + (time - last.Time) * flowrate)
			// // 			continue;
			// // 	}
			// // 	seen[key] = (time, released);

			// // 	var maxrelease = released + flowrate * (minutes - time);
			// // 	var maxflow = graph.Vertices.Values.Where(x => (opened & x.Value.Bit) == 0).Sum(x => x.Value.Flow);
			// // 	var maxmore = (minutes -1 /*- x.Value.Distance*/ - time)*maxflow;
			// // 	if (maxrelease + maxmore <= max)
			// // 		continue;
				

			// // 	// var valueOfDests = graph.Vertices
			// // 	// 	.Where(x => x.Key.Id != v.Value.Id)
			// // 	// 	.Select(x =>
			// // 	// 	{
			// // 	// 		var (valve, vertex) = x;
			// // 	// 		var di = distinfo[v][vertex];
			// // 	// 		var value = 0;
			// // 	// 		if ((opened & valve.Bit) == 0)
			// // 	// 			value += (minutes - 1 - di.Distance - time)*valve.Flow;
			// // 	// 		return (di.Direction, value);
			// // 	// 	})
			// // 	// 	.GroupBy(x => x.Direction)
			// // 	// 	.Select(x => (x, x.Sum(i=>i.value)))
			// // 	// 	.ToDictionary(x => x.Item1.Key, x => x.Item2);

			// // 	if (time < minutes)
			// // 	{
			// // 		if ((opened & v.Value.Bit) == 0)
			// // 		{
			// // 			// Open it
			// // 			var opened2 = opened | v.Value.Bit;
			// // 			var flowrate2 = flowrate + v.Value.Flow;
			// // 			var release2 = released+flowrate2 + (minutes-(time+1))*flowrate2;
			// // 			queue.Enqueue((v, time+1, flowrate2, released+flowrate2, opened2), -release2);
			// // 		}
			// // 		foreach (var t in v.Edges)
			// // 		{
			// // 			var release2 = released+flowrate*t.Value + (minutes-(time+t.Value))*flowrate;
			// // 			// if (valueOfDests.ContainsKey(t.Key))
			// // 			// 	release2 += valueOfDests[t.Key];
			// // 		 	queue.Enqueue((t.Key, time+t.Value, flowrate, released+flowrate*t.Value, opened), -release2); // go there
			// // 		}
			// // 	}
			// // }
			// // return max;
		}

		private record Valve(string Name, int Flow)
		{
			public uint Bit;
			public int Id;
		}

		protected override long Part2(string[] input)
		{
			var network = Network.Parse(input);

		// Round=111558820:
		//  opened=                 111111111001101
		//     you=                   1111011000100  1EC4
		//     ele=                 110000100001001  6109
		// Round 115929605: #q=0 maxpres=2306

    // you=                   1111011000100 0x1EC4
    // ele=                 110000100001001	0x6109

	
			var graph2 = network.Graph2;
			graph2.Reduce(v => v.Name != "AA" && v.Flow == 0);
			var starts2 = graph2["AA"].Edges.Select(x => (x.Node as Network.Valvex, x.Weight)).ToList();
			graph2.Reduce(v => v.Flow == 0);

			var bit = 1u;
			foreach (var n in graph2.Nodes)
			{
				n.Bit = bit;
				bit <<= 1;
			}

			var maxv = 0;

			//  mask=                   1111011000100 1EC4
			//    ox=                   1111011000100
			//    mx=1362
			//  mask=                   1111011!!01!0 1EF6
			//    ox=                   1111011000100
			//    mx=1306			

			//  mask=                 110000100001001 6109
			//    ox=                 110000100001001
			//    mx=944
			//  mask=                 110000100!!10!1 613B
			//    ox=                 110000000001011
			//    mx=855			

			// var maskx = 0x1EF6u;
			// //maskx = 0x1EC4u;
			// var (mx1, ox1) = FindMaxReleased(network, starts2, 26, maskx);
			// Console.WriteLine($" mask={Convert.ToString(maskx, 2), 32}");
			// Console.WriteLine($"   ox={Convert.ToString(ox1, 2), 32}");
			// Console.WriteLine($"   mx={mx1}");
			// maskx = 0x613Bu;
			// //maskx = 0x6109u;
			// var (mx2, ox2) = FindMaxReleased(network, starts2, 26, maskx);
			// Console.WriteLine($" mask={Convert.ToString(maskx, 2), 32}");
			// Console.WriteLine($"   ox={Convert.ToString(ox2, 2), 32}");
			// Console.WriteLine($"   mx={mx2}");
			// Console.WriteLine($"m1+m2={mx1+mx2}");
			// return mx1+mx2;
			// // expect 944

			for (var i = 0u; i < bit; i++)
			{
				var mask = i;
				var maskv = ~i;

				// // if ((mask & 0x1EC4u)==0x1EC4u && (maskv & 0x6109u)==0x6109u)
				// // {}
				// // else continue;
				// if ((mask & 0x1EC4u) != 0x1EC4u)
				// 	continue;
	

				var (m1,o1) = FindMaxReleased(network, starts2, 26, mask);
				var (m2,o2) = FindMaxReleased(network, starts2, 26, maskv);
				// // if ((o1 & 0x1EC4u)==0x1EC4u)
				// // 	Console.Write("a");
				// // if ((o2 & 0x6109u)==0x6109u)
				// // 	Console.Write("b");
				// Console.WriteLine($" mask={Convert.ToString(mask, 2), 32}");
				// Console.WriteLine($"~mask={Convert.ToString(maskv, 2), 32}");
				// Console.WriteLine($"   o1={Convert.ToString(o1, 2), 32}");
				// Console.WriteLine($"   o2={Convert.ToString(o2, 2), 32}");
				// Console.WriteLine($"   m1={m1}");
				// Console.WriteLine($"   m2={m2}");
				// Console.WriteLine($"  rel={m1+m2}");
				
				if (m1+m2 > maxv)
					maxv = m1+m2;
			}
			return maxv;




			var graph = network.Graph;
			var valves = graph.Vertices;

			var valvesWithFlow = valves.Values.Count(x => x.Value.Flow > 0);
			var minutes = 26;

			var seen = new Dictionary<ulong, (int Time, long Released, int Flowrate)>();
			var start = valves.Values.Single(x => x.Value.Name == "AA");

			var index = 0;
			var bit2 = 1U;			
			foreach (var v in graph.Vertices.Values)
			{
				v.Value.Id = index++;
				if (v.Value.Flow > 0)
				{
					v.Value.Bit = bit2;
					bit2 <<= 1;
				}				
			}			

			var distinfo = valves.Values
				.Select(v => (Vertex:v, Dists:graph.ShortestPathToAllDijkstra(v)))
				.ToDictionary(x => x.Vertex, x => x.Dists);

			var maxflowrate = graph.Vertices.Values.Where(x => x.Value.Flow > 0).Sum(x => x.Value.Flow);

			var statSkipTimeRelease = 0;
			var statSkipTimeRelease2 = 0;
			var statMinutesReached = 0;
			var statAllOpenValves = 0;
			var statMaxRelease = 0;
			var statMaxRelease2 = 0;
			var statRounds = 0L;

			var maxpressure = 0;
			var queue = new PriorityQueue<(WeightedGraph<Valve>.Vertex, WeightedGraph<Valve>.Vertex, int, int, int, uint, (uint, uint)), int>();
			//var queue = new Queue<(WeightedGraph<Valve>.Vertex, WeightedGraph<Valve>.Vertex, int, int, int, uint)>();
			queue.Enqueue((start, start, 1, 0, 0, 0, (0,0)), 0);
			while (queue.TryDequeue(out var item, out var _))
			{
				var (youv, elev, time, flowrate, released, openbits, (youbits, elebits)) = item;
			//	Console.WriteLine($"{v.Name} t={time} flow={flowrate} rel={released} open={openvalves} bits={openbits} #q={queue.Count()}");

				// if (youv.Value.Id > elev.Value.Id)
				// 	(youv, elev) = (elev, youv);
				
				var you = youv.Value;
				var ele = elev.Value;

				var openCount = openbits.NumberOfSetBits();
				if (++statRounds % 10000000 == 0)// || openCount == valvesWithFlow)
				{
					Console.WriteLine($"Round {statRounds:N0}: #q={queue.Count} skiptime={statSkipTimeRelease:N0} skiptime2={statSkipTimeRelease2:N0} minreached={statMinutesReached:N0} maxrelease={statMaxRelease:N0} maxrelease2={statMaxRelease2:N0} allopen={statAllOpenValves:N0} maxpres={maxpressure} time={time} opened={openCount}");
				}

				// // var remainingValces = valvesWithFlow-openCount;
				// // if (time + remainingValces >= minutes)
				// // 	continue;


				//var key = (ulong)(((ulong)you.Id)<<25) | (ulong)(((ulong)ele.Id)<<20) | (ulong)openbits;
				var key = (ulong)(((ulong)you.Id)<<24) | (ulong)(((ulong)ele.Id)<<16) | openbits;
				if (seen.TryGetValue(key, out var xx))
				{
					var (time2, released2, flowrate2) = xx;
					//Console.WriteLine($"  dt={time - time2}");
					// if (time >= time2)
					// {

					// if (time >= time2 && released <= released2 + (time - time2) * flowrate) // why not 2?
					// {
					// 	statSkipTimeRelease++;
					// 	continue;
					// }
			//		Debug.Assert(time >= time2);
					if (time >= time2 && released <= released2 + (time - time2) * flowrate2)
					//if (released <= released2 + (time - time2) * flowrate2)
					{
						statSkipTimeRelease++;
						continue;
					}


					if (time >= time2 && flowrate <= flowrate2)
						continue;
					if (time<time2)
					{
						var maxmoreflow = graph.Vertices.Values.Where(x => x.Value.Flow > 0 && (openbits & x.Value.Bit) == 0)
							.Select(x => x.Value.Flow)
							.OrderByDescending(x => x)
							.Select((f,idx) => Math.Max(0, (time2-time+idx-1)*f))
							.Take(time2-time)
							.Sum();
						if (released + flowrate*(time2-time) + maxmoreflow <= released2)
						{
							statSkipTimeRelease2++;
							continue;
						}
					}
						;
					// }
					// else
					// 	;
		
					// else if (time2 > time)
					// {
					// 	if (released + (time2 - time) * flowrate <= released2)
					// 		continue;
					// }
					// else if (time == time2)
					// {
					// 	if (released <= released2)
					// 		continue;
					// }
				}
				seen[key] = (time, released, flowrate);

				if (time == minutes)
				{
					if (released > maxpressure)
						maxpressure = released;
					statMinutesReached++;

					if (released == 2306)
					{
						Console.WriteLine($"Round={statRounds}:");
						Console.WriteLine($" opened={Convert.ToString(openbits, 2), 32}");
						Console.WriteLine($"    you={Convert.ToString(youbits, 2), 32}");
						Console.WriteLine($"    ele={Convert.ToString(elebits, 2), 32}");
					}

					continue;
				}

				if (openCount == valvesWithFlow)
				{
					var fullrelease = released + flowrate * (minutes - time);
					//Console.WriteLine(fullrelease);
					if (fullrelease > maxpressure)
						maxpressure = fullrelease;
					//queue.Enqueue((v, time+1, flowrate, released + flowrate, openvalves, openbits)); // just stay
					statAllOpenValves++;
					continue;
				}

				if (released + maxflowrate * (minutes - time) <= maxpressure)
				{
					statMaxRelease++;
					continue;
				}

				// var maxrelease = released + flowrate * (minutes - time);
				// var unopened = graph.Vertices.Values.Where(x => x.Value.Flow > 0 && (openbits & x.Value.Bit) == 0).ToArray();
				// var moreflow = unopened.Sum(x => x.Value.Flow);
				// //var mindist = unopened.Min()
				// var maxmore = (minutes /*- x.Value.Distance*/ - time)*moreflow;
				// if ((openbits & youv.Value.Bit) == 0)
				// 	maxmore += youv.Value.Flow;
				// else if ((openbits & elev.Value.Bit) == 0)
				// 	maxmore += elev.Value.Flow;
				// if (maxrelease + maxmore <= maxpressure)
				// {
				// 	statMaxRelease2++;
				// 	continue;
				// }

				var youCanOpen = you.Flow > 0 && (openbits & you.Bit) == 0;
				var eleCanOpen = ele.Flow > 0 && (openbits & ele.Bit) == 0;

				//int Priority(int newOpen, int flowr) => -(released + (minutes-(time))*flowr);// + 500*(openCount+newOpen));
				int Priority(int newOpen, int flowr) => -(openCount+newOpen);
				//int Priority(int newOpen, int flowr) => 0;

				if (you.Id == ele.Id)
				{
					if (youCanOpen) // both can open; just let you do it
					{
						var nextopenbits = openbits | you.Bit;
						var nextflowrate = flowrate + you.Flow;
						foreach (var nextele in elev.Edges)
						{
							var bits = (youbits|you.Bit, elebits);
							queue.Enqueue((youv, nextele.Key, time+1, nextflowrate, released + nextflowrate, nextopenbits, bits), Priority(1, nextflowrate)); // go there
						}
					}
				}
				else
				{
					if (youCanOpen && eleCanOpen)
					{
						var nextopenbits = openbits | you.Bit | ele.Bit;
						var nextflowrate = flowrate + you.Flow + ele.Flow;
						var bits = (youbits|you.Bit, elebits|ele.Bit);
						queue.Enqueue((youv, elev, time+1, nextflowrate, released + nextflowrate, nextopenbits, bits), Priority(2, nextflowrate)); // go and open
					}
					if (youCanOpen && !eleCanOpen)
					{
						var nextopenbits = openbits | you.Bit;
						var nextflowrate = flowrate + you.Flow;
						var bits = (youbits|you.Bit, elebits);
						foreach (var nextele in elev.Edges)
						{
							queue.Enqueue((youv, nextele.Key, time+1, nextflowrate, released + nextflowrate, nextopenbits, bits), Priority(1, nextflowrate)); // go there
						}
					}
					if (!youCanOpen && eleCanOpen)
					{
						var nextopenbits = openbits | ele.Bit;
						var nextflowrate = flowrate + ele.Flow;
						var bits = (youbits, elebits|ele.Bit);
						foreach (var nextyou in youv.Edges)
						{
							queue.Enqueue((nextyou.Key, elev, time+1, nextflowrate, released + nextflowrate, nextopenbits, bits), Priority(1, nextflowrate)); // go there
						}
					}
				}

				// var keyx = (ulong)(((ulong)you.Number)<<24) | (ulong)(((ulong)ele.Number)<<16) | openbits;
				// if (seen.TryGetValue(keyx, out var xx2))
				// {
				// 	var (time2, released2) = xx2;
				// 	if (time+1 >= time2 && released+flowrate <= released2 + (time+1 - time2) * flowrate)
				// 		continue;
				// }

				var seennow = new HashSet<uint>();
				foreach (var nextyou in youv.Edges)
				{
					foreach (var nextele in elev.Edges)
					{
						//var (y, e) = nextyou.Key.Value.Id < nextele.Key.Value.Id ? (nextyou.Key, nextele.Key) : (nextele.Key, nextyou.Key);
						var (y, e) = (nextyou.Key, nextele.Key);
						var keynow = ((uint)y.Value.Id)<<16 | (uint)(e.Value.Id);
						if (seennow.Contains(keynow))
							continue;
						seennow.Add(keynow);
						queue.Enqueue((nextyou.Key, nextele.Key, time+1, flowrate, released + flowrate, openbits, (youbits, elebits)), Priority(0, flowrate)); // go there
					}
				}	
			}

			Console.WriteLine($"Round {statRounds}: #q={queue.Count} maxpres={maxpressure}");
			return maxpressure;
		}


	}
}
