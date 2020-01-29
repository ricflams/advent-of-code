using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using AdventOfCode.Helpers;

namespace AdventOfCode.Y2015.Day14
{
	internal class Puzzle14
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2015/Day14/input.txt");
			var duration = 2503;

			var reindeers = input.Select(Reindeer.ParseFrom).ToList();
			var maxDistance = reindeers
				.Select(r => r.DistanceAfterDuration(duration))
				.Max();

			Console.WriteLine($"Day 14 Puzzle 1: {maxDistance}");
			Debug.Assert(maxDistance == 2640);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2015/Day14/input.txt");
			var duration = 2503;

			var reindeers = input.Select(Reindeer.ParseFrom).ToList();

			var distances = reindeers
				.Select(r => r.DistanceOverTime().Take(duration).ToArray())
				.ToList();

			var points = new int[reindeers.Count()];
			for (var t = 0; t < duration; t++)
			{
				// Find the reíndeers that have travelled the longest distance
				var leaders = distances
					.Select((travelled, i) => new
					{
						Travelled = travelled[t],
						Index = i
					})
					.OrderByDescending(x => x.Travelled)
					.GroupBy(x => x.Travelled)
					.First();
				foreach (var leader in leaders)
				{
					points[leader.Index]++;
				}
			}

			var winnerPoints = points.Max();
			Console.WriteLine($"Day 14 Puzzle 2: {winnerPoints}");
			Debug.Assert(winnerPoints == 1102);
		}

		private class Reindeer
		{
			public string Name { get; set; }
			public int FlyVelocity { get; set; }
			public int FlyDuration { get; set; }
			public int RestDuration { get; set; }

			public static Reindeer ParseFrom(string line)
			{
				// Example:
				// Rudolph can fly 11 km/s for 5 seconds, but then must rest for 48 seconds.
				var val = SimpleRegex.Match(line, "%s can fly %d km/s for %d seconds, but then must rest for %d");
				return new Reindeer
				{
					Name = val[0],
					FlyVelocity = int.Parse(val[1]),
					FlyDuration = int.Parse(val[2]),
					RestDuration = int.Parse(val[3])
				};
			}

			public int DistanceAfterDuration(int t)
			{
				var flycycle = FlyDuration + RestDuration;
				var periods = t / flycycle;
				var finalFlyDuration = Math.Min(FlyDuration, t % flycycle);
				var distance = FlyVelocity * (periods * FlyDuration + finalFlyDuration);
				return distance;
			}

			public IEnumerable<int> DistanceOverTime()
			{
				var distance = 0;
				while (true)
				{
					for (var i = 0; i < FlyDuration; i++)
					{
						distance += FlyVelocity;
						yield return distance;
					}
					for (var i = 0; i < RestDuration; i++)
					{
						yield return distance;
					}
				}
			}
		}
	}
}
