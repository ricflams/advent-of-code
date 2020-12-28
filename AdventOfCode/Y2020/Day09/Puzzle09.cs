using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day09
{
	internal class Puzzle : ComboParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Encoding Error";
		protected override int Year => 2020;
		protected override int Day => 9;

		private int _preambleLength;

		public void Run()
		{
			_preambleLength = 5;
			RunFor("test1", 127, 62);

			_preambleLength = 25;
			RunFor("input", 217430975, 28509180);
		}

		protected override (long, long) Part1And2(string[] input)
		{
			var data = input.Select(long.Parse).ToArray();
			var len = _preambleLength;

			// Use a sliding window of hashed preambles for really fast sum-lookup
			var result1 = 0L;
			var preamble = new HashSet<long>(data.Take(len));
			for (var i = len; i < data.Length; i++)
			{
				var d = data[i];
				if (!preamble.Any(f => preamble.Contains(d - f) && d != f * f))
				{
					result1 = d;
					break;
				}
				preamble.Remove(data[i - len]);
				preamble.Add(data[i]);
			}

			var seek = result1;
			var result2 = 0L;
			for (var i = 0; i < data.Length; i++)
			{
				var sum = 0L;
				for (var j = i; j < data.Length && sum < seek; j++)
				{
					sum += data[j];
					if (sum == seek && j > i) // Use j>i because set must contain more than 1 number
					{
						var set = data[i..j];
						result2 = set.Min() + set.Max();
						goto end;
					}
				}
			}
			end:

			return (result1, result2);
		}
	}
}
