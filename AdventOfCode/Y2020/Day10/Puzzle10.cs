using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day10
{
	internal class Puzzle10
	{
		public static void Run()
		{
			var input = File.ReadAllLines("Y2020/Day10/input.txt");

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
			var result1 = diff1 * diff3;
			Console.WriteLine($"Day 10 Puzzle 1: {result1}");
			Debug.Assert(result1 == 2070);

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
			long result2 = CountCombinations(0, 0, joltages);
			Console.WriteLine($"Day 10 Puzzle 2: {result2}");
			Debug.Assert(result2 == 24179327893504);
		}
	}
}
