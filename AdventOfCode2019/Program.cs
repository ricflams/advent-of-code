using System;

namespace AdventOfCode2019
{
	internal class Program
	{
		private static void Main()
		{
			Exercise(() =>
			{
				Day01.Puzzle.Run();
				Day02.Puzzle.Run();
				Day03.Puzzle.Run();
				Day04.Puzzle.Run();
				Day05.Puzzle.Run();
				Day06.Puzzle.Run();
				Day07.Puzzle.Run();
				Day08.Puzzle.Run();
				Day09.Puzzle.Run();
				Day10.Puzzle.Run();
				Day11.Puzzle.Run();
				Day12.Puzzle.Run();
				Day13.Puzzle.Run();
				Day14.Puzzle.Run();
				Day15.Puzzle.Run();
				//Day16.Puzzle.Run();
				Day17.Puzzle.Run();
				//Day18.Puzzle.Run();
				Day19.Puzzle.Run();
				//Day20.Puzzle.Run();
				//Day21.Puzzle.Run();
				Day22.Puzzle.Run();
				//Day23.Puzzle.Run();
				Day24.Puzzle.Run();
				//Day25.Puzzle.Run();
			});
			Console.Write("Done - press any key");
			Console.ReadKey();
		}

		private static void Exercise(Action action)
		{
			action();
#if false
			var iterations = 20;
			var sw = System.Diagnostics.Stopwatch.StartNew();
			for (var i = 0; i < iterations; i++)
			{
				action();
			}
			Console.WriteLine($"Elapsed: {(int)(sw.ElapsedMilliseconds/ iterations)} ms");
#endif
		}
	}
}
