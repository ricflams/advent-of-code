using AdventOfCode.Helpers;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day06
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Custom Customs";
		public override int Year => 2020;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(11).Part2(6);
			Run("input").Part1(6683).Part2(3122);
			Run("extra").Part1(6633).Part2(3202);
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
