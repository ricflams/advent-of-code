using System;

namespace AdventOfCode.Helpers.Puzzles
{
	public class PuzzleOptions
	{
		private Func<string, int, int, bool> _filter = (_,_,_) => true;
		public void RunOnly(Func<string, int, int, bool> filter) { _filter = filter; }

		public bool OnlyRunForInputs { get; set; } = false;
		public bool Silent { get; set; } = false;
		public int Iterations { get; set; } = 1;

		public bool ShouldRun(IPuzzle puzzle, string filename)
		{
			if (!_filter(filename, puzzle.Year, puzzle.Day))
				return false;
			if (OnlyRunForInputs)
				return filename == "input";
			return true;
		}
	}
}
