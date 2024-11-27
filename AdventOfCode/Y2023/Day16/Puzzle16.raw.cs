using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using Microsoft.VisualBasic;

namespace AdventOfCode.Y2023.Day16.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 16;

		public override void Run()
		{
			Run("test1").Part1(46).Part2(51);
		//	Run("test2").Part1(0).Part2(0);
			Run("input").Part1(7562).Part2(0);
		//	Run("extra").Part1(0).Part2(0);
		}

		
		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var p0 = Pose.From(0, 0, Direction.Right);
			return Energize(map, p0);
		}

		private static int Energize(CharMap map, Pose p0)
		{
			var seen = new HashSet<string>();

			var queue = new Queue<Pose>();
			queue.Enqueue(p0.Copy());
			var (w, h) = map.Size();
			while (queue.TryDequeue(out var p))
			{
				// if (p.Point.X < 0)
				// 	p.MoveRight(w);
				// else if (p.Point.X == w)
				// 	p.MoveLeft(w);
				// else if (p.Point.Y < 0)
				// 	p.MoveDown(h);
				// else if (p.Point.Y == h)
				// 	p.MoveUp(h);

				if (p.Point.X < 0 || p.Point.X == w || p.Point.Y < 0 || p.Point.Y == h)
					continue;

				if (seen.Contains(p.ToString()))
					continue;
				seen.Add(p.ToString());

				if (map[p.Point] == '-')
				{
					if (p.Direction is Direction.Up or Direction.Down)
					{
						// split
						var p1 = p.Copy(); p1.TurnLeft(); p1.Move();
						var p2 = p.Copy(); p2.TurnRight(); p2.Move();
						queue.Enqueue(p1);
						queue.Enqueue(p2);
					}
					else
					{
						p.Move();
						queue.Enqueue(p.Copy());						
					}
				}
				else if (map[p.Point] == '|')
				{
					if (p.Direction is Direction.Right or Direction.Left)
					{
						// split
						var p1 = p.Copy(); p1.TurnLeft(); p1.Move();
						var p2 = p.Copy(); p2.TurnRight(); p2.Move();
						queue.Enqueue(p1);
						queue.Enqueue(p2);
					}
					else
					{
						p.Move();
						queue.Enqueue(p.Copy());						
					}
				}
				else if (map[p.Point] == '/')
				{
					if (p.Direction is Direction.Up or Direction.Down)
						p.TurnRight();
					else
						p.TurnLeft();
					p.Move();
					queue.Enqueue(p.Copy());
				}
				else if (map[p.Point] == '\\')
				{
					if (p.Direction is Direction.Up or Direction.Down)
						p.TurnLeft();
					else
						p.TurnRight();
					p.Move();
					queue.Enqueue(p.Copy());
				}
				else
				{
					p.Move();
					queue.Enqueue(p.Copy());
				}
			}

			var e = seen.Select(x => x[..^1]).Distinct().Count();

			return e;			
		}


		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (w, h) = map.Size();

			var maxE = 0;
			for (var x = 0; x < w; x++)
			{
				var e1 = Energize(map, Pose.From(x, 0, Direction.Down));
				var e2 = Energize(map, Pose.From(x, h-1, Direction.Up));
				if (e1 > maxE)
					maxE = e1;
				if (e2 > maxE)
					maxE = e2;
			}
			for (var y = 0; y < h; y++)
			{
				var e1 = Energize(map, Pose.From(0, y, Direction.Right));
				var e2 = Energize(map, Pose.From(w-1, y, Direction.Left));
				if (e1 > maxE)
					maxE = e1;
				if (e2 > maxE)
					maxE = e2;
			}
			return maxE;
		}
	}
}
