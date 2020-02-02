using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day17
{
	internal class Puzzle17
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2015/Day17/input.txt");
			var goal = 150;

			var containers = input.Select(int.Parse).ToArray();
			var combinations = NumberOfCombinations(containers, goal);

			Console.WriteLine($"Day 17 Puzzle 1: {combinations}");
			Debug.Assert(combinations == 4372);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2015/Day17/input.txt");
			var goal = 150;


			var containers = input.Select(int.Parse).ToArray();

			containers = new int[] { 20, 15, 10, 5, 5 };
			goal = 25;

			var combinations = NumberOfCombinations2(containers, goal);

			Console.WriteLine($"Day 17 Puzzle 2: {combinations}");
			//Debug.Assert(result == );
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

		private static int NumberOfCombinations2(int[] list, int initialGoal)
		{
			var memo = new Dictionary<string, List<int>>();
			var combos = new List<List<int>>();
			var Empty = new List<int>();

			var ss = NumberOfCombinations2(list.Length, new List<int>(), initialGoal);
			var yy = ss.OrderBy(x => x).ToList();
			var xx = ss.GroupBy(x => x).ToDictionary(x => x.Key, x => x);
			var zz = ss.GroupBy(x => x).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x);
			var nn = ss.GroupBy(x => x).OrderBy(g => g.Key).First().Count();
			return nn;


			List<int> NumberOfCombinations2(int size, List<int> combo, int goal)
			{
				Console.WriteLine($"size={size} goal={goal}");
				if (goal == 0)
				{
					Console.WriteLine($"  goal=0: ######## ");
					combos.Add(combo);
					return combo;
				}

				if (goal < 0 || size == 0)
				{
					Console.WriteLine($"  (no match)");
					return Empty;
				}

				var id = $"{size}:{goal}";
				if (!memo.TryGetValue(id, out var steps))
				{
					var s1 = NumberOfCombinations2(size - 1, combo, goal);
					var s2 = NumberOfCombinations2(size - 1, combo.Append(list[size - 1]).ToList(), goal - list[size - 1]);
					steps = s1.Concat(s2).ToList();
					Console.WriteLine($"  calc {id}  steps={steps.ToCommaString()}  - s1={s1.ToCommaString()} s2={s2.ToCommaString()}: ");
					memo[id] = steps;
				}
				else
				{
					Console.WriteLine($"  memo {id}  steps={steps.ToCommaString()}");
				}
				return steps;
			}
		}
	}
}
