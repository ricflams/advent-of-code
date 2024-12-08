using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Data;

namespace AdventOfCode.Y2023.Day10.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 10;

		public override void Run()
		{
		//	Run("test1").Part1(8);
			Run("test2").Part2(4);
			Run("test3").Part2(8);
			Run("test4").Part2(10);			
			Run("input").Part1(6875).Part2(471);
			// 470 too low

			Run("extra").Part1(7030).Part2(285);
			// 281 not correct
			// 291 too high
			// 286 not right
		}

        //   v^<
        //   < >
        //   >v^	

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var p0 = map.AllPointsWhere(ch => ch  == 'S').First();

			var steps = 0;
			var p = FirstStep(p0);
			do
			{
				steps++;
				NextStep(p);
			} while (p.Point != p0);

			return (int)Math.Ceiling(steps / 2.0);

			// JFJ
			// 7S7
			// |.L
			Tracer FirstStep(Point p)
			{
				// F7
				// LJ
				if (map[p.Right] is '-' or '7' or 'J')
					return new Tracer(p, Direction.Right);
				if (map[p.Down] is '|' or 'J' or 'L')
					return new Tracer(p, Direction.Down);
				if (map[p.Left] is '-' or 'L' or 'F')
					return new Tracer(p, Direction.Left);
				if (map[p.Up] is '|' or 'F' or '7')
					return new Tracer(p, Direction.Left);
				throw new Exception($"Can't move from {p}");
			}

			void NextStep(Tracer p)
			{
				var peek = map[p.PeekAhead];
				p.Move();
				switch (peek)
				{
					case '7': p.TurnWhenFacing(Direction.Right); break;
					case 'J': p.TurnWhenFacing(Direction.Down); break;
					case 'L': p.TurnWhenFacing(Direction.Left); break;
					case 'F': p.TurnWhenFacing(Direction.Up); break;
				}
			}

		}

		private class Tracer(Point p, Direction d) : Pose(p, d)
		{
			public int RightTurns = 0;

			// public new static Tracer From(Point p, Direction d) => new Tracer(p, d);

			public void TurnWhenFacing(Direction dir)
			{
				if (Direction == dir)
				{
					TurnRight();
					RightTurns++;
				}
				else
				{
					TurnLeft();
					RightTurns--;
				}
			}
		}



		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var p0 = map.AllPointsWhere(ch => ch  == 'S').First();

			var path = new List<Pose>();

			var p1 = FirstStep(p0);
			do
			{
				path.Add(p1.Copy());
				NextStep(p1);
			} while (p1.Point != p0);

			Console.WriteLine($"Tracer {p1} RightTurns={p1.RightTurns}");

			var isClockwise = p1.RightTurns > 0;

			var pathPoints = new HashSet<Point>(path.Select(x => x.Point));
			var inner = new HashSet<Point>();
			// var outer  = new HashSet<Point>();

			// var phole = Point.From(48, 69);

			foreach (var p in path)
			{
				
				// var dx = Math.Abs(phole.X - p.Point.X);
				// var dy = Math.Abs(phole.Y - p.Point.Y);
				// if (dx <= 1 && dy <= 1)
				// {
				// 	Console.WriteLine("#");
				// }

				var innerp = isClockwise ? p.PeekRight : p.PeekLeft;
				var outerp = !isClockwise ? p.PeekRight : p.PeekLeft;
				if (!pathPoints.Contains(innerp))
					inner.Add(innerp);
				// if (!pathPoints.Contains(outerp))
				// 	outer.Add(outerp);
				var nextp = p.Copy();
				nextp.Move();
				var innerp2 = isClockwise ? nextp.PeekRight : nextp.PeekLeft;					
				if (!pathPoints.Contains(innerp2))
					inner.Add(innerp2);				
				// var outerp2 = !isClockwise ? nextp.PeekRight : nextp.PeekLeft;					
				// if (!pathPoints.Contains(outerp2))
				// 	outer.Add(outerp2);								
			}

			foreach (var p in inner.ToArray())
			{
				Fill(p);
			}

			void Fill(Point p)
			{
				var fills = p.LookDiagonallyAround().Where(x => !inner.Contains(x) && !pathPoints.Contains(x)).ToArray();
				foreach (var f in fills)
					inner.Add(f);
				foreach (var f in fills)
					Fill(f);
			}

			Console.WriteLine($"icClockwise={isClockwise} inner:{inner.Count}");

			var map2 = new CharMap(' ');
			foreach (var p in path)
			{
				map2[p.Point] = p.Direction.AsChar();
			}
			foreach (var p in inner)
			{
				map2[p] = 'I';
			}
		// 	foreach (var p in outer)
		// 	{
		// 		map2[p] = 'X';
		// 	}

		// 	var map3 = map2.ResetToOrigin();
		// //	map3.ConsoleWrite();

		// 	var (min, max) = map2.MinMax();
		// 	map2[phole] = '?';

			// var missing = new HashSet<Point>();
			// var (w, h) = map2.Size();
			// for (var y = 0; y < h; y++)
			// {
			// 	var spacelook = false;
			// 	for (var x = 0; x < w; x++)
			// 	{
			// 		var p = Point.From(x, y);
			// 		if (y == 97 && x == 96)
			// 			;
			// 		if (map2[x, y] != ' ')
			// 			spacelook = true;
			// 		if (spacelook && !pathPoints.Contains(p))
			// 		{
			// 			// if (map2[x+1, y] == ' ')
			// 			// 	spacelook = false;
			// 			// else
			// 			if (p.LookDiagonallyAround().All(x => pathPoints.Contains(p)))
			// 				missing.Add(p);
			// 		}
			// 	}
			// }
			// foreach (var p in missing)
			// {
			// //	map2[p] = '#';
			// }
			// foreach (var p in map.AllPoints().Where(p => !pathPoints.Contains(p)))
			// {
			// 	if (p.LookDiagonallyAround().Count(x => map[p] != ' ') == 8)
			// 	{
			// 		missing.Add(p);
			// 		map2[p] = '#';
			// 	}
			// }

//			Console.WriteLine($"missing:{missing.Count}");
		//	map2.ConsoleWrite();


			var innercount = inner.Count();

			return innercount;

			// JFJ
			// 7S7
			// |.L
			Tracer FirstStep(Point p)
			{
				// F7
				// LJ
				if (map[p.Right] is '-' or '7' or 'J')
					return new Tracer(p, Direction.Right);
				if (map[p.Down] is '|' or 'J' or 'L')
					return new Tracer(p, Direction.Down);
				if (map[p.Left] is '-' or 'L' or 'F')
					return new Tracer(p, Direction.Left);
				if (map[p.Up] is '|' or 'F' or '7')
					return new Tracer(p, Direction.Left);
				throw new Exception($"Can't move from {p}");
			}

			void NextStep(Tracer p)
			{
				var peek = map[p.PeekAhead];
				p.Move();
				switch (peek)
				{
					case '7': p.TurnWhenFacing(Direction.Right); break;
					case 'J': p.TurnWhenFacing(Direction.Down); break;
					case 'L': p.TurnWhenFacing(Direction.Left); break;
					case 'F': p.TurnWhenFacing(Direction.Up); break;
				}
			}
		}
	}
}
