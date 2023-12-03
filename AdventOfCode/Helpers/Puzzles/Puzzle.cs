using System;
using System.Diagnostics;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class Puzzle<T1,T2> : RunnablePuzzle
	{
		private static readonly string TimingBar = new('#', 10);
		private static readonly TimeSpan TimingBarUnit = TimeSpan.FromMilliseconds(100);

		protected abstract T1 Part1(string[] input);
		protected abstract T2 Part2(string[] input);

		public Runner Run(string testname)
		{
			return new Runner(this, testname, testname);
		}

		internal class Runner
		{
			private readonly Puzzle<T1,T2> _puzzle;
			private readonly string _testname;
			private readonly string _filename;

			public Runner(Puzzle<T1,T2> puzzle, string testname, string filename) =>
				(_puzzle, _testname, _filename) = (puzzle, testname, filename);

			public Runner Part1(T1 expectedResult)
			{
				_puzzle.RunPart(_testname, _filename, 1, _puzzle.Part1, expectedResult);
				return this;
			}

			public Runner Part2(T2 expectedResult)
			{
				_puzzle.RunPart(_testname, _filename, 2, _puzzle.Part2, expectedResult);
				return this;
			}
		}

		internal void RunPart<T>(string testname, string filename, int part, Func<string[],T> solution, T expectedResult)
		{
			if (!Options.ShouldRun(this, testname))
				return;

			T result = default;
			var input = FileSystem.Instance.ReadFile(Year, Day, filename);
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < Options.Iterations; i++)
			{
				result = solution(input);
			}
			if (!Options.Silent)
			{
				WriteName(sw.Elapsed / Options.Iterations, testname, part);
				WriteResult(result, expectedResult);
				Console.WriteLine();
			}
		}

		private void WriteName(TimeSpan elapsed, string filename, int part)
		{
			var units = Math.Round(elapsed / TimingBarUnit, MidpointRounding.AwayFromZero);
			var bars = (int)Math.Min(units, TimingBar.Length);
			var t = 1000.0 * elapsed.Ticks / Stopwatch.Frequency;
			Console.Write($"[{TimingBar.Substring(0, bars),-10}] {t,7:F2} ms: ");
			if (!Options.OnlyRunForInputs)
			{
				Console.Write($"[{filename}] ");
			}
			Console.Write($"{Year} Day {Day,2} Part {part} of {Name}: ");
		}

		protected static void WriteResult<T>(T result, T expectedResult)
		{
			Console.Write(result);
			if (!expectedResult.Equals(result))
			{
		        Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"  ****FAIL**** expected {expectedResult}");
		        Console.ResetColor();
			}
		}
	}
}
