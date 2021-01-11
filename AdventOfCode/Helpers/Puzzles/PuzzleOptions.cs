using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers.Puzzles
{
	public static class PuzzleOptions
	{
		private static int? _year;
		private static HashSet<int> _days;
		static public bool OnlyRunForInputs { get; set; } = false;
		static public bool ShowTimings { get; set; } = false;
		static public int TimingLoops { get; set; } = 0;
		static public void RunOnly(int year, params int[] days)
		{
			_year = year;
			_days = new HashSet<int>(days);
		}

		static public bool ShouldRun(IPuzzle puzzle, string filename)
		{
			if (_year.HasValue && puzzle.Year != _year)
				return false;
			if (_days.Any() && !_days.Contains(puzzle.Day))
				return false;
			if (OnlyRunForInputs)
				return filename == "input";
			return true;
		}
	}
}
