using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day17
{
	internal class Puzzle17
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2015/Day17/input.txt");
			var goal = 150;

			var containers = input.Select(int.Parse).ToArray();

			var all = NumberOfCombinations(containers, goal);
			Console.WriteLine($"Day 17 Puzzle 1: {all}");
			Debug.Assert(all == 4372);

			var shortest = NumberOfShortestCombinations(containers, goal);
			Console.WriteLine($"Day 17 Puzzle 2: {shortest}");
			Debug.Assert(shortest == 4);
		}

		private static int NumberOfCombinations(int[] list, int initialGoal)
		{
			var memo = new Dictionary<string, int>();
			return NumberOfCombinations(list.Length, initialGoal);

			int NumberOfCombinations(int size, int goal)
			{
				if (goal == 0)
					return 1;

				if (goal < 0 || size == 0)
					return 0;

				var id = $"{size}:{goal}";
				if (!memo.TryGetValue(id, out var n))
				{
					n = NumberOfCombinations(size - 1, goal) +
					   NumberOfCombinations(size - 1, goal - list[size - 1]); // decrement set because we don't reuse containers (unlike coins)
					memo[id] = n;
				}
				return n;
			}
		}

		private static int NumberOfShortestCombinations(int[] list, int initialGoal)
		{
			var memo = new SimpleMemo<ulong>();
			var lengths = new SafeDictionary<int, int>();

			Explore(list.Length, 0, initialGoal);
			var shortestCombinations = lengths.OrderBy(x => x.Key).First().Value;
			return shortestCombinations;

			void Explore(int size, uint pickmask, int goal)
			{
				if (goal == 0)
				{
					var len = pickmask.NumberOfSetBits();
					lengths[len]++;
					return;
				}

				if (goal < 0 || size == 0)
				{
					return;
				}

				if (memo.IsSeenBefore((ulong)size << 32 | pickmask))
				{
					return;
				}

				// For skipped item, pass 0 into the bit-mask (ie do nothing); for used item pass 1.
				Explore(size - 1, pickmask << 1, goal);
				Explore(size - 1, (pickmask << 1) + 1, goal - list[size - 1]);
			}
		}
	}
}
