using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

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

		public static uint Sum(this uint[] values)
		{
			var sum = 0u;
			foreach (var v in values)
			{
				sum += v;
			}
			return sum;
		}

		public static ulong Sum(this ulong[] values)
		{
			var sum = 0ul;
			foreach (var v in values)
			{
				sum += v;
			}
			return sum;
		}

		public static BigInteger ModInverse(this BigInteger a, BigInteger m)
		{
			if (m == 1) return 0;
			var m0 = m;
			(var x, var y) = (BigInteger.One, BigInteger.Zero);

			while (a > 1)
			{
				var q = a / m;
				(a, m) = (m, a % m);
				(x, y) = (y, x - q * y);
			}
			return x < 0 ? x + m0 : x;
		}

		public static int ModInverse(this int a, int m)
		{
			if (m == 1) return 0;
			var m0 = m;
			(var x, var y) = (1, 0);

			while (a > 1)
			{
				var q = a / m;
				(a, m) = (m, a % m);
				(x, y) = (y, x - q * y);
			}
			return x < 0 ? x + m0 : x;
		}

		public static int SolveChineseRemainderTheorem(int[] n, int[] a)
		{
            var prod = (int)n.Prod();
            var sum = 0;
            for (int i = 0; i < n.Length; i++)
            {
                var p = prod / n[i];
                sum += a[i] * p.ModInverse(n[i]) * p;
				//sum += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
            }
            return sum % prod;
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

		public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> x)
		{
			if (x.Count() == 1)
			{
				yield return x;
			}
			else
			{
				foreach (var head in x)
				{
					foreach (var perm in Permute(x.Where(y => !y.Equals(head))))
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


		public static IEnumerable<T[]> Combinations<T>(IEnumerable<T> set, int size)
		{
			Debug.Assert(size > 0);

			var elements = set.ToArray();
			var data = new T[size];
			foreach (var combination in Combination(0, elements.Length, 0))
            {
				yield return combination;
			}

			IEnumerable<T[]> Combination(int start, int end, int index)
            {
				if (index == size)
                {
					yield return data;
					yield break;
				}
				for (int i = start; i < end && size - index < end - i + 1; i++)
				{
					data[index] = elements[i];
					foreach (var x in Combination(i + 1, end, index + 1))
                    {
						yield return x;
					}
				}
            }
		}

		private static Random Random = new Random();
		public static T PickRandom<T>(this T[] set)
		{
			var N = set.Length;
			var n = Random.Next(N);
			return set[n];
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
