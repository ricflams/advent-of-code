using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2017.Day11
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Hex Ed";
		public override int Year => 2017;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").Part1(3);
			Run("test2").Part1(0);
			Run("test3").Part1(2);
			Run("test4").Part1(3);
			Run("input").Part1(687).Part2(1483);
		}

		protected override int Part1(string[] input)
		{
			var moves = ReadHexMoves(input[0]);
			var dest = moves.Aggregate(Point.Origin, (pos, mov) => pos + mov);
			var dist = dest.DiagonalDistanceTo(Point.Origin);
			return dist;
		}

		protected override int Part2(string[] input)
		{
			var moves = ReadHexMoves(input[0]);

			var pos = Point.Origin;
			var maxDist = 0;
			foreach (var move in moves)
			{
				pos += move;
				var dist = pos.DiagonalDistanceTo(Point.Origin);
				if (dist > maxDist)
				{
					maxDist = dist;
				}
			}
			return maxDist;
		}

		private static Point[] ReadHexMoves(string input)
		{
			// Translate into axial coordinates ("skewed") and map to 2D-moves.
			// The important part is that all opposite hex-directions must map
			// to opposite 2D-directions; eg sw (-1,1) is opposite ne (1,-1)
			var moves = input
				.Split(',')
				.Select(dir => dir switch
				{
					"n" => Point.From(0, -1),
					"ne" => Point.From(1, -1),
					"se" => Point.From(1, 0),
					"s" => Point.From(0, 1),
					"sw" => Point.From(-1, 1),
					"nw" => Point.From(-1, 0),
					_ => throw new Exception($"Unknown direction {dir}")
				})
				.ToArray();
			return moves;
		}
	}
}
