using AdventOfCode2019.Helpers;
using System;
using System.Linq;
using System.Diagnostics;
using AdventOfCode2019.Intcode;

namespace AdventOfCode.Y2019.Day05
{
	internal class Puzzle05
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var result = new Engine()
				.WithMemoryFromFile("Y2019/Day05/input.txt")
				.WithInput(1)
				.Execute()
				.Output.TakeAll()
				.SkipWhile(x => x == 0)
				.First();
			Console.WriteLine($"Day  5 Puzzle 1: {result}");
			Debug.Assert(result == 7566643);
		}

		private static void Puzzle2()
		{
			var result = new Engine()
				.WithMemoryFromFile("Y2019/Day05/input.txt")
				.WithInput(5)
				.Execute()
				.Output.Take();
			Console.WriteLine($"Day  5 Puzzle 2: {result}");
			Debug.Assert(result == 9265694);
		}
	}
}