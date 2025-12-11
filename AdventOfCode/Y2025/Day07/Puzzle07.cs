using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2025.Day07
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Laboratories";
		public override int Year => 2025;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(21).Part2(40);
			Run("input").Part1(1562).Part2(24292631346665);
			Run("extra").Part1(1690).Part2(221371496188107);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var maxY = input.Length;
			var tachyons = new HashSet<Point>();
			var p0 = map.AllPointsWhere(c => c == 'S').Single();

			return CountSplits(p0);

			int CountSplits(Point p)
			{
				while (p.Y < maxY)
				{
					if (tachyons.Contains(p))
						return 0;
					tachyons.Add(p);
					p = p.Down;
					if (map[p] == '^')
					{
						return 1 + CountSplits(p.Left) + CountSplits(p.Right);
					}
				}
				return 0;
			}
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var maxY = input.Length;
			var timelines = new Dictionary<Point, long>();
			var p0 = map.AllPointsWhere(c => c == 'S').Single();

			return CountTimelines(p0);

			long CountTimelines(Point p)
			{
				while (p.Y < maxY)
				{
					p = p.Down;
					if (timelines.TryGetValue(p, out var n))
						return n;
					if (map[p] == '^')
					{
						timelines[p] = CountTimelines(p.Left) + CountTimelines(p.Right);
						return timelines[p];
					}
				}
				return 1;
			}
		}
	}
}
