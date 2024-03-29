using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Linq;

namespace AdventOfCode.Y2017.Day06
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Memory Reallocation";
		public override int Year => 2017;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(5).Part2(4);
			Run("input").Part1(3156).Part2(1610);
			Run("extra").Part1(12841).Part2(8038);
		}

		protected override int Part1(string[] input)
		{
			var banks = input[0].ToIntArray();
			return FindFirstCycle(banks);
		}

		protected override int Part2(string[] input)
		{
			var banks = input[0].ToIntArray();
			
			// Find the id to look for, the first bank of the cycle
			FindFirstCycle(banks);
			var id = Hashing.KnuthHash(banks);

			// Keep redistributing until a loop is detected
			var cycle = 0;
			do
			{
				Redistribute(banks);
				cycle++;
			} while (Hashing.KnuthHash(banks) != id);

			return cycle;
		}

		private static int FindFirstCycle(int[] banks)
		{
			var memo = new SimpleMemo<ulong>();
			var cycle = 0;
			do
			{
				Redistribute(banks);
				cycle++;
			} while (!memo.IsSeenBefore(Hashing.KnuthHash(banks)));
			return cycle;

		}

		private static void Redistribute(int[] banks)
		{
			// First, find position for the (first) maximum value
			var pos = 0;
			for (var i = 1; i < banks.Length; i++)
			{
				if (banks[i] > banks[pos])
				{
					pos = i;
				}
			}

			// Remove the blocks to redistribute
			var blocks = banks[pos];
			banks[pos] = 0;

			// Redistribute all
			while (blocks-- > 0)
			{
				pos = (pos + 1) % banks.Length;
				banks[pos]++;
			}
		}
	}
}
