using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode2019.AoC2015.Day01
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
			var input = File.ReadAllLines("AoC2015/Day01/input.txt").First();

			var floor = input.Count(c => c == '(') - input.Count(c => c == ')');

			Console.WriteLine($"Day  1 Puzzle 1: {floor}");
			Debug.Assert(floor == 280);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("AoC2015/Day01/input.txt").First();

			var moves = 0;
			for (var level = 0; level >= 0; level += input[moves++] == '(' ? 1 : -1)
			{
			}

			Console.WriteLine($"Day  1 Puzzle 2: {moves}");
			Debug.Assert(moves == 1797);
		}

		//private static int FuelForMass(int mass) => mass / 3 - 2;
	}
}