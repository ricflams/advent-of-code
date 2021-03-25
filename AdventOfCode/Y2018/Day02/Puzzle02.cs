using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2018.Day02
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Inventory Management System";
		public override int Year => 2018;
		public override int Day => 2;

		public void Run()
		{
			Run("test1").Part1(12).Part2("abcde");

 			// https://www.reddit.com/r/adventofcode/comments/a2rt9s/2018_day_2_part_2_here_are_some_big_inputs_to/
			// Input with 100,000 strings runs in 850ms
			Run("test2").Part2("imobgvpsuafxboeufbjpmstiw");

			Run("input").Part1(6150).Part2("rteotyxzbodglnpkudawhijsc");
		}

		protected override int Part1(string[] input)
		{
			var ids = input;

			var n2 = 0;
			var n3 = 0;
			foreach (var id in ids)
			{
				// Count occurrences of each char in a simple array for all the
				// letters a-z. When done, check if any counts were 2 or 3.
				var seen = new int[26]; // a-z
				foreach (var c in id)
				{
					seen[c - 'a']++;
				}
				if (seen.Any(x => x == 2))
				{
					n2++;
				}
				if (seen.Any(x => x == 3))
				{
					n3++;
				}
			}

			var checksum = n2*n3;
			return checksum;
		}

		protected override string Part2(string[] input)
		{
			var ids = input;

			// Instead of doing the trivial N^2 set of comparisons (where each comparison
			// would even need to step through all chars to find just one mismatch) do this:
			// For every string, remember n versions of it with each letter, one at a time,
			// swapped out with a dot. If we ever come across a similar string then we've
			// found the single-letter-mismatch. The result is the string except that dot
			// at the mismatched position - very nice.
			var seen = new SimpleMemo<string>();
			foreach (var id in ids)
			{
				var letters = id.ToCharArray();
				for (var i = 0; i < letters.Length; i++)
				{
					var tmp = letters[i];
					letters[i] = '.';
					var s = new string(letters);
					if (seen.IsSeenBefore(s))
					{
						// We found the mismatch
						return s.Replace(".", "");
					}
					letters[i] = tmp;
				}
			}
			throw new Exception("Not found");
		}
	}
}
