using System;
using System.IO;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class Puzzle<T>
	{
		private static readonly string TimingBar = new string('#', 10);
		private static readonly TimeSpan TimingBarUnit = TimeSpan.FromMilliseconds(100);
		protected abstract int Year { get; }
		protected abstract int Day { get; }

		protected string[] ReadInput(string filename) => File.ReadAllLines($"Y{Year}/Day{Day:D2}/{filename}.txt");

		protected void WriteAndVerifyfyResult(TimeSpan? elapsed, string filename, int part, T result, T expectedResult)
		{
			if (PuzzleOptions.ShowTimings)
			{
				if (elapsed.HasValue)
				{
					var units = Math.Round(elapsed.Value / TimingBarUnit, MidpointRounding.AwayFromZero);
					var bars = (int)Math.Min(units, TimingBar.Length);
					var t = (int)Math.Ceiling(elapsed.Value.TotalMilliseconds);
					Console.Write($"[{TimingBar.Substring(0, bars),-10}  {t,4} ms]  ");
				}
				else
				{
					Console.Write($"^{"",19}^  ");
				}
			}
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
