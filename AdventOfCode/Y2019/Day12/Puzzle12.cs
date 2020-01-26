using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode.Y2019.Day12
{
    internal static class Puzzle12
    {
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var mooninfo = File.ReadAllLines("Y2019/Day12/input.txt").ToArray();

			var moons = mooninfo
				.Select(x => Moon.ParseFrom(x))
				.ToArray();
			var jupiter = new Planet(moons);
			for (var i = 0; i < 1000; i++)
			{
				jupiter.SimulateMotionStep();
			}
			var totalEnergy = jupiter.TotalEnergy;
			Console.WriteLine($"Day 12 Puzzle 1: {totalEnergy}");
			Debug.Assert(totalEnergy == 10845);
		}

		private static void Puzzle2()
		{
			var mooninfo = File.ReadAllLines("Y2019/Day12/input.txt").ToArray();

			var moons = mooninfo
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
			Console.WriteLine($"Day 12 Puzzle 2: {cycle}");
			Debug.Assert(cycle == 551272644867044);
		}
	}
}
