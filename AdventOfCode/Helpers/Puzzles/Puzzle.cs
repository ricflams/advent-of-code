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

		protected void WriteName(TimeSpan? elapsed, string filename, int part)
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
			Console.Write($"{Year} Day {Day,2} Part {part} of {Name}: ");
		}

		protected void WriteResult(T result, T expectedResult)
		{
			Console.Write(result);
			if (result != null && !result.Equals(expectedResult))
			{
		        Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"  ****FAIL**** expected {expectedResult}");
		        Console.ResetColor();
			}
		}

		protected void WriteSpeedup(TimeSpan elapsed, TimeSpan optimized)
		{
			var speed =
				elapsed.TotalMilliseconds == 0 ? "(???)" :
				(optimized / elapsed).ToString(".#%");
	        Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.Write($"  Optimized runtime: {speed}");
	        Console.ResetColor();
		}
	}
}
