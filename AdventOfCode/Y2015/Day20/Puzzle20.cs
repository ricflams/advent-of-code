using AdventOfCode.Helpers.Puzzles;
using System;

namespace AdventOfCode.Y2015.Day20
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Infinite Elves and Infinite Houses";
		public override int Year => 2015;
		public override int Day => 20;

		public void Run()
		{
			Run("input").Part1(831600).Part2(884520);
		}

		protected override int Part1(string[] input)
		{
			var number = int.Parse(input[0]);

			var target = number / 10;
			var houses = new int[target];
			for (var elf = 1; elf < target; elf++)
			{
				for (var i = elf; i < target; i += elf)
				{
					houses[i] += elf * 10;
				}
			}

			var house = Array.FindIndex(houses, v => v >= number);
			return house;
		}

		protected override int Part2(string[] input)
		{
			var number = int.Parse(input[0]);

			var target = number / 11;
			var houses = new int[target];
			var deliveries = new int[target];
			for (var elf = 1; elf < target; elf++)
			{
				for (var i = elf; i < target && deliveries[elf]++ < 50; i += elf)
				{
					houses[i] += elf * 11;
				}
			}

			var house = Array.FindIndex(houses, v => v >= number);
			return house;
		}
	}
}
