using AdventOfCode.Helpers.Puzzles;
using System;
using System.Diagnostics;

namespace AdventOfCode
{
	public class Program
	{
		static void Main(string[] _)
		{
			PuzzleOptions.RunOnly(2022, 1);
			//PuzzleOptions.OnlyRunForInputs = true;

			var iterations = 1;
			//iterations = 5;

			if (iterations == 1)
			{
				PuzzleOptions.Iterations = 1;
				var sw = Stopwatch.StartNew();
				AllPuzzles();
				Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
			}
			else
			{
				//PuzzleOptions.OnlyRunForInputs = true;
				PuzzleOptions.Silent = true;
				PuzzleOptions.Iterations = 1;
				AllPuzzles();
				PuzzleOptions.Silent = false;
				PuzzleOptions.Iterations = iterations;
				var sw = Stopwatch.StartNew();
				AllPuzzles();
				var elapsedMsec = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;
				Console.WriteLine($"Elapsed: {elapsedMsec / iterations:F2} ms");
			}

			//if (Debugger.IsAttached)
			//{
			//	Console.Write("Press any key to close ");
			//	Console.ReadKey();
			//}
		}

		private static void AllPuzzles()
		{
			// Y2015Puzzles();
			// Y2016Puzzles();
			// Y2017Puzzles();
			Y2018Puzzles();
			// Y2019Puzzles();
			// Y2020Puzzles();
			// Y2021Puzzles();
			Y2022Puzzles();
		}

