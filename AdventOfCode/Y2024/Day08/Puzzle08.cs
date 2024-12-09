using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day08
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Resonant Collinearity";
		public override int Year => 2024;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(14).Part2(34);
			Run("input").Part1(313).Part2(1064);
			Run("extra").Part1(361).Part2(1249);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var antinodes = new HashSet<Point>();

			var antennas = map.AllWhere(ch => ch != '.').ToArray().GroupBy(x => x.Value);
			foreach (var a in antennas)
			{
				foreach (var pair in MathHelper.Combinations(a, 2))
				{
					var (p1, p2) = (pair[0].Point, pair[1].Point);
					antinodes.Add(p2 + (p2 - p1));
					antinodes.Add(p1 - (p2 - p1));
				}
			}

			var unique = antinodes.Count(map.Exists);

			return unique;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var antinodes = new HashSet<Point>();

			var antennas = map.AllWhere(ch => ch != '.').ToArray().GroupBy(x => x.Value);
			foreach (var a in antennas)
			{
				foreach (var pair in MathHelper.Combinations(a, 2))
				{
					var (p1, p2) = (pair[0].Point, pair[1].Point);
					var vector = p2 - p1;
					for (var p = p1; map.Exists(p); p -= vector)
						antinodes.Add(p);
					for (var p = p2; map.Exists(p); p += vector)
						antinodes.Add(p);
				}
			}

			var unique = antinodes.Count();

			return unique;
		}
	}
}
