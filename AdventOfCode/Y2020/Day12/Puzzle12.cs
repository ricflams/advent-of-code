using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode.Y2020.Day12
{
	internal class Puzzle12
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2020/Day12/input.txt");

			//Action N means to move north by the given value.
			//Action S means to move south by the given value.
			//Action E means to move east by the given value.
			//Action W means to move west by the given value.
			//Action L means to turn left the given number of degrees.
			//Action R means to turn right the given number of degrees.
			//Action F means to move forward by the given value in the direction the ship is currently facing.

			var ship = new PointWithDirection(Point.Origin, Direction.Right);
			foreach (var line in input)
			{
				var n = int.Parse(line.Substring(1));
				switch (line[0])
				{
					case 'N': ship.MoveUp(n); break;
					case 'S': ship.MoveDown(n); break;
					case 'E': ship.MoveRight(n); break;
					case 'W': ship.MoveLeft(n); break;
					case 'L': ship.RotateLeft(n); break;
					case 'R': ship.RotateRight(n); break;
					case 'F': ship.Move(n); break;
					default:
						throw new Exception($"Unknown action in {line}");
				}
			}
			var result1 = ship.Point.ManhattanDistanceTo(Point.Origin);
			Console.WriteLine($"Day 12 Puzzle 1: {result1}");
			Debug.Assert(result1 == 2297);
		}

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2020/Day12/input.txt");

			var ship = Point.Origin;
			var waypoint = Point.From(10, -1);
			foreach (var line in input)
			{
				var n = int.Parse(line.Substring(1));
				switch (line[0])
				{
					case 'N': waypoint = waypoint.MoveUp(n); break;
					case 'S': waypoint = waypoint.MoveDown(n); break;
					case 'E': waypoint = waypoint.MoveRight(n); break;
					case 'W': waypoint = waypoint.MoveLeft(n); break;
					case 'L': waypoint = waypoint.RotateLeft(n); break;
					case 'R': waypoint = waypoint.RotateRight(n); break;
					case 'F': ship = ship.Move(waypoint, n); break;
					default:
						throw new Exception($"Unknown action in {line}");
				}
			}
			var result2 = ship.ManhattanDistanceTo(Point.Origin);
			Console.WriteLine($"Day 12 Puzzle 2: {result2}");
			Debug.Assert(result2 == 89984);
		}
	}
}
