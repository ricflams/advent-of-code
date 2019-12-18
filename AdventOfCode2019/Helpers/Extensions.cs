using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AdventOfCode2019.Helpers
{
    static class Extensions
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
	}
}
