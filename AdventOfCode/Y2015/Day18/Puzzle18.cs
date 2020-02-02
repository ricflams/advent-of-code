using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day18
{
	internal class Puzzle18
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = CharMap.FromFile("Y2015/Day18/input.txt");

			var lights = new SparseMap<bool>();
			foreach (var p in input.AllPoints())
			{
				lights[p] = input[p] == '#';
			}

			var n1 = CountLightsOnAfter(lights, 100, false);
			Console.WriteLine($"Day 18 Puzzle 1: {n1}");
			Debug.Assert(n1 == 768);

			var n2 = CountLightsOnAfter(lights, 100, true);
			Console.WriteLine($"Day 18 Puzzle 2: {n2}");
			Debug.Assert(n2 == 781);
		}

		private static int CountLightsOnAfter(SparseMap<bool> lights, int rounds, bool cornersAreStuck)
		{
			var corners = lights.Span().ToList();

			// A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
			// A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.
			for (var i = 0; i < rounds; i++)
			{
				var nextlights = new SparseMap<bool>();
				foreach (var (p, on) in lights.AllValues())
				{
					if (cornersAreStuck && corners.Any(corner => p == corner))
					{
						nextlights[p] = true;
					}
					else
					{
						var n = p.LookDiagonallyAround().Count(x => lights[x]);
						nextlights[p] = on && (n == 2 || n == 3) || !on && (n == 3);
					}
				}
				lights = nextlights;
			}
			var lightsOn = lights.AllPoints(c => c).Count();
			return lightsOn;
		}
	}
}
