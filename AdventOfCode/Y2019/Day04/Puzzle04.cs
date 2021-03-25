using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Secure Container";
		public override int Year => 2019;
		public override int Day => 4;

		public void Run()
		{
			Run("input").Part1(460).Part2(290);
		}

		protected override int Part1(string[] input)
		{
			var (begin, end) = input[0].RxMatch("%d-%d").Get<int, int>();
			var matches = CalcMatches(begin, end).Count(v => SequenceLengths(v).Any(seq => seq >= 2));
			return matches;
		}

		protected override int Part2(string[] input)
		{
			var (begin, end) = input[0].RxMatch("%d-%d").Get<int, int>();
			var matches = CalcMatches(begin, end).Count(v => SequenceLengths(v).Any(seq => seq == 2));
			return matches;
		}

		private static IEnumerable<int> SequenceLengths(IReadOnlyList<int> value)
		{
			var digit = value[0]; // Assume at least 1-digit values
			var seqlen = 1;
			for (var pos = 1; pos < value.Count; pos++)
			{
				if (value[pos] == digit)
				{
					seqlen++;
				}
				else
				{
					yield return seqlen;
					digit = value[pos];
					seqlen = 1;
				}
			}
			yield return seqlen;
		}

		private static IEnumerable<int[]> CalcMatches(int begin, int end)
		{
			// Pick out digits from begin-value; add leading 0 to avoid special case for overflow
			var digits = $"0{begin.ToString()}".ToCharArray().Select(x => int.Parse($"{x}")).ToArray();
			while (true)
			{
				// Increment number until it consist only of increasing digits
				for (var pos = 1; pos < digits.Length; pos++)
				{
					if (digits[pos] < digits[pos - 1])
					{
						digits[pos] = digits[pos - 1];
					}
				}

				// Stop if we've moved beyond the end
				var digitValue = digits.Aggregate(0, (sum, digit) => sum * 10 + digit);
				if (digitValue > end)
				{
					break;
				}

				// This is a candidate
				yield return digits;

				// Increment the number one digit at a time, starting from the least significant digit
				// Example: 456789 -> 456790
				// Example: 678999 -> 679000
				for (var pos = digits.Length - 1; pos >= 0 && ++digits[pos] > 9; pos--)
				{
					digits[pos] = 0;
				}
			}
		}
	}
}