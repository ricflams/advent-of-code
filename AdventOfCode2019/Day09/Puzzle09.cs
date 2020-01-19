using AdventOfCode2019.Intcode;
using System;
using System.Diagnostics;

namespace AdventOfCode2019.Day09
{
	internal class Puzzle09
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var result = new Engine()
				.WithMemoryFromFile("Day09/input.txt")
				.WithInput(1)
				.Execute()
				.Output.Take();
			Console.WriteLine($"Day  9 Puzzle 1: {result}");
			Debug.Assert(result == 2682107844);
		}

		private static void Puzzle2()
		{
			var result = new Engine()
				.WithMemoryFromFile("Day09/input.txt")
				.WithInput(2)
				.Execute()
				.Output.Take();
			Console.WriteLine($"Day  9 Puzzle 2: {result}");
			Debug.Assert(result == 34738);
		}
	}
}