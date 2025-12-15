using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2025.Day03
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Lobby";
		public override int Year => 2025;
		public override int Day => 3;

		public override void Run()
		{
            Run("test1").Part1(357).Part2(3121910778619);
            Run("input").Part1(17244).Part2(171435596092638);
        }

		protected override long Part1(string[] input)
		{
			// TODO optimize

			var banks = input.Select(line => line.ToCharArray().Select(c => c - '0').ToArray()).ToArray();

			var output = 0;
			foreach (var b in banks)
			{
                var pos1 = 0;
                var maxd1 = b[pos1];
				for (var i = pos1 + 1; i < b.Length - 1; i++)
				{
					if (b[i] > maxd1)
					{
						maxd1 = b[i];
						pos1 = i;
					}
				}
                var pos2 = pos1 + 1;
                var maxd2 = b[pos2];
                for (var i = pos2 + 1; i < b.Length; i++)
                {
                    if (b[i] > maxd2)
                    {
                        maxd2 = b[i];
                        pos2 = i;
                    }
                }
				var sum = maxd1 * 10 + maxd2;
				output += sum;
            }

            return output;
		}

        private Dictionary<string, long> _memo = new Dictionary<string, long>();

        protected override long Part2(string[] input)
		{
            var banks = input.Select(line => line.ToCharArray().Select(c => c - '0').ToArray()).ToArray();

            var output = 0L;
			var exp = (long)Math.Pow(10, 11);
            foreach (var b in banks)
            {
				_memo.Clear();
                var max = MaxJoltage(b, 0, 12, exp);
				//Console.WriteLine(max);
				output += max;
            }

            return output;
        }

		private long MaxJoltage(int[] bank, int pos, int remains, long exp)
		{
			var key = $"{pos}-{remains}-{exp}";
			if (_memo.TryGetValue(key, out var value))
				return value;

			if (pos + remains > bank.Length)
				return -1;
			if (exp == 0)
                return 0;
            var j1 = bank[pos] * exp + MaxJoltage(bank, pos + 1, remains - 1, exp/10);
			var j2 = MaxJoltage(bank, pos + 1, remains, exp);
			var joltage = Math.Max(j1, j2);

			_memo.Add(key, joltage);
			return joltage;
        }
    }
}
