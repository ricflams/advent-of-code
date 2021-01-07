using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
    public static class CharMatrix
    {
        public static char[,] RotateClockwise(this char[,] map, int angle)
		{
			var w = map.GetLength(0);
			var h = map.GetLength(1);

			Func<int, int, (int, int)> rotate = (int x, int y) => (x, y);

			var rotated = map;
			switch (angle % 360)
			{
				case 0:
					rotated = new char[w, h];
					break;
				case 90:
					rotated = new char[h, w];
					rotate = (int x, int y) => (h - 1 - y, x);
					break;
				case 180:
					rotated = new char[w, h];
					rotate = (int x, int y) => (w - 1 - x, h - 1 - y);
					break;
				case 270:
					rotated = new char[h, w];
					rotate = (int x, int y) => (y, w - 1 - x);
					break;
			}

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					var (rotx, roty) = rotate(x, y);
					rotated[rotx, roty] = map[x, y];
				}
			}
			return rotated;
		}

		public static char[,] FlipV(this char[,] map)
		{
			var w = map.GetLength(0);
			var h = map.GetLength(1);

			var flipped = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					flipped[x, y] = map[w - 1 - x, y];
				}
			}
			return flipped;
		}

		public static char[,] FlipH(this char[,] map)
		{
			var w = map.GetLength(0);
			var h = map.GetLength(1);

			var flipped = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					flipped[x, y] = map[x, h - 1 - y];
				}
			}
			return flipped;
		}

		public static char[,] ToCharMatrix(this string[] arrays)
		{
			var w = arrays[0].Length;
			var h = arrays.Length;
			var map = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					map[x, y] = arrays[y][x];
				}
			}
			return map;
		}

		public static string[] ToStringArray(this char[,] map)
		{
			var w = map.GetLength(0);
			var h = map.GetLength(1);

			return Enumerable.Range(0, h)
				.Select(y => new string(Enumerable.Range(0, w).Select(x => map[x,y]).ToArray()))
				.ToArray();
		}
	
		public static char[,] ExpandBy(this char[,] map, int n, char defaultChar)
		{
			var w = map.GetLength(0);
			var h = map.GetLength(1);
			var newmap = new char[w + 2*n, h + 2*n];
			for (var x = 0; x < w + 2*n; x++)
			{
				for (var y = 0; y < h + 2*n; y++)
				{
					newmap[x, y] = defaultChar;
				}
			}
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					newmap[x + n, y + n] = map[x, y];
				}
			}
			return newmap;
		}

		public static int CountChar(this char[,] map, char searched)
		{
			var count = 0;
			foreach (var ch in map)
			{
				if (ch == searched)
				{
					count++;
				}
			}
			return count;
		}

		public static IEnumerable<Point> PositionsOf(this char[,] map, char searched)
		{
			var w = map.GetLength(0);
			var h = map.GetLength(1);

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					if (map[x, y] == searched)
					{
						yield return Point.From(x, y);
					}
				}
			}
		}

		public static char CharAt(this char[,] map, Point p) => map[p.X, p.Y];
    }
}