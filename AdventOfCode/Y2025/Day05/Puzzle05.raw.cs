using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2025.Day05.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2025;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(3).Part2(14);
			Run("input").Part1(733).Part2(345821388687084);
		}

		protected override long Part1(string[] input)
		{
			var things = input
				.GroupByEmptyLine().ToArray();
			var ranges = things[0]
				.Select(x => x.Split('-').Select(long.Parse).ToArray())
				.Select(x => new Interval<long>(x[0], x[1]+1));
			var ids = things[1].Select(long.Parse);

			var fresh = ids.Count(id => ranges.Any(r => r.Contains(id)));

            return fresh;
        }

        protected override long Part2(string[] input)
		{
            var things = input
                .GroupByEmptyLine().ToArray();
            var ranges = things[0]
                .Select(x => x.Split('-').Select(long.Parse).ToArray())
                .Select(x => new Interval<long>(x[0], x[1] + 1));

			var ranges2 = ranges.Reduce();
			var tot = ranges2.TotalLength();

            return tot;
        }
    }
}
