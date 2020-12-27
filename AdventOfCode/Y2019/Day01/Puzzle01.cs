﻿using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2019.Day01
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 1;

		public void Run()
		{
			RunFor("input", 3228475, 4839845);
		}

		protected override int Part1(string[] input)
		{
			var masses = input.Select(int.Parse);
			var mass = masses.Sum(FuelForMass);
			return mass;
		}

		protected override int Part2(string[] input)
		{
			var masses = input.Select(int.Parse);
			var mass = masses.Sum(x =>
			{
				var totalFuel = FuelForMass(x);
				var fuelMass = totalFuel;
				while (true)
				{
					var extraFuel = FuelForMass(fuelMass);
					if (extraFuel <= 0)
						break;
					totalFuel += extraFuel;
					fuelMass = extraFuel;
				}
				return totalFuel;
			});
			return mass;
		}

		private static int FuelForMass(int mass) => mass / 3 - 2;
	}
}