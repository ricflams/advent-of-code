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

		public static Direction TurnRight(this Direction direction)
		{
			switch (direction)
			{
				case Direction.Up: return Direction.Right;
				case Direction.Right: return Direction.Down;
				case Direction.Down: return Direction.Left;
				case Direction.Left: return Direction.Up;
			}
			throw new Exception($"{nameof(TurnRight)}: Unknown direction {direction}");
		}

		public static Direction TurnLeft(this Direction direction)
		{
			switch (direction)
			{
				case Direction.Up: return Direction.Left;
				case Direction.Right: return Direction.Up;
				case Direction.Down: return Direction.Right;
				case Direction.Left: return Direction.Down;
			}
			throw new Exception($"{nameof(TurnLeft)}: Unknown direction {direction}");
		}

		public static IEnumerable<Direction> LookAroundDirection()
		{
			yield return Direction.Up;
			yield return Direction.Right;
			yield return Direction.Down;
			yield return Direction.Left;
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
	}
}
