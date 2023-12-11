using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public class Point
    {
		public static readonly Point Origin = Point.From(0, 0);

		public int X { get; private set; }
		public int Y { get; private set; }

		private Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		static public Point From(int x, int y) => new(x, y);
		static public Point From((int x, int y) coords) => new(coords.x, coords.y);

		static public Point Parse(string s)
		{
			var v = s.Split(',', StringSplitOptions.RemoveEmptyEntries);
			return From(int.Parse(v[0]), int.Parse(v[1]));
		}

		public static bool operator ==(Point p1, Point p2) => p1 is null ? p2 is null : p1.Equals(p2);
		public static bool operator !=(Point p1, Point p2) => !(p1 == p2);
		public override string ToString() => $"({X},{Y})";
		public override int GetHashCode() => X * 397 ^ Y;
		public override bool Equals(object o) => o is Point p && X == p.X && Y == p.Y;

		public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

		public static Point operator -(Point p) => Point.From(-p.X, -p.Y); // unary minus
		public static Point operator -(Point p1, Point p2) => Point.From(p1.X - p2.X, p1.Y - p2.Y);
		public static Point operator +(Point p1, Point p2) => Point.From(p1.X + p2.X, p1.Y + p2.Y);
		public static Point operator *(Point p, int n) => Point.From(p.X * n, p.Y * n);
		public static Point operator *(int n, Point p) => Point.From(p.X * n, p.Y * n);

		public Point N => Up;
		public Point E => Right;
		public Point S => Down;
		public Point W => Left;
		public Point NE => DiagonalUpRight;
		public Point NW => DiagonalUpLeft;
		public Point SE => DiagonalDownRight;
		public Point SW => DiagonalDownLeft;

		public Point Up => new Point(X, Y - 1);
		public Point Right => new Point(X + 1, Y);
		public Point Down => new Point(X, Y + 1);
		public Point Left => new Point(X - 1, Y);
		public Point DiagonalUpRight => new Point(X + 1, Y - 1);
		public Point DiagonalUpLeft => new Point(X - 1, Y - 1);
		public Point DiagonalDownRight => new Point(X + 1, Y + 1);
		public Point DiagonalDownLeft => new Point(X - 1, Y + 1);

		public Point MoveUp(int n) => new Point(X, Y - n);
		public Point MoveRight(int n) => new Point(X + n, Y);
		public Point MoveDown(int n) => new Point(X, Y + n);
		public Point MoveLeft(int n) => new Point(X - n, Y);
		public Point MoveDiagonalUpRight(int n) => new Point(X + n, Y - n);
		public Point MoveDiagonalUpLeft(int n) => new Point(X - n, Y - n);
		public Point MoveDiagonalDownRight(int n) => new Point(X + n, Y + n);
		public Point MoveDiagonalDownLeft(int n) => new Point(X - n, Y + n);

		public static Point MoveUp(Point p) => p.Up;
		public static Point MoveRight(Point p) => p.Right;
		public static Point MoveDown(Point p) => p.Down;
		public static Point MoveLeft(Point p) => p.Left;
		public static Point MoveDiagonalUpRight(Point p) => p.DiagonalUpRight;
		public static Point MoveDiagonalUpLeft(Point p) => p.DiagonalUpLeft;
		public static Point MoveDiagonalDownRight(Point p) => p.DiagonalDownRight;
		public static Point MoveDiagonalDownLeft(Point p) => p.DiagonalDownLeft;

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

		public Point Move(Direction direction, int n)
		{
			switch (direction)
			{
				case Direction.Up: return MoveUp(n);
				case Direction.Right: return MoveRight(n);
				case Direction.Down: return MoveDown(n);
				case Direction.Left: return MoveLeft(n);
			}
			throw new Exception($"{nameof(Move)}: Unknown direction {direction}");
		}

		public Point Move(char ch)
		{
			switch (ch)
			{
				case '^': return Up;
				case '>': return Right;
				case 'v': return Down;
				case '<': return Left;
			}
			throw new Exception($"{nameof(Move)}: Unknown direction {ch}");
		}

		public Point Move(Point vector, int factor = 1)
		{
			return new Point(X + vector.X * factor, Y + vector.Y * factor);
		}

		public IEnumerable<Point> LookAround()
		{
			yield return Up;
			yield return Right;
			yield return Down;
			yield return Left;
		}

		public IEnumerable<Point> LookDiagonallyAround()
		{
			yield return Up;
			yield return DiagonalUpRight;
			yield return Right;
			yield return DiagonalDownRight;
			yield return Down;
			yield return DiagonalDownLeft;
			yield return Left;
			yield return DiagonalUpLeft;
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

		public int ManhattanDistanceTo(Point pos)
		{
			return Math.Abs(X - pos.X) + Math.Abs(Y - pos.Y);
		}

		public int DiagonalDistanceTo(Point pos)
		{
			var dx = Math.Abs(X - pos.X);
			var dy = Math.Abs(Y - pos.Y);
			return Math.Max(dx, dy);
		}

		public Point RotateRight(int angle)
		{
			switch (angle)
			{
				case 0: return this;
				case 90: return new Point(-Y, X);
				case 180: return new Point(-X, -Y);
				case 270: return new Point(Y, -X);
				default:
					throw new Exception($"Unsupported angle {angle}");
			}
		}

		public Point RotateLeft(int angle)
		{
			switch (angle)
			{
				case 0: return this;
				case 90: return new Point(Y, -X);
				case 180: return new Point(-X, -Y);
				case 270: return new Point(-Y, X);
				default:
					throw new Exception($"Unsupported angle {angle}");
			}
		}

		public IEnumerable<Point> LineTo(Point p)
		{
			if (X == p.X)
			{
				var (miny, maxy) = (Math.Min(Y, p.Y), Math.Max(Y, p.Y));
				for (var y = miny; y <= maxy; y++)
				{
					yield return Point.From(X, y);
				}
				yield break;
			}
			if (Y == p.Y)
			{
				var (minx, maxx) = (Math.Min(X, p.X), Math.Max(X, p.X));
				for (var x = minx; x <= maxx; x++)
				{
					yield return Point.From(x, Y);
				}
				yield break;
			}
			throw new Exception($"{this} -> {p} is not straight");
		}

		public bool Within(int maxX, int maxY) => X >= 0 && X < maxX && Y >= 0 && Y < maxY;
	}

	internal static class PointExtensions
	{
		public static IEnumerable<Point> Within(this IEnumerable<Point> points, int maxX, int maxY)
		{
			return points.Where(p => p.X >= 0 && p.X < maxX && p.Y >= 0 && p.Y < maxY);
		}

		public static IEnumerable<Point> Within(this IEnumerable<Point> points, Point max)
		{
			return points.Where(p => p.X >= 0 && p.X < max.X && p.Y >= 0 && p.Y < max.Y);
		}

		public static IEnumerable<Point> Within(this IEnumerable<Point> points, Point min, Point max)
		{
			return points.Where(p => p.X >= min.X && p.X < max.X && p.Y >= min.Y && p.Y < max.Y);
		}

		public static IEnumerable<Point> Within(this IEnumerable<Point> points, char[,] mx)
		{
			var (w, h) = mx.Dim();
			return points.Where(p => p.X >= 0 && p.X < w && p.Y >= 0 && p.Y < w);
		}
	}
}
