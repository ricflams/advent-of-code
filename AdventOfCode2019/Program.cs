using System;

namespace AdventOfCode2019
{
	internal class Program
	{
		private static void Main()
		{
			Exercise(() =>
			{
				//AoC2015.Day01.Puzzle01.Run();
				//Day01.Puzzle01.Run();
				//Day02.Puzzle02.Run();
				//Day03.Puzzle03.Run();
				//Day04.Puzzle04.Run();
				//Day05.Puzzle05.Run();
				//Day06.Puzzle06.Run();
				//Day07.Puzzle07.Run();
				//Day08.Puzzle08.Run();
				//Day09.Puzzle09.Run();
				//Day10.Puzzle10.Run();
				//Day11.Puzzle11.Run();
				//Day12.Puzzle12.Run();
				//Day13.Puzzle13.Run();
				//Day14.Puzzle14.Run();
				//Day15.Puzzle15.Run();
				//Day16.Puzzle16.Run();
				//Day17.Puzzle17.Run();
				//Day18.Puzzle18.Run();
				//Day19.Puzzle19.Run();
				Day20.Puzzle20.Run();
				//Day21.Puzzle21.Run();
				//Day22.Puzzle22.Run();
				//Day23.Puzzle23.Run();
				//Day24.Puzzle24.Run();
				//Day25.Puzzle25.Run();
			});
			Console.Write("Done - press any key");
			Console.ReadKey();
		}

		private static void Exercise(Action action)
		{
#if true
			var sw = System.Diagnostics.Stopwatch.StartNew();
			action();
			Console.WriteLine($"Elapsed: {(int)sw.ElapsedMilliseconds} ms");
#else
			action();
			var sw = System.Diagnostics.Stopwatch.StartNew();
			var iterations = 10;
			for (var i = 0; i < iterations; i++)
			{
				action();
			}
			Console.WriteLine($"Elapsed: {(int)(sw.ElapsedMilliseconds / iterations)} ms");
#endif
		}
	}
}
