using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day20
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Grove Positioning System";
		public override int Year => 2022;
		public override int Day => 20;

		public void Run()
		{
			Run("test1").Part1(3).Part2(1623178306);
			Run("input").Part1(9687).Part2(1338310513297);
		}

		protected override long Part1(string[] input)
		{
			return SumAfterRounds(input, 1, 1);
		}

		protected override long Part2(string[] input)
		{
			return SumAfterRounds(input, 811589153, 10);
		}

		private static long SumAfterRounds(string[] input, long factor, int rounds)
		{
			var N = input.Length;

			var values = input.Select(int.Parse).Select(x => x * factor).ToArray();
			var prev = new int[N];
			var next = new int[N];

			for (var i = 0; i < N; i++)
			{
				prev[i] = i-1;
				next[i] = i+1;
			}
			next[^1] = 0;
			prev[0] = N-1;

			for (var j = 0; j < N*rounds; j++)
			{
				var k = j % N;

				// Nothing to move for value 0
				var value = values[k];
				if (value == 0)
					continue;

				// Take k'th number out of the list
				next[prev[k]] = next[k];
				prev[next[k]] = prev[k];

				// Go backward/forward through list, now wihout the removed number
				value %= N-1;
				var pos = k;
				if (value < 0)
				{
					for (var i = 0; i < -value+1; i++)
						pos = prev[pos];
				}
				else
				{
					for (var i = 0; i < value; i++)
						pos = next[pos];
				}

				// Insert the k'th number after pos
				prev[k] = pos;
				next[k] = next[pos];
				prev[next[pos]] = k;
				next[pos] = k;
			}

			var sum = 0L;
			var idx = values.IndexOf(x => x == 0);
			for (var i = 0; i < 3; i++)
			{
				for (var j = 0; j < 1000; j++)
					idx = next[idx];
				sum += values[idx];
			}

			return sum;			
		}
	}
}
