using System;
using System.IO;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class Puzzle<T>
	{
		protected abstract int Year { get; }
		protected abstract int Day { get; }

		protected string[] ReadInput(string filename) => File.ReadAllLines($"Y{Year}/Day{Day:D2}/{filename}.txt");

		protected void VeryfyResult(string filename, int part, T result, T expectedResult)
		{
			Console.Write($"Y{Year} Day {Day:D2} Part {part} for {filename}: {result}");
			if (result.Equals(expectedResult))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine($"  ****FAIL**** expected {expectedResult}");
			}
		}
	}
}
