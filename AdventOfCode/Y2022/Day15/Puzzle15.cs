using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day15
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Beacon Exclusion Zone";
		public override int Year => 2022;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(26).Part2(56000011);
			Run("input").Part1(5403290).Part2(10291582906626);
			Run("extra").Part1(5564017).Part2(11558423398893);
		}

		protected override long Part1(string[] input)
		{
			var (sensors, beacons) = ReadSensors(input);

			// Find the coverages of all sensors that reach the desired Y-coordinate
			// and then reduce those coverage-intervals as much as possible
			var y0 = sensors.Length == 14 ? 10 : 2000000;
			var coverages = sensors
				.Select(s =>
				{
					var dy0 = Math.Abs(s.P.Y - y0);
					var w = s.Size - dy0;
					return (s.P.X, Width: w);
				})
				.Where(s => s.Width >= 0)
				.Select(s => new Interval<int>(s.X - s.Width, s.X + s.Width + 1))
				.Reduce();

			// Beacons can't appear in these spots, not counting spots where a beacon already exists
			var nonBeaconSpots = coverages.Sum(r => r.Length);
			nonBeaconSpots -= beacons.Count(b => b.Y == y0 && coverages.Any(r => r.Contains(b.X)));

			return nonBeaconSpots;	
		}

		protected override long Part2(string[] input)
		{
			var (sensors, _) = ReadSensors(input);

			// Find all sensors that have exactly 1 space between them
			// Their size is to their edge so the manhattan distance between two sensors
			// that are right up against eachother is their sizes +1 (+1 to go from one
			// to the other) so if they are to be exactly one space apart we should look
			// for sensors where that distance is one more; ie add TWO to their sizes.
			var neighbors = MathHelper.Combinations(sensors, 2)
				.Select(x => (S1:x[0], S2:x[1]))
				.Where(x => x.S1.P.ManhattanDistanceTo(x.S2.P) == x.S1.Size + x.S2.Size + 2)
				.ToArray();

			// For all combinations of sensor-pairs that are close, calculate the two "lines"
			// running in between them and see if they intersect. If they do and that spot is
			// vacant then we've found the answer.
			// In practice the input has just exactly two such sensor-pairs, ie two lines, ie
			// just enough to solve the puzzle.
			foreach (var n in MathHelper.Combinations(neighbors, 2))
			{
				var (a1, b1) = Line(n[0].S1, n[0].S2);
				var (a2, b2) = Line(n[1].S1, n[1].S2);

				if (a1 == a2)
					continue; // parallel lines

				// Solve y=a1x + b1, y=a2x + b2
				var x = (b2 - b1) / (a1 - a2);
				var y = a1*x + b1;

				if (sensors.All(ss => ss.P.ManhattanDistanceTo(Point.From(x, y)) > ss.Size))
				{
					return 4000000L * x + y;
				}				

				//     #D
				//    ###D   U
				//   #####D U
				//  ###S###X
				//   #####U D
				//    ###U   D
				//     #U
				// For the sensor at S (size 3 in this example) the line between it and
				// the adjacent sensor either goes "up" (U) if the adjacent sensor is
				// further "down" (ie has a higher! Y-coordinate (Y goes downwards)) or
				// it goes "down" (D) if the the adjacent sensor is further "up".
				// The formulas for up and down are:
				//     Up: y = -x + (y0 + x0 + dist)
				//   Down: y =  x + (y0 - x0 - dist)
				// Sanitycheck: for x==x0 this means
				//     Up: y = -x0 + (y0 + x0 + dist) = y0 + dist, ie dist higher that y0 at x0 - check
				//     Up: y =  x0 + (y0 - x0 - dist) = y0 - dist, ie dist lower that y0 at x0 - check
				static (int, int) Line(Sensor s1, Sensor s2)
				{
					(s1, s2) = (s1.P.X < s2.P.X) ? (s1, s2) : (s2, s1);
					var upwards = s1.P.Y < s2.P.Y;
					return upwards
						? (-1, s1.P.Y + s1.P.X + (s1.Size+1))
						: ( 1, s1.P.Y - s1.P.X - (s1.Size+1));
				}
			}
			throw new Exception("No vacant spot found");
		}


		private record Sensor(Point P, int Size);

		private static (Sensor[] Sensors, HashSet<Point> Beacons) ReadSensors(string[] input)
		{
			var beacons = new HashSet<Point>();
			var sensors = input
				.Select(s =>
				{
					// Sensor at x=2557568, y=3759110: closest beacon is at x=2594124, y=3746832
					var (sx, sy, bx, by) = s.RxMatch("Sensor at x=%d, y=%d: closest beacon is at x=%d, y=%d").Get<int, int, int, int>();
					var beacon = Point.From(bx, by);
					var sensor = Point.From(sx, sy);
					beacons.Add(beacon);
					return new Sensor(sensor, sensor.ManhattanDistanceTo(beacon));
				})
				.ToArray();
			return (sensors, beacons);
		}
	}
}
