using System;
using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode.Helpers.Puzzles
{
	internal abstract class SoloParts<T> : Puzzle<T>
	{
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
			return method != null && method.DeclaringType != typeof(SoloParts<T>);
		}
	}
}
