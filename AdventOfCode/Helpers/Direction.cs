using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	public class Directions
	{
		public static readonly Direction[] All = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];
		public static IEnumerable<Direction> AllExcept(Direction skip) => All.Where(d => d != skip);
	}

	public static class DirectionExtensions
	{
		public static char AsChar(this Direction direction) => direction switch
		{
			Direction.Left => '<',
			Direction.Right => '>',
			Direction.Up => '^',
			Direction.Down => 'v',
			_ => throw new Exception($"Unsupported direction {direction}")
		};

		public static Direction AsDirection(this char ch) => ch switch
		{
			'<' => Direction.Left,
			'>' => Direction.Right,
			'^' => Direction.Up,
			'v' => Direction.Down,
			_ => throw new Exception($"Unsupported direction {ch}")
		};

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

		public static Direction TurnAround(this Direction direction)
		{
			switch (direction)
			{
				case Direction.Up: return Direction.Down;
				case Direction.Right: return Direction.Left;
				case Direction.Down: return Direction.Up;
				case Direction.Left: return Direction.Right;
			}
			throw new Exception($"{nameof(TurnLeft)}: Unknown direction {direction}");
		}

		public static Direction RotateRight(this Direction direction, int angle)
		{
			switch (angle)
			{
				case 0: return direction;
				case 90: return direction.TurnRight();
				case 180: return direction.TurnAround();
				case 270: return direction.TurnLeft();
				default:
					throw new Exception($"Unsupported angle {angle}");
			}
		}

		public static Direction RotateLeft(this Direction direction, int angle)
		{
			switch (angle)
			{
				case 0: return direction;
				case 90: return direction.TurnLeft();
				case 180: return direction.TurnAround();
				case 270: return direction.TurnRight();
				default:
					throw new Exception($"Unsupported angle {angle}");
			}
		}

		public static IEnumerable<Direction> LookAroundDirection()
		{
			yield return Direction.Up;
			yield return Direction.Right;
			yield return Direction.Down;
			yield return Direction.Left;
		}
	}
}
