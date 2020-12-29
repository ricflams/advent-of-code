using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2020.Day15
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Rambunctious Recitation";
		public override int Year => 2020;
		public override int Day => 15;

		public void Run()
		{
			RunFor("test1", 436, 175594);
			RunFor("test2", 1, 2578);
			RunFor("test3", 10, 3544142);
			RunFor("test4", 27, 261214);
			RunFor("test5", 78, 6895259);
			RunFor("test6", 438, 18);
			RunFor("test7", 1836, 362);
			RunFor("input", 412, 243);
		}

		protected override int Part1(string[] input)
		{
			var seq = input[0].Split(",").Select(int.Parse).ToArray();
			return LastSpokenNumber(seq, 2020);
		}

		protected override int Part2(string[] input)
		{
			var seq = input[0].Split(",").Select(int.Parse).ToArray();
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
