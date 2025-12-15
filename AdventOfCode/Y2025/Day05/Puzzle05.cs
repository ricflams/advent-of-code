using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2025.Day05
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Cafeteria";
		public override int Year => 2025;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(3).Part2(14);
			Run("input").Part1(733).Part2(345821388687084);
		}

		protected override long Part1(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();
			var ranges = parts[0]
				.Select(x => x.Split('-').Select(long.Parse).ToArray())
				.Select(x => new Interval<long>(x[0], x[1]+1));
			var ids = parts[1].Select(long.Parse);

            // Reducing the ranges, like in part 2, speed up finding the
			// ingredients by a massive factor 10x - so let's do that
            ranges = ranges.Reduce();

            var fresh = ids.Count(id => ranges.Any(r => r.Contains(id)));

            return fresh;
        }

        protected override long Part2(string[] input)
		{
            var parts = input.GroupByEmptyLine().ToArray();
            var ranges = parts[0]
                .Select(x => x.Split('-').Select(long.Parse).ToArray())
                .Select(x => new Interval<long>(x[0], x[1] + 1));

			// Find total set of ranges
			var total = ranges.Reduce().TotalLength();

            return total;
        }
    }
}
