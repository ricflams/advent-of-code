using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day08.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(14).Part2(34);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(313).Part2(1064);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var antinodes = new HashSet<Point>();
			var antennas = map.AllWhere(ch => ch != '.').ToArray().GroupBy(x => x.Item2);
			foreach (var a in antennas)
			{
				var key = a.Key;
				foreach (var pair in MathHelper.Combinations(a, 2))
				{
					var (p1, p2) = (pair[0].Item1, pair[1].Item1);

					antinodes.Add(p2 + (p2 - p1));
					antinodes.Add(p1 - (p2 - p1));

					//Console.WriteLine($"{key} {p1} {p2}");
				}
			}

			var unique = antinodes.Count(map.Exists);

			return unique;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (maxx_, maxy) = map.Size();

			var antinodes = new HashSet<Point>();

			var antennas = map.AllWhere(ch => ch != '.').ToArray().GroupBy(x => x.Item2);
			foreach (var a in antennas)
			{
				var key = a.Key;
				foreach (var pair in MathHelper.Combinations(a, 2))
				{
					var (p1, p2) = (pair[0].Item1, pair[1].Item1);

					// antinodes.Add(p1);
					// antinodes.Add(p2);

					var vector = p2 - p1;
					for (var p = p1; map.Exists(p); p -= vector)
						antinodes.Add(p);
					for (var p = p2; map.Exists(p); p += vector)
						antinodes.Add(p);
					// antinodes.Add(p2 + (p2 - p1));
					// antinodes.Add(p1 - (p2 - p1));

					//Console.WriteLine($"{key} {p1} {p2}");
				}
			}

			var unique = antinodes.Count(map.Exists);

			return unique;
		}
	}
}
