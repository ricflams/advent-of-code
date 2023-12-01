using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers.Puzzles
{
	public static class PuzzleOptions
	{
		private static Func<int, int, bool> Filter = (_,_) => true;
		static public void RunOnly(Func<int, int, bool> filter) { Filter = filter; }

		static public bool OnlyRunForInputs { get; set; } = false;
		static public bool Silent { get; set; } = false;
		static public int Iterations { get; set; } = 1;

		static public bool ShouldRun(IPuzzle puzzle, string filename)
		{
			if (!Filter(puzzle.Year, puzzle.Day))
				return false;
			if (OnlyRunForInputs)
				return filename == "input";
			return true;
		}
	}
}
