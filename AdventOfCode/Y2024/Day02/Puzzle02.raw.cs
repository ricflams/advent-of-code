using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Xml.Schema;

namespace AdventOfCode.Y2024.Day02.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(2).Part2(4);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			
			var levels = input.Select(x => x.ToIntArray()).ToArray();

			var safe = 0;
			foreach (var v in levels)
			{
				if (IsSafe(v))
				{
					safe++;
				}
				// if (diffs.All(x => x > 0)) {
				// 	if (diffs.All(x => x >= 1 & x <= 3))
				// 	{
				// 		safe++;
				// 	}
				// 	continue;
				// }
				// if (diffs.All(x => x < 0)) {
				// 	if (diffs.All(x => -x >= 1 & -x <= 3))
				// 	{
				// 		safe++;
				// 	}
				// 	continue;
				// }
			}

			return safe;
		}

		private static bool IsSafe(int [] v)
		{
			var diffs = new int[v.Length-1];
			for (var i = 0; i < v.Length-1; i++)
			{
				diffs[i] = v[i+1] - v[i];
			}

			if (diffs.All(x => x > 0)) {
				if (diffs.All(x => x >= 1 & x <= 3))
				{
					return true;
				}
			}
			if (diffs.All(x => x < 0)) {
				if (diffs.All(x => -x >= 1 & -x <= 3))
				{
					return true;
				}
			}
			return false;
		}

		protected override long Part2(string[] input)
		{
			var levels = input.Select(x => x.ToIntArray()).ToArray();

			var safe = 0;
			foreach (var v in levels)
			{
				for (var j = 0; j < v.Length; j++)
				{
					var xx = v.ToList();
					xx.RemoveAt(j);
					var vv = xx.ToArray();

					if (IsSafe(vv))
					{
						safe++;
						break;
					}

				}
			}

			return safe;
		}
	}
}
