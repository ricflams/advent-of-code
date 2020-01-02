using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Helpers
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
	}
}
