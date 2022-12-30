using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 19";
		public override int Year => 2022;
		public override int Day => 19;

		public void Run()
		{
//			Run("test1").Part1(33); // no part 2
//			Run("test9").Part1(1346).Part2(7644);
			Run("input").Part1(1466);//.Part2(8250);
		}

		private const int Ore = 0;
		private const int Clay = 1;
		private const int Obs = 2;
		private const int Geo = 3;
		// private enum Material
		// {
		// 	Ore = 0, Clay, Obs, Geo
		// }
		private record RobotDef(int[] Costs);
		private class Blueprint
		{
			public RobotDef[] Defs;
			public int ValueOf(int[] m)
			{
				var a = Defs[0].Costs[0];
				var b = Defs[1].Costs[0];
				var c = Defs[2].Costs[0] + Defs[2].Costs[1] * b;
				var d = Defs[3].Costs[0] + Defs[3].Costs[2] * c;
				var value = a*m[0] + b*m[1] + c*m[2] + d*m[3];
				return value;
			}
		}

		protected override long Part1(string[] input)
		{
			// Blueprint 16: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 4 ore and 11 clay. Each geode robot costs 3 ore and 15 obsidian.
			var blueprints = input
				.Select(s =>
				{
					var (_, a, b, c, d, e, f) = s.RxMatch("Blueprint %d: Each ore robot costs %d ore. Each clay robot costs %d ore. Each obsidian robot costs %d ore and %d clay. Each geode robot costs %d ore and %d obsidian.").Get<int, int, int, int, int, int, int>();
					return new Blueprint
					{
						Defs = new RobotDef[]
						{
							new RobotDef(new [] { a, 0, 0, 0 }),
							new RobotDef(new [] { b, 0, 0, 0 }),
							new RobotDef(new [] { c, d, 0, 0 }),
							new RobotDef(new [] { e, 0, f, 0 })
						}
					};
				})
				.ToArray();

			var sum = blueprints
				.Select((b, idx) => FindMaxGeodesOpened(b, idx, 24) * (idx+1))
				.Sum();

			return sum;
		}



		protected override long Part2(string[] input)
		{
			// Blueprint 16: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 4 ore and 11 clay. Each geode robot costs 3 ore and 15 obsidian.
			var blueprints = input
				.Take(3)
				.Select(s =>
				{
					var (_, a, b, c, d, e, f) = s.RxMatch("Blueprint %d: Each ore robot costs %d ore. Each clay robot costs %d ore. Each obsidian robot costs %d ore and %d clay. Each geode robot costs %d ore and %d obsidian.").Get<int, int, int, int, int, int, int>();
					return new Blueprint
					{
						Defs = new RobotDef[]
						{
							new RobotDef(new [] { a, 0, 0, 0 }),
							new RobotDef(new [] { b, 0, 0, 0 }),
							new RobotDef(new [] { c, d, 0, 0 }),
							new RobotDef(new [] { e, 0, f, 0 })
						}
					};
				})
				.ToArray();

			var prod = blueprints
				.Select((b, idx) => (int)FindMaxGeodesOpened(b, idx,32))
				.Prod();
			return prod;
		}		

		private static long FindMaxGeodesOpened(Blueprint bp, int index, int minutes)
		{
			var seen = new Dictionary<long, List<int[]>>();
			var seenhighest = new Dictionary<int, int[]>();
			var seenvalue= new Dictionary<int, int>();
			var seenrobots = new Dictionary<long, int>();
			var seengeodes = new Dictionary<long, int>();
			var statesseen = new HashSet<long>();

			long Key(int[] x) {
				var x0 = x[0]<<21;
				var x1 = x[1]<<14;
				var x2 = x[2]<<7;
				var x3 = x[3];
				var xx = x0 + x1 + x2 + x3;
				var xxx = (long)xx;
				return xxx;//(long)(x0 + x[1]<<16 + x[2]<<8 + x[3]);
			}
			long State(int[] materials, int[] robots) => (Key(materials) << 28) + Key(robots);

			var highestgeodes = new Dictionary<int, int>();

			var materials0 = new[] { 0, 0, 0, 0}; // 1 ore
			var robots0 = new[] { 1, 0, 0, 0}; // 1 ore-producing robot
			var state0 = State(materials0, robots0);

			var N = 4;

			var maxtime = 0;
			var maxproduced = 0L;
			var queue = new PriorityQueue<(long State, int[] Materials, int[] Robots, int Time), int>();
			queue.Enqueue((state0, materials0, robots0, 0), 0);
			while (queue.TryDequeue(out var item, out var _))
			{
				var (state, materials, robots, time) = item;
				//Console.WriteLine($"t={time} #queue={queue.Count} robots={string.Join(' ', robots)} materials={string.Join(' ', materials)}");

				if (time > maxtime)
				{
				//	Console.WriteLine($"time={time}");
					maxtime = time;
				}
				var geodes = materials[Geo];

				if (time == minutes)
				{
					//Console.WriteLine($"t={time} robots={string.Join(' ', robots)} materials={string.Join(' ', materials)}");
					if (geodes > maxproduced)
					{
						maxproduced = geodes;
				//		Console.WriteLine($"t={time} #queue={queue.Count} robots={string.Join(' ', robots)} materials={string.Join(' ', materials)} max={maxproduced}");
					}
					break;
					continue;
				}

				var maxmore = Enumerable.Range(robots[Geo], minutes - time).Sum();
				if (materials[Geo] + maxmore <= maxproduced)
					continue;


				var rstate = (long)Key(robots) * minutes + time;

				if (!seen.TryGetValue(rstate, out var mats))
				{
					mats = seen[rstate] = new List<int[]>();
				}
				if (mats.Any(m => IsAllLessThanOrEqual(materials, m)))
					continue;
				foreach (var lower in mats.Where(m => IsAllLessThanOrEqual(m, materials)).ToArray())
				{
					//Console.Write("-");
					mats.Remove(lower);
				}
				mats.Add(materials);


				bool IsAllLessThanOrEqual(int[] a, int[] b)
				{
					for (var i = 0; i < N; i++)
						if (a[i] > b[i])
							return false;
					return true;
				}


				bool CanBuildRobot(int[] m2, int material)
				{
					var costs = bp.Defs[material].Costs;
					for (var i = 0; i < N; i++)
					{
						if (costs[i] > m2[i])
							return false;
					}
					return true;
				}

				void BuildRobot(int[] r2, int[] m2, int material)
				{
					var costs = bp.Defs[material].Costs;
					for (var i = 0; i < N; i++)
					{
						m2[i] -= costs[i];
					}
					r2[material]++;
				}	

				for (var material = N-1; material >= 0; material--)
				{
					var r2 = robots.ToArray();
					var m2 = materials.ToArray();
					if (CanBuildRobot(m2, material))
					{
						BuildRobot(r2, m2, material);
					}

					for (var i = 0; i < N; i++)
					{
						m2[i] += robots[i];
					}

					var s2 = State(m2, r2) * minutes + time;
					if (statesseen.Contains(s2))
						continue;
					statesseen.Add(s2);

					var maxmore2 = Enumerable.Range(r2[Geo], minutes - (time + 1)).Sum();
					var m2maxproduce = m2[Geo] + maxmore2;
					if (m2maxproduce <= maxproduced)
						continue;

					//var prio = 10000000*(minutes-time) - ((r2[Geo]+m2[Geo])*10000 + (r2[Obs]+m2[Obs])*100); 250sec
					//var prio = time*5 - ((r2[Geo]+m2[Geo])*10 + (r2[Obs]+m2[Obs])); // 11sec
					//var prio = time*10 - ((r2[Geo]+m2[Geo])*10 + (r2[Obs]+m2[Obs])); // 11sec
					var prio = -m2maxproduce;
					
					//queue.Enqueue((s2, m2, r2, time + 1), prio);
					queue.Enqueue((s2, m2, r2, time + 1), prio);
				}


			}

			Console.WriteLine($"##### {index} max={maxproduced}");
			// Console.WriteLine();
			return maxproduced;

		}



	}
}
