using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day13
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Mine Cart Madness";
		public override int Year => 2018;
		public override int Day => 13;

		public void Run()
		{
			Run("test1").Part1("7,3");
			//Run("test2").Part1(0).Part2(0); 3679 too low,  3797 too high
			Run("input").Part1("139,65").Part2("");
		}

		internal class Car
		{
			public Car(Pose pose) => (Pose, NextTurn) = (pose, Direction.Left);
			public Pose Pose { get; }
			public Direction NextTurn  { get; set; }
			public bool IsCrashed { get; set; }
		}

		protected override string Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var cars = map
				.AllPoints(c => "v^<>".Contains(c))
				.Select(p =>
				{
					var direction = map[p] switch 
					{
						'>' => Direction.Right,
						'v' => Direction.Down,
						'<' => Direction.Left,
						'^' => Direction.Up,
						_ => throw new Exception("Unknown direction")
					};
					var pose = Pose.From(p, direction);
					return new Car(pose);
				})
				.ToArray();
			
			foreach (var c in cars)
			{
				map[c.Pose.Point] = c.Pose.Direction == Direction.Up || c.Pose.Direction == Direction.Down ? '|' : '-';
			}

			var crashpoint = "";
			while (crashpoint == "")
			{
				cars = cars.OrderBy(c => c.Pose.Point.Y).ThenBy(c => c.Pose.Point.X).ToArray();
				foreach (var c in cars)
				{
					c.Pose.Move(1);

					if (cars.Any(c2 => c2 != c && c2.Pose.Point == c.Pose.Point))
					{
						crashpoint = $"{c.Pose.Point.X},{c.Pose.Point.Y}";
						break;
					}

					switch (map[c.Pose.Point])
					{
						case '+':
							// Time to turn
							switch (c.NextTurn)
							{
								case Direction.Up:
									c.NextTurn = Direction.Right;
									break;
								case Direction.Right:
									c.Pose.TurnRight();
									c.NextTurn = Direction.Left;
									break;
								case Direction.Left:
									c.Pose.TurnLeft();
									c.NextTurn = Direction.Up;
									break;
							}
							break;
						case '/':
							switch (c.Pose.Direction)
							{
								case Direction.Up: c.Pose.TurnRight(); break;
								case Direction.Right: c.Pose.TurnLeft(); break;
								case Direction.Down: c.Pose.TurnRight(); break;
								case Direction.Left: c.Pose.TurnLeft(); break;
							}
							break;
						case '\\':
							switch (c.Pose.Direction)
							{
								case Direction.Up: c.Pose.TurnLeft(); break;
								case Direction.Right: c.Pose.TurnRight(); break;
								case Direction.Down: c.Pose.TurnLeft(); break;
								case Direction.Left: c.Pose.TurnRight(); break;
							}
							break;
					}
				}
			}


			return crashpoint;
		}

		protected override string Part2(string[] input)
		{


			var map = CharMap.FromArray(input);

			var cars = map
				.AllPoints(c => "v^<>".Contains(c))
				.Select(p =>
				{
					var direction = map[p] switch 
					{
						'>' => Direction.Right,
						'v' => Direction.Down,
						'<' => Direction.Left,
						'^' => Direction.Up,
						_ => throw new Exception("Unknown direction")
					};
					var pose = Pose.From(p, direction);
					return new Car(pose);
				})
				.ToArray();
			
			foreach (var c in cars)
			{
				map[c.Pose.Point] = c.Pose.Direction == Direction.Up || c.Pose.Direction == Direction.Down ? '|' : '-';
			}

			var crashpoint = "";
			while (crashpoint == "")
			{
				cars = cars.Where(c => !c.IsCrashed).OrderBy(c => c.Pose.Point.Y).ThenBy(c => c.Pose.Point.X).ToArray();

				if (cars.Count(c => !c.IsCrashed) == 1)
				{
					var c  = cars.First(c => !c.IsCrashed);
					crashpoint = $"{c.Pose.Point.X},{c.Pose.Point.Y}";
					break;
				}

				foreach (var c in cars)
				{
					if (c.IsCrashed)
						continue;
					c.Pose.Move(1);

					var crashedwith = cars.FirstOrDefault(c2 => c2 != c && c2.Pose.Point == c.Pose.Point);
					if (crashedwith != null)
					{
						c.IsCrashed = true;
						crashedwith.IsCrashed = true;
						//crashpoint = $"{c.Pose.Point.X},{c.Pose.Point.Y}";
						continue;
					}

					switch (map[c.Pose.Point])
					{
						case '+':
							// Time to turn
							switch (c.NextTurn)
							{
								case Direction.Up:
									c.NextTurn = Direction.Right;
									break;
								case Direction.Right:
									c.Pose.TurnRight();
									c.NextTurn = Direction.Left;
									break;
								case Direction.Left:
									c.Pose.TurnLeft();
									c.NextTurn = Direction.Up;
									break;
							}
							break;
						case '/':
							switch (c.Pose.Direction)
							{
								case Direction.Up: c.Pose.TurnRight(); break;
								case Direction.Right: c.Pose.TurnLeft(); break;
								case Direction.Down: c.Pose.TurnRight(); break;
								case Direction.Left: c.Pose.TurnLeft(); break;
							}
							break;
						case '\\':
							switch (c.Pose.Direction)
							{
								case Direction.Up: c.Pose.TurnLeft(); break;
								case Direction.Right: c.Pose.TurnRight(); break;
								case Direction.Down: c.Pose.TurnLeft(); break;
								case Direction.Left: c.Pose.TurnRight(); break;
							}
							break;
					}
				}
			}


			return crashpoint;
		}
	}
}
