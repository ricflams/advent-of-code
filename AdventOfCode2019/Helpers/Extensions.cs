using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AdventOfCode2019.Helpers
{
    static class Extensions
    {
		public static IEnumerable<T> TakeAll<T>(this BlockingCollection<T> collection)
		{
			while (collection.TryTake(out var value))
			{
				yield return value;
			}
		}
    }
}
