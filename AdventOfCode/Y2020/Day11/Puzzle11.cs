using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2020.Day11
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Seating System";
		protected override int Year => 2020;
		protected override int Day => 11;

		public void Run()
		{
			RunFor("test1", 37, 26);
			RunFor("input", 2265, 2045);
		}

		protected override int Part1(string[] input)
		{
			var seats = CharMap.FromArray(input);
			var occupied = 0;
			while (true)
			{
				seats = seats.Transform((p, ch) =>
					ch == 'L' && p.LookDiagonallyAround().All(c => seats[c] != '#') ? '#' :
					ch == '#' && p.LookDiagonallyAround().Count(c => seats[c] == '#') >= 4 ? 'L' :
					ch
				);
				var n = seats.Count('#');
				if (occupied == n)
				{
					break;
				}
				occupied = n;
			}
			return occupied;
		}

		protected override int Part2(string[] input)
		{
			static int Adjacent(CharMap map, Point p, Func<Point, Point> move)
			{
				while (true)
				{
					p = move(p);
					var ch = map[p];
					if (ch != '.')
					{
						return ch == '#' ? 1 : 0;
					}
				}
			}

			var seats = CharMap.FromArray(input);
			var occupied = 0;
			while (true)
			{
				seats = seats.Transform((p, ch) =>
				{
					if (ch == '.')
						return ch;
					var adjacents =
						Adjacent(seats, p, Point.MoveUp) +
						Adjacent(seats, p, Point.MoveDiagonalUpRight) +
						Adjacent(seats, p, Point.MoveRight) +
						Adjacent(seats, p, Point.MoveDiagonalDownRight) +
						Adjacent(seats, p, Point.MoveDown) +
						Adjacent(seats, p, Point.MoveDiagonalDownLeft) +
						Adjacent(seats, p, Point.MoveLeft) +
						Adjacent(seats, p, Point.MoveDiagonalUpLeft);
					return
						ch == 'L' && adjacents == 0 ? '#' :
						ch == '#' && adjacents >= 5 ? 'L' :
						ch;
				});
				var n = seats.Count('#');
				if (occupied == n)
				{
					break;
				}
				occupied = n;
			}
			return occupied;
		
		}
	}
}
