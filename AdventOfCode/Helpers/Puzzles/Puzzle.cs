using System;
using System.Diagnostics;
using System.Reflection;
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

		protected abstract T Part1(string[] input);
		protected abstract T Part2(string[] input);
		protected virtual T Part1Optimized(string[] input) => default(T);
		protected virtual T Part2Optimized(string[] input) => default(T);

		protected void RunFor(string filename, T expectedResult1, T expectedResult2)
		{
			RunPart1For(filename, expectedResult1);
			RunPart2For(filename, expectedResult2);
		}

		public void RunPart1For(string filename, T expectedResult)
		{
			if (PuzzleOptions.ShouldRun(this, filename))
			{
				var elapsed = RunPart(filename, 1, Part1, expectedResult);
				Console.WriteLine();
				if (ShouldRunOptimizedPart(nameof(Part1Optimized)))
				{
					var optimized = RunPart(filename, 1, Part1Optimized, expectedResult);
					WriteSpeedup(elapsed, optimized);
					Console.WriteLine();
				}
			}
		}

		public void RunPart2For(string filename, T expectedResult)
		{
			if (PuzzleOptions.ShouldRun(this, filename))
			{
				var elapsed = RunPart(filename, 2, Part2, expectedResult);
				Console.WriteLine();
				if (ShouldRunOptimizedPart(nameof(Part2Optimized)))
				{
					var optimized = RunPart(filename, 2, Part2Optimized, expectedResult);
					WriteSpeedup(elapsed, optimized);
					Console.WriteLine();
				}
			}
		}

		private string[] ReadInput(string filename) => File.ReadAllLines($"Y{Year}/Day{Day:D2}/{filename}.txt");

		private TimeSpan RunPart(string filename, int part, Func<string[],T> solution, T expectedResult)
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
			WriteName(elapsed, filename, part);
			WriteResult(result, expectedResult);
			return elapsed;
		}

		private bool ShouldRunOptimizedPart(string methodName)
		{
			if (PuzzleOptions.TimingLoops == 0)
			{
				return false;
			}
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var method = GetType()?.GetMethod(methodName, flags);
			return method != null;
		}

		private void WriteName(TimeSpan? elapsed, string filename, int part)
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
			if (!expectedResult.Equals(result))
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
