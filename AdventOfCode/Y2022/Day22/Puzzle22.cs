using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Monkey Map";
		public override int Year => 2022;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(6032).Part2(5031);
			Run("input").Part1(66292).Part2(127012);
			Run("extra").Part1(181128).Part2(52311);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input[0..^2], ' ');
			var turns = input[^1].Replace("R",",R,").Replace("L", ",L,").Split(',');

			var (w, h) = map.Size();

			var pose = new Pose(Point.Origin, Direction.Right);
			while (map[pose.Point] == ' ')
				pose.Move();
			
			foreach (var turn in turns)
			{
				if (turn == "R")
					pose.TurnRight();
				else if (turn == "L")
					pose.TurnLeft();
				else
				{
					var steps = int.Parse(turn);
					while (map[PeekAhead().Point] == '.' && steps-- > 0)
						pose = PeekAhead();
				}
			}

			Pose PeekAhead()
			{
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

			// Can only solve cubes with 3 inner corners, not 2 or 4
			//
			//           #            #   #
			//    yes:  ##      no:   #   #
			//          #            ###  #
			//         ##             #  ###

			var (w, h) = map.Size();
			bool OnMap(Point p) => map[p] == '.' || map[p] == '#';

			// var pose = new Pose(Point.Origin, Direction.Right);
			// while (map[pose.Point] != '.')
			// 	pose.Move();

			var start = Pose.From(Point.Origin, Direction.Right);
			while (map[start.Point] != '.')
				start.Move();

			// Find the 3 inner corners
			var cornerInfo = InnerCorners().Skip(1).Take(3).ToArray();
			IEnumerable<(Pose Corner, int Dist)> InnerCorners()
			{
				var p = start.Copy();
				var dist = 0;
				while (true)
				{
					if (OnMap(p.PeekAhead))
						p.Move();
					else
						p.TurnRight();
					dist++;
					if (OnMap(p.PeekLeft)) // just ahead of an inner corner
					{
						yield return (p.Copy(), dist);
						p.TurnLeft();
						p.Move();
						dist = 0;
					}
				}
			}

			// Create corner-mappers for the 3 inner corners
			var corners = cornerInfo
				.Select(c =>
				{
					// They all look like this, but in all kind of directions.
					// Here, > marks the corner's Pose.
					//
					//  #
					//  #
					//  A
					//  vB##
					//
					// The two tracer-poses, A and B, goes in each their direction:
					// A points "left" and moves "backwards", counter-clockwise
					// B points "back" and moves "left", clockwise
					var p = c.Corner;
					var corner = new Corner
					{
						Distance = c.Dist,
						M1 = new Corner.Mapper
						{
							IsClockwise = false,
							Pose = Pose.From(p.PeekBehind, p.Direction.TurnLeft()),
							Dir = p.Direction.TurnAround(),
						},
						M2 = new Corner.Mapper
						{
							IsClockwise = true,
							Pose = Pose.From(p.PeekLeft, p.Direction.TurnAround()),
							Dir = p.Direction.TurnLeft()
						}
					};					
					return corner;
				})
				.ToArray();

			// Determine the "width" of each mapper. The full span of mapped points going
			// out from each inner corner is the distance between it and its two neighbors
			// minus the distance between them; ie each mapper's length is (A+B-C)/2
			for (var i = 0; i < 3; i++)
			{
				var n1 = corners[i].Distance;
				var n2 = corners[(i+1)%3].Distance;
				var n3 = corners[(i+2)%3].Distance;
				corners[i].Length = (n1 + n2 - n3) / 2;
			}

			// Now build the edge-mappings. Move the mappers from their starting
			// points to create mappings from each edge-pose in both directions.
			var mappings = new Dictionary<string, Pose>();
			foreach (var mapper in corners)
			{
				for (var i = 0; i < mapper.Length; i++)				
				{
					var m1back = mapper.M1.Pose.Copy(); m1back.TurnAround();
					var m2back = mapper.M2.Pose.Copy(); m2back.TurnAround();
					AddMapping(mapper.M1, m2back);
					AddMapping(mapper.M2, m1back);
				}

				void AddMapping(Corner.Mapper m, Pose dest)
				{
					mappings[m.Pose.ToString()] = dest;
					m.Pose.Move(m.Dir);
					if (!OnMap(m.Pose.Point))
					{
						if (m.IsClockwise) // for clockwise, always turn right
						{
							m.Pose.Move(m.Dir.TurnAround());
							m.Pose.TurnRight();
							m.Dir = m.Dir.TurnRight();
						}
						else // for counter-clockwise, always turn left
						{
							m.Pose.Move(m.Dir.TurnAround());
							m.Pose.TurnLeft();
							m.Dir = m.Dir.TurnLeft();
						}
					}
				}
			}

			// Finally traverse the cube
			var pose = start;
			foreach (var turn in turns)
			{
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
							next = moveTo.Copy();
						else
							next.Move();
						if (map[next.Point] == '#')
							break;
						pose = next;
					}
				}
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

		private class Corner
		{
			public int Length;
			public Mapper M1;
			public Mapper M2;
			public int Distance;

			public class Mapper
			{
				public bool IsClockwise;
				public Pose Pose;
				public Direction Dir;
			}
		}
	}
}
