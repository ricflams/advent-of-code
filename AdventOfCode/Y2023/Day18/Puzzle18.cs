using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day18
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Lavaduct Lagoon";
		public override int Year => 2023;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").Part1(62).Part2(952408144115);
			Run("input").Part1(26857).Part2(129373230496292);
		}

		protected override long Part1(string[] input)
		{
			var plan = input
				.Select(s => s.RxMatch("%c %d (#%s)").Get<char, int, string>())
				.ToArray();

			var p = Pose.From(0, 0, Direction.Up);
			var points = plan
				.Select(dig =>
				{
					var (dir, n, _) = dig;
					p.Direction = dir switch
					{
						'R' => Direction.Right,
						'D' => Direction.Down,
						'L' => Direction.Left,
						'U' => Direction.Up,
						_ => throw new Exception()
					};
					p.Move(n);
					return p.Point;
				});

			var area = MathHelper.AreaByShoelace(points);
			return area;
		}

		protected override long Part2(string[] input)
		{
			var plan = input
				.Select(s => s.RxMatch("%c %d (#%s)").Get<char, int, string>())
				.ToArray();

			var p = Pose.From(0, 0, Direction.Up);
			var points = plan
				.Select(dig =>
				{
					var (_, _, rgb) = dig;
					p.Direction = rgb[^1] switch
					{
						'0' => Direction.Right,
						'1' => Direction.Down,
						'2' => Direction.Left,
						'3' => Direction.Up,
						_ => throw new Exception()
					};
					var n = Convert.ToInt32(rgb[..^1], 16);
					p.Move(n);
					return p.Point;
				});

			var area = MathHelper.AreaByShoelace(points);
			return area;
		}

	}
}
