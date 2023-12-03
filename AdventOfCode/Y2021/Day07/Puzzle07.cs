using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day07
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "The Treachery of Whales";
		public override int Year => 2021;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(37).Part2(168);
			Run("input").Part1(344535).Part2(95581659);
		}

		protected override int Part1(string[] input)
		{
			var v = input.First().ToIntArray();

			var min = v.Min();
			var max = v.Max();
			var minFuel = int.MaxValue;
			for (var xpos = min; xpos <= max; xpos++)
			{
				var fuel = v.Select(x => Math.Abs(x - xpos)).Sum();
				if (fuel < minFuel)
				{
					minFuel = fuel;
				}
			}

			return minFuel;
		}

		protected override int Part2(string[] input)
		{
			var v = input.First().ToIntArray();

			var min = v.Min();
			var max = v.Max();
			var minFuel = int.MaxValue;
			for (var xpos = min; xpos <= max; xpos++)
			{
				var fuel = v
					.Select(x => Math.Abs(x - xpos))
					.Select(d => d * (d + 1) / 2)
					.Sum();
				if (fuel < minFuel)
				{
					minFuel = fuel;
				}
			}

			return minFuel;
		}
	}
}
