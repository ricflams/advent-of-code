using System;

namespace AdventOfCode.Helpers
{
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public class PointWithDirection
	{
		public PointWithDirection(int x, int y, Direction direction)
		{
			Point = Point.From(x, y);
			Direction = direction;
		}

		public PointWithDirection(Point p, Direction direction)
		{
			Point = p;
			Direction = direction;
		}

		static public PointWithDirection From(int x, int y, Direction direction) => new PointWithDirection(x, y, direction);
		static public PointWithDirection From(Point p, Direction direction) => new PointWithDirection(p, direction);

		public Point Point { get; private set; }
		public Direction Direction { get; private set; }

		public override int GetHashCode() => Point.GetHashCode() * 397 ^ Direction.GetHashCode();
		public override string ToString() => $"{Point}{Direction.ToString()[0]}";

		public void Move(int n) => Point = Point.Move(Direction, n);

		public void MoveUp(int n) => Point = Point.MoveUp(n);
		public void MoveRight(int n) => Point = Point.MoveRight(n);
		public void MoveDown(int n) => Point = Point.MoveDown(n);
		public void MoveLeft(int n) => Point = Point.MoveLeft(n);
		public void MoveDiagonalUpRight(int n) => Point = Point.MoveDiagonalUpRight(n);
		public void MoveDiagonalUpLeft(int n) => Point = Point.MoveDiagonalUpLeft(n);
		public void MoveDiagonalDownRight(int n) => Point = Point.MoveDiagonalDownRight(n);
		public void MoveDiagonalDownLeft(int n) => Point = Point.MoveDiagonalDownLeft(n);

		public void Move(Direction direction) => Point = Point.Move(direction);
		public void Move(Direction direction, int n) => Point = Point.Move(direction, n);
		public void Move(char ch) => Point = Point.Move(ch);
		public void Move(Point vector, int factor = 1) => Point = Point.Move(vector, factor);

		public void TurnRight() => Direction = Direction.TurnRight();
		public void TurnLeft() => Direction = Direction.TurnLeft();
		public void Turn(DirectionTurn turn)
		{
 			if (turn == DirectionTurn.Right)
				TurnRight();
			else
				TurnLeft();
		}
		public void Turn(char ch)
		{
			switch (ch)
			{
				case 'R':
					TurnRight();
					break;
				case 'L':
					TurnLeft();
					break;
				default:
					throw new Exception($"Unknown turn {ch}");
			}
		}

		public void RotateRight(int angle) => Direction = Direction.RotateRight(angle);
		public void RotateLeft(int angle) => Direction = Direction.RotateLeft(angle);
	}
}
