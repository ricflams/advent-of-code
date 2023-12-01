using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day01
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Trebuchet?!";
		public override int Year => 2023;
		public override int Day => 1;

		public void Run()
		{
			Run("test1").Part1(142);
			Run("test2").Part2(281);
			Run("input").Part1(54927).Part2(54581);
		}

		protected override long Part1(string[] input)
		{
			var sum = input.Sum(s =>
			{
				var d1 = s.First(char.IsDigit) - '0';
				var d2 = s.Last(char.IsDigit) - '0';
				return d1 * 10 + d2;
			});
			return sum;
		}

		protected override long Part2(string[] input)
		{
			var digitnames = new[]
			{
				"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"
			};

			var sum = input.Sum(s =>
			{
				var d1 = 0;
				var d2 = 0;
				for (var i = 0; i < s.Length; i++)
				{
					if (TryReadDigit(s.AsSpan(i), ref d1))
						break;
				}
				for (var i = s.Length-1; i >= 0; i--)
				{
					if (TryReadDigit(s.AsSpan(i), ref d2))
						break;
				}
				return d1 * 10 + d2;
			});

			return sum;

			bool TryReadDigit(ReadOnlySpan<char> s, ref int digit)
			{
				if (char.IsDigit(s[0]))
				{
					digit = s[0] - '0';
					return true;
				}

				for (var i = 0; i < digitnames.Length; i++)
				{
					if (s.StartsWith(digitnames[i]))
					{
						digit = i;
						return true;
					}
				}

				return false;
			}
		}

	}
}
