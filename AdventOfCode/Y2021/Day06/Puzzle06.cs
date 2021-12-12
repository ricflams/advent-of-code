using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day06
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Lanternfish";
		public override int Year => 2021;
		public override int Day => 6;

		public void Run()
		{
			Run("test1").Part1(5934).Part2(26984457539);
			Run("input").Part1(360268).Part2(1632146183902);
		}

		protected override long Part1(string[] input)
		{
			var v = input.First().ToIntArray();
			var result = FishesAfter(v, 80);
			return result;
		}

		protected override long Part2(string[] input)
		{
			var v = input.First().ToIntArray();
			var result = FishesAfter(v, 256);
			return result;

		}

		private static long FishesAfter(int[] timers, int loops)
		{
			// Deduce the fish-totals by just storing the number of fish of
			// each timer-value, including the initial set of fish!
			var v = new long[9];
			foreach (var t in timers)
			{
				v[t]++;
			}

			// Now simulate the growth
			for (var day = 0; day < loops; day++)
			{
				var v0 = v[0];
				v[0] = v[1];
				v[1] = v[2];
				v[2] = v[3];
				v[3] = v[4];
				v[4] = v[5];
				v[5] = v[6];
				v[6] = v[7] + v0;
				v[7] = v[8];
				v[8] = v0;
			}

			return v.Sum();
		}
	}
}
