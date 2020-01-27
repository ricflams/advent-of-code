using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2015.Day05
{
	internal class Puzzle05
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2015/Day05/input.txt");

			var nice = input.Count(IsNicePuzzle1);

			Console.WriteLine($"Day  5 Puzzle 1: {nice}");
			Debug.Assert(nice == 236);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2015/Day05/input.txt");

			var nice = input.Count(IsNicePuzzle2);

			Console.WriteLine($"Day  5 Puzzle 2: {nice}");
			Debug.Assert(nice == 51);
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
