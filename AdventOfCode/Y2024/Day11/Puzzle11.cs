using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day11
{
	internal class Puzzle : PuzzleWithParameter<int, long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Plutonian Pebbles";
		public override int Year => 2024;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").WithParameter(1).Part1(7);
			Run("test2").WithParameter(1).Part1(3);
			Run("test2").WithParameter(2).Part1(4);
			Run("test2").WithParameter(3).Part1(5);
			Run("test2").WithParameter(4).Part1(9);
			Run("test2").WithParameter(5).Part1(13);
			Run("test2").WithParameter(6).Part1(22);
			Run("test2").WithParameter(25).Part1(55312);
			Run("input").WithParameter(25).Part1(183484);
			Run("input").WithParameter(75).Part2(218817038947400);
			Run("extra").WithParameter(25).Part1(193269);
			Run("extra").WithParameter(75).Part2(228449040027793);			
		}

		protected override long Part1(string[] input)
		{
			return Blinks(input, PuzzleParameter);
		}

		protected override long Part2(string[] input)
		{
			return Blinks(input, PuzzleParameter);
		}

		private static long Blinks(string[] input, int blinks)
		{
			var stones = input[0].SplitSpace().Select(long.Parse).ToArray();

			var memo = new Dictionary<long,long>();
			var maxblinks = blinks + 1;

			// If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
			// If the stone is engraved with a number that has an even number of digits, it is replaced by two stones. The left half of the digits are engraved on the new left stone, and the right half of the digits are engraved on the new right stone. (The new numbers don't keep extra leading zeroes: 1000 would become stones 10 and 0.)
			// If none of the other rules apply, the stone is replaced by a new stone; the old stone's number multiplied by 2024 is engraved on the new stone.
			long Blink(long stone, int blinks)
			{
				if (blinks == 0)
					return 1;

				var key = stone*maxblinks + blinks;
				if (memo.TryGetValue(key, out var v))
					return v;

				var n =
					stone == 0 ? Blink(1, blinks - 1) :
					stone.CanSplitInTwo(out var nn) ? Blink(nn.S1, blinks - 1) + Blink(nn.S2, blinks - 1) :
					Blink(stone * 2024, blinks - 1);
				memo[key] = n;
				return n;
			}

			var len = stones.Sum(s => Blink(s, blinks));

			return len;			
		}
	}
}
