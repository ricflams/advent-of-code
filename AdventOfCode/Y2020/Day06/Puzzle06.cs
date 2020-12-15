using AdventOfCode.Helpers;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day06
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 6;

		public void Run()
		{
			RunFor("test1", 11, 6);
			RunFor("input", 6683, 3122);
		}

		protected override long Part1(string[] input)
		{
			var groups = input.GroupByEmptyLine();

			var result =
				groups.Select(g => string.Concat(g).ToCharArray().Distinct().Count())
				.Sum();

			return result;
		}

		protected override long Part2(string[] input)
		{
			var groups = input.GroupByEmptyLine();

			var result =
				groups.Select(g => string.Concat(g).ToCharArray().GroupBy(x => x).Count(x => x.Count() == g.Length))
				.Sum();

			return result;
		}
	}
}
