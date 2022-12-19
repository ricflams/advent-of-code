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
			Run("test1").Part1(33).Part2(0);
			//Run("test2").Part1(0).Part2(0);
			//Run("input").Part1(0).Part2(0);
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

		protected override long Part1(string[] input)
		{
			// Blueprint 16: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 4 ore and 11 clay. Each geode robot costs 3 ore and 15 obsidian.
			var blueprints = input
				.Select(s =>
				{
					var (_, a, b, c, d, e, f) = s.RxMatch("Blueprint %d: Each ore robot costs %d ore. Each clay robot costs %d ore. Each obsidian robot costs %d ore and %d clay. Each geode robot costs %d ore and %d obsidian.").Get<int, int, int, int, int, int, int>();
					return new RobotDef[]
					{
						new RobotDef(new [] { a, 0, 0, 0 }),
						new RobotDef(new [] { b, 0, 0, 0 }),
						new RobotDef(new [] { c, d, 0, 0 }),
						new RobotDef(new [] { e, 0, f, 0 })
					};
				})
				.ToArray();

			var sum = blueprints
				.Select((b, idx) => FindMaxGeodesOpened(b, 24) * (idx+1))
				.Sum();

			return sum;
		}

		private static long FindMaxGeodesOpened(RobotDef[] robotdefs, int minutes)
		{
			var seen = new Dictionary<long, int>();
			var seenrobots = new Dictionary<long, int>();
			var seengeodes = new Dictionary<long, int>();

			long Key(int[] x) {
				var x0 = x[0]<<24;
				var x1 = x[1]<<16;
				var x2 = x[2]<<8;
				var x3 = x[3];
				var xx = x0 + x1 + x2 + x3;
				var xxx = (long)xx;
				return xxx;//(long)(x0 + x[1]<<16 + x[2]<<8 + x[3]);
			}
			long State(int[] materials, int[] robots) => (Key(materials) << 32) + Key(robots);

			var materials0 = new[] { 0, 0, 0, 0}; // 1 ore
			var robots0 = new[] { 1, 0, 0, 0}; // 1 ore-producing robot
			var state0 = State(materials0, robots0);
			// var x1 = (long)(materials0[0]<<24 + materials0[1]<<16 + materials0[2]<<8 + materials0[3]);
			// var mm = Key(materials0);
			// var rr = Key(robots0);
			var N = 4;

			var maxtime = 0;
			var maxproduced = 0L;
			var queue = new Queue<(long State, int[] Materials, int[] Robots, int Time)>();
			queue.Enqueue((state0, materials0, robots0, 0));
			while (queue.Any())
			{
				var (state, materials, robots, time) = queue.Dequeue();

				if (seen.TryGetValue(state, out var t) && t <= time)
					continue;
				seen[state] = time;

				if (materials[Ore] > 12)
					continue;
				if (materials[Clay] > 30)
					continue;					

				// var rstate = Key(robots);
				// if (seenrobots.TryGetValue(rstate, out var geodes2))
				// {
				// 	if (geodes2 >= materials[(int)Material.Geo])
				// 		continue;
				// }

				if (time > maxtime)
				{
					Console.WriteLine($"time={time}");
					maxtime = time;
				}

				if (time == minutes)
				{
					var geodes = materials[Geo];
					if (geodes > maxproduced)
						maxproduced = geodes;
					continue;
				}

				var oldrobots = robots.ToArray();

				while (CanBuildRobot(Geo))
				{
					BuildNewRobot(Geo);
				}
				while (true)
				{
					BuildClayAndOreRobots();
					if (CanBuildRobot(Obs))
						BuildNewRobot(Obs);
					else
						break;
				}

				void BuildClayAndOreRobots()
				{
					var newrobots = robots.ToArray();
					var newmaterials = materials.ToArray();

					var maxClayRobots = materials[Ore] / robotdefs[Clay].Costs[Ore];
					var maxOreRobots = materials[Ore] / robotdefs[Ore].Costs[Ore];
					for (var nclays = 0; nclays <= maxClayRobots; nclays++)
					{
						for (var nOre = 0; nOre <= maxOreRobots; nOre++)
						{
							robots = newrobots.ToArray();
							materials = newmaterials.ToArray();
							for (var ic = 0; ic < nclays && CanBuildRobot(Clay); ic++)
							{
								BuildNewRobot(Clay);
							}
							for (var io = 0; io < nOre && CanBuildRobot(Ore); io++)
							{
								BuildNewRobot(Ore);
							}
							for (var i = 0; i < N; i++)
							{
								materials[i] += oldrobots[i];
							}
							queue.Enqueue((State(materials, robots), materials, robots, time + 1));


							robots = newrobots.ToArray();
							materials = newmaterials.ToArray();
							for (var io = 0; io < nOre && CanBuildRobot(Ore); io++)
							{
								BuildNewRobot(Ore);
							}
							for (var ic = 0; ic < nclays && CanBuildRobot(Clay); ic++)
							{
								BuildNewRobot(Clay);
							}
							for (var i = 0; i < N; i++)
							{
								materials[i] += oldrobots[i];
							}
							queue.Enqueue((State(materials, robots), materials, robots, time + 1));
						}
					}
					robots = newrobots.ToArray();
					materials = newmaterials.ToArray();					
				}

				bool CanBuildRobot(int mat)
				{
					var costs = robotdefs[mat].Costs;
					for (var i = 0; i < N; i++)
					{
						if (materials[i] < costs[i])
							return false;
					}
					return true;
				}

				void BuildNewRobot(int mat)
				{
					var costs = robotdefs[mat].Costs;
					for (var i = 0; i < N; i++)
					{
						materials[i] -= costs[i];
					}
					robots[mat]++;
				}
				
				// void DoBuilds(int[][] builds)
				// {
				// 	foreach (var build in builds)
				// 	{
				// 		DoBuild(build);
				// 	}
				// }

				// void DoBuild(int[] build)
				// {
				// 	var m2 = materials.ToArray();
				// 	for (var i = 0; i < N; i++)
				// 	{
				// 		m2[i] += robots[i];
				// 		m2[i] -= build.Zip(robotdefs).Select(x => x.First * x.Second.Costs[i]).Sum();
				// 	}
				// 	var r2 = robots.ToArray();
				// 	for (var i = 0; i < N; i++)
				// 	{
				// 		r2[i] += build[i];
				// 	}
				// 	// if (m2.Any(x => x < 0))
				// 	// 	;
				// 	queue.Enqueue((State(m2, r2), m2, r2, time + 1));
				// }

			}



			// IEnumerable<int[]> BuildRobots(int[] materials)
			// {
			// 	// Find max number of robots of each kind that these materials could produce
			// 	var canbuild = robotdefs
			// 		.Select(def =>
			// 		{
			// 			var max = int.MaxValue;
			// 			for (var i = 0; i < N; i++)
			// 			{
			// 				var available = materials[i];
			// 				var cost = def.Costs[i];
			// 				if (cost == 0)
			// 					continue;
			// 				var canbuild = available / cost;
			// 				if (canbuild < max)
			// 					max = canbuild;
			// 			}
			// 			return max == int.MaxValue ? 0 : max;
			// 		})
			// 		.ToArray();
			// 	for (var a = 0; a <= canbuild[0]; a++)
			// 	{
			// 		for (var b = 0; b <= canbuild[1]; b++)
			// 		{
			// 			for (var c = 0; c <= canbuild[2]; c++)
			// 			{
			// 				for (var d = 0; d <= canbuild[3]; d++)
			// 				{
			// 					if (a+b+c+d == 0)								
			// 						continue;
			// 					var robots = new [] { a, b, c, d };
			// 					yield return robots;
			// 				}
			// 			}
			// 		}
			// 	}
			// }

			Console.WriteLine($"max={maxproduced}");
			return maxproduced;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}

	}
}
