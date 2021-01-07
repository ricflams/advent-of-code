namespace AdventOfCode.Helpers.Puzzles
{
	public static class PuzzleOptions
	{
		private static int? _year;
		private static int? _day;
		static public bool OnlyRunForInputs { get; set; } = false;
		static public bool ShowTimings { get; set; } = false;
		static public int TimingLoops { get; set; } = 0;
		static public void RunOnly(int year, int? day = null)
		{
			_year = year;
			_day = day;
		}

		static public bool ShouldRun(IPuzzle puzzle, string filename)
		{
			if (_year.HasValue && puzzle.Year != _year)
				return false;
			if (_day.HasValue && puzzle.Day != _day)
				return false;
			if (OnlyRunForInputs)
				return filename == "input";
			return true;
		}
	}
}
