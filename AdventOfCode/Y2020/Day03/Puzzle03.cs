using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day03
{
	internal class Puzzle03
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2020/Day03/input.txt");

			var result = input
				.Select((line, i) => line[i * 3 % line.Length] == '#' ? 1 : 0)
				.Sum();

			Console.WriteLine($"Day 03 Puzzle 1: {result}");
			Debug.Assert(result == 237);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2020/Day03/input.txt");

			var result =
				CountTrees(input, 1, 1) *
				CountTrees(input, 3, 1) *
				CountTrees(input, 5, 1) *
				CountTrees(input, 7, 1) *
				CountTrees(input, 1, 2);

			Console.WriteLine($"Day 03 Puzzle 2: {result}");
			Debug.Assert(result == 2106818610);
		}

		private static long CountTrees(string[] input, int right, int down)
		{
			var trees = 0;
			for (var (xpos, ypos) = (0, 0); ypos < input.Length; ypos += down, xpos += right)
			{
				var line = input[ypos];
				if (line[xpos % line.Length] == '#')
				{
					trees++;
				}
			}
			return trees;
		}
	}
}
