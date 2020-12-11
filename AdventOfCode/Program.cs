using System;

namespace AdventOfCode
{
	internal class Program
	{
		private static void Main()
		{
			//Helpers.PuzzleDay.GeneratePuzzles.Generate(2020);

			Exercise(() =>
			{
				Y2015Puzzles();
				//Y2019Puzzles();
				//Y2020Puzzles();
			});
			Console.Write("Done - press any key");
			Console.ReadKey();
		}

		private static void Y2015Puzzles()
		{
			Y2015.Day01.Puzzle01.Run();
			Y2015.Day02.Puzzle02.Run();
			Y2015.Day03.Puzzle03.Run();
			Y2015.Day04.Puzzle04.Run();
			Y2015.Day05.Puzzle05.Run();
			Y2015.Day06.Puzzle06.Run();
			Y2015.Day07.Puzzle07.Run();
			Y2015.Day08.Puzzle08.Run();
			Y2015.Day09.Puzzle09.Run();
			Y2015.Day10.Puzzle10.Run();
			Y2015.Day11.Puzzle11.Run();
			Y2015.Day12.Puzzle12.Run();
			Y2015.Day13.Puzzle13.Run();
			Y2015.Day14.Puzzle14.Run();
			Y2015.Day15.Puzzle15.Run();
			Y2015.Day16.Puzzle16.Run();
			Y2015.Day17.Puzzle17.Run();
			Y2015.Day18.Puzzle18.Run();
			Y2015.Day19.Puzzle19.Run();
			//Y2015.Day20.Puzzle20.Run();
			//Y2015.Day21.Puzzle21.Run();
			//Y2015.Day22.Puzzle22.Run();
			Y2015.Day23.Puzzle23.Run();
			Y2015.Day24.Puzzle24.Run();
			Y2015.Day25.Puzzle25.Run();
		}

		private static void Y2019Puzzles()
		{
			Y2019.Day01.Puzzle01.Run();
			Y2019.Day02.Puzzle02.Run();
			Y2019.Day03.Puzzle03.Run();
			Y2019.Day04.Puzzle04.Run();
			Y2019.Day05.Puzzle05.Run();
			Y2019.Day06.Puzzle06.Run();
			Y2019.Day07.Puzzle07.Run();
			Y2019.Day08.Puzzle08.Run();
			Y2019.Day09.Puzzle09.Run();
			Y2019.Day10.Puzzle10.Run();
			Y2019.Day11.Puzzle11.Run();
			Y2019.Day12.Puzzle12.Run();
			Y2019.Day13.Puzzle13.Run();
			Y2019.Day14.Puzzle14.Run();
			Y2019.Day15.Puzzle15.Run();
			Y2019.Day16.Puzzle16.Run();
			Y2019.Day17.Puzzle17.Run();
			Y2019.Day18.Puzzle18.Run();
			Y2019.Day19.Puzzle19.Run();
			Y2019.Day20.Puzzle20.Run();
			Y2019.Day21.Puzzle21.Run();
			Y2019.Day22.Puzzle22.Run();
			Y2019.Day23.Puzzle23.Run();
			Y2019.Day24.Puzzle24.Run();
			Y2019.Day25.Puzzle25.Run();
		}

		private static void Y2020Puzzles()
		{
			//Y2020.Day01.Puzzle01.Run();
			//Y2020.Day02.Puzzle02.Run();
			//Y2020.Day03.Puzzle03.Run();
			//Y2020.Day04.Puzzle04.Run();
			Y2020.Day05.Puzzle05.Run();
			//Y2020.Day06.Puzzle06.Run();
			//Y2020.Day07.Puzzle07.Run();
			//Y2020.Day08.Puzzle08.Run();
			//Y2020.Day09.Puzzle09.Run();
			//Y2020.Day10.Puzzle10.Run();
			Y2020.Day11.Puzzle11.Run();
			//Y2020.Day12.Puzzle12.Run();
			//Y2020.Day13.Puzzle13.Run();
			//Y2020.Day14.Puzzle14.Run();
			//Y2020.Day15.Puzzle15.Run();
			//Y2020.Day16.Puzzle16.Run();
			//Y2020.Day17.Puzzle17.Run();
			//Y2020.Day18.Puzzle18.Run();
			//Y2020.Day19.Puzzle19.Run();
			//Y2020.Day20.Puzzle20.Run();
			//Y2020.Day21.Puzzle21.Run();
			//Y2020.Day22.Puzzle22.Run();
			//Y2020.Day23.Puzzle23.Run();
			//Y2020.Day24.Puzzle24.Run();
			//Y2020.Day25.Puzzle25.Run();
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
			var iterations = 10000;
			for (var i = 0; i < iterations; i++)
			{
				action();
			}
			var elapsed = sw.ElapsedTicks;
			Console.WriteLine($"Elapsed: {(double)elapsed / (TimeSpan.TicksPerMillisecond * iterations):F3} ms");
#endif
		}
	}
}
