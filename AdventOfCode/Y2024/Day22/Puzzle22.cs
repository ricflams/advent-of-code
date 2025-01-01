using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Monkey Market";
		public override int Year => 2024;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(37327623);
			Run("test2").Part2(23);
			Run("input").Part1(17577894908).Part2(1931);
			Run("extra").Part1(20411980517).Part2(2362);
		}

		protected override long Part1(string[] input)
		{
			var numbers = input.Select(uint.Parse).ToArray();

			var sum = numbers.Sum(n =>
			{
				for (var i = 0; i < 2000; i++)
				{
					n ^= n << 6;
					n %= 0x1000000;
					n ^= n / (2 << 4);
					n %= 0x1000000;
					n ^= n << 11;
					n %= 0x1000000;
				}
				return n;
			});

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var numbers = input.Select(uint.Parse).ToArray();

			// Shift each digit-range [-9..9] into range [0..18] and shift each diff's
			// 19 values so 4 diffs can be encoded into one single number that can be
			// used as indexes in a single array (there are about 130K sequences)
			static int DiffSequence(int[] v, int i) => (v[i - 3] + 9) * 19 * 19 * 19 + (v[i - 2] + 9) * 19 * 19 + (v[i - 1] + 9) * 19 + v[i] + 9;
			var SeqN = DiffSequence([9, 9, 9, 9], 3);

			var bananas = new int[SeqN];

			foreach (var number in numbers)
			{
				var N = 2001;

				var digits = new int[N];
				var n = number;
				for (var i = 0; i < N; i++)
				{
					digits[i] = (int)(n % 10);
					n ^= n << 6;
					n %= 0x1000000;
					n ^= n / (2 << 4);
					n %= 0x1000000;
					n ^= n << 11;
					n %= 0x1000000;
				}

				var diffs = new int[N]; // 0 is unused
				for (var i = 1; i < diffs.Length; i++)
				{
					diffs[i] = digits[i] - digits[i - 1];
				}

				// Only register the first seen price for any given diff-sequence
				// Add number of bananas for each diff straight into the one single
				// bananas-total-array.
				var seen = new bool[SeqN];
				for (var i = 4; i < diffs.Length; i++)
				{
					var seq = DiffSequence(diffs, i);
					if (seen[seq])
						continue;
					bananas[seq] += digits[i];
					seen[seq] = true;
				}
			}

			return bananas.Max();
		}
	}
}
