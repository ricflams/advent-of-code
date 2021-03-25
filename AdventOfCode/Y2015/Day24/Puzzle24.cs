using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Y2015.Day24
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "It Hangs in the Balance";
		public override int Year => 2015;
		public override int Day => 24;

		public void Run()
		{
			Run("test1").Part1(99).Part2(44);
			Run("input").Part1(11846773891).Part2(80393059);
		}

		protected override long Part1(string[] input)
		{
			return CalcQuantumEntanglement(input, 3);
		}

		protected override long Part2(string[] input)
		{
			return CalcQuantumEntanglement(input, 4);
		}

		private long CalcQuantumEntanglement(string[] input, int groups)
		{
			// Important to sort descending, so we fill in the largest numbers first.
			// The combinations will always end up in ordered groups because we always
			// either skip numbers or append a number to a result, which will always be
			// smaller than the smallest seen so far in that result.
			var weights = input
				.Select(int.Parse)
				.OrderByDescending(x => x)
				.ToArray();

			var totalsum = weights.Sum();
			var goal = totalsum / groups;
			Debug.Assert(totalsum % groups == 0);

			var minPackageCount = int.MaxValue;
			var minQuantumEntanglement = long.MaxValue;
			foreach (var packages in PackageFinder(goal, new int[0], weights))
			{
				// We will never return a set of packages longer than the minimum seen
				// so far so there's no need to go "if (n<min) min=n"; we know this is
				// as short or shorter than the shortest list of packages seen so far.
				minPackageCount = packages.Count();
				var qe = packages.Prod();
				if (qe < minQuantumEntanglement)
				{
					minQuantumEntanglement = qe;
				}
			}

			return minQuantumEntanglement;

			IEnumerable<int[]> PackageFinder(int goal, int[] picked, int[] remains)
			{
				// Don't keep looking if we've already at the lowest number of packages
				// seen so far; this call will always yield a group that is one bigger than
				// what's currently picked so it can't ever be a candidate for "least amount
				// of packages seen".
				if (picked.Length >= minPackageCount)
				{
					yield break;
				}
				for (var i = 0; i < remains.Length; i++)
				{
					var w = remains[i];
					if (w > goal)
						continue;
					if (w == goal)
					{
						yield return picked.Append(w).ToArray();
						continue;
					}
					var remainsNext = remains[(i+1)..];
					var pickedNext = picked.Append(w).ToArray();
					foreach (var g in PackageFinder(goal - w, pickedNext, remainsNext))
					{
						yield return g;
					}
				}
			}
		}
	}
}
