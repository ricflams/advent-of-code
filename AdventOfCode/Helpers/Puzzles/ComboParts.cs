using System.Diagnostics;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class ComboParts<T> : Puzzle<T>
	{
		protected abstract (T, T) Part1And2(string[] input);

		protected void RunFor(string filename, T expectedResult1, T expectedResult2)
		{
			if (PuzzleOptions.ShouldRun(filename))
			{
				var loops = 1;
				var sw = Stopwatch.StartNew();
				var input = ReadInput(filename);
				var (result1, result2) = Part1And2(input);
				if (PuzzleOptions.TimingLoops > 0)
				{
					sw.Restart();
					for (var i = 0; i < PuzzleOptions.TimingLoops; i++)
					{
						input = ReadInput(filename);
						(result1, result2) = Part1And2(input);
					}
					loops = PuzzleOptions.TimingLoops;
				}
				var elapsed = sw.Elapsed / loops;
				WriteAndVerifyfyResult(elapsed, filename, 1, result1, expectedResult1);
				WriteAndVerifyfyResult(null, filename, 2, result2, expectedResult2);
			}
		}
	}
}
