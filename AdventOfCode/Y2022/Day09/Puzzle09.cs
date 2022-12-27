using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Rope Bridge";
		public override int Year => 2022;
		public override int Day => 9;

		public void Run()
		{
			Run("test1").Part1(13).Part2(1);
			Run("test2").Part2(36);
			Run("test9").Part1(6175).Part2(2578);
			Run("input").Part1(5981).Part2(2352);
		}

		protected override long Part1(string[] input)
		{
			var head = Point.Origin;
			var tail = Point.Origin;

			var map = new CharMap();
			foreach (var s in input)
			{
				var (dir, n) = s.RxMatch("%c %d").Get<char, int>();
				while (n-- > 0)
				{
					head = Move(dir, head);
					tail = Pull(head, tail);
					map[tail] = '#';
				}
			}

			return map.Count('#');
		}

		protected override long Part2(string[] input)
		{
			var length = 10;
			var rope = Enumerable.Range(0, length).Select(_ => Point.Origin).ToArray();

			var map = new CharMap();
			foreach (var s in input)
			{
				var (dir, n) = s.RxMatch("%c %d").Get<char, int>();
				while (n-- > 0)
				{
					rope[0] = Move(dir, rope[0]);
					for (var j = 1; j < length; j++)
					{
						rope[j] = Pull(rope[j-1], rope[j]);
					}
					map[rope.Last()] = '#';
				}
			}

			return map.Count('#');
		}

		private static Point Move(char dir, Point p) =>
			dir switch
			{
				'R' => p.Right,
				'L' => p.Left,
				'U' => p.Up,
				'D' => p.Down,
				_ => throw new Exception()
			};

		private static Point Pull(Point head, Point tail)
		{
			var dx = head.X - tail.X;
			var dy = head.Y - tail.Y;
			var dxAbs = Math.Abs(dx);
			var dyAbs = Math.Abs(dy);

			// For nearby knots the robe isn't being pulled at all
			if (dxAbs < 2 && dyAbs < 2)
				return tail;

			// Pure up/down, which may pull the robe
			if (dxAbs == 0)
				return dy switch
				{
					2 => tail.Down,
					-2 => tail.Up,
					_ => tail
				};

			// Pure left/right, which may pull the robe
			if (dyAbs == 0)
				return dx switch
				{
					2 => tail.Right,
					-2 => tail.Left,
					_ => tail
				};

			// Rope is now definitely being pulled diagonally
			if (dx > 0 && dy > 0)
				return tail.DiagonalDownRight;
			if (dx > 0 && dy < 0)
				return tail.DiagonalUpRight;
			if (dx < 0 && dy < 0)
				return tail.DiagonalUpLeft;
			if (dx < 0 && dy > 0)
				return tail.DiagonalDownLeft;

			throw new Exception("Unhandled situation");
		}
	}
}
