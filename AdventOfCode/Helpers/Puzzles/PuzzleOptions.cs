using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers.Puzzles
{
	public static class PuzzleOptions
	{
		private static List<(int Year, int Day)> PuzzleDays = new();
		static public bool OnlyRunForInputs { get; set; } = false;
		static public bool Silent { get; set; } = false;
		static public int Iterations { get; set; } = 1;
		static public void RunOnly(int year, int day)
		{
			PuzzleDays.Add((year, day));
		}

		static public bool ShouldRun(IPuzzle puzzle, string filename)
		{
			if (PuzzleDays.Any() && !PuzzleDays.Contains((puzzle.Year, puzzle.Day)))
				return false;
			if (OnlyRunForInputs)
				return filename == "input";
			return true;
		}
	}
}
