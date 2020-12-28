using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Helpers
{
    internal static class Extensions
    {
		public static IEnumerable<T> TakeAll<T>(this BlockingCollection<T> collection)
		{
			while (collection.TryTake(out var value))
			{
				yield return value;
			}
		}

		public static BigInteger ModInverse(this BigInteger a, BigInteger m)
		{
			if (m == 1) return 0;
			var m0 = m;
			(var x, var y) = (BigInteger.One, BigInteger.Zero);

			while (a > 1)
			{
				var q = a / m;
				(a, m) = (m, a % m);
				(x, y) = (y, x - q * y);
			}
			return x < 0 ? x + m0 : x;
		}

		public static string MultiLine(this string str)
		{
			return string.Concat(str.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim() + "\n"));
		}

		public static string TrimAll(this string str)
		{
			return str.Replace(" ", "");
		}

		public static string ToCommaString(this IEnumerable<int> list) => string.Join(',', list);

		public static IEnumerable<string[]> GroupByEmptyLine(this IEnumerable<string> input)
		{
			var group = new List<string>();
			foreach (var line in input)
			{
				if (line.Length == 0)
				{
					yield return group.ToArray();
					group.Clear();
					continue;
				}
				group.Add(line);
			}
			yield return group.ToArray();
		}

		public static ulong Sum(this IEnumerable<ulong> values)
		{
			var sum = 0UL;
			foreach (var v in values)
			{
				sum += v;
			}
			return sum;
		}

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

		public static char[,] ToMultiDim(this string[] arrays)
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
	}
}
