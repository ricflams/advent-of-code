﻿using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Y2020.Day13
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Shuttle Search";
		public override int Year => 2020;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(295).Part2(1068781);
			Run("test2").Part2(3417);
			Run("test3").Part2(754018);
			Run("test4").Part2(779210);
			Run("test5").Part2(1261476);
			Run("test6").Part2(1202161486);
			Run("input").Part1(4135).Part2(640856202464541);
			Run("extra").Part1(5257).Part2(538703333547789);
		}

		protected override long Part1(string[] input)
		{
			var departure = int.Parse(input[0]);
			var id = input[1]
				.Replace(",x", "")
				.Split(",")
				.Select(int.Parse)
				.Select(id => new
				{
					Id = id,
					Time = id - departure % id
				})
				.OrderBy(x => x.Time)
				.First();
			return id.Id * id.Time;
		}

		protected override long Part2(string[] input)
		{
			var bus = input[1]
				.Split(",")
				.Select((x, i) =>
					x == "x"
						? null
						: new
						{
							Id = int.Parse(x),
							Position = i
						}
				)
				.Where(x => x != null)
				.ToArray();

			static long FindNextTimestamp(long a, long b, long n, int position)
			{
				// Find the factor x that satisfies this next timestamp (all mod n):
				//       ax + b ≡ n - position
				// <=>   ax ≡ n - position - b
				// <=>   ax ≡ target, where target = n - offset - b
				// <=>   a⁻¹ax = a⁻¹ target
				// <=>   x ≡ a⁻¹ target
				var target = ((n - position - b) % n + n) % n; // make sure target is positive, mod n
				var ainv = (long)new BigInteger(a).ModInverse(n);
				var x = (ainv * target) % n;
				return a * x + b;
			}

			long timestamp = 0;
			long cyclus = bus[0].Id;
			for (var i = 1; i < bus.Length; i++)
			{
				timestamp = FindNextTimestamp(cyclus, timestamp, bus[i].Id, bus[i].Position);
				cyclus *= bus[i].Id;
			}
			return timestamp;
		}
	}
}
