using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2019.Day12
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The N-Body Problem";
		public override int Year => 2019;
		public override int Day => 12;

		public void Run()
		{
			RunPart2For("test1", 4686774924);
			RunFor("input", 10845, 551272644867044);
		}

		protected override long Part1(string[] input)
		{
			var moons = input
				.Select(x => Moon.ParseFrom(x))
				.ToArray();
			var jupiter = new Planet(moons);
			for (var i = 0; i < 1000; i++)
			{
				jupiter.SimulateMotionStep();
			}
			var totalEnergy = jupiter.TotalEnergy;
			return totalEnergy;
		}

		protected override long Part2(string[] input)
		{
			var moons = input
				.Select(x => Moon.ParseFrom(x))
				.ToArray();
			var planet = new Planet(moons);
			var xvec = planet.XVectors.ToArray();
			var yvec = planet.YVectors.ToArray();
			var zvec = planet.ZVectors.ToArray();
			var xcycle = 0L;
			var ycycle = 0L;
			var zcycle = 0L;

			while (xcycle == 0 || ycycle == 0 || zcycle == 0)
			{
				planet.SimulateMotionStep();
				if (xcycle == 0 && planet.XVectors.SequenceEqual(xvec))
				{
					xcycle = planet.SimulationStep;
				}
				if (ycycle == 0 && planet.YVectors.SequenceEqual(yvec))
				{
					ycycle = planet.SimulationStep;
				}
				if (zcycle == 0 && planet.ZVectors.SequenceEqual(zvec))
				{
					zcycle = planet.SimulationStep;
				}
			}

			var cycle = MathHelper.LeastCommonMultiple(xcycle, ycycle, zcycle);
			return cycle;
		}
	}
}
