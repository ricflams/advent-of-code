using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Linen Layout";
		public override int Year => 2024;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(6).Part2(16);
			Run("input").Part1(365).Part2(730121486795169);
			Run("extra").Part1(350).Part2(769668867512623);
		}

		protected override long Part1(string[] input)
		{
			var patterns = input[0].SplitByComma();
			var designs = input[2..];

			var memo = new Dictionary<string, bool>();
			bool IsPossible(string des)
			{
				if (des.Length == 0)
					return true;
				if (!memo.TryGetValue(des, out var isPossible))
				{
					isPossible = memo[des] =
						patterns.Any(p => des.StartsWith(p) && IsPossible(des[p.Length..]));
				}
				return isPossible;
			}

			var count = designs.Count(IsPossible);
			return count;
		}

		protected override long Part2(string[] input)
		{
			var patterns = input[0].SplitByComma();
			var designs = input[2..];

			var memo = new SafeDictionary<string, long>();
			long CountVariations(string des)
			{
				if (des.Length == 0)
					return 1;
				if (!memo.TryGetValue(des, out var n))
				{
					n = memo[des] = patterns.Where(des.StartsWith).Sum(p => CountVariations(des[p.Length..]));
				}
				return n;
			}

			var sum = designs.Sum(CountVariations);
			return sum;
		}
	}
}
