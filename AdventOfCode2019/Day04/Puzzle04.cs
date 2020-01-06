using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace AdventOfCode2019.Day04
{
	internal class Puzzle04
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var matches1 = CalcMatches(382345, 843167).Count(v => SequenceLengths(v).Any(seq => seq >= 2));
			Console.WriteLine($"Day  4 Puzzle 1: {matches1}");
			Debug.Assert(matches1 == 460);

			var matches2 = CalcMatches(382345, 843167).Count(v => SequenceLengths(v).Any(seq => seq == 2));
			Console.WriteLine($"Day  4 Puzzle 2: {matches2}");
			Debug.Assert(matches2 == 290);
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