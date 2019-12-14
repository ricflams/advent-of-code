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
			Puzzle2();
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
			//Debug.Assert(totalEnergy == 10845);
		}

		private static void Puzzle2()
		{
			var mooninfo = File.ReadAllLines("Day12/input.txt").ToArray();

			//var moons = mooninfo
			//	.Select(x => NBody.Moon.ParseFrom(x))
			//	.ToArray();

			//var mooninfo = new string[]
			//{
			//	"<x=-1, y=0, z=2>",
			//	"<x=2, y=-10, z=-7>",
			//	"<x=4, y=-8, z=8>",
			//	"<x=3, y=5, z=-1>"
			//};

			//// Big
			//var mooninfo = new string[]
			//{
			//	"<x=-8, y=-10, z=0>",
			//	"<x=5, y=5, z=10>",
			//	"<x=2, y=-7, z=3>",
			//	"<x=9, y=-8, z=-3>"
			//};


			////long lasti = 0;
			////var moonx = moons1[0];

			////Console.WriteLine(planet.TotalEnergy);

			var orig = mooninfo
				.Select(x => NBody.Moon.ParseFrom(x))
				.ToArray();
			//var orig = moons2.Select(mm => mm.PotentialEnergy).ToList();

			var moons = mooninfo
				.Select(x => NBody.Moon.ParseFrom(x))
				.ToArray();
			var planet = new NBody.Planet(moons);

			var o = orig[1];
			var m = moons[1];
			var hist = new List<long>();
			for (long step = 0; ; )
			{
				planet.SimulateMotionStep();
				step++;
				if (m.IsSamePosition(o))
				{
					Console.Write($" {step}");
					hist.Add(step);
					step = 0;
					if (hist.Count > 1)
					{
						for (var i = 1; i < hist.Count / 2; i++)
						{
							var s1 = hist.GetRange(0, i).ToList();
							var s2 = hist.GetRange(i, i).ToList();
							if (s1.SequenceEqual(s2))
							{
								Console.WriteLine();
								Console.WriteLine($"################### {s1.Sum()} from {string.Join("+", s1)}");
							}
						}
					}
				}
			}

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
