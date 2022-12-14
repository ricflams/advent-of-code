using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day14.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 14";
		public override int Year => 2022;
		public override int Day => 14;

		public void Run()
		{
			Run("test1").Part1(24).Part2(93);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(618).Part2(26358);
		}

		protected override long Part1(string[] input)
		{
			var map = new CharMap('.');
			var abyssY = 0;
			foreach (var s in input)
			{
				Point pos = null;
				foreach (var pts in s.Split("->"))
				{
					var (x, y) = pts.Trim().RxMatch("%d,%d").Get<int,int>();
					if (pos != null)
					{
						if (x == pos.X)
						{
							var (miny, maxy) = (Math.Min(y, pos.Y), Math.Max(y, pos.Y));
							for (var yy = miny; yy <= maxy; yy++)
							{
								map[x][yy] = '#';
							}
							if (maxy > abyssY)
								abyssY = maxy;
						}
						else
						{
							var (minx, maxx) = (Math.Min(x, pos.X), Math.Max(x, pos.X));
							for (var xx = minx; xx <= maxx; xx++)
							{
								map[xx][y] = '#';
							}
						}
					}
					pos = Point.From(x, y);
				}
			}

			//map.ConsoleWrite();
			var sand = Point.From(500, 0);

			for (var unit = 0;; unit++)
			{
				if (PourSandGoesIntoAbyss(map, sand, abyssY))
					return unit;
			}
		}

		private bool PourSandGoesIntoAbyss(CharMap map, Point sand, int abyssY)
		{
			while (sand.Y <= abyssY)
			{
				if (map[sand.Down] == '.')
				{
					sand = sand.Down;
				}
				else if (map[sand.DiagonalDownLeft] == '.')
				{
					sand = sand.DiagonalDownLeft;
				}
				else if (map[sand.DiagonalDownRight] == '.')
				{
					sand = sand.DiagonalDownRight;
				}
				else
				{
					map[sand] = 'o';
					return false;
				}
			}
			return true;
		}

		protected override long Part2(string[] input)
		{
			var map = new CharMap('.');
			var abyssY = 0;
			foreach (var s in input)
			{
				Point pos = null;
				foreach (var pts in s.Split("->"))
				{
					var (x, y) = pts.Trim().RxMatch("%d,%d").Get<int,int>();
					if (pos != null)
					{
						if (x == pos.X)
						{
							var (miny, maxy) = (Math.Min(y, pos.Y), Math.Max(y, pos.Y));
							for (var yy = miny; yy <= maxy; yy++)
							{
								map[x][yy] = '#';
							}
							if (maxy > abyssY)
								abyssY = maxy;
						}
						else
						{
							var (minx, maxx) = (Math.Min(x, pos.X), Math.Max(x, pos.X));
							for (var xx = minx; xx <= maxx; xx++)
							{
								map[xx][y] = '#';
							}
						}
					}
					pos = Point.From(x, y);
				}
			}

			//map.ConsoleWrite();
			var sand = Point.From(500, 0);
			var floorY = abyssY + 2;

			for (var unit = 1;; unit++)
			{
				if (PourSandGoesIntoAbyssWithFloor(map, sand, floorY))
					return unit;
			}

		;
		}

		private bool PourSandGoesIntoAbyssWithFloor(CharMap map, Point sand, int floorY)
		{
			while (true)
			{
				if (sand.Y+1 == floorY)
				{
					map[sand] = 'o';
					return false;
				}
				if (map[sand.Down] == '.')
				{
					sand = sand.Down;
				}
				else if (map[sand.DiagonalDownLeft] == '.')
				{
					sand = sand.DiagonalDownLeft;
				}
				else if (map[sand.DiagonalDownRight] == '.')
				{
					sand = sand.DiagonalDownRight;
				}
				else
				{
					map[sand] = 'o';
					return sand.Y == 0;
				}
			}
		}
	}
}
