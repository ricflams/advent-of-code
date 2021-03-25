using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2019.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Tyranny of the Rocket Equation";
		public override int Year => 2019;
		public override int Day => 1;

		public void Run()
		{
			Run("input").Part1(3228475).Part2(4839845);
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