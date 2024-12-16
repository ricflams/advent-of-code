using System;

namespace AdventOfCode.Helpers
{
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public class Pose
	{
		public Pose(int x, int y, Direction direction)
		{
			Point = Point.From(x, y);
			Direction = direction;
		}

		public Pose(Point p, Direction direction)
		{
			Point = p;
			Direction = direction;
		}

		static public Pose From(int x, int y, Direction direction) => new Pose(x, y, direction);
		static public Pose From(Point p, Direction direction) => new Pose(p, direction);

		public Point Point { get; private set; }
		public Direction Direction { get; set; }

		public static bool operator ==(Pose p1, Pose p2) => p1 is null ? p2 is null : p1.Equals(p2);
		public static bool operator !=(Pose p1, Pose p2) => !(p1 == p2);
		public override string ToString() => $"{Point}{Direction.AsChar()}";
		public override int GetHashCode() => Point.GetHashCode() * 397 ^ Direction.GetHashCode();
		public override bool Equals(object o) => o is Pose p && Point == p.Point && Direction == p.Direction;

		public Pose Copy() => From(Point, Direction);

		public void Move() => Point = Point.Move(Direction, 1);
		public void Move(int n) => Point = Point.Move(Direction, n);
		public void MoveBack() => Point = Point.Move(Direction.TurnAround(), 1);

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

		public Point PeekRight => Point.Move(Direction.TurnRight());
		public Point PeekLeft => Point.Move(Direction.TurnLeft());
		public Point PeekAhead => Point.Move(Direction);
		public Point PeekBehind => Point.Move(Direction.TurnAround());
		
		public void TurnRight() => Direction = Direction.TurnRight();
		public void TurnLeft() => Direction = Direction.TurnLeft();
		public void TurnAround() => Direction = Direction.TurnAround();
		public void Turn(DirectionTurn turn)
		{
 			if (turn == DirectionTurn.Right)
				TurnRight();
			else
				TurnLeft();
		}

		public void RotateRight(int angle) => Direction = Direction.RotateRight(angle);
		public void RotateLeft(int angle) => Direction = Direction.RotateLeft(angle);
	}
}
