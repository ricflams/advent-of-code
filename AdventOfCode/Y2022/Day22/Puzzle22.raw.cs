using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day22.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 22";
		public override int Year => 2022;
		public override int Day => 22;

		public void Run()
		{
			Run("test1").Part1(6032).Part2(5031);
			Run("input").Part1(66292).Part2(127012);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input[0..^2], ' ');
			var turns = input[^1].Replace("R",",R,").Replace("L", ",L,").Split(',');

			var (w, h) = map.Size();

		//	map.ConsoleWrite();

			var pose = new Pose(Point.Origin, Direction.Right);
			while (map[pose.Point] != '.')
				pose.Move();
			
	//		Draw();
			foreach (var turn in turns)
			{
				//Console.WriteLine(turn);
				if (turn == "R")
					pose.TurnRight();
				else if (turn == "L")
					pose.TurnLeft();
				else
				{
					var steps = int.Parse(turn);
					while (map[PeekAhead2().Point] == '.' && steps-- > 0)
						Move();
				}
		//		Draw();				
			}

			Pose PeekAhead2()
			{
				Check();
				var move = pose;
				while (true)
				{
					var p = move.PeekAhead;
					var (x, y) = (p.X, p.Y);
					if (x >= w)
						x = -1;
					else if (x < 0)
						x = w;
					else if (y >= h)
						y = -1;
					else if (y < 0)
						y = h;
					else if (map[p] != ' ')
						return Pose.From(x, y, move.Direction);
					move = Pose.From(x, y, move.Direction);
				}
			}

			void Check()
			{
				if (pose.Point.X < 0 || pose.Point.X >= w)
					throw new Exception();
				if (pose.Point.Y < 0 || pose.Point.Y >= h)
					throw new Exception();
			
			}
			void Move()
			{
				pose = PeekAhead2();
				Draw();
			}

			void Draw()
			{
				// var lines = map.Render((p, ch) => p == pose.Point ? pose.Direction.AsChar() : ch).ToArray();
				// var from = Math.Max(0, pose.Point.Y - 2);
				// var to = Math.Min(h-1, pose.Point.Y + 2);
				// for(var y = from; y <= to; y++)
				// {
				// 	Console.WriteLine(lines[y]);
				// }
				// Console.WriteLine();
			}

			// Facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)
			var facing = pose.Direction switch
			{
				Direction.Right => 0,	
				Direction.Down => 1,	
				Direction.Left => 2,	
				Direction.Up => 3,	
				_ => throw new Exception()
			};
			var password = (pose.Point.Y + 1) * 1000 + (pose.Point.X + 1) * 4 + facing;

			return password;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input[0..^2]);
			var turns = input[^1].Replace("R",",R,").Replace("L", ",L,").Split(',');

			var (w, h) = map.Size();

			var innerCorners = new List<Point>();
			for (var x = 0; x < w-1; x++)
			{
				for (var y = 0; y < h-1; y++)
				{
					if (On(x, y) + On(x+1, y) + On(x, y+1) + On(x+1, y+1) == 3)
					{
						var p = Point.From(x, y);
						innerCorners.Add(p);
						//Console.WriteLine(p);
					}
					int On(int x, int y) => OnMap(x, y) ? 1 : 0;
				}
			}
			bool OnMap(int x, int y) => map[x, y] == '.' || map[x, y] == '#';
			bool OnMap2(Point p) => map[p] == '.' || map[p] == '#';

			var mappings = new Dictionary<string, Pose>();

			var mappers = innerCorners
				.Select(corner =>
				{
					//  A##B
					// ######
					//  D##C
					Pose p1, p2; // p1 goes counter-clockwise ccw, p2 goes clockwise
					Direction d1, d2;
					if (!OnMap2(corner)) // A
					{
						p1 = Pose.From(corner.Down, Direction.Up); d1 = Direction.Left;
						p2 = Pose.From(corner.Right, Direction.Left); d2 = Direction.Up;
					}
					else if (!OnMap2(corner.Right)) // B
					{
						p1 = Pose.From(corner, Direction.Right); d1 = Direction.Up;
						p2 = Pose.From(corner.DiagonalDownRight, Direction.Up); d2 = Direction.Right;
					}
					else if (!OnMap2(corner.DiagonalDownRight)) // C
					{
						p1 = Pose.From(corner.Right, Direction.Down); d1 = Direction.Right;
						p2 = Pose.From(corner.Down, Direction.Right); d2 = Direction.Down;
					}
					else if (!OnMap2(corner.Down)) // D
					{
						p1 = Pose.From(corner.DiagonalDownRight, Direction.Left); d1 = Direction.Down;
						p2 = Pose.From(corner, Direction.Down); d2 = Direction.Left;
					}
					else
						throw new Exception();
					return new MapperPair
					{
						M1 = new Mapper
						{
							IsClockwise = false,
							Pose = p1,
							Dir = d1
						},
						M2 = new Mapper
						{
							IsClockwise = true,
							Pose = p2,
							Dir = d2
						}
					};
				})
				.ToArray();

			if (w > 100)
			{
				mappers[0].Length = 200;
				mappers[1].Length = 50;
				mappers[2].Length = 100;
			}
			else
			{
				mappers[0].Length = 8;
				mappers[1].Length = 12;
				mappers[2].Length = 8;
			}

			foreach (var mapper in mappers)
			{
				for (var i = 0; i < mapper.Length; i++)				
				{
					var m1copy = mapper.M1.Pose.Copy(); m1copy.TurnAround();
					var m2copy = mapper.M2.Pose.Copy(); m2copy.TurnAround();
					MoveIt(mapper.M1, m2copy);
					MoveIt(mapper.M2, m1copy);
				}
		//		DrawMappings();
			}

			bool MoveIt(Mapper m, Pose dest)
			{
				mappings[m.Pose.ToString()] = dest;
				m.Pose.Move(m.Dir);
				if (!OnMap2(m.Pose.Point))
				{
					if (m.IsClockwise) // for clockwise, always turn right
					{
						m.Pose.Move(m.Dir.TurnAround(), 1);
						m.Pose.TurnRight();
						m.Dir = m.Dir.TurnRight();
					}
					else // for counter-clockwise, always turn left
					{
						m.Pose.Move(m.Dir.TurnAround(), 1);
						m.Pose.TurnLeft();
						m.Dir = m.Dir.TurnLeft();
					}
				}
				return true;
			}

			// void DrawMappings()
			// {
			// 	map.ConsoleWrite((p, ch) =>
			// 	{
			// 		var ps = p.ToString();
			// 		var mapping = mappings.Keys.Where(k => k[..^1] == ps).ToArray();
			// 		return mapping.Length == 0
			// 			? ch
			// 			: mapping.Length == 1
			// 				? mapping[0][^1]
			// 				: (char)('0'+mapping.Length);
			// 	});
			// 	Console.WriteLine();
			// }


	//		DrawMappings();

			var pose = new Pose(Point.Origin, Direction.Right);
			while (map[pose.Point] != '.')
				pose.Move();
			
			//Draw();
			foreach (var turn in turns)
			{
				//Console.WriteLine(turn);
				if (turn == "R")
					pose.TurnRight();
				else if (turn == "L")
					pose.TurnLeft();
				else
				{
					var steps = int.Parse(turn);
					while (steps-- > 0)
					{
						var next = pose.Copy();
						if (mappings.TryGetValue(pose.ToString(), out var moveTo))
						{
							next = moveTo.Copy();
						}
						else
						{
							next.Move();
						}
						if (map[next.Point] == '#')
							break;
						pose = next;
					}
				}
			}


			// void Draw()
			// {
			// 	var lines = map.Render((p, ch) => p == pose.Point ? pose.Direction.AsChar() : ch).ToArray();
			// 	var from = Math.Max(0, pose.Point.Y - 2);
			// 	var to = Math.Min(h-1, pose.Point.Y + 2);
			// 	for(var y = from; y <= to; y++)
			// 	{
			// 		Console.WriteLine(lines[y]);
			// 	}
			// 	Console.WriteLine();
			// }

			// Facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)
			var facing = pose.Direction switch
			{
				Direction.Right => 0,	
				Direction.Down => 1,	
				Direction.Left => 2,	
				Direction.Up => 3,	
				_ => throw new Exception()
			};
			var password = (pose.Point.Y + 1) * 1000 + (pose.Point.X + 1) * 4 + facing;

			return password;
		}
		private class MapperPair
		{
			public int Length;
			public Mapper M1;
			public Mapper M2;
		}
		private class Mapper
		{
			public bool IsClockwise;
			public Pose Pose;
			public Direction Dir;
		}
	}

}
