using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Helpers
{
    internal static class Extensions
    {
		public static IEnumerable<T> TakeAll<T>(this BlockingCollection<T> collection)
		{
			while (collection.TryTake(out var value))
			{
				yield return value;
			}
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

		public static string MultiLine(this string str)
		{
			return string.Concat(str.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim() + "\n"));
		}

		public static string TrimAll(this string str)
		{
			return str.Replace(" ", "");
		}

		public static string ToCommaString(this IEnumerable<int> list) => string.Join(',', list);

		public static IEnumerable<string[]> GroupByEmptyLine(this IEnumerable<string> input)
		{
			var group = new List<string>();
			foreach (var line in input)
			{
				if (line.Length == 0)
				{
					yield return group.ToArray();
					group.Clear();
					continue;
				}
				group.Add(line);
			}
			yield return group.ToArray();
		}

		public static ulong Sum(this IEnumerable<ulong> values)
		{
			var sum = 0UL;
			foreach (var v in values)
			{
				sum += v;
			}
			return sum;
		}
	}
}
