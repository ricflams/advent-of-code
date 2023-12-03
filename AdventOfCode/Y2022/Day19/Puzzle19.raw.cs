using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day19.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 19";
		public override int Year => 2022;
		public override int Day => 19;

		public override void Run()
		{
			//Run("test1").Part1(33);//.Part2(0);
			//Run("test2").Part2(0);
			//Run("input").Part1(1466).Part2(0);
			Run("input").Part2(8250);
			// 1670 too high
			// 1421 too low

			// 49500 too high
			// 4560 too low for part 2
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
			var statesseen = new HashSet<string>();

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

			var highestgeodes = new Dictionary<int, int>();

			var materials0 = new[] { 0, 0, 0, 0}; // 1 ore
			var robots0 = new[] { 1, 0, 0, 0}; // 1 ore-producing robot
			var state0 = State(materials0, robots0);
			// var x1 = (long)(materials0[0]<<24 + materials0[1]<<16 + materials0[2]<<8 + materials0[3]);
			// var mm = Key(materials0);
			// var rr = Key(robots0);
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
					Console.WriteLine($"time={time}");
					maxtime = time;
				}
				var geodes = materials[Geo];
				// if (highestgeodes.TryGetValue(time, out var g))
				// {
				// 	if (g-1 > geodes)
				// 		continue;
				// 	if (g < geodes)
				// 		highestgeodes[time] = geodes;
				// }
				// else
				// 	highestgeodes[time] = geodes;

				if (time == minutes)
				{
					//Console.WriteLine($"t={time} robots={string.Join(' ', robots)} materials={string.Join(' ', materials)}");
					if (geodes > maxproduced)
					{
						maxproduced = geodes;
						Console.WriteLine($"t={time} #queue={queue.Count} robots={string.Join(' ', robots)} materials={string.Join(' ', materials)} max={maxproduced}");
					}
					continue;
				}

				var maxmore = Enumerable.Range(robots[Geo], minutes - time).Sum();
				if (materials[Geo] + maxmore <= maxproduced)
					continue;

				// var valueProduced = bp.ValueOf(materials);// + bp.ValueOf(robots);
				// if (!seenvalue.TryGetValue(time, out var seenval))
				// {
				// 	seenval = seenvalue[time] = valueProduced;
				// }
				// if (valueProduced < seenval/2)
				// 	continue;
				// seenvalue[time] = valueProduced;


				// if (maxproduced > 0)// && time > minutes - 4)
				// {
				// 	if (materials[Geo] + (minutes - time) * robots[Geo] < maxproduced - (minutes-time)*2) // -4 is shady
				// 		continue;
				// }



				// if (maxproduced > 1 && geodes == 0) // let's just
				// 	continue;

				// var rstate = (long)Key(robots) * 100 + time;

				// if (!seen.TryGetValue(rstate, out var mats))
				// {
				// 	mats = seen[rstate] = new List<int[]>();
				// }
				// if (mats.Any(m => IsAllLessThanOrEqual(materials, m)))
				// 	continue;
				// foreach (var lower in mats.Where(m => IsAllLessThanOrEqual(m, materials)).ToArray())
				// {
				// 	//Console.Write("-");
				// 	mats.Remove(lower);
				// }
				// mats.Add(materials);




				// if (seenhighest.TryGetValue(rstate, out var highest))
				// {
				// 	if (IsGreaterThanOrEqual(highest, materials))
				// 		continue;
				// }
				// seenhighest[rstate] = materials;

				// if (seen.TryGetValue(state, out var t) && t <= time)
				// 	continue;
				// seen[state] = time;

				// if (materials[Ore] > 12)
				// 	continue;
				// if (materials[Clay] > 40)
				// 	continue;					

				// var rstate = Key(robots);
				// if (seenrobots.TryGetValue(rstate, out var geodes2))
				// {
				// 	if (geodes2 > materials[Geo])
				// 		continue;
				// }
				// seenrobots[rstate] = materials[Geo];
				// bool IsAllLessThanOrEqual(int[] a, int[] b)
				// {
				// 	for (var i = 0; i < N; i++)
				// 		if (a[i] > b[i])
				// 			return false;
				// 	return true;
				// }
	
				// bool IsGreaterThanOrEqual(int[] a, int[] b)
				// {
				// 	for (var i = 0; i < N; i++)
				// 		if (a[i] < b[i])
				// 			return false;
				// 	return true;
				// }




				// // Find max number of robots of each kind that these materials could produce
				// var canbuild = robotdefs
				// 	.Select(def =>
				// 	{
				// 		var max = int.MaxValue;
				// 		for (var i = 0; i < N; i++)
				// 		{
				// 			var available = materials[i];
				// 			var cost = def.Costs[i];
				// 			if (cost == 0)
				// 				continue;
				// 			var canbuild = available / cost;
				// 			if (canbuild < max)
				// 				max = canbuild;
				// 		}
				// 		return max == int.MaxValue ? 0 : max;
				// 	})
				// 	.ToArray();


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

				for (var material = 0; material < N; material++)
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

					var s2 = State(m2, r2);
					if (statesseen.Contains($"{s2}-{time}"))
						continue;
					statesseen.Add($"{s2}-{time}");

					var maxmore2 = Enumerable.Range(r2[Geo], minutes - (time + 1)).Sum();

					if (m2[Geo] + maxmore2 <= maxproduced)
						continue;


					var prio = 10000000 - ((r2[Geo]+m2[Geo])*10000 + (r2[Obs]+m2[Obs])*100);
					queue.Enqueue((s2, m2, r2, time + 1), prio);
				}


				// for (var a = 0; a <= canbuild[3]; a++)
				// {
				// 	for (var b = 0; b <= canbuild[2]; b++)
				// 	{
				// 		for (var c = 0; c <= canbuild[1]; c++)
				// 		{
				// 			for (var d = 0; d <= canbuild[0]; d++)
				// 			{
				// 				var r2 = robots.ToArray();
				// 				var m2 = materials.ToArray();
				// 				// Try building one of these
				// 				for (var i = 0; i < a && CanBuildRobot(m2, 3); i++)
				// 				{
				// 					BuildRobot(r2, m2, 3);
				// 				}
				// 				for (var i = 0; i < b && materials[2] < robotdefs[3].Costs[2]*2 && CanBuildRobot(m2, 2); i++)
				// 				{
				// 					BuildRobot(r2, m2, 2);
				// 				}
				// 				for (var i = 0; i < c && materials[1] < robotdefs[2].Costs[1]*2 && CanBuildRobot(m2, 1); i++)
				// 				{
				// 					BuildRobot(r2, m2, 1);
				// 				}
				// 				for (var i = 0; i < d && materials[0] < robotdefs[1].Costs[0]*2 && CanBuildRobot(m2, 0); i++)
				// 				{
				// 					BuildRobot(r2, m2, 0);
				// 				}

				// 				var used = new int[N];
				// 				for (var i = 0; i < N; i++)
				// 				{
				// 					var newbots = r2[i] - robots[i];
				// 					for (var j = 0; j < N; j++)
				// 					{
				// 						used[j] += newbots*robotdefs[i].Costs[j];
				// 					}
				// 				}
				// 				for (var i = 0; i < N; i++)
				// 				{
				// 					Debug.Assert(materials[i] == m2[i] + used[i]);
				// 				}

				// 				for (var i = 0; i < N; i++)
				// 				{
				// 					m2[i] += robots[i];
				// 				}

				// 				if (m2[Geo] == 10 && r2[Geo] == 5)
				// 					;

				// 				//Console.WriteLine($"  next: t={time+1} robots={string.Join(' ', r2)} materials={string.Join(' ', m2)}");
				// 				var s2 = State(m2, r2);
				// 				if (statesseen.Contains(s2))
				// 					continue;
				// 				statesseen.Add(s2);

				// 				// var rstate2 = (int)Key(r2);
				// 				// if (seenhighest.TryGetValue(rstate2, out var highest2))
				// 				// {
				// 				// 	if (IsGreaterThanOrEqual(highest2, m2))
				// 				// 		continue;
				// 				// }

				// 				var prio = 10000000 - ((r2[Geo]+m2[Geo])*10000 + (r2[Obs]+m2[Obs])*100);

				// 				queue.Enqueue((s2, m2, r2, time + 1), prio);

				// 				bool CanBuildRobot(int[] m2, int material)
				// 				{
				// 					var costs = robotdefs[material].Costs;
				// 					for (var i = 0; i < N; i++)
				// 					{
				// 						if (costs[i] > m2[i])
				// 							return false;
				// 					}
				// 					return true;
				// 				}

				// 				void BuildRobot(int[] r2, int[] m2, int material)
				// 				{
				// 					var costs = robotdefs[material].Costs;
				// 					for (var i = 0; i < N; i++)
				// 					{
				// 						m2[i] -= costs[i];
				// 					}
				// 					r2[material]++;
				// 				}	
				// 			}
				// 		}
				// 	}
				// }
			}

			Console.WriteLine($"##### {index} max={maxproduced}");
			Console.WriteLine();
			return maxproduced;




				// while (CanBuildRobot(Geo))
				// {
				// 	BuildNewRobot(Geo);
				// }
				// while (true)
				// {
				// 	BuildClayAndOreRobots();
				// 	if (CanBuildRobot(Obs))
				// 		BuildNewRobot(Obs);
				// 	else
				// 		break;
				// }

				// void BuildClayAndOreRobots()
				// {
				// 	var newrobots = robots.ToArray();
				// 	var newmaterials = materials.ToArray();

				// 	var maxClayRobots = materials[Ore] / robotdefs[Clay].Costs[Ore];
				// 	var maxOreRobots = materials[Ore] / robotdefs[Ore].Costs[Ore];
				// 	for (var nclays = 0; nclays <= maxClayRobots; nclays++)
				// 	{
				// 		for (var nOre = 0; nOre <= maxOreRobots; nOre++)
				// 		{
				// 			robots = newrobots.ToArray();
				// 			materials = newmaterials.ToArray();
				// 			for (var ic = 0; ic < nclays && CanBuildRobot(Clay); ic++)
				// 			{
				// 				BuildNewRobot(Clay);
				// 			}
				// 			for (var io = 0; io < nOre && CanBuildRobot(Ore); io++)
				// 			{
				// 				BuildNewRobot(Ore);
				// 			}
				// 			for (var i = 0; i < N; i++)
				// 			{
				// 				materials[i] += oldrobots[i];
				// 			}
				// 			queue.Enqueue((State(materials, robots), materials, robots, time + 1));


				// 			robots = newrobots.ToArray();
				// 			materials = newmaterials.ToArray();
				// 			for (var io = 0; io < nOre && CanBuildRobot(Ore); io++)
				// 			{
				// 				BuildNewRobot(Ore);
				// 			}
				// 			for (var ic = 0; ic < nclays && CanBuildRobot(Clay); ic++)
				// 			{
				// 				BuildNewRobot(Clay);
				// 			}
				// 			for (var i = 0; i < N; i++)
				// 			{
				// 				materials[i] += oldrobots[i];
				// 			}
				// 			queue.Enqueue((State(materials, robots), materials, robots, time + 1));
				// 		}
				// 	}
				// 	robots = newrobots.ToArray();
				// 	materials = newmaterials.ToArray();									
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

			//}



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
		}



	}
}
