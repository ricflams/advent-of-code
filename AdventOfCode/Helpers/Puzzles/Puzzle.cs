using System;
using System.IO;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class Puzzle<T> : IPuzzle
	{
		private static readonly string TimingBar = new string('#', 10);
		private static readonly string NoTiming = new string(' ', 22);
		private static readonly TimeSpan TimingBarUnit = TimeSpan.FromMilliseconds(100);
		public abstract string Name { get; }
		public abstract int Year { get; }
		public abstract int Day { get; }

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
					Console.Write($"[{TimingBar.Substring(0, bars),-10}] {t,4} ms: ");
				}
				else
				{
					Console.Write(NoTiming);
				}
			}
			if (!PuzzleOptions.OnlyRunForInputs)
			{
				Console.Write($"[{filename}] ");
			}
			Console.Write($"{Year} Day {Day,2} Part {part} of {Name}: {result}");
			if (result?.Equals(expectedResult) ?? false)
			{
				Console.WriteLine();
			}
			else
			{
		        Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"  ****FAIL**** expected {expectedResult}");
		        Console.ResetColor();
			}
		}
	}
}
