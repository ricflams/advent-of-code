using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day10
{
	internal class Puzzle : PuzzleRunner<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 10;

		public void Run()
		{
			RunPuzzles("test1.txt", 35, 8);
			RunPuzzles("test2.txt", 220, 19208);
			RunPuzzles("input.txt", 2070, 24179327893504);
		}

		protected override long Puzzle1(string[] input)
		{
			var joltages = input
				.Select(int.Parse)
				.OrderBy(x => x)
				.ToArray();

			var adapters = joltages
				.Prepend(0)
				.Append(joltages.Max() + 3)
				.ToArray();
			var diffs = adapters.Skip(1).Select((x, i) => x - adapters[i]).ToArray();
			var diff1 = diffs.Count(x => x == 1);
			var diff3 = diffs.Count(x => x == 3);
			return diff1 * diff3;
		}

		protected override long Puzzle2(string[] input)
		{
			var joltages = input
				.Select(int.Parse)
				.OrderBy(x => x)
				.ToArray();

			var memo = new Dictionary<int, long>();
			long CountCombinations(int joltage, int pos, IEnumerable<int> chain)
			{
				if (!memo.ContainsKey(pos))
				{
					memo[pos] = chain.Any()
						? chain
							.TakeWhile(x => x <= joltage + 3)
							.Select((jolt, i) => CountCombinations(jolt, pos + i + 1, chain.Skip(i + 1)))
							.Sum()
						: 1;
				}
				return memo[pos];
			}
			return CountCombinations(0, 0, joltages);
		}
	}
}
