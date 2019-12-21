using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Day12
{
    internal static class Puzzle
    {
		public static void Run()
		{
			Puzzle1();
			//Puzzle2();
		}

		private static void Puzzle1()
		{
			var mooninfo = File.ReadAllLines("Day12/input.txt").ToArray();

			var moons = mooninfo
				.Select(x => NBody.Moon.ParseFrom(x))
				.ToArray();
			var jupiter = new NBody.Planet(moons);
			for (var i = 0; i < 1000; i++)
			{
				jupiter.SimulateMotionStep();
			}
			var totalEnergy = jupiter.TotalEnergy;
			Console.WriteLine($"Day 12 Puzzle 1: {totalEnergy}");
			System.Diagnostics.Debug.Assert(totalEnergy == 10845);
		}

		class StepInfo
		{
			public long Step;
			public string Velocity;
		}

		private static void Puzzle2()
		{
			var mooninfo = File.ReadAllLines("Day12/input.txt").ToArray();

			//var moons = mooninfo
			//	.Select(x => NBody.Moon.ParseFrom(x))
			//	.ToArray();

			//mooninfo = new string[]
			//{
			//	"<x=-1, y=0, z=2>",
			//	"<x=2, y=-10, z=-7>",
			//	"<x=4, y=-8, z=8>",
			//	"<x=3, y=5, z=-1>"
			//};

			// Big
			mooninfo = new string[]
			{
				"<x=-8, y=-10, z=0>",
				"<x=5, y=5, z=10>",
				"<x=2, y=-7, z=3>",
				"<x=9, y=-8, z=-3>"
			};


			////long lasti = 0;
			////var moonx = moons1[0];

			////Console.WriteLine(planet.TotalEnergy);

			var orig = mooninfo
				.Select(x => NBody.Moon.ParseFrom(x))
				.ToArray();
			//var orig = moons2.Select(mm => mm.PotentialEnergy).ToList();

			//var initialPotentialEnergy = planet.PotentialEnergy;
			//Console.WriteLine(initialPotentialEnergy);

			for (var i = 1; i < 4; i++)
			{
				var moons = mooninfo
					.Select(x => NBody.Moon.ParseFrom(x))
					.ToArray();
				var planet = new NBody.Planet(moons);
				var history = new Dictionary<string, List<StepInfo>>();
				var energyhistory = new Dictionary<int, long>();
				var m = moons[i];
				for (long step = 0; ;)
				{
					planet.SimulateMotionStep();
					step++;
					var energy = planet.TotalEnergy;
					if (energyhistory.ContainsKey(energy))
					{
						var step0 = energyhistory[energy];
						Console.WriteLine($"step0={step0} stepN={step} cycle={step - step0}");
					}
					energyhistory[planet.TotalEnergy] = step;

					//var pos = $"{m.X}-{m.Y}-{m.Z}";
					//if (!history.ContainsKey(pos))
					//{
					//	history[pos] = new List<StepInfo>();
					//}
					//var stepinfo = new StepInfo { Step = step, Velocity = $"{m.Vx}-{m.Vy}-{m.Vz}" };
					//var match = history[pos].OrderByDescending(x => x.Step).FirstOrDefault(x => x.Velocity == stepinfo.Velocity);
					//if (match != null)
					//{
					//	Console.WriteLine($"step0={match.Step} stepN={stepinfo.Step} cycle={stepinfo.Step - match.Step}");
					//	//break;
					//}
					//history[pos].Add(stepinfo);
				}
			}
			//Console.WriteLine($"steps={hist.Sum()}");


			//var moonindex = 3;
			//var o = orig[moonindex];
			//var m = moons[moonindex];
			//var hist = new List<long>();
			//for (long step = 0; ; )
			//{
			//	planet.SimulateMotionStep();
			//	step++;
			//	if (m.KineticEnergy == 0)
			//	{
			//		Console.Write($" {step}");
			//		hist.Add(step);
			//		step = 0;
			//		if (hist.Count > 0)
			//		{
			//			for (var i = 1; i < hist.Count; i++)
			//			{
			//				var s1 = hist.GetRange(0, i).Sum();
			//				for (var j = i; j < hist.Count - i; j++)
			//				{
			//					var s2 = hist.GetRange(i, j).Sum();
			//					if (s1 == s2)
			//					{
			//						Console.WriteLine();
			//						Console.WriteLine($"################### {s1} from 0-{i}-{j}");
			//					}
			//				}
			//			}
			//		}
			//		if (Enumerable.Range(0, orig.Length).All(i => moons[i].IsSame(orig[i])))
			//		{
			//			Console.WriteLine();
			//			break;
			//		}
			//	}
			//}
			//Console.WriteLine($"steps={hist.Sum()}");

			//var lasti = 0L;
			//for (long i = 1; ; i++)
			//{
			//	planet.SimulateMotionStep();

			//	if (moons1.Select(mm=>mm.PotentialEnergy).SequenceEqual(orig))
			//	{
			//		Console.WriteLine($"################### " + i);
			//	}

			//	//Console.WriteLine(planet.TotalEnergy);
			//	//if (planet.TotalEnergy == 0)
			//	//{
			//	//	Console.WriteLine($"################### " + i);
			//	//}
			//	//if (m1.IsSamePosition(m2))
			//	//{
			//	//	Console.WriteLine($"Day 12 Puzzle 27: {i}");
			//	//	break;
			//	//}
			//	//if (m1.KineticEnergy == 0 && m1.X == m2.X && m1.Y == m2.Y && m1.Z == m2.Z)
			//	//if (moons1[1].KineticEnergy == 0)
			//	//var xx = moons1.Count(m => m.KineticEnergy == 0);
			//	//if (xx > 1)
			//	//{
			//	//	Console.WriteLine($"xxDay 12 Puzzle 27: {i} {xx} {moons1.Sum(m=>m.TotalEnergy)} {string.Join(" ", moons1.Select(m => $"{m.X},{m.Y},{m.Z}"))}");
			//	//	lasti = i;
			//	//	//if (xx == 4)
			//	//	//{
			//	//	//	Console.WriteLine($"###################");
			//	//	//}
			//	//}
			//}



			//var results = Enumerable.Range(0, 4)
			//	.Select(mi =>
			//	{
			//		var moons1 = mooninfo
			//			.Select(x => NBody.Moon.ParseFrom(x))
			//			.ToArray();
			//		var planet = new NBody.Planet(moons1);
			//		//var initialEnergy = planet.TotalEnergy;
			//		var m1 = moons1[mi];
			//		//var m2 = moons2[mi];
			//		for (long i = 1; ; i++)
			//		{
			//			planet.SimulateMotionStep();
			//			//if (m1.IsSamePosition(m2))
			//			//{
			//			//	Console.WriteLine($"Day 12 Puzzle 27: {i}");
			//			//	break;
			//			//}
			//			//if (m1.KineticEnergy == 0 && m1.X == m2.X && m1.Y == m2.Y && m1.Z == m2.Z)
			//			if (m1.X == m2.X && m1.Y == m2.Y && m1.Z == m2.Z)
			//			{
			//				Console.WriteLine($"xxDay 12 Puzzle 27: {i}");
			//				return i;
			//			}
			//		}
			//	});
			//var prod = results.Aggregate(1L, (result, val) => result * val);


			//var moons1 = mooninfo
			//	.Select(x => NBody.Moon.ParseFrom(x))
			//	.ToArray();
			//var planet = new NBody.Planet(moons1);

			//var m = moons1[0];
			//string a="[", b="[", c="[";
			//for (var i = 0; i < 2772; i++)
			//{
			//	planet.SimulateMotionStep();
			//	//Console.WriteLine($"{m.X}\t{m.Y}\t{m.Z}");
			//	a += m.X.ToString() + " ";
			//	b += m.Y.ToString() + " ";
			//	c += m.Z.ToString() + " ";
			//}
			//Console.WriteLine(a + "]");
			//Console.WriteLine(b + "]");
			//Console.WriteLine(c + "]");

			//var midx = 0;
			//var target = moons1[0].Copy();
			//for (long i = 1; ; i++)
			//{
			//	planet.SimulateMotionStep();
			//	if (moons1[midx].IsSamePosition(target))
			//	{
			//		Console.WriteLine($"xxDay 12 Puzzle 27: {i}");
			//		if (midx == 2)
			//			break;
			//		target = moons1[++midx].Copy();
			//	}
			//	//if (m1.IsSamePosition(m2))
			//	//{
			//	//	Console.WriteLine($"Day 12 Puzzle 27: {i}");
			//	//	break;
			//	//}
			//	//if (m1.KineticEnergy == 0 && m1.X == m2.X && m1.Y == m2.Y && m1.Z == m2.Z)
			//	//if (m1.X == m2.X && m1.Y == m2.Y && m1.Z == m2.Z)
			//	//{
			//	//	Console.WriteLine($"xxDay 12 Puzzle 27: {i}");
			//	//	return i;
			//	//}
			//}



			//var results = Enumerable.Range(0, 4)
			//	.Select(mi =>
			//	{
			//		//var initialEnergy = planet.TotalEnergy;
			//		var m1 = moons1[mi];
			//		//var m2 = moons2[mi];
			//	});
			//var prod = results.Aggregate(1L, (result, val) => result * val);


			////var result2 = 0;
			//Console.WriteLine($"Day 12 Puzzle 2: {prod}");
			////Debug.Assert(result2 == ...)
		}
	}
}
