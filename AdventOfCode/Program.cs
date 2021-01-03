using AdventOfCode.Helpers.Puzzles;
using System;
using System.Diagnostics;

namespace AdventOfCode
{
	internal class Program
	{
		private static void Main()
		{
			//Helpers.PuzzleDay.GeneratePuzzles.Generate(2015);
			var sw = Stopwatch.StartNew();

			// PuzzleOptions.OnlyRunForInputs = true;
			PuzzleOptions.ShowTimings = true;
			// PuzzleOptions.TimingLoops = 2;

			Y2015Puzzles();
			Y2016Puzzles();
			Y2019Puzzles();
			Y2020Puzzles();

			Console.WriteLine($"Elapsed: {(int)(sw.ElapsedMilliseconds / (1 + PuzzleOptions.TimingLoops))} ms");
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
			// Y2016.Day02.Puzzle.Instance.Run();
			// Y2016.Day03.Puzzle.Instance.Run();
			// Y2016.Day04.Puzzle.Instance.Run();
			// Y2016.Day05.Puzzle.Instance.Run();
			// Y2016.Day06.Puzzle.Instance.Run();
			// Y2016.Day07.Puzzle.Instance.Run();
			// Y2016.Day08.Puzzle.Instance.Run();
			// Y2016.Day09.Puzzle.Instance.Run();
			// Y2016.Day10.Puzzle.Instance.Run();
			// Y2016.Day11.Puzzle.Instance.Run();
			// Y2016.Day12.Puzzle.Instance.Run();
			// Y2016.Day13.Puzzle.Instance.Run();
			// Y2016.Day14.Puzzle.Instance.Run();
			// Y2016.Day15.Puzzle.Instance.Run();
			// Y2016.Day16.Puzzle.Instance.Run();
			// Y2016.Day17.Puzzle.Instance.Run();
			// Y2016.Day18.Puzzle.Instance.Run();
			// Y2016.Day19.Puzzle.Instance.Run();
			// Y2016.Day20.Puzzle.Instance.Run();
			// Y2016.Day21.Puzzle.Instance.Run();
			// Y2016.Day22.Puzzle.Instance.Run();
			// Y2016.Day23.Puzzle.Instance.Run();
			// Y2016.Day24.Puzzle.Instance.Run();
			// Y2016.Day25.Puzzle.Instance.Run();
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
	}
}
