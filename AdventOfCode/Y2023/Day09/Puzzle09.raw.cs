using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day09.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(114).Part2(2);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var oasis = input
				.Select(s => s.ToIntArray())
				.ToArray();

			var sum = oasis.Sum(LastNumber);
			// foreach (var h in oasis)
			// {
			// 	var diffs = new List<int[]>();
			// 	var hist = h;
			// 	while (true)
			// 	{
			// 		var diff = Enumerable.Range(0, hist.Length - 1).Select(i => hist[i+1] - hist[i]).ToArray();
			// 		diffs.Add(diff);
			// 		if (diff.All(x => x == 0))
			// 			break;
			// 	}
			// 	var last = diffs.Reverse()
			// 	foreach (var)
			// }

			int LastNumber(int[] hist)
			{
				var diff = Enumerable.Range(0, hist.Length - 1).Select(i => hist[i+1] - hist[i]).ToArray();
				if (diff.All(x => x == 0))
					return hist.Last();
				return hist.Last() + LastNumber(diff);
			}

			return sum;
		}

		

		protected override long Part2(string[] input)
		{
			var oasis = input
				.Select(s => s.ToIntArray())
				.ToArray();

			var sum = oasis.Sum(LastNumber);
			// foreach (var h in oasis)
			// {
			// 	var diffs = new List<int[]>();
			// 	var hist = h;
			// 	while (true)
			// 	{
			// 		var diff = Enumerable.Range(0, hist.Length - 1).Select(i => hist[i+1] - hist[i]).ToArray();
			// 		diffs.Add(diff);
			// 		if (diff.All(x => x == 0))
			// 			break;
			// 	}
			// 	var last = diffs.Reverse()
			// 	foreach (var)
			// }

			int LastNumber(int[] hist)
			{
				var diff = Enumerable.Range(0, hist.Length - 1).Select(i => hist[i+1] - hist[i]).ToArray();
				if (diff.All(x => x == 0))
					return hist.Last();
				return hist.First() - LastNumber(diff);
			}

			return sum;
		}
	}
}
