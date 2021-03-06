using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
    public static class CharMatrix
    {
        public static int Width(this char[,] mx) => mx.GetLength(0);
        public static int Height(this char[,] mx) => mx.GetLength(1);
        public static (int, int) Dim(this char[,] mx) => (mx.Width(), mx.Height());

		public static char[,] Create(int w, int h, char defaultChar)
		{
			var map = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					map[x, y] = defaultChar;
				}
			}
			return map;
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

		public static string[] ToStringArray(this char[,] mx)
		{
			var (w, h) = mx.Dim();

			return Enumerable.Range(0, h)
				.Select(y => new string(Enumerable.Range(0, w).Select(x => mx[x,y]).ToArray()))
				.ToArray();
		}

        public static char[,] RotateClockwise(this char[,] mx, int angle)
		{
			var (w, h) = mx.Dim();

			Func<int, int, (int, int)> rotate = (int x, int y) => (x, y);

			var rotated = mx;
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
					rotated[rotx, roty] = mx[x, y];
				}
			}
			return rotated;
		}

		public static char[,] FlipV(this char[,] mx)
		{
			var (w, h) = mx.Dim();

			var flipped = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					flipped[x, y] = mx[w - 1 - x, y];
				}
			}
			return flipped;
		}

		public static char[,] FlipH(this char[,] mx)
		{
			var (w, h) = mx.Dim();

			var flipped = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					flipped[x, y] = mx[x, h - 1 - y];
				}
			}
			return flipped;
		}

		public static void ShiftRowRight(this char[,] mx, int row, int n)
		{
			var (w, h) = mx.Dim();

			n = (n % w + w) % w; // normalized n % w
			if (n == 0)
				return;
			
			var shifted = new char[w];
			for (var x = 0; x < w; x++)
			{
				shifted[n] = mx[x, row];
				if (++n == w)
					n = 0;
			}
			for (var x = 0; x < w; x++)
			{
				mx[x, row] = shifted[x];
			}
		}

		public static void ShiftColDown(this char[,] mx, int col, int n)
		{
			var (w, h) = mx.Dim();

			n = (n % h + h) % h; // normalized n % w
			if (n == 0)
				return;
			
			var shifted = new char[h];
			for (var y = 0; y < h; y++)
			{
				shifted[n] = mx[col, y];
				if (++n == h)
					n = 0;
			}
			for (var y = 0; y < h; y++)
			{
				mx[col, y] = shifted[y];
			}
		}

		public static char[,] ExpandBy(this char[,] mx, int n, char defaultChar)
		{
			var (w, h) = mx.Dim();

			var expanded = new char[w + 2*n, h + 2*n];
			for (var x = 0; x < w + 2*n; x++)
			{
				for (var y = 0; y < h + 2*n; y++)
				{
					expanded[x, y] = defaultChar;
				}
			}
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					expanded[x + n, y + n] = mx[x, y];
				}
			}
			return expanded;
		}

		public static char[,] CopyPart(this char[,] mx, int x, int y, int w, int h)
		{
			var part = new char[w, h];

			for (var i = 0; i < w; i++)
			{
				for (var j = 0; j < h; j++)
				{
					part[i, j] = mx[x + i, y + j];
				}
			}
			return part;
		}

		public static void PastePart(this char[,] mx, int x, int y, char[,] part)
		{
			var (w, h) = part.Dim();
			for (var i = 0; i < w; i++)
			{
				for (var j = 0; j < h; j++)
				{
					mx[x + i, y + j] = part[i, j];
				}
			}
		}

		public static bool Match(this char[,] mx, int x, int y, char[,] part)
		{
			var (w, h) = part.Dim();
			for (var i = 0; i < w; i++)
			{
				for (var j = 0; j < h; j++)
				{
					if (part[i, j] != mx[x + i, y + j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int CountChar(this char[,] mx, char searched)
		{
			var count = 0;
			foreach (var ch in mx)
			{
				if (ch == searched)
				{
					count++;
				}
			}
			return count;
		}

		public static IEnumerable<Point> PositionsOf(this char[,] mx, char searchFor)
		{
			var (w, h) = mx.Dim();

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					if (mx[x, y] == searchFor)
					{
						yield return Point.From(x, y);
					}
				}
			}
		}

		public static char CharAt(this char[,] mx, Point p) => mx[p.X, p.Y];

		public static void ConsoleWrite(this char[,] mx)
		{
			var (w, h) = mx.Dim();

			Console.Write('\\');
			for (var x = 0; x < w; x++)
			{
				Console.Write(Legend(x+1));
			}
			Console.WriteLine();
			for (var y = 0; y < h; y++)
			{
				Console.Write(Legend(y+1));
				for (var x = 0; x < w; x++)
				{
					Console.Write(mx[x,y]);
				}
				Console.WriteLine();
			}

			static char Legend(int x) =>
				x % 10 == 0 ? (x/10).ToString().Last() :
				x % 5 == 0 ? ',' :
				'.';
		}
    }
}