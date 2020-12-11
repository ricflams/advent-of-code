using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day09
{
	internal class Puzzle09
	{
		public static void Run()
		{
			var input = File.ReadAllLines("Y2020/Day09/input.txt");
			var data = input.Select(long.Parse).ToArray();

			long Puzzle1()
			{
				// Use a sliding window of hashed preambles for really fast sum-lookup
				var len = 25;
				var preamble = new HashSet<long>(data.Take(len));
				for (var i = len; i < data.Length; i++)
				{
					var d = data[i];
					if (!preamble.Any(f => preamble.Contains(d - f) && d != f * f))
					{
						return d;
					}
					preamble.Remove(data[i - len]);
					preamble.Add(data[i]);
				}
				throw new Exception("No sum found");
			}
			var result1 = Puzzle1();
			Console.WriteLine($"Day 09 Puzzle 1: {result1}");
			Debug.Assert(result1 == 217430975);

			long Puzzle2(long seek)
			{
				for (var i = 0; i < data.Length; i++)
				{
					var sum = 0L;
					for (var j = i; j < data.Length && sum < seek; j++)
					{
						sum += data[j];
						if (sum == seek && j > i) // Use j>i because set must contain more than 1 number
						{
							var set = data[i..j];
							return set.Min() + set.Max();
						}
					}
				}
				throw new Exception($"No set found for {seek}");
			}
			var result2 = Puzzle2(result1);
			Console.WriteLine($"Day 09 Puzzle 2: {result2}");
			Debug.Assert(result2 == 28509180);
		}
	}
}
