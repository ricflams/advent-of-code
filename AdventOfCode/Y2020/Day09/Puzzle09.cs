using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day09
{
	internal class Puzzle : PuzzleWithParameter<int, long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Encoding Error";
		public override int Year => 2020;
		public override int Day => 9;

		public void Run()
		{
			Run("test1").WithParameter( 5).Part1(127).Part2(62);
			Run("input").WithParameter(25).Part1(217430975).Part2(28509180);
		}

		protected override long Part1(string[] input)
		{
			var data = input.Select(long.Parse).ToArray();
			var len = PuzzleParameter;

			// Use a sliding window of hashed preambles for really fast sum-lookup
			var result = 0L;
			var preamble = new HashSet<long>(data.Take(len));
			for (var i = len; i < data.Length; i++)
			{
				var d = data[i];
				if (!preamble.Any(f => preamble.Contains(d - f) && d != f * f))
				{
					result = d;
					break;
				}
				preamble.Remove(data[i - len]);
				preamble.Add(data[i]);
			}

			return result;
		}

		protected override long Part2(string[] input)
		{
			var data = input.Select(long.Parse).ToArray();

			var seek = Part1(input);
			var result = 0L;
			for (var i = 0; i < data.Length; i++)
			{
				var sum = 0L;
				for (var j = i; j < data.Length && sum < seek; j++)
				{
					sum += data[j];
					if (sum == seek && j > i) // Use j>i because set must contain more than 1 number
					{
						var set = data[i..j];
						result = set.Min() + set.Max();
						goto end;
					}
				}
			}
			end:

			return result;
		}
	}
}
