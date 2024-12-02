using System;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day02
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Red-Nosed Reports";
		public override int Year => 2024;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(2).Part2(4);
			Run("input").Part1(606).Part2(644);
		}

		protected override long Part1(string[] input)
		{
			var levels = input.Select(x => x.ToIntArray()).ToArray();

			var safe = levels.Count(IsSafe);

			return safe;
		}

		private static bool IsSafe(int[] line)
		{
			var delta = new int[line.Length-1];
			for (var i = 0; i < delta.Length; i++)
			{
				delta[i] = line[i+1] - line[i];
			}

			if (delta.All(x => x >= 1 && x <= 3)) return true;
			if (delta.All(x => -x >= 1 && -x <= 3)) return true;
			return false;
		}

		protected override long Part2(string[] input)
		{
			var levels = input.Select(x => x.ToIntArray()).ToArray();

			static bool IsSafeWithTolerance(int[] line)
			{
				for (var i = 0; i < line.Length; i++)
				{
					if (IsSafe(line.Where((_, idx) => idx != i).ToArray()))
						return true;
				}
				return false;
			}

			var safe = levels.Count(IsSafeWithTolerance);

			return safe;
		}
	}
}
