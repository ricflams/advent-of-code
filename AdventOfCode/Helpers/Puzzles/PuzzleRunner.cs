using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode.Helpers.Puzzles
{
	abstract class PuzzleRunner<T>
		where T : struct
	{
		protected abstract int Year { get; }
		protected abstract int Day { get; }

		protected void RunPuzzles(string filename, T? expectedResult1, T? expectedResult2)
		{
			var puzzleFilename = $"Y{Year}/Day{Day:D2}/{filename}";
			var input = File.ReadAllLines(puzzleFilename);

			var result1 = Puzzle1(input);
			Console.WriteLine($"Day {Day:D2} Puzzle 1: {result1}");
			if (expectedResult1.HasValue)
			{
				Debug.Assert(result1.Equals(expectedResult1.Value));
			}

			var result2 = Puzzle2(input);
			Console.WriteLine($"Day {Day:D2} Puzzle 2: {result2}");
			if (expectedResult2.HasValue)
			{
				Debug.Assert(result2.Equals(expectedResult2.Value));
			}
		}

		protected abstract T Puzzle1(string[] input);
		protected abstract T Puzzle2(string[] input);
	}

	abstract class PuzzleRunner<T1, T2>
		where T1 : struct
		where T2 : struct
	{
		protected abstract int Year { get; }
		protected abstract int Day { get; }

		protected void RunPuzzles(string filename, T1? expectedResult1, T2? expectedResult2)
		{
			var puzzleFilename = $"Y{Year}/Day{Day:D2}/{filename}";
			var input = File.ReadAllLines(puzzleFilename);

			var (result1, result2) = Puzzle1And2(input);
			Console.WriteLine($"Day {Day:D2} Puzzle 1: {result1}");
			if (expectedResult1.HasValue)
			{
				Debug.Assert(result1.Equals(expectedResult1.Value));
			}
			Console.WriteLine($"Day {Day:D2} Puzzle 2: {result2}");
			if (expectedResult2.HasValue)
			{
				Debug.Assert(result2.Equals(expectedResult2.Value));
			}
		}

		protected abstract (T1, T2) Puzzle1And2(string[] input);
	}
}
