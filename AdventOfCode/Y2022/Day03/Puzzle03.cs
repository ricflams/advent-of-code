using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day03
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Rucksack Reorganization";
		public override int Year => 2022;
		public override int Day => 3;

		public void Run()
		{
			Run("test1").Part1(157).Part2(70);
			Run("input").Part1(7701).Part2(2644);
		}

		protected override long Part1(string[] input)
		{
			var sum = 0;
			foreach (var i in input)
			{
				var len = i.Length / 2;
				var part1 = i[0..len];
				var part2 = i[len..];
				sum += FindPriority(part1, part2);
			}
			return sum;
		}

		protected override long Part2(string[] input)
		{
			var sum = input
				.Chunk(3)
				.Sum(FindPriority);
			return sum;
		}

		private int FindPriority(params string[] rucksacks)
		{
			var ch = rucksacks
				.IntersectMany()
				.Single();
			return ch >= 'a'
				? (ch - 'a' + 1)
				: (ch - 'A' + 27);
		}
	}
}
