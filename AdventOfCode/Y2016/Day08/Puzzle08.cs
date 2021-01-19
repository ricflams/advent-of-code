using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;

namespace AdventOfCode.Y2016.Day08
{
	internal class Puzzle : SoloParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Two-Factor Authentication";
		public override int Year => 2016;
		public override int Day => 8;

		public void Run()
		{
			RunFor("input", "115", "EFEYKFRFIJ");
		}

		private const int Width = 50;
		private const int Height = 6;

		protected override string Part1(string[] input)
		{
			var screen = ReadScreen(input);
			var lit = screen.CountChar('#');
			return lit.ToString();
		}

		protected override string Part2(string[] input)
		{
			var screen = ReadScreen(input);
			var letters = LetterScanner.Scan(screen);
			return letters;
		}

		private static char[,] ReadScreen(string[] input)
		{
			var screen = CharMatrix.Create(Width, Height, ' ');

			foreach (var line in input)
			{
				// rect 49x1
				// rotate row y=2 by 34
				// rotate column x=44 by 1
				if (line.IsRxMatch("rect %dx%d", out var captures))
				{
					var (w, h) = captures.Get<int, int>();
					for (var x = 0; x < w; x++)
					{
						for (var y = 0; y < h; y++)
						{
							screen[x, y] = '#';
						}
					}
				}
				else if (line.IsRxMatch("rotate row y=%d by %d", out captures))
				{
					var (row, dx) = captures.Get<int, int>();
					screen.ShiftRowRight(row, dx);
				}
				else if (line.IsRxMatch("rotate column x=%d by %d", out captures))
				{
					var (col, dy) = captures.Get<int, int>();
					screen.ShiftColDown(col, dy);
				}
				else
				{
					throw new Exception($"Unhandled line '{line}'");
				}
			}
			return screen;
		}
	}
}
