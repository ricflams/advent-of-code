using System;

namespace AdventOfCode.Helpers.Puzzles
{
	public class PuzzleOptions
	{
		private Func<int, int, bool> _filter = (_,_) => true;
		public void RunOnly(Func<int, int, bool> filter) { _filter = filter; }

		public bool OnlyRunForInputs { get; set; } = false;
		public bool Silent { get; set; } = false;
		public int Iterations { get; set; } = 1;

		public bool ShouldRun(IPuzzle puzzle, string filename)
		{
			if (!_filter(puzzle.Year, puzzle.Day))
				return false;
			//return filename == "extra";
			if (OnlyRunForInputs)
				return filename == "input";
			return true;
		}
	}
}
