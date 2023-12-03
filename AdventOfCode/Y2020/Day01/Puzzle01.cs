using AdventOfCode.Helpers;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Report Repair";
		public override int Year => 2020;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(514579).Part2(241861950);
			Run("input").Part1(542619).Part2(32858450);
		}

		protected override int Part1(string[] input)
		{
			var expenses = input.Select(int.Parse);
			var expenseLookup = new HashSet<int>(expenses);

			int result = 0;
			foreach (var e1 in expenses)
			{
				var e2 = 2020 - e1;
				if (expenseLookup.Contains(e2) && MathHelper.AreDistinct(e1, e2))
				{
					result = e1 * e2;
					break;
				}
			}

			return result;
		}

		protected override int Part2(string[] input)
		{
			var expenses = input.Select(int.Parse);
			var expenseLookup = new HashSet<int>(expenses);

			int result = 0;
			foreach (var e1 in expenses)
			{
				foreach (var e2 in expenses)
				{
					var e3 = 2020 - (e1 + e2);
					if (expenseLookup.Contains(e3) && MathHelper.AreDistinct(e1, e2, e3))
					{
						result = e1 * e2 * e3;
						break;
					}
				}
			}

			return result;
		}
	}
}
