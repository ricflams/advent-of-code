using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Y2020.Day11
{
	internal class Puzzle11
	{
		public static void Run()
		{
			var input = CharMap.FromFile("Y2020/Day11/input.txt");

			var result1 = 0;
			for (var seats = input; ;)
			{
				seats = seats.Transform((p, ch) =>
					ch == 'L' && p.LookDiagonallyAround().All(c => seats[c] != '#') ? '#' :
					ch == '#' && p.LookDiagonallyAround().Count(c => seats[c] == '#') >= 4 ? 'L' :
					ch
				);
				var occupied = seats.Count('#');
				if (occupied == result1)
				{
					break;
				}
				result1 = occupied;
			}

			Console.WriteLine($"Day 11 Puzzle 1: {result1}");
			Debug.Assert(result1 == 2265);


			var result2 = 0;
			for (var seats = input; ;)
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

				var occupied = seats.Count('#');
				if (occupied == result2)
				{
					break;
				}
				result2 = occupied;
			}

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

			Console.WriteLine($"Day 11 Puzzle 2: {result2}");
			Debug.Assert(result2 == 2045);
		}
	}
}
