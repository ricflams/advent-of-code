using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
    public static class MathHelper
    {
		public static long GreatestCommonFactor(long a, long b)
		{
			while (b != 0)
			{
				var temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}

		public static long LeastCommonMultiple(params long[] values)
		{
			return values.Length == 1
				? values[0]
				: Lcm(values[0], LeastCommonMultiple(values.Skip(1).ToArray()));

			long Lcm(long a, long b) => (a / GreatestCommonFactor(a, b)) * b;
		}

		public static long Prod(this long[] values)
		{
			var prod = 1L;
			foreach (var v in values)
			{
				prod *= v;
			}
			return prod;
		}

		public static long Prod(this int[] values)
		{
			var prod = 1L;
			foreach (var v in values)
			{
				prod *= v;
			}
			return prod;
		}

		public static long Prod(this IEnumerable<int> values)
		{
			var prod = 1L;
			foreach (var v in values)
			{
				prod *= v;
			}
			return prod;
		}

		public static ulong Prod(this uint[] values)
		{
			var prod = 1UL;
			foreach (var v in values)
			{
				prod *= v;
			}
			return prod;
		}

		public static ulong Prod(this ulong[] values)
		{
			var prod = 1UL;
			foreach (var v in values)
			{
				prod *= v;
			}
			return prod;
		}

		public static IEnumerable<long> Factorize(long n)
		{
			var max = (long)Math.Ceiling(Math.Sqrt(n));
			for (var d = 1; d < max; d++)
			{
				if (n % d == 0)
				{
					yield return d;
					if (d != max)
					{
						yield return n / d;
					}
				}
			}
		}

		public static IEnumerable<IEnumerable<int>> Permute(IEnumerable<int> x)
		{
			if (x.Count() == 1)
			{
				yield return x;
			}
			else
			{
				foreach (var head in x)
				{
					foreach (var perm in Permute(x.Where(y => y != head)))
					{
						yield return new[] { head }.Concat(perm);
					}
				}
			}
		}

		public static IEnumerable<IEnumerable<int>> AllPermutations(int length)
		{
			return Permute(Enumerable.Range(0, length));
		}

		public static IEnumerable<int[]> CountInBaseX(int numberBase, int numberOfDigits)
		{
			var n = numberOfDigits;
			var digits = Enumerable.Repeat(0, n).ToArray();
			var iterations = (int)Math.Pow(numberBase, n);
			while (iterations-- > 0)
			{
				yield return digits;
				for (var pos = n - 1; pos >= 0 && ++digits[pos] >= numberBase; pos--)
				{
					digits[pos] = 0;
				}
			}
		}

		public static IEnumerable<T[]> AllCombinations<T>(T[][] sequence)
		{
			var n = sequence.Length;
			var lengths = sequence.Select(x => x.Length).ToArray();
			var indexes = new int[n];
			var iterations = lengths.Prod();
			while (iterations-- > 0)
			{
				yield return sequence.Select((x, i) => x[indexes[i]]).ToArray();
				for (var pos = n - 1; pos >= 0 && ++indexes[pos] >= lengths[pos]; pos--)
				{
					indexes[pos] = 0;
				}
			}
		}

		public static IEnumerable<int[]> PlusZeroMinusSequence(int numberOfDigits)
		{
			var n = numberOfDigits;
			var digits = Enumerable.Repeat(-1, numberOfDigits).ToArray();
			var iterations = (int)Math.Pow(3, n);
			while (iterations-- > 0)
			{
				yield return digits;
				for (var pos = n - 1; pos >= 0 && ++digits[pos] > 1; pos--)
				{
					digits[pos] = -1;
				}
			}
		}

		public static int[] DivideEvenly(int value, int parts)
		{
			var values = new int[parts];
			Array.Fill(values, value / parts);
			for (var i = 0; i < value % parts; i++)
			{
				values[i]++;
			}
			return values;
		}

		public static int NumberOfSetBits(this uint value)
		{
			var i = value;
			i -= (i >> 1) & 0x55555555;
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return (int)((((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
		}

		public static IEnumerable<uint> Bits(this uint value)
		{
			for (uint bit = 1; value > 0; bit <<= 1, value >>= 1)
			{
				if ((value & 1) == 1)
				{
					yield return bit;
				}
			}
		}

		public static ulong ReverseBits(this ulong value, int len)
		{
			uint reversed = 0;
			for (uint bit = 1U << (len-1); value > 0 && bit > 0; bit >>= 1, value >>= 1)
			{
				if ((value & 1) == 1)
				{
					reversed |= bit;
				}
			}
			return reversed;
		}

		public static bool HasAnyHexSequence(this byte[] ba, int length, out byte val)
		{
			var digits = new byte[ba.Length*2];
			var di = 0;
			for (var i = 0; i < ba.Length; i++)
			{
				digits[di++] = (byte)((ba[i] & 0xf0) >> 4);
				digits[di++] = (byte)(ba[i] & 0x0f);
			}

			for (var i = 0; i < digits.Length - length + 1; i++)
			{
				var match = true;
				for (var j = 1; match && j < length; j++)
				{
					match = digits[i] == digits[i+j];
				}
				if (match)
				{
					val = digits[i];
					return true;
				}
			}
			val = 0;
			return false;
		}

		public static bool HasHexSequence(this byte[] ba, int length, byte val)
		{
			var digits = new byte[ba.Length*2];
			var di = 0;
			for (var i = 0; i < ba.Length; i++)
			{
				digits[di++] = (byte)((ba[i] & 0xf0) >> 4);
				digits[di++] = (byte)(ba[i] & 0x0f);
			}

			for (var i = 0; i < digits.Length - length + 1; i++)
			{
				if (digits[i] == val)
				{
					var match = true;
					for (var j = 1; match && j < length; j++)
					{
						match = digits[i+j] == val;
					}
					if (match)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static string[] HexDigitTable = null;
		public static string FormatAsHex(this byte[] ba)
		{
			if (HexDigitTable == null)
			{
				HexDigitTable = Enumerable.Range(0, 256)
					.Select(x => x.ToString("x2"))
					.ToArray();
			}

			return string.Create(ba.Length * 2, ba, (chars, ba) =>
			{
				for (var i = 0; i < ba.Length; i++)
				{
					HexDigitTable[ba[i]].AsSpan().CopyTo(chars.Slice(i * 2));
				}
			});
		}

		public static bool AreDistinct(params int[] values)
		{
			for (var i = 0; i < values.Length - 1; i++)
			{
				if (values.Skip(i + 1).Contains(values[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
