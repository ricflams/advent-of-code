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




		// protected void RunFor(string filename, T1 expectedResult1, T2 expectedResult2)
		// {
		// 	RunPart1For(filename, expectedResult1);
		// 	RunPart2For(filename, expectedResult2);
		// }

		// public void RunPart1For(string filename, T1 expectedResult)
		// {
		// 	RunPart(filename, filename, 1, Part1, expectedResult);
		// }

		// public void RunPart2For(string filename, T2 expectedResult)
		// {
		// 	RunPart(filename, filename, 2, Part2, expectedResult);
		// }

		private string[] ReadInput(string filename) =>
			filename == null ? null : File.ReadAllLines($"Y{Year}/Day{Day:D2}/{filename}.txt");

		internal void RunPart<T>(string testname, string filename, int part, Func<string[],T> solution, T expectedResult)
		{
			if (!PuzzleOptions.ShouldRun(this, testname))
				return;

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
					if (!expectedResult.Equals(result))
					{
						WriteResult(result, expectedResult);
						Console.WriteLine();
					}
				}
				loops = PuzzleOptions.TimingLoops;
			}
			var elapsed = sw.Elapsed / loops;
			WriteName(elapsed, testname, part);
			WriteResult(result, expectedResult);
			Console.WriteLine();
		}

		private void WriteName(TimeSpan elapsed, string filename, int part)
		{
			var units = Math.Round(elapsed / TimingBarUnit, MidpointRounding.AwayFromZero);
			var bars = (int)Math.Min(units, TimingBar.Length);
			var t = (int)Math.Ceiling(elapsed.TotalMilliseconds);
			Console.Write($"[{TimingBar.Substring(0, bars),-10}] {t,4} ms: ");
			if (!PuzzleOptions.OnlyRunForInputs)
			{
				Console.Write($"[{filename}] ");
			}
			Console.Write($"{Year} Day {Day,2} Part {part} of {Name}: ");
		}

		protected void WriteResult<T>(T result, T expectedResult)
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
