using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	internal static class CollectionsExtensions
	{
		public static IEnumerable<T[]> Windowed<T>(this IEnumerable<T> collection, int windowSize)
		{
			var v = collection.ToArray();
			var n = v.Length - windowSize + 1;
			for (var i = 0; i < n; i++)
			{
				yield return v[i..(i + windowSize)];
			}
		}
	}
}
