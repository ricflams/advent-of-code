using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode.Helpers.Puzzles
{

	internal abstract class PuzzleWithParam<TP, T1,T2> : Puzzle<T1, T2>
	{
		protected TP Param { get; set; }

		public new PuzzleRunnerWithParam Run(string testname)
		{
			return new PuzzleRunnerWithParam(this, testname, testname);
		}

		public PuzzleRunner RunParamOnly(string testname)
		{
			return new PuzzleRunner(this, testname, null);
		}

		internal class PuzzleRunnerWithParam
		{

			private readonly PuzzleWithParam<TP, T1, T2> _puzzle;
			private readonly string _testname;
			private string _filename;			
			public PuzzleRunnerWithParam(PuzzleWithParam<TP, T1, T2> puzzle, string testname, string filename) =>
				(_puzzle, _testname, _filename) = (puzzle, testname, filename);

			public PuzzleRunnerWithParam WithParam(TP param)
			{
				_puzzle.Param = param;
				return this;
			}

			public PuzzleRunnerWithParam WithNoInput()
			{
				_filename = null;
				return this;
			}

			public PuzzleRunnerWithParam Part1(T1 expectedResult)
			{
				_puzzle.RunPart(_testname, _filename, 1, _puzzle.Part1, expectedResult);
				return this;
			}
			public PuzzleRunnerWithParam Part2(T2 expectedResult)
			{
				_puzzle.RunPart(_testname, _filename, 2, _puzzle.Part2, expectedResult);
				return this;
			}			
		}


	}

	internal abstract class Puzzle<T1,T2> : IPuzzle
	{
		private static readonly string TimingBar = new string('#', 10);
		private static readonly TimeSpan TimingBarUnit = TimeSpan.FromMilliseconds(100);
		public abstract string Name { get; }
		public abstract int Year { get; }
		public abstract int Day { get; }

		

		protected abstract T1 Part1(string[] input);
		protected abstract T2 Part2(string[] input);


		internal class PuzzleRunner
		{
			private readonly Puzzle<T1,T2> _puzzle;
			private readonly string _testname;
			private readonly string _filename;
			public PuzzleRunner(Puzzle<T1,T2> puzzle, string testname, string filename) =>
				(_puzzle, _testname, _filename) = (puzzle, testname, filename);
			public PuzzleRunner Part1(T1 expectedResult)
			{
				_puzzle.RunPart(_testname, _filename, 1, _puzzle.Part1, expectedResult);
				return this;
			}
			public PuzzleRunner Part2(T2 expectedResult)
			{
				_puzzle.RunPart(_testname, _filename, 2, _puzzle.Part2, expectedResult);
				return this;
			}
		}

		public PuzzleRunner Run(string testname)
		{
			return new PuzzleRunner(this, testname, testname);
		}

		private string[] ReadInput(string filename)
		{
			return filename == null
				? null
				: File.ReadAllLines($"Y{Year}/Day{Day:D2}/{filename}.txt");
		}

		internal void RunPart<T>(string testname, string filename, int part, Func<string[],T> solution, T expectedResult)
		{
			if (!PuzzleOptions.ShouldRun(this, testname))
				return;

			T result = default;
			var input = ReadInput(filename);
			var sw = Stopwatch.StartNew();
			for (var i = 0; i < PuzzleOptions.Iterations; i++)
			{
				result = solution(input);
			}
			if (!PuzzleOptions.Silent)
			{
				WriteName(sw.Elapsed / PuzzleOptions.Iterations, testname, part);
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
			if (!PuzzleOptions.OnlyRunForInputs)
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
