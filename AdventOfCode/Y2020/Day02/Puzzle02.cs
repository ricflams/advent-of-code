using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day02
{
	internal class Puzzle02
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2020/Day02/input.txt");

			var result1 = 0;
			var result2 = 0;
			foreach (var line in input)
			{
				SimpleRegex.Capture(line, "%d-%d %c: %s")
					.Get(out int min)
					.Get(out int max)
					.Get(out char c)
					.Get(out string pwd);

				var n = pwd.Count(x => x == c);
				if (n >= min && n <= max)
				{
					result1++;
				}

				if (pwd[min - 1] == c ^ pwd[max - 1] == c)
				{
					result2++;
				}
			}

			Console.WriteLine($"Day 02 Puzzle 1: {result1}");
			Console.WriteLine($"Day 02 Puzzle 1: {result2}");
			Debug.Assert(result1 == 607);
			Debug.Assert(result2 == 321);
		}
	}
}
