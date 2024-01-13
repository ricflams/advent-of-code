using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day13
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Mine Cart Madness";
		public override int Year => 2018;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1("7,3");
			Run("input").Part1("139,65").Part2("40,77");
			Run("extra").Part1("45,34").Part2("91,25");
		}

		protected override string Part1(string[] input)
		{
			var (map, cars) = ReadMapAndCars(input);

			var crash = FirstCollision();
			return crash.Position;

			Car FirstCollision()
			{
				while (true)
				{
					cars = cars.OrderBy(c => c.Pose.Point.Y).ThenBy(c => c.Pose.Point.X).ToArray();
					foreach (var c in cars)
					{
						c.Move(map);
						if (c.CollidedWithAny(cars, out var _))
						{
							return c;
						}
					}
				}
			}
		}

		protected override string Part2(string[] input)
		{
			var (map, cars) = ReadMapAndCars(input);

			// Keep on driving until there's just one car left. At every tick, move
			// the cars (in order) and check for collisions and remove those that
			// have collided. It's a bit tricky to do nicely, because the colliding
			// car may already have taken its turn or have yet to take it.
			while (cars.Count() > 1)
			{
				cars = cars.OrderBy(c => c.Pose.Point.Y).ThenBy(c => c.Pose.Point.X).ToArray();
				var crashed = new HashSet<Car>();
				foreach (var c in cars)
				{
					if (crashed.Contains(c))
						continue;
					c.Move(map);
					if (c.CollidedWithAny(cars, out var crash))
					{
						crashed.Add(c);
						crashed.Add(crash);
					}
				}
				if (crashed.Any())
				{
					cars = cars.Where(c => !crashed.Contains(c)).ToArray();
				}
			}

			return cars.First().Position;
		}

		private static (CharMap, Car[]) ReadMapAndCars(string[] input)
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
			
			// Use the map as-is for driving the cars, so wipe the actual
			// car-symbols away from it.
			foreach (var c in cars)
			{
				map[c.Pose.Point] = c.Pose.Direction == Direction.Up || c.Pose.Direction == Direction.Down ? '|' : '-';
			}

			return (map, cars);
		}

		internal class Car
		{
			public Car(Pose pose) => (Pose, NextTurn) = (pose, Direction.Left);
			public Pose Pose { get; }
			public Direction NextTurn  { get; set; }

			public string Position => $"{Pose.Point.X},{Pose.Point.Y}";

			public bool CollidedWithAny(IEnumerable<Car> cars, out Car crash)
			{
				crash = cars.FirstOrDefault(c => c != this && c.Pose.Point == Pose.Point);
				return crash != null;
			}

			public void Move(CharMap map)
			{
				Pose.Move(1);
				switch (map[Pose.Point])
				{
					case '+':
						// Time to turn
						switch (NextTurn)
						{
							case Direction.Up: // Up means "go straight" in this puzzle
								NextTurn = Direction.Right;
								break;
							case Direction.Right:
								Pose.TurnRight();
								NextTurn = Direction.Left;
								break;
							case Direction.Left:
								Pose.TurnLeft();
								NextTurn = Direction.Up;
								break;
						}
						break;
					case '/':
						switch (Pose.Direction)
						{
							case Direction.Up: Pose.TurnRight(); break;
							case Direction.Right: Pose.TurnLeft(); break;
							case Direction.Down: Pose.TurnRight(); break;
							case Direction.Left: Pose.TurnLeft(); break;
						}
						break;
					case '\\':
						switch (Pose.Direction)
						{
							case Direction.Up: Pose.TurnLeft(); break;
							case Direction.Right: Pose.TurnRight(); break;
							case Direction.Down: Pose.TurnLeft(); break;
							case Direction.Left: Pose.TurnRight(); break;
						}
						break;
				}
			}
		}
	}
}
