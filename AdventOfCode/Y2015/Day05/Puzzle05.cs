using AdventOfCode.Helpers.Puzzles;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2015.Day05
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Doesn't He Have Intern-Elves For This?";
		public override int Year => 2015;
		public override int Day => 5;

		public void Run()
		{
			Run("input").Part1(236).Part2(51);
		}

		protected override int Part1(string[] input)
		{
			var nice = input.Count(IsNicePuzzle1);
			return nice;
		}

		protected override int Part2(string[] input)
		{
			var nice = input.Count(IsNicePuzzle2);
			return nice;
		}

		private static bool IsNicePuzzle1(string s)
		{
			// A nice string is one with all of the following properties:
			// It contains at least three vowels(aeiou only), like aei, xazegov, or aeiouaeiouaeiou.
			// It contains at least one letter that appears twice in a row, like xx, abcdde(dd), or aabbccdd(aa, bb, cc, or dd).
			// It does not contain the strings ab, cd, pq, or xy, even if they are part of one of the other requirements.

			bool HasDoubleLetter() => Enumerable.Range(0, s.Length - 1).Any(i => s[i] == s[i + 1]);
			if (!HasDoubleLetter())
			{
				return false;
			}

			bool HasThreeVowels()
			{
				var n = 0;
				foreach (var ch in s)
				{
					if ("aeiou".Contains(ch) && ++n == 3)
						return true;
				}
				return false;
			}
			if (!HasThreeVowels())
			{
				return false;
			}

			bool Contains(string substring) => s.Contains(substring);
			if (Contains("ab") || Contains("cd") || Contains("pq") || Contains("xy"))
			{
				return false;
			}

			return true;
		}

		private static bool IsNicePuzzle2(string s)
		{
			// A nice string is one with all of the following properties:
			// It contains a pair of any two letters that appears at least twice in the string without overlapping, like xyxy(xy) or aabcdefgaa(aa), but not like aaa(aa, but it overlaps).
			// It contains at least one letter which repeats with exactly one letter between them, like xyx, abcdefeghi(efe), or even aaa.

			return Regex.IsMatch(s, @"(\w\w).*\1") && Regex.IsMatch(s, @"(\w).\1");
		}
	}
}
