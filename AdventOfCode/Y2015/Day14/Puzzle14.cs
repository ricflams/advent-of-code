using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day14
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Reindeer Olympics";
		public override int Year => 2015;
		public override int Day => 14;

		public void Run()
		{
			RunFor("input", 2640, 1102);
		}

		protected override int Part1(string[] input)
		{
			var duration = 2503;

			var reindeers = input.Select(Reindeer.ParseFrom).ToList();
			var maxDistance = reindeers
				.Select(r => r.DistanceAfterDuration(duration))
				.Max();

			return maxDistance;
		}

		protected override int Part2(string[] input)
		{
			var duration = 2503;

			var reindeers = input.Select(Reindeer.ParseFrom).ToList();

			var distances = reindeers
				.Select(r => r.DistanceOverTime().Take(duration).ToArray())
				.ToList();

			var points = new int[reindeers.Count()];
			for (var t = 0; t < duration; t++)
			{
				// Find the re�ndeers that have travelled the longest distance
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
			return winnerPoints;
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
				var (name, velocity, fly, rest) = line
					.RxMatch("%s can fly %d km/s for %d seconds, but then must rest for %d")
					.Get<string, int, int, int>();
				return new Reindeer
				{
					Name = name,
					FlyVelocity = velocity,
					FlyDuration = fly,
					RestDuration = rest
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
