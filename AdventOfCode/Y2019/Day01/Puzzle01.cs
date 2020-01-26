using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode.Y2019.Day01
{
	internal class Puzzle01
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var masses = File.ReadAllLines("Y2019/Day01/input.txt").Select(int.Parse);

			var mass = masses.Sum(FuelForMass);
			Console.WriteLine($"Day  1 Puzzle 1: {mass}");
			Debug.Assert(mass == 3228475);
		}

		private static void Puzzle2()
		{
			var masses = File.ReadAllLines("Y2019/Day01/input.txt").Select(int.Parse);

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

			Console.WriteLine($"Day  1 Puzzle 2: {mass}");
			Debug.Assert(mass == 4839845);
		}

		private static int FuelForMass(int mass) => mass / 3 - 2;
	}
}