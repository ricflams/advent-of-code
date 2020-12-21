using System;

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
			var input = ReadInput(filename);
			var result = solution(input);
			VeryfyResult(filename, part, result, expectedResult);
		}
	}
}
