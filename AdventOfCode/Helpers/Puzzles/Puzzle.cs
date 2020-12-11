using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode.Helpers.Puzzles
{
	abstract class Puzzle
	{
		protected abstract int Year { get; }
		protected abstract int Day { get; }

		public void RunPuzzle()
		{
			Run();
		}

		public abstract void Run();

		protected void VerifyPart1(string actual, string expected)
		{
			Console.WriteLine($"Day {Day:D2} Puzzle 1: {actual}");
			Debug.Assert(actual == expected);
		}

		protected void VerifyPart2(long actual, long expected)
		{
			Console.WriteLine($"Day {Day:D2} Puzzle 2: {actual}");
			Debug.Assert(actual == expected);
		}
	}
}
