using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day07.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2025;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(21).Part2(40);
			//Run("test2").Part1(0).Part2(0);

			// 2088 2089 not right

			Run("input").Part1(1562).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var height = input.Length;
			var tachyons = new HashSet<Point>();

			var p0 = map.AllPointsWhere(c => c == 'S').Single();

			var splits = CountSplits(p0);

			return splits;

			int CountSplits(Point p)
			{
				while (p.Y < height)
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
			var height = input.Length;
			var memo = new Dictionary<Point, long>();

			var p0 = map.AllPointsWhere(c => c == 'S').Single();

			var count = CountTimelines(p0);

			return count;

			long CountTimelines(Point p)
			{
				var pp = p.Down.Up;
				if (memo.TryGetValue(p, out var n))
					return n;
				while (p.Y < height)
				{
					if (memo.TryGetValue(p, out n))
						return n;
					p = p.Down;
					if (memo.TryGetValue(p, out n))
						return n;
					if (map[p] == '^')
					{
						n = CountTimelines(p.Left) + CountTimelines(p.Right);
						memo[p] = n;
						return n;
					}
				}
				memo[pp] = 1;
				return 1;
			}
		}
	}
}
