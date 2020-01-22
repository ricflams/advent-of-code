using System;
using System.Collections.Generic;

namespace AdventOfCode2019.Helpers
{
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	internal class Point
    {
		public int X { get; private set; }
		public int Y { get; private set; }

		private Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		static public Point From(int x, int y) => new Point(x, y);

		public static bool operator ==(Point p1, Point p2) => (object)p1 == null ? (object)p2 == null : p1.Equals(p2);
		public static bool operator !=(Point p1, Point p2) => !(p1 == p2);
		public override bool Equals(object p) => Equals(p as Point);
		public bool Equals(Point p) => X == p?.X && Y == p?.Y;
		public override int GetHashCode() => X * 397 ^ Y;
		public override string ToString() => $"({X},{Y})";

		public Point Up => new Point(X, Y - 1);
		public Point Right => new Point(X + 1, Y);
		public Point Down => new Point(X, Y + 1);
		public Point Left => new Point(X - 1, Y);

		public Point Move(Direction direction)
		{
			switch (direction)
			{
				case Direction.Up: return Up;
				case Direction.Right: return Right;
				case Direction.Down: return Down;
				case Direction.Left: return Left;
			}
			throw new Exception($"{nameof(Move)}: Unknown direction {direction}");
		}

		public IEnumerable<Point> LookAround()
		{
			yield return Up;
			yield return Right;
			yield return Down;
			yield return Left;
		}

		public IEnumerable<Point> SpiralFrom()
		{
			var p = this;
			yield return p;
			var direction = Direction.Left;
			for (var len = 1; ; len++)
			{
				for (var side = 0; side < 2; side++)
				{
					for (var step = 0; step < len; step++)
					{
						p = p.Move(direction);
						yield return p;
					}
					direction = direction.TurnLeft();
				}
			}
		}
	}
}
