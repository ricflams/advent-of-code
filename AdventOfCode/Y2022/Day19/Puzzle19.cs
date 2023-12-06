using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Not Enough Minerals";
		public override int Year => 2022;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(33); // no part 2
			Run("input").Part1(1466).Part2(8250);
			Run("extra").Part1(1346).Part2(7644);
		}

		protected override long Part1(string[] input)
		{
			var sum = input
				.Select(Blueprint.Parse)
				.Select((b, idx) => b.FindMaxGeodesOpened(24) * (idx+1))
				.Sum();
			return sum;
		}

		protected override long Part2(string[] input)
		{
			var prod = input
				.Take(3)
				.Select(Blueprint.Parse)
				.Select(b => b.FindMaxGeodesOpened(32))
				.Prod();
			return prod;
		}		

		private class Blueprint
		{
			private record RobotDef(int[] Costs) {};
			private RobotDef[] Defs;
			private const int Ore = 0;
			private const int Clay = 1;
			private const int Obs = 2;
			private const int Geo = 3;

			private int MaxOreCost;
			private int MaxClayCost;
			private int MaxObsCost;

			public static Blueprint Parse(string s)
			{
				// Blueprint 16: Each ore robot costs 4 ore. Each clay robot costs 3 ore. Each obsidian robot costs 4 ore and 11 clay. Each geode robot costs 3 ore and 15 obsidian.
				var (_, a, b, c, d, e, f) = s.RxMatch("Blueprint %d: Each ore robot costs %d ore. Each clay robot costs %d ore. Each obsidian robot costs %d ore and %d clay. Each geode robot costs %d ore and %d obsidian.").Get<int, int, int, int, int, int, int>();
				return new Blueprint
				{
					Defs = new RobotDef[]
					{
						new RobotDef(new [] { a, 0, 0, 0 }),
						new RobotDef(new [] { b, 0, 0, 0 }),
						new RobotDef(new [] { c, d, 0, 0 }),
						new RobotDef(new [] { e, 0, f, 0 })
					},
					MaxOreCost = new[] {b, c, e}.Max(),
					MaxClayCost = d,
					MaxObsCost = f
				};
			}

			public int FindMaxGeodesOpened(int minutes)
			{
				var N = 4;

				var maxGeodes = 0;

				// Optimization
				var seenRobots = new Dictionary<long, List<int[]>>();

				var stack = new Stack<(int[], int[], int)>();
				var robots0 = new[] { 1, 0, 0, 0}; // 1 ore-producing robot
				var materials0 = new[] { 0, 0, 0, 0};
				stack.Push((robots0, materials0, 0));

				while (stack.TryPop(out var item))
				{
					var (robots, materials, time) = item;

					// Record the max number of geodes produced
					if (time == minutes)
					{
						var geodes = materials[Geo];
						if (geodes > maxGeodes)
							maxGeodes = geodes;
						continue;
					}

					// Optimization: Skip if this state can't ever possibly produce more
					// geodes than the maximum we've seen so far
					var maxmoreGeodes = robots[Geo] == 0
						? (minutes-time)*(minutes-time+1)/2
						: Enumerable.Range(robots[Geo], minutes - time).Sum();
					if (materials[Geo] + maxmoreGeodes <= maxGeodes)
						continue;

					// Optimization: Skip if we've seen this set of robots before in an
					// earlier state at the same time, but with less materials been produced
					static long Key(int[] x) => (long)(((long)x[0]<<21) + ((long)x[1]<<14) + (x[2]<<7) + x[3]);
					var rstate = (long)Key(robots) * minutes + time;
					if (!seenRobots.TryGetValue(rstate, out var seenMaterials))
						seenMaterials = seenRobots[rstate] = new List<int[]>();
					if (seenMaterials.Any(m => IsAllLessThanOrEqual(materials, m)))
						continue;
					foreach (var lower in seenMaterials.Where(m => IsAllLessThanOrEqual(m, materials)).ToArray())
						seenMaterials.Remove(lower);
					seenMaterials.Add(materials);

					bool IsAllLessThanOrEqual(int[] a, int[] b)
					{
						for (var i = 0; i < N; i++)
							if (a[i] > b[i])
								return false;
						return true;
					}

					// Check if we've got robots enough already to produce any kind of
					// desired robot at every round so we don't need to build more of them
					var hasEnoughObs = robots[Obs] >= MaxObsCost; // possibly fewer needed, not counting existing materials
					var hasEnoughOre = robots[Ore] >= MaxOreCost;
					var hasEnoughClay = materials[Clay] >= MaxClayCost;

					// If we've not yet enough Obs/Orb robots then idle for a round
					if (!hasEnoughObs || !hasEnoughOre)
					{
						var m2 = materials.ToArray();
						for (var i = 0; i < N; i++)
							m2[i] += robots[i];
						stack.Push((robots, m2, time + 1));
					}

					// We always want to try to build a Geodes-robot. If there's time left
					// for newly built robots to actually be productive then we also want
					// to try to build them; but only those we need.

					TryBuildRobot(Geo);
					if (time < minutes)
					{
						if (!hasEnoughObs)
							TryBuildRobot(Obs);
						if (!hasEnoughClay)
							TryBuildRobot(Clay);
						if (!hasEnoughOre)
							TryBuildRobot(Ore);
					}

					void TryBuildRobot(int material)
					{
						// Check if there's enough materials for this type of robot
						var costs = Defs[material].Costs;
						for (var i = 0; i < N; i++)
						{
							if (costs[i] > materials[i])
								return;
						}

						// Build new robot out of materials and add it to set of robots
						var r2 = robots.ToArray();
						var m2 = materials.ToArray();
						for (var i = 0; i < N; i++)
						{
							m2[i] -= costs[i];
						}
						r2[material]++;

						// Let existing robots produce their materials and submit the state
						for (var i = 0; i < N; i++)
							m2[i] += robots[i];
						stack.Push((r2, m2, time + 1));
					}				
				}

				return maxGeodes;
			}
		}
	}
}
