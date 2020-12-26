using AdventOfCode.Helpers.Puzzles;
using System;

namespace AdventOfCode.Y2015.Day20
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2015;
		protected override int Day => 20;

		public void Run()
		{
			RunFor("input", 831600, 884520);
		}

		protected override int Part1(string[] _)
		{
			var input = 36000000;

			var target = input / 10;
			var houses = new int[target];
			for (var elf = 1; elf < target; elf++)
			{
				for (var i = elf; i < target; i += elf)
				{
					houses[i] += elf * 10;
				}
			}

			var house = Array.FindIndex(houses, v => v >= input);
			return house;
		}

		protected override int Part2(string[] _)
		{
			var input = 36000000;

			var target = input / 11;
			var houses = new int[target];
			var deliveries = new int[target];
			for (var elf = 1; elf < target; elf++)
			{
				for (var i = elf; i < target && deliveries[elf]++ < 50; i += elf)
				{
					houses[i] += elf * 11;
				}
			}

			var house = Array.FindIndex(houses, v => v >= input);
			return house;
		}
	}
}
