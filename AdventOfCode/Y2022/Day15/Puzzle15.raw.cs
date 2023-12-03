using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day15.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 15";
		public override int Year => 2022;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(26).Part2(56000011);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(5403290).Part2(10291582906626);
		}

		protected override long Part1(string[] input)
		{
			// Sensor at x=2557568, y=3759110: closest beacon is at x=2594124, y=3746832
			var beacons = new List<Point>();
			var sensors = input
				.Select(s =>
				{
					var (sx, sy, bx, by) = s.RxMatch("Sensor at x=%d, y=%d: closest beacon is at x=%d, y=%d").Get<int, int, int, int>();
					var beacon = Point.From(bx, by);
					var sensor = Point.From(sx, sy);
					beacons.Add(beacon);
					return (sensor, sensor.ManhattanDistanceTo(beacon));
				})
				.ToArray();

			// var map2 = new CharMap('.');
			// foreach (var (s, dist) in sensors)
			// {
			// 	map2[s] = 'S';
			// }
			// foreach (var b in beacons)
			// {
			// 	map2[b] = 'B';
			// }
			// map2.ConsoleWrite();

			var map = new HashSet<int>();
			var y0 = sensors.Length < 15 ? 10 : 2000000;
			foreach (var (s, dist) in sensors)
			{
				var dy0 = Math.Abs(s.Y - y0);
				var w = dist - dy0;
				for (var d = 0; d <= w; d++)
				{
					map.Add(s.X-d);
					map.Add(s.X+d);
				}
			}
			foreach (var b in beacons.Where(x => x.Y == y0))
			{
				map.Remove(b.X);
			}

// 4797230 not right
			return map.Count();	
		}

		protected override long Part2(string[] input)
		{
			// Sensor at x=2557568, y=3759110: closest beacon is at x=2594124, y=3746832
			var beacons = new List<Point>();
			var sensors = input
				.Select(s =>
				{
					var (sx, sy, bx, by) = s.RxMatch("Sensor at x=%d, y=%d: closest beacon is at x=%d, y=%d").Get<int, int, int, int>();
					var beacon = Point.From(bx, by);
					var sensor = Point.From(sx, sy);
					beacons.Add(beacon);
					return (sensor, sensor.ManhattanDistanceTo(beacon));
				})
				.ToArray();

			// var map2 = new CharMap('.');
			// foreach (var (s, dist) in sensors)
			// {
			// 	map2[s] = 'S';
			// }
			// foreach (var b in beacons)
			// {
			// 	map2[b] = 'B';
			// }
			// map2.ConsoleWrite();


			var maxw = sensors.Length < 15 ? 20 : 4000000;

			for (var y0 = 0; y0 <= maxw; y0++)
			{
				var map = new HashSet<int>();
				var ranges = new List<Range>();

				foreach (var (s, dist) in sensors)
				{
					var dy0 = Math.Abs(s.Y - y0);
					var w = dist - dy0;
					if (w < 0)
						continue;
					var range = new Range(Math.Max(s.X - w, 0), Math.Min(s.X + w + 1, maxw+1));
					ranges.Add(range);
				}


				while (Reduce())
					{}

				if (ranges.Count > 1)
				{
					var x = ranges
						.OrderBy(x => x.Start.Value)
						.First()
						.End.Value;
					var freq = 4000000L * x + y0;
					//Console.WriteLine($"at {y0}: {freq}");
					return freq;
				}

				bool Reduce()
				{
					for (var i = 0; i < ranges.Count; i++)
					{
						for (var j = i+1; j < ranges.Count; j++)
						{
							var (a, b) = (ranges[i], ranges[j]);
							// var (a1, a2, b1, b2) = line.RxMatch("%d-%d,%d-%d").Get<int, int, int, int>();
							// return a1 <= b2 && b1 <= a2;							
							if (a.Start.Value < b.End.Value && b.Start.Value < a.End.Value)
							{
								// overlap; reduce
								var combined = new Range(Math.Min(a.Start.Value, b.Start.Value), Math.Max(a.End.Value, b.End.Value));
								ranges[i] = combined;
								ranges.RemoveAt(j);
								return true;
							}
						}
					}
					return false;
				}

				// 841265410 not right

			}
			throw new Exception();

		}

	}
}
