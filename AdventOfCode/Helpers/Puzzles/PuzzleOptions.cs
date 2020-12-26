using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Helpers.Puzzles
{
	public static class PuzzleOptions
	{
		static public bool OnlyRunForInputs { get; set; } = false;
		static public bool ShowTimings { get; set; } = false;
		static public int TimingLoops { get; set; } = 0;

		static public bool ShouldRun(string filename) => !OnlyRunForInputs || filename == "input";
	}
}
