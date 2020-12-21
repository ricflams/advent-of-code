namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class ComboParts<T> : Puzzle<T>
	{
		protected abstract (T, T) Part1And2(string[] input);

		protected void RunFor(string filename, T expectedResult1, T expectedResult2)
		{
			var input = ReadInput(filename);
			var (result1, result2) = Part1And2(input);
			VeryfyResult(filename, 1, result1, expectedResult1);
			VeryfyResult(filename, 2, result2, expectedResult2);
		}
	}
}
