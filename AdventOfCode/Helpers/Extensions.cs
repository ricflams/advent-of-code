using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
					if (group.Any())
					{
						yield return group.ToArray();
						group.Clear();
					}
					continue;
				}
				group.Add(line);
			}
			if (group.Any())
			{
				yield return group.ToArray();
			}
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

		public static Tv GetOrAdd<Tk,Tv>(this IDictionary<Tk,Tv> dict, Tk key, Func<Tv> producer)
		{
			if (!dict.TryGetValue(key, out var value))
			{
				value = dict[key] = producer();
			}
			return value;
		}
	}
}
