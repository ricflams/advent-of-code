using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Mirage Maintenance";
		public override int Year => 2023;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(114).Part2(2);
			Run("input").Part1(1731106378).Part2(1087);
			Run("extra").Part1(1708206096).Part2(1050);
		}

		protected override long Part1(string[] input)
		{
			var reports = input.Select(s => s.ToIntArray());

			var sum = reports.Sum(ExtrapolateNextValue);
			return sum;

			static int ExtrapolateNextValue(int[] seq)
			{
				// Make new array of all the diffs
				// If they're all 0 then we're done; else repeat, adding to the last number
				var nextSeq = seq.Skip(1).Select((v, i) => v - seq[i]).ToArray();
				if (nextSeq.All(x => x == 0))
					return seq.Last();
				return seq.Last() + ExtrapolateNextValue(nextSeq);
			}
		}

		protected override long Part2(string[] input)
		{
			var reports = input.Select(s => s.ToIntArray());

			var sum = reports.Sum(ExtrapolatePrevValue);
			return sum;

			static int ExtrapolatePrevValue(int[] seq)
			{
				// Make new array of all the diffs
				// If they're all 0 then we're done; else repeat, subtracting from the first number
				var nextSeq = seq.Skip(1).Select((v, i) => v - seq[i]).ToArray();
				if (nextSeq.All(x => x == 0))
					return seq.Last();
				return seq.First() - ExtrapolatePrevValue(nextSeq);
			}
		}
	}
}
