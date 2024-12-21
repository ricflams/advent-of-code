using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public static class CharMatrix
	{
		public static int Width<T>(this T[,] mx) => mx.GetLength(0);
		public static int Height<T>(this T[,] mx) => mx.GetLength(1);
		public static (int, int) Dim<T>(this T[,] mx) => (mx.Width(), mx.Height());

		public static (Point, Point) MinMax(this char[,] mx)
		{
			var (w, h) = mx.Dim();
			return (Point.Origin, Point.From(w - 1, h - 1));
		}

		public static (Point, Point) Range(this char[,] mx)
		{
			var (w, h) = mx.Dim();
			return (Point.Origin, Point.From(w, h));
		}

		public static bool InRange(this char[,] mx, Point p)
		{
			var (w, h) = mx.Dim();
			return p.X >= 0 && p.Y >= 0 && p.X < w && p.Y < h;
		}

		public static char Get(this char[,] mx, Point pos) => mx[pos.X, pos.Y];
		public static void Set(this char[,] mx, Point pos, char ch) => mx[pos.X, pos.Y] = ch;

		public static T[,] Create<T>(int w, int h, T defaultValue)
		{
			var map = new T[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					map[x, y] = defaultValue;
				}
			}
			return map;
		}

		public static char[,] FromArray(this string[] arrays) => ToCharMatrix(arrays);

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
				.Select(y => new string(Enumerable.Range(0, w).Select(x => mx[x, y]).ToArray()))
				.ToArray();
		}

		public static char[,] Copy(this char[,] mx)
		{
			var (w, h) = mx.Dim();
			var map = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					map[x, y] = mx[x, y];
				}
			}
			return map;
		}

		public static T[,] RotateClockwise<T>(this T[,] mx, int angle)
		{
			var (w, h) = mx.Dim();

			Func<int, int, (int, int)> rotate = (int x, int y) => (x, y);

			var rotated = mx;
			switch (angle % 360)
			{
				case 0:
					rotated = new T[w, h];
					break;
				case 90:
					rotated = new T[h, w];
					rotate = (int x, int y) => (h - 1 - y, x);
					break;
				case 180:
					rotated = new T[w, h];
					rotate = (int x, int y) => (w - 1 - x, h - 1 - y);
					break;
				case 270:
					rotated = new T[h, w];
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

		public static char[,] FlipXY(this char[,] mx) => mx.RotateClockwise(90).FlipV();

		public static char[,] Transform(this char[,] mx, Func<char, char[], char> transform)
		{
			var (w, h) = mx.Dim();

			var transformed = new char[w, h];
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					var adjacents = new char[8]
					{
						y > 0 ? mx[x, y-1] : (char)0,
						y > 0 && x > 0 ? mx[x-1, y-1] : (char)0,
						y > 0 && x < w-1 ? mx[x+1, y-1] : (char)0,
						x > 0 ? mx[x-1, y] : (char)0,
						x < w-1 ? mx[x+1, y] : (char)0,
						y < h-1 ? mx[x, y+1] : (char)0,
						y < h-1 && x > 0 ? mx[x-1, y+1] : (char)0,
						y < h-1 && x < w-1 ? mx[x+1, y+1] : (char)0
					};
					transformed[x, y] = transform(mx[x, y], adjacents);
				}
			}
			return transformed;
		}

		public static void Map(this char[,] mx, Func<char, char> transform)
		{
			var (w, h) = mx.Dim();

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					mx[x, y] = transform(mx[x, y]);
				}
			}
		}

		public static void Visit<T>(this T[,] mx, Action<int, int> action)
		{
			var (w, h) = mx.Dim();

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					action(x, y);
				}
			}
		}

		public static IEnumerable<T> AllValues<T>(this T[,] mx)
		{
			var (w, h) = mx.Dim();

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					yield return mx[x, y];
				}
			}
		}

		public static IEnumerable<Point> AllPoints(this char[,] mx, Func<char, bool> predicate = null)
		{
			var (w, h) = mx.Dim();

			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					if (predicate == null || predicate(mx[x, y]))
					{
						yield return Point.From(x, y);
					}
				}
			}
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

			var (w2, h2) = (w + 2 * n, h + 2 * n);
			var expanded = new char[w2, h2];

			// Fill the expanded area
			for (var x = 0; x < w2; x++)
			{
				for (var y0 = 0; y0 < n; y0++)
				{
					expanded[x, y0] = defaultChar;
					expanded[x, h2 - 1 - y0] = defaultChar;
				}
			}
			for (var x0 = 0; x0 < n; x0++)
			{
				for (var y = n; y < h2 - n; y++)
				{
					expanded[x0, y] = defaultChar;
					expanded[w2 - 1 - x0, y] = defaultChar;
				}
			}

			// Copy the middle part
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

		public static bool Match(this char[,] mx, char[,] other)
		{
			return Match(mx, 0, 0, other);
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

		public static T Hash<T>(this char[,] mx, Func<IEnumerable<byte>, T> hashing)
		{
			var (w, h) = mx.Dim();

			return hashing(AsStream());

			IEnumerable<byte> AsStream()
			{
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						yield return (byte)mx[x, y];
					}
				}
			}
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
				Console.Write(Legend(x + 1, '|'));
			}
			Console.WriteLine();
			for (var y = 0; y < h; y++)
			{
				Console.Write(Legend(y + 1, '-'));
				for (var x = 0; x < w; x++)
				{
					Console.Write(mx[x, y]);
				}
				Console.WriteLine();
			}

			static char Legend(int x, char fivemark) =>
				x % 10 == 0 ? (x / 10).ToString().Last() :
				x % 5 == 0 ? fivemark :
				' ';
		}
	}
}