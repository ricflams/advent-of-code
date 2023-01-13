using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day16
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Proboscidea Volcanium";
		public override int Year => 2022;
		public override int Day => 16;

		public void Run()
		{
			Run("test1").Part1(1651).Part2(1707);

			// https://www.reddit.com/r/adventofcode/comments/znklnh/2022_day_16_some_extra_test_cases_for_day_16/
			Run("test2").Part1(2640).Part2(2670);
			Run("test3").Part1(13468).Part2(12887);
			Run("test4").Part1(1288).Part2(1484);
			Run("test5").Part1(2400).Part2(3680);

		 	Run("test9").Part1(1850).Part2(2306);
		 	Run("input").Part1(1915).Part2(2772);
		}

		protected override long Part1(string[] input)
		{
			var volcano = new Volcano(input);

			return volcano.AllMaximums(30).Max();
		}

		protected override long Part2(string[] input)
		{
			var volcano = new Volcano(input);

			// Find the maximums of all viable routes. The maximums is an array of ints
			// indexed by a bitmask indicating the visited nodes (eg 1011=A_CD). Weed out
			// the nearly 90% unviable routes so we get a list of (nodeset,maximum) to
			// look at in the next step.
			var routes = volcano
				.AllMaximums(26)
				.Select((distance, nodeset) => (Nodeset: (uint)nodeset, Distance: distance))
				.Where(x => x.Distance != 0)
				.ToArray();

			// Loop all combinations and check the combined total of any pair that doesn't
			// share any visited nodes, ie where nodeset1 & nodeset2 == 0.
			var max = 0;
			for (var i = 0; i < routes.Length; i++)
			{
				for (var j = i+1; j < routes.Length; j++)
				{
					if ((routes[i].Nodeset & routes[j].Nodeset) == 0)
					{
						var total = routes[i].Distance + routes[j].Distance;
						if (total > max)
							max = total;
					}
				}
			}
			return max;
		}


		private class Volcano
		{
			private readonly Graphx<Valve> _graph = new();
			private readonly int[,] _distances;
			private readonly int[] _distancesFromStart;

			internal class Valve : GraphxNode
			{
				public int Flow;
				public uint Bit;
				public override string ToString() => $"{Name} Index={Index} Bit={Bit} Flow={Flow}";
			}

			public Volcano(string[] input)
			{
				// Parse the valves and add the nodes+edges to a graph
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
				foreach (var v in valves.Values)
				{
					foreach (var tn in v.TunnelNames)
					{
						var t = valves[tn];
						var node = _graph.AddEdge(v.Name, t.Name, 1);
						node.Flow = v.Flow;
					}
				}

				// Reduce away all the valve-less nodes except AA. Then calculate the
				// distance from AA to any other node (for the initial step) and reduce
				// AA itself away, leaving only nodes that has a valve to open.
				_graph.Reduce(v => v.Name != "AA" && v.Flow == 0);
				var start = _graph["AA"];
				var distancesFromStart = _graph.ShortestPathToAllDijkstra(start);
				_graph.Reduce(v => v.Flow == 0);

				// Assign a bit to each node for more efficient nodesets 
				var bit = 1u;
				foreach (var n in _graph.Nodes)
				{
					n.Bit = bit;
					bit <<= 1;
				}

				// Find all the shortest paths and also create a newly indexed start-distances
				_distances = _graph.FloydWarshallShortestPaths();
				_distancesFromStart = distancesFromStart
					.Where(x => x.Key != start)
					.OrderBy(x => x.Key.Index)
					.Select(x => x.Value)
					.ToArray();
			}

			public int[] AllMaximums(int minutes)
			{
				var valves = _graph.Nodes;

				// Store all maximums in an array big enough to hold all nodeset-combinations.
				// The Explore-function will always assume it's looking at a newly opened valve,
				// so the first steps should be to spend x minutes moving to the valve and 1
				// minute opening it. The first steps tage 1+dist+1 min because start is t=1 min.
				var maximums = new int[1u << valves.Count];
				foreach (var v in valves)
				{
					var dist = _distancesFromStart[v.Index];
					Explore(v, 1 + dist+1, v.Flow, v.Flow, v.Bit);
				}
				return maximums;

				void Explore(Volcano.Valve valve, int t, int flowrate, int released, uint opened)
				{
					// Remember the highest total release for this particular set of nodes
					var total = released + flowrate * (minutes - t);
					if (total > maximums[opened])
						maximums[opened] = total;

					// Visit each un-opened valve
					foreach (var v in valves)
					{
						if ((opened & v.Bit) != 0)
							continue;
						// Only explore this valve if we can visit it before time runs out. It will
						// be reached in the t+dist minute and then it takes 1 minute to open it; only
						// if there's time left (ie that time is at most minutes) will it yield any
						// flow, so only explore it if t+dist+1 <= minute.
						var dist = _distances[valve.Index, v.Index];
						if (t+dist+1 <= minutes)
						{
							// Go there and open it. The parameters passed here is what the flowrate
							// and released amount will be at that next time; only in the last minute
							// of the dist+1 duration does the new flowrate kick in.
							var opened2 = opened | v.Bit;
							var flowrate2 = flowrate + v.Flow;
							Explore(v, t+dist+1, flowrate2, released + flowrate*dist + flowrate2, opened2);
						}
					}
				}
			}
		}		
	}
}
