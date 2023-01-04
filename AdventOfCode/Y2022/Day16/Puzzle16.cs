using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

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
	//		Run("test1").Part1(1651);//.Part2(1707);
	//		Run("test9").Part1(1850).Part2(1707);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(1915);//.Part2(2772); // 1331 too low, 2582 wrong
			// 649 not right
			
		}

		private record Valve2(string Name, int Flow) {}


		private class Network
		{
			public WeightedGraph<Valve2> Graph = new();

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
						network.Graph.AddVertices(new Valve2(v.Name, v.Flow), new Valve2(t.Name, t.Flow), 1);
					}
				}
				return network;
			}

			public void Draw()
			{
				Graph.WriteAsGraphwiz();
			}
		}

		protected override long Part1(string[] input)
		{
			var network = Network.Parse(input);

			network.Graph.WriteAsGraphwiz();
			network.Graph.Reduce(v => v.Value.Name != "AA" && v.Value.Flow == 0);
			network.Graph.WriteAsGraphwiz();

			var starts = network.Graph.Vertices.Single(v => v.Key.Name == "AA").Value.Edges;
			network.Graph.Reduce(v => v.Value.Flow == 0);
			network.Graph.WriteAsGraphwiz();

			// return 0;

			// var valves = new Valve[1];

			// var n = 0;
			// var bit = 1U;
			// foreach (var v in valves)
			// {
			// 	v.Number = n++;
			// 	if (v.Flow > 0)
			// 	{
			// 		v.OpenBit = bit;
			// 		bit <<= 1;
			// 	}
			// 	v.Tunnels = v.TunnelNames.Select(x => valves.Single(v => v.Name == x)).ToArray();
			// }
			var valvesWithFlow = network.Graph.Vertices.Count; // No flow in AA

			var minutes = 30;

			var seenTime = new Dictionary<string, long>();
			var seenRelease = new Dictionary<string, int>();
			var seenOpenValves = new Dictionary<string, Dictionary<string,(int Time,int Released, int Flow)>>();
			// var start = valves.Single(x => x.Name == "AA");

			var maxpressure = 0L;
			var stack = new Stack<(WeightedGraph<Valve2>.Vertex, int, int, int, string[])>();
			foreach (var s in starts)
			{
				stack.Push((s.Key, 1+s.Value, 0, 0, new string[0]));//s.Value);
			}

			var maxopen = 0;

			while (stack.TryPop(out var item))
			{
				var (v, time, flowrate, released, opened) = item;

				if (opened.Length == valvesWithFlow)
				{
					var fullrelease = released + flowrate * (minutes - time);
					//Console.WriteLine(fullrelease);
					if (fullrelease > maxpressure)
						maxpressure = fullrelease;
					//queue.Enqueue((v, time+1, flowrate, released + flowrate, openvalves, openbits)); // just stay
					continue;
				}
				if (time == minutes)
				{
					if (released > maxpressure)
						maxpressure = released;
					continue;
				}

				var key = "x"+string.Join("", opened);
				//Console.WriteLine($"{v.Value.Name} t={time} flow={flowrate} rel={released} open={key} #q={queue.Count}");

				if (!seenOpenValves.TryGetValue(key, out var seen))
				{
					seen = seenOpenValves[key] = new();
				}
				var key2 = $"{v.Value.Name}";//-{opened.Contains(v.Value.Name)}";
				if (seen.TryGetValue(key2, out var xx))
				{
					if (released < xx.Released + (time - xx.Time) * flowrate)
						continue;					
					//  if (xx.Time < time && xx.Released > released)
					//  	continue;
				}
				seen[key2] = (time, released, flowrate);

				if (opened.Length > maxopen)
				{
					maxopen = opened.Length;
					Console.WriteLine($"maxopen={maxopen}");
				}

				if (time < minutes-1) // -1
				{
					if (!opened.Contains(v.Value.Name))
					{
						// Open it
						var opened2 = opened.Append(v.Value.Name).OrderBy(x=>x).ToArray();
						var flowrate2 = flowrate + v.Value.Flow;
						stack.Push((v, time+1, flowrate2, released+flowrate2, opened2));
					}
					foreach (var t in v.Edges.Where(e => time+e.Value < minutes-2)) // -2 because no sense in getting there if we can't open it and see it flow
					{
						var valve = t.Key;
						var name = valve.Value.Name;
						// if (seen.ContainsKey(name) && opened.Contains(name))
						// if (seen.ContainsKey(name) && opened.Contains(name))
						// 	continue;
					 	stack.Push((valve, time+t.Value, flowrate, released+flowrate*t.Value, opened)); // go there
					}
				}
			}


			return maxpressure;
		}

		internal class Valve
		{
			public int Number;
			public uint OpenBit;
			public string Name;
			public int Flow;
			public Valve[] Tunnels;
			public string[] TunnelNames;

		}

		protected override long Part2(string[] input)
		{
			var valves = input
				.Select(s =>
				{
					var (name, flow, _, __, ___, dir) = s.RxMatch("Valve %s has flow rate=%d; %s %s to %s %*").Get<string, int, string, string, string, string>();
					return new Valve
					{
						Name = name,
						Flow = flow,
						TunnelNames = dir.Split(", ").ToArray()				
					};
				})
				.ToArray();
			var n = 0;
			var bit = 1U;
			foreach (var v in valves)
			{
				v.Number = n++;
				if (v.Flow > 0)
				{
					v.OpenBit = bit;
					bit <<= 1;
				}
				v.Tunnels = v.TunnelNames.Select(x => valves.Single(v => v.Name == x)).ToArray();
			}
			var valvesWithFlow = valves.Count(x => x.Flow > 0);
			var minutes = 26;

			var seen = new Dictionary<ulong, (int Time, long Released)>();
			var start = valves.Single(x => x.Name == "AA");

			var maxpressure = 0L;
			var queue = new Queue<(Valve, Valve, int, long, long, int, uint)>();
			queue.Enqueue((start, start, 0, 0L, 0L, 0, 0));
			while (queue.Any())
			{
				var (you, ele, time, flowrate, released, openvalves, openbits) = queue.Dequeue();
			//	Console.WriteLine($"{v.Name} t={time} flow={flowrate} rel={released} open={openvalves} bits={openbits} #q={queue.Count()}");

				if (you.Number > ele.Number)
					(you, ele) = (ele, you);

				var key = (ulong)(((ulong)you.Number)<<24) | (ulong)(((ulong)ele.Number)<<16) | openbits;
				if (seen.TryGetValue(key, out var xx))
				{
					var (time2, released2) = xx;
					//Console.WriteLine($"  dt={time - time2}");
					// if (time >= time2)
					// {
						Debug.Assert(time >= time2);
						if (released <= released2 + (time - time2) * flowrate)
							continue;
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
				seen[key] = (time, released);

				if (time == minutes)
				{
					if (released > maxpressure)
						maxpressure = released;
					continue;
				}

				if (openvalves == valvesWithFlow)
				{
					var fullrelease = released + flowrate * (minutes - time);
					//Console.WriteLine(fullrelease);
					if (fullrelease > maxpressure)
						maxpressure = fullrelease;
					//queue.Enqueue((v, time+1, flowrate, released + flowrate, openvalves, openbits)); // just stay
				}
				else
				{
					var youCanOpen = you.Flow > 0 && (openbits & you.OpenBit) == 0;
					var eleCanOpen = ele.Flow > 0 && (openbits & ele.OpenBit) == 0;

					if (you == ele)
					{
						if (youCanOpen) // both can open; just let you do it
						{
							var nextopenvalves = openvalves + 1;
							var nextopenbits = openbits | you.OpenBit;
							var nextflowrate = flowrate + you.Flow;
							foreach (var nextele in ele.Tunnels)
							{
								queue.Enqueue((you, nextele, time+1, nextflowrate, released + flowrate,  nextopenvalves, nextopenbits)); // go there
							}
						}
						foreach (var nextyou in you.Tunnels)
						{
							foreach (var nextele in ele.Tunnels)
							{
								queue.Enqueue((nextyou, nextele, time+1, flowrate, released + flowrate, openvalves, openbits)); // go there
							}
						}	
					}
					else
					{
						if (youCanOpen && eleCanOpen)
						{
							var nextopenvalves = openvalves + 2;
							var nextopenbits = openbits | you.OpenBit | ele.OpenBit;
							var nextflowrate = flowrate + you.Flow + ele.Flow;
							queue.Enqueue((you, ele, time+1, nextflowrate, released + flowrate, nextopenvalves, nextopenbits)); // go and open
						}
						if (youCanOpen && !eleCanOpen)
						{
							var nextopenvalves = openvalves + 1;
							var nextopenbits = openbits | you.OpenBit;
							var nextflowrate = flowrate + you.Flow;
							foreach (var nextele in ele.Tunnels)
							{
								queue.Enqueue((you, nextele, time+1, nextflowrate, released + flowrate,  nextopenvalves, nextopenbits)); // go there
							}
						}
						if (!youCanOpen && eleCanOpen)
						{
							var nextopenvalves = openvalves + 1;
							var nextopenbits = openbits | ele.OpenBit;
							var nextflowrate = flowrate + ele.Flow;
							foreach (var nextyou in you.Tunnels)
							{
								queue.Enqueue((nextyou, ele, time+1, nextflowrate, released + flowrate,  nextopenvalves, nextopenbits)); // go there
							}
						}
						foreach (var nextyou in you.Tunnels)
						{
							foreach (var nextele in ele.Tunnels)
							{
								queue.Enqueue((nextyou, nextele, time+1, flowrate, released + flowrate, openvalves, openbits)); // go there
							}
						}	
					}

				}
			}


			return maxpressure;
		}


	}
}
