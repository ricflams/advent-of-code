using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.Strings;
using System;
using System.Linq;

namespace AdventOfCode.Y2017.Day02
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Corruption Checksum";
		public override int Year => 2017;
		public override int Day => 2;

		public void Run()
		{
			RunPart1For("test1", 18);
			RunPart2For("test2", 9);
			RunFor("input", 34925, 221);
		}

		protected override int Part1(string[] input)
		{
			var sheet = ReadSheet(input);
			var checksum = sheet.Sum(s => s.Max() - s.Min());
			return checksum;
		}

		protected override int Part2(string[] input)
		{
			var sheet = ReadSheet(input);
			var checksum = sheet.Sum(s =>
			{
				foreach (var i in s)
				{
					foreach (var j in s)
					{
						// Find pairs where i/j is a whole number, and they're not the same number
						if (i % j == 0 && i != j)
							return i / j;
					}
				}
				throw new Exception($"No divider-pairs found in {s}");
			});
			return checksum;
		}

		private static int[][] ReadSheet(string[] input)
		{
			var sheet = input
				.Select(line => line.ToIntArray())
				.ToArray();
			return sheet;
		}
	}
}
