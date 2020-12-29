using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2015.Day11
{
	internal class Puzzle : ComboParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Corporate Policy";
		public override int Year => 2015;
		public override int Day => 11;

		public void Run()
		{
			RunFor("input", "cqjxxyzz", "cqkaabcc");
		}

		protected override (string, string) Part1And2(string[] input)
		{
			var password0 = input[0];
			var password1 = NextPassword(password0);
			var password2 = NextPassword(password1);
			return (password1, password2);
		}

		private static string NextPassword(string password)
		{
			// Passwords must include one increasing straight of at least three letters, like abc, bcd, cde, and so on, up to xyz. They cannot skip letters; abd doesn't count.
			// Passwords may not contain the letters i, o, or l, as these letters can be mistaken for other characters and are therefore confusing.
			// Passwords must contain at least two different, non-overlapping pairs of letters, like aa, bb, or zz.
			var letters = password.ToCharArray();
			var end = new string('z', letters.Length).ToCharArray();
			while (true)
			{
				// Increment the password one letters at a time, starting from the least significant letter
				// Example: abcdef -> abcdeg
				// Example: abczzz -> abdaaa
				for (var pos = letters.Length - 1; pos >= 0 && ++letters[pos] > 'z'; pos--)
				{
					letters[pos] = 'a';
				}

				// Do the easy and very frequently occurring check first
				if (HasValidLetters(letters) && HasThreeIncreasingLetters(letters) && HasTwoOverlappingPairs(letters))
				{
					return new string(letters);
				}

				if (letters.SequenceEqual(end))
				{
					return null;
				}
			}

			static bool HasValidLetters(char[] pwd)
			{
				// May not contain the letters i, o, or l
				foreach (var c in pwd)
				{
					switch (c)
					{
						case 'i':
						case 'o':
						case 'l':
							return false;
					}
				}
				return true;
			}

			static bool HasThreeIncreasingLetters(char[] pwd)
			{
				// Increasing straight of at least three letters
				var seqlen = 1;
				for (var i = 0; i < pwd.Length - 1; i++)
				{
					seqlen = pwd[i] + 1 == pwd[i + 1]
						? seqlen + 1
						: 1;
					if (seqlen == 3)
					{
						return true;
					}
				}
				return false;
			}

			static bool HasTwoOverlappingPairs(char[] pwd)
			{
				// Must contain at least two different, non-overlapping pairs of letters
				char lastpair = (char)0;
				for (var i = 0; i < pwd.Length - 1; i++)
				{
					if (pwd[i] == pwd[i + 1])
					{
						if (lastpair != 0 && lastpair != pwd[i])
						{
							return true;
						}
						lastpair = pwd[i];
						i += 1;
					}
				}
				return false;
			}
		}
	}
}
