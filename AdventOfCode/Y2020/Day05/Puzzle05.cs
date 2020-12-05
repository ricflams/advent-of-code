using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day05
{
	internal class Puzzle05
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2020/Day05/input.txt");

			var ids = input
				.Select(x => x.Replace("F", "0").Replace("B", "1").Replace("L", "0").Replace("R", "1"))
				.Select(x => Convert.ToInt32(x, 2))
				.ToArray();

			var result1 = ids.Max();

			ids = ids.OrderBy(x => x).ToArray();
			var result2 = ids
				.SkipWhile((id, i) => id + 1 == ids[i + 1])
				.First() + 1;

			Console.WriteLine($"Day 05 Puzzle 1: {result1}");
			Console.WriteLine($"Day 05 Puzzle 2: {result2}");
			Debug.Assert(result1 == 855);
			Debug.Assert(result2 == 552);
		}
	}
}
