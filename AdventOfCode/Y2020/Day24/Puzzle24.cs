using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day24
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 24;

		public void Run()
		{
			RunFor("test1", 10, 2208);
			RunFor("input", 459, 4150);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var moves = input
				.Select(line =>
				{
					var tilemoves = new List<Point>();
					for (var i = 0; i < line.Length; i++)
					{
						// COnverter directions: e, se, sw, w, nw, and ne
						// Shift coordinates for the diagonals so:
						// ne -> upright, nw -> up, sw -> downleft, se -> s
						switch (line[i])
						{
							case 'e': tilemoves.Add(Point.From(1, 0)); break;
							case 'w': tilemoves.Add(Point.From(-1, 0)); break;
							case 's': tilemoves.Add(line[++i] == 'w' ? Point.From(-1, 1) : Point.From(0, 1)); break;
							case 'n': tilemoves.Add(line[++i] == 'w' ? Point.From(0, -1) : Point.From(1, -1)); break;
						}
					}
					return tilemoves;
				})
				.ToArray();

			var lobby = new CharMap('.');
			foreach (var m in moves)
			{
				var tile = m.Aggregate(Point.Origin, (pos, mov) => pos + mov);
				var color = lobby[tile];
				lobby[tile] = color == '.' ? '#' : '.';
			}

			var result1 = lobby.Count('#');


			static Point[] Adjacents(Point p) => new Point[]
			{
				p.Up,
				p.DiagonalUpRight,
				p.Right,
				p.Down,
				p.DiagonalDownLeft,
				p.Left
			};

			for (var i = 0; i < 100; i++)
			{
				lobby = lobby.TransformAutomata(Adjacents, (p, ch, adjcount) =>
				{
					var isBlack = ch == '#';
					if (isBlack && (adjcount == 0 || adjcount > 2))
					{
						return '.';
					}
					if (isBlack || !isBlack && adjcount == 2)
					{
						return '#';
					}
					return ch;
				});
			}

			var result2 = lobby.Count('#');

			return (result1, result2);
		}
	}
}
