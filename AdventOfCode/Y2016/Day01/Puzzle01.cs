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

		public override void Run()
		{
			Run("test1").Part1(12);
			Run("test2").Part2(4);
			Run("input").Part1(146).Part2(131);
			Run("extra").Part1(236).Part2(182);			
		}

		protected override int Part1(string[] input)
		{
			var steps = input[0].Split(", ");

			// Move point until all steps are completed
			var pose = Pose.From(Point.Origin, Direction.Up);
			foreach (var step in steps)
			{
				var turn = step[0];
				var dist = int.Parse(step[1..]);
				pose.Turn(turn);
				pose.Move(dist);
			}

			var blocks = pose.Point.ManhattanDistanceTo(Point.Origin);
			return blocks;
		}

		protected override int Part2(string[] input)
		{
			var steps = input[0].Split(", ");

			// Move point until all steps are completed, but move along the
			// entire path while keeping track of previously seen positions.
			var seen = new HashSet<Point>();
			var pose = Pose.From(Point.Origin, Direction.Up);
			foreach (var step in steps)
			{
				var turn = step[0];
				var dist = int.Parse(step[1..]);
				pose.Turn(turn);
				for (var i = 0; i < dist; i++)
				{
					pose.Move(1);
					if (seen.Contains(pose.Point))
					{
						var blocks = pose.Point.ManhattanDistanceTo(Point.Origin);
						return blocks;
					}
					seen.Add(pose.Point);
				}
			}
			throw new Exception("No solution");
		}
	}

	internal static class Extensions
	{
		public static void Turn(this Pose p, char ch)
		{
			switch (ch)
			{
				case 'R':
					p.TurnRight();
					break;
				case 'L':
					p.TurnLeft();
					break;
				default:
					throw new Exception($"Unknown turn {ch}");
			}
		}
	}
}
