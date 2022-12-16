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
		Run("test1").Part1(1651).Part2(1707);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(1915).Part2(0); // 1331 too low, 2582 wrong
			// 649 not right
			
		}

		protected override long Part1(string[] input)
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

			// Console.WriteLine(valves.Count());
			Console.WriteLine(valves.Count(x => x.Flow > 0));
			// Console.WriteLine("digraph {");
			// foreach (var v in valves)
			// {
			// 	foreach (var e in v.Tunnels)
			// 	{
			// 		Console.WriteLine($"  \"{v.Name}\" -> \"{e.Name}\"");
			// 	}
			// }
			// Console.WriteLine("}");

			var minutes = 30;

			var seen = new Dictionary<ulong, (int Time, long Released)>();
			var start = valves.Single(x => x.Name == "AA");

			var maxpressure = 0L;
			var queue = new Queue<(Valve, int, long, long, int, uint)>();
			queue.Enqueue((start, 0, 0L, 0L, 0, 0));
			while (queue.Any())
			{
				var (v, time, flowrate, released, openvalves, openbits) = queue.Dequeue();
			//	Console.WriteLine($"{v.Name} t={time} flow={flowrate} rel={released} open={openvalves} bits={openbits} #q={queue.Count()}");

				var key = (ulong)v.Number<<32 | openbits;
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
					Console.WriteLine(fullrelease);
					if (fullrelease > maxpressure)
						maxpressure = fullrelease;
					//queue.Enqueue((v, time+1, flowrate, released + flowrate, openvalves, openbits)); // just stay
				}
				else
				{
					if (v.Flow > 0 && (openbits & v.OpenBit) == 0) // open it
					{
						var nextopenvalves = openvalves + 1;
						var nextopenbits = openbits | v.OpenBit;
						var nextflowrate = flowrate + v.Flow;
						queue.Enqueue((v, time+1, nextflowrate, released + flowrate, nextopenvalves, nextopenbits)); // go and open
					}
					foreach (var next in v.Tunnels)
					{
						queue.Enqueue((next, time+1, flowrate, released + flowrate, openvalves, openbits)); // go there
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

			// Console.WriteLine(valves.Count());
			Console.WriteLine(valves.Count(x => x.Flow > 0));
			// Console.WriteLine("digraph {");
			// foreach (var v in valves)
			// {
			// 	foreach (var e in v.Tunnels)
			// 	{
			// 		Console.WriteLine($"  \"{v.Name}\" -> \"{e.Name}\"");
			// 	}
			// }
			// Console.WriteLine("}");

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
					Console.WriteLine(fullrelease);
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
						// foreach (var nextyou in you.Tunnels)
						// {
						// 	foreach (var nextele in ele.Tunnels)
						// 	{
						// 		queue.Enqueue((nextyou, nextele, time+1, flowrate, released + flowrate, openvalves, openbits)); // go there
						// 	}
						// }
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


			return maxpressure;
		}


	}
}