		private static void Y2015Puzzles()
		{
			Y2015.Day01.Puzzle.Instance.Run();
			Y2015.Day02.Puzzle.Instance.Run();
			Y2015.Day03.Puzzle.Instance.Run();
			Y2015.Day04.Puzzle.Instance.Run();
			Y2015.Day05.Puzzle.Instance.Run();
			Y2015.Day06.Puzzle.Instance.Run();
			Y2015.Day07.Puzzle.Instance.Run();
			Y2015.Day08.Puzzle.Instance.Run();
			Y2015.Day09.Puzzle.Instance.Run();
			Y2015.Day10.Puzzle.Instance.Run();
			Y2015.Day11.Puzzle.Instance.Run();
			Y2015.Day12.Puzzle.Instance.Run();
			Y2015.Day13.Puzzle.Instance.Run();
			Y2015.Day14.Puzzle.Instance.Run();
			Y2015.Day15.Puzzle.Instance.Run();
			Y2015.Day16.Puzzle.Instance.Run();
			Y2015.Day17.Puzzle.Instance.Run();
			Y2015.Day18.Puzzle.Instance.Run();
			//Y2015.Day19.Puzzle.Instance.Run();
			Y2015.Day20.Puzzle.Instance.Run();
			Y2015.Day21.Puzzle.Instance.Run();
			//Y2015.Day22.Puzzle.Instance.Run();
			Y2015.Day23.Puzzle.Instance.Run();
			Y2015.Day24.Puzzle.Instance.Run();
			Y2015.Day25.Puzzle.Instance.Run();
		}
		private static void Y2016Puzzles()
		{
			Y2016.Day01.Puzzle.Instance.Run();
			Y2016.Day02.Puzzle.Instance.Run();
			Y2016.Day03.Puzzle.Instance.Run();
			Y2016.Day04.Puzzle.Instance.Run();
			Y2016.Day05.Puzzle.Instance.Run();
			Y2016.Day06.Puzzle.Instance.Run();
			Y2016.Day07.Puzzle.Instance.Run();
			Y2016.Day08.Puzzle.Instance.Run();
			Y2016.Day09.Puzzle.Instance.Run();
			Y2016.Day10.Puzzle.Instance.Run();
			Y2016.Day11.Puzzle.Instance.Run();
			Y2016.Day12.Puzzle.Instance.Run();
			Y2016.Day13.Puzzle.Instance.Run();
			Y2016.Day14.Puzzle.Instance.Run();
			Y2016.Day15.Puzzle.Instance.Run();
			Y2016.Day16.Puzzle.Instance.Run();
			Y2016.Day17.Puzzle.Instance.Run();
			Y2016.Day18.Puzzle.Instance.Run();
			Y2016.Day19.Puzzle.Instance.Run();
			Y2016.Day20.Puzzle.Instance.Run();
			Y2016.Day21.Puzzle.Instance.Run();
			Y2016.Day22.Puzzle.Instance.Run();
			Y2016.Day23.Puzzle.Instance.Run();
			Y2016.Day24.Puzzle.Instance.Run();
			Y2016.Day25.Puzzle.Instance.Run();
		}
		private static void Y2017Puzzles()
		{
			Y2017.Day01.Puzzle.Instance.Run();
			Y2017.Day02.Puzzle.Instance.Run();
			Y2017.Day03.Puzzle.Instance.Run();
			Y2017.Day04.Puzzle.Instance.Run();
			Y2017.Day05.Puzzle.Instance.Run();
			Y2017.Day06.Puzzle.Instance.Run();
			Y2017.Day07.Puzzle.Instance.Run();
			Y2017.Day08.Puzzle.Instance.Run();
			Y2017.Day09.Puzzle.Instance.Run();
			Y2017.Day10.Puzzle.Instance.Run();
			Y2017.Day11.Puzzle.Instance.Run();
			Y2017.Day12.Puzzle.Instance.Run();
			Y2017.Day13.Puzzle.Instance.Run();
			Y2017.Day14.Puzzle.Instance.Run();
			Y2017.Day15.Puzzle.Instance.Run();
			Y2017.Day16.Puzzle.Instance.Run();
			Y2017.Day17.Puzzle.Instance.Run();
			Y2017.Day18.Puzzle.Instance.Run();
			Y2017.Day19.Puzzle.Instance.Run();
			Y2017.Day20.Puzzle.Instance.Run();
			Y2017.Day21.Puzzle.Instance.Run();
			Y2017.Day22.Puzzle.Instance.Run();
			Y2017.Day23.Puzzle.Instance.Run();
			Y2017.Day24.Puzzle.Instance.Run();
			Y2017.Day25.Puzzle.Instance.Run();
		}
		private static void Y2018Puzzles()
		{
			Y2018.Day01.Puzzle.Instance.Run();
			Y2018.Day02.Puzzle.Instance.Run();
			Y2018.Day03.Puzzle.Instance.Run();
			Y2018.Day04.Puzzle.Instance.Run();
			Y2018.Day05.Puzzle.Instance.Run();
			Y2018.Day06.Puzzle.Instance.Run();
			Y2018.Day07.Puzzle.Instance.Run();
			Y2018.Day08.Puzzle.Instance.Run();
			Y2018.Day09.Puzzle.Instance.Run();
			Y2018.Day10.Puzzle.Instance.Run();
			Y2018.Day11.Puzzle.Instance.Run();
			Y2018.Day12.Puzzle.Instance.Run();
			Y2018.Day13.Puzzle.Instance.Run();
			Y2018.Day14.Puzzle.Instance.Run();
			Y2018.Day15.Puzzle.Instance.Run();
			Y2018.Day16.Puzzle.Instance.Run();
			Y2018.Day17.Puzzle.Instance.Run();
			Y2018.Day18.Puzzle.Instance.Run();
			Y2018.Day19.Puzzle.Instance.Run();
			Y2018.Day20.Puzzle.Instance.Run();
			Y2018.Day21.Puzzle.Instance.Run();
			//Y2018.Day22.Puzzle.Instance.Run();
			//Y2018.Day23.Puzzle.Instance.Run();
			//Y2018.Day24.Puzzle.Instance.Run();
			//Y2018.Day25.Puzzle.Instance.Run();
		}
		private static void Y2019Puzzles()
		{
			Y2019.Day01.Puzzle.Instance.Run();
			Y2019.Day02.Puzzle.Instance.Run();
			Y2019.Day03.Puzzle.Instance.Run();
			Y2019.Day04.Puzzle.Instance.Run();
			Y2019.Day05.Puzzle.Instance.Run();
			Y2019.Day06.Puzzle.Instance.Run();
			Y2019.Day07.Puzzle.Instance.Run();
			Y2019.Day08.Puzzle.Instance.Run();
			Y2019.Day09.Puzzle.Instance.Run();
			Y2019.Day10.Puzzle.Instance.Run();
			Y2019.Day11.Puzzle.Instance.Run();
			Y2019.Day12.Puzzle.Instance.Run();
			Y2019.Day13.Puzzle.Instance.Run();
			Y2019.Day14.Puzzle.Instance.Run();
			Y2019.Day15.Puzzle.Instance.Run();
			Y2019.Day16.Puzzle.Instance.Run();
			Y2019.Day17.Puzzle.Instance.Run();
			Y2019.Day18.Puzzle.Instance.Run();
			Y2019.Day19.Puzzle.Instance.Run();
			Y2019.Day20.Puzzle.Instance.Run();
			Y2019.Day21.Puzzle.Instance.Run();
			Y2019.Day22.Puzzle.Instance.Run();
			Y2019.Day23.Puzzle.Instance.Run();
			Y2019.Day24.Puzzle.Instance.Run();
			Y2019.Day25.Puzzle.Instance.Run();
		}
		private static void Y2020Puzzles()
		{
			Y2020.Day01.Puzzle.Instance.Run();
			Y2020.Day02.Puzzle.Instance.Run();
			Y2020.Day03.Puzzle.Instance.Run();
			Y2020.Day04.Puzzle.Instance.Run();
			Y2020.Day05.Puzzle.Instance.Run();
			Y2020.Day06.Puzzle.Instance.Run();
			Y2020.Day07.Puzzle.Instance.Run();
			Y2020.Day08.Puzzle.Instance.Run();
			Y2020.Day09.Puzzle.Instance.Run();
			Y2020.Day10.Puzzle.Instance.Run();
			Y2020.Day11.Puzzle.Instance.Run();
			Y2020.Day12.Puzzle.Instance.Run();
			Y2020.Day13.Puzzle.Instance.Run();
			Y2020.Day14.Puzzle.Instance.Run();
			Y2020.Day15.Puzzle.Instance.Run();
			Y2020.Day16.Puzzle.Instance.Run();
			Y2020.Day17.Puzzle.Instance.Run();
			Y2020.Day18.Puzzle.Instance.Run();
			Y2020.Day19.Puzzle.Instance.Run();
			Y2020.Day20.Puzzle.Instance.Run();
			Y2020.Day21.Puzzle.Instance.Run();
			Y2020.Day22.Puzzle.Instance.Run();
			Y2020.Day23.Puzzle.Instance.Run();
			Y2020.Day24.Puzzle.Instance.Run();
			Y2020.Day25.Puzzle.Instance.Run();
		}
		private static void Y2021Puzzles()
		{
			Y2021.Day01.Puzzle.Instance.Run();
			Y2021.Day02.Puzzle.Instance.Run();
			Y2021.Day03.Puzzle.Instance.Run();
			Y2021.Day04.Puzzle.Instance.Run();
			Y2021.Day05.Puzzle.Instance.Run();
			Y2021.Day06.Puzzle.Instance.Run();
			Y2021.Day07.Puzzle.Instance.Run();
			Y2021.Day08.Puzzle.Instance.Run();
			Y2021.Day09.Puzzle.Instance.Run();
			Y2021.Day10.Puzzle.Instance.Run();
			Y2021.Day11.Puzzle.Instance.Run();
			Y2021.Day12.Puzzle.Instance.Run();
			Y2021.Day13.Puzzle.Instance.Run();
			Y2021.Day14.Puzzle.Instance.Run();
			Y2021.Day15.Puzzle.Instance.Run();
			Y2021.Day16.Puzzle.Instance.Run();
			Y2021.Day17.Puzzle.Instance.Run();
			Y2021.Day18.Puzzle.Instance.Run();
	//		Y2021.Day19.Puzzle.Instance.Run();
			Y2021.Day20.Puzzle.Instance.Run();
			Y2021.Day21.Puzzle.Instance.Run();
			Y2021.Day22.Puzzle.Instance.Run();
			Y2021.Day23.Puzzle.Instance.Run();
			Y2021.Day24.Puzzle.Instance.Run();
			Y2021.Day25.Puzzle.Instance.Run();
		}
		private static void Y2022Puzzles()
		{
			Y2022.Day01.Puzzle.Instance.Run();
			Y2022.Day02.Puzzle.Instance.Run();
			Y2022.Day03.Puzzle.Instance.Run();
			Y2022.Day04.Puzzle.Instance.Run();
			Y2022.Day05.Puzzle.Instance.Run();
			Y2022.Day06.Puzzle.Instance.Run();
			Y2022.Day07.Puzzle.Instance.Run();
			Y2022.Day08.Puzzle.Instance.Run();
			Y2022.Day09.Puzzle.Instance.Run();
			Y2022.Day10.Puzzle.Instance.Run();
			Y2022.Day11.Puzzle.Instance.Run();
			Y2022.Day12.Puzzle.Instance.Run();
			Y2022.Day13.Puzzle.Instance.Run();
			Y2022.Day14.Puzzle.Instance.Run();
			Y2022.Day15.Puzzle.Instance.Run();
			Y2022.Day16.Puzzle.Instance.Run();
			Y2022.Day17.Puzzle.Instance.Run();
			Y2022.Day18.Puzzle.Instance.Run();
			Y2022.Day19.Puzzle.Instance.Run();
			Y2022.Day20.Puzzle.Instance.Run();
			Y2022.Day21.Puzzle.Instance.Run();
			Y2022.Day22.Puzzle.Instance.Run();
			Y2022.Day23.Puzzle.Instance.Run();
			Y2022.Day24.Puzzle.Instance.Run();
			Y2022.Day25.Puzzle.Instance.Run();
		}
	}
}
