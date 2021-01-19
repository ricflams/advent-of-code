using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;

namespace AdventOfCode.Y2016.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "No Time for a Taxicab";
		public override int Year => 2016;
		public override int Day => 1;

		public void Run()
		{
			RunPart1For("test1", 12);
			RunPart2For("test2", 4);
			RunFor("input", 146, 131);
		}

		protected override int Part1(string[] input)
		{
			var steps = input[0].Split(", ");

			// Move point until all steps are completed
			var pos = PointWithDirection.From(Point.Origin, Direction.Up);
			foreach (var step in steps)
			{
				var turn = step[0];
				var dist = int.Parse(step[1..]);
				pos.Turn(turn);
				pos.Move(dist);
			}

			var blocks = pos.Point.ManhattanDistanceTo(Point.Origin);
			return blocks;
		}

		protected override int Part2(string[] input)
		{
			var steps = input[0].Split(", ");

			// Move point until all steps are completed, but move along the
			// entire path while keeping track of previously seen positions.
			var seen = new HashSet<Point>();
			var pos = PointWithDirection.From(Point.Origin, Direction.Up);
			foreach (var step in steps)
			{
				var turn = step[0];
				var dist = int.Parse(step[1..]);
				pos.Turn(turn);
				for (var i = 0; i < dist; i++)
				{
					pos.Move(1);
					if (seen.Contains(pos.Point))
					{
						var blocks = pos.Point.ManhattanDistanceTo(Point.Origin);
						return blocks;
					}
					seen.Add(pos.Point);
				}
			}
			throw new Exception("No solution");
		}
	}
}
