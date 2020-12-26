using System;
using System.Diagnostics;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class SoloParts<T> : Puzzle<T>
	{
		protected abstract T Part1(string[] input);
		protected abstract T Part2(string[] input);

		protected void RunFor(string filename, T expectedResult1, T expectedResult2)
		{
			RunPart1For(filename, expectedResult1);
			RunPart2For(filename, expectedResult2);
		}

		public void RunPart1For(string filename, T expectedResult) => RunPart(filename, 1, Part1, expectedResult);

		public void RunPart2For(string filename, T expectedResult) => RunPart(filename, 2, Part2, expectedResult);

		private void RunPart(string filename, int part, Func<string[],T> solution, T expectedResult)
		{
			if (PuzzleOptions.ShouldRun(filename))
			{
				var loops = 1;
				var sw = Stopwatch.StartNew();
				var input = ReadInput(filename);
				var result = solution(input);
				if (PuzzleOptions.TimingLoops > 0)
				{
					sw.Restart();
					for (var i = 0; i < PuzzleOptions.TimingLoops; i++)
					{
						input = ReadInput(filename);
						result = solution(input);
					}
					loops = PuzzleOptions.TimingLoops;
				}
				var elapsed = sw.Elapsed / loops;
				WriteAndVerifyfyResult(elapsed, filename, part, result, expectedResult);
			}
		}
	}
}
