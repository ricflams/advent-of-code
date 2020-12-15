using System;
using System.IO;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class Puzzle<T>
		where T : struct
	{
		protected abstract int Year { get; }
		protected abstract int Day { get; }
		protected string[] ReadInput(string filename) => File.ReadAllLines($"Y{Year}/Day{Day:D2}/{filename}.txt");
		protected void VeryfyResult(string filename, int part, T result, T? expectedResult)
		{
			if (expectedResult.HasValue)
			{
				Console.WriteLine($"Day {Day:D2} Part {part} for {filename}: {result}");
				if (!result.Equals(expectedResult.Value))
				{
					Console.Error.WriteLine($"**** FAIL ****: expected {expectedResult}\n");
				}
			}
		}
	}
}
