using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
    internal static class MathHelper
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
