using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2020.Day01
{
	internal class Puzzle01
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var expenses = File.ReadLines("Y2020/Day01/input.txt").Select(int.Parse);
			var expenseLookup = new HashSet<int>(expenses);

			int result = 0;
			foreach (var e1 in expenses)
			{
				var e2 = 2020 - e1;
				if (expenseLookup.Contains(e2))
				{
					result = e1 * e2;
					break;
				}
			}

			Console.WriteLine($"Day 01 Puzzle 1: {result}");
			Debug.Assert(result == 542619);
		}

		private static void Puzzle2()
		{
			var expenses = File.ReadLines("Y2020/Day01/input.txt").Select(int.Parse);
			var expenseLookup = new HashSet<int>(expenses);

			int result = 0;
			foreach (var e1 in expenses)
			{
				foreach (var e2 in expenses.Where(x => x != e1))
				{
					var e3 = 2020 - (e1 + e2);
					if (expenseLookup.Contains(e3))
					{
						result = e1 * e2 * e3;
						break;
					}
				}
			}

			Console.WriteLine($"Day 01 Puzzle 2: {result}");
			Debug.Assert(result == 32858450);
		}
	}
}
