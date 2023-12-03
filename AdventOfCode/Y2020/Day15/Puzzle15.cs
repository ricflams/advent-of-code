using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Linq;

namespace AdventOfCode.Y2020.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Rambunctious Recitation";
		public override int Year => 2020;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(436).Part2(175594);
			Run("test2").Part1(1).Part2(2578);
			Run("test3").Part1(10).Part2(3544142);
			Run("test4").Part1(27).Part2(261214);
			Run("test5").Part1(78).Part2(6895259);
			Run("test6").Part1(438).Part2(18);
			Run("test7").Part1(1836).Part2(362);
			Run("input").Part1(412).Part2(243);
		}

		protected override int Part1(string[] input)
		{
			var seq = input[0].ToIntArray();
			return LastSpokenNumber(seq, 2020);
		}

		protected override int Part2(string[] input)
		{
			var seq = input[0].ToIntArray();
			return LastSpokenNumber(seq, 30000000);
		}

		private static int LastSpokenNumber(int[] seed, int count)
		{
			var seen = new int[count];

			for (var i = 0; i < seed.Length - 1; i++)
			{
				seen[seed[i]] = i + 1;
			}

			var lastSeen = seed.Last();
			for (var i = seed.Length; i < count; i++)
			{
				// Check if number was already seen before adding it
				var seenAt = seen[lastSeen];
				seen[lastSeen] = i;
				if (seenAt > 0)
				{
					lastSeen = i - seenAt;
				}
				else
				{
					lastSeen = 0;
				}
			}

			return lastSeen;
		}
	}
}
