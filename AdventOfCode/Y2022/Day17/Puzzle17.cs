using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day17
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 17";
		public override int Year => 2022;
		public override int Day => 17;

		public void Run()
		{
			Run("test1").Part1(3068);// TODO .Part2(1514285714288);
			Run("input").Part1(3133).Part2(1547953216393);
		}

		private readonly string[][] Rocks = new string[][]
		{
			new string[]{ "####" },
			new string[]{ ".#.", "###", ".#." },
			new string[]{ "..#", "..#", "###" },
			new string[]{ "#", "#", "#", "#" },
			new string[]{ "##", "##"}
		};

		protected override long Part1(string[] input)
		{
			var jets = input[0];

			var width = 7;
			var top = 0;

			var map = new CharMap('.');
			for (var x = 1; x < width+1; x++)
				map[x][0] = '-';
			map[0][0] = map[width+1][0] = '+';			

			void FillEmptyRow(int y)
			{
				for (var x = 1; x < width+1; x++)
					map[x][y] = '.';
				map[0][y] = map[width+1][y] = '|';
			}

			var ip = 0;
			for (var j = 0; j < 2022; j++)
			{
				var rock = Rocks[j % Rocks.Length];
				for (var i = 0; i < 3 + rock.Length; i++)
				{
					FillEmptyRow(top - 1 - i);
				}

				var w = rock[0].Length;
				var x = 2 + 1; // [0] is wall
				var y = top - (3 + rock.Length);

				while (true)
				{
					var jet = jets[ip++ % jets.Length];
					if (jet == '<')
					{
						if (CanMove(rock, x-1, y))
							x--;
					}
					else
					{
						if (CanMove(rock, x+1, y))
							x++;
					}
					if (!CanMove(rock, x, y+1))
						break;
					y++;
				}
				Draw(rock, x, y);
				if (y < top)
					top = y;
			}

			bool CanMove(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#' && map[x0+x][y0+y] != '.')
							return false;
					}
				}
				return true;
			}

			void Draw(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#')
							map[x0+x][y0+y] = '#';
					}
				}
			}

			return -top;
		}

		protected override long Part2(string[] input)
		{
			var jets = input[0];

			var width = 7;
			var top = 0;

			var map = new CharMap('.');
			for (var x = 1; x < width+1; x++)
				map[x][0] = '-';
			map[0][0] = map[width+1][0] = '+';			

			void FillEmptyRow(int y)
			{
				for (var x = 1; x < width+1; x++)
					map[x][y] = '.';
				map[0][y] = map[width+1][y] = '|';
			}

			var ip = 0L;
			var heights = new List<int>();

			var delta0 = -1;
			var j0 = 0;

			for (var j = 0;; j++)
			{
				heights.Add(-top);

				var rock = Rocks[j % Rocks.Length];
				for (var i = 0; i < 3 + rock.Length; i++)
				{
					FillEmptyRow(top - 1 - i);
				}

				var w = rock[0].Length;
				var x = 2 + 1; // [0] is wall
				var y = top - (3 + rock.Length);

				while (true)
				{
					if (ip % jets.Length == 0 && j % Rocks.Length == 0)
					{
						var delta = heights.Last() - heights[j0];
						if (delta == delta0)
						{
							var rocks = 1000000000000 - j0;
							var rocksPerCycle = j - j0;
							var fullCycles = rocks / rocksPerCycle;

							var remainingRocks = (int)(rocks % rocksPerCycle);
							var remainingHeight = heights[j0 + remainingRocks];

							var fullHeight = fullCycles * delta + remainingHeight;
							return fullHeight;					
						}
						delta0 = delta;
						j0 = j;
					}

					var jet = jets[(int)(ip++ % jets.Length)];
					if (jet == '<')
					{
						if (CanMove(rock, x-1, y))
							x--;
					}
					else
					{
						if (CanMove(rock, x+1, y))
							x++;
					}
					if (!CanMove(rock, x, y+1))
						break;
					y++;
				}
				Draw(rock, x, y);
				if (y < top)
					top = y;
		 	}

			bool CanMove(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#' && map[x0+x][y0+y] != '.')
							return false;
					}
				}
				return true;
			}

			void Draw(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#')
							map[x0+x][y0+y] = '#';
					}
				}
			}
		}
	}
}
