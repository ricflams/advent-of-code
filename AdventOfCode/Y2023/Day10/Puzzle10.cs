using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Data;

namespace AdventOfCode.Y2023.Day10
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Pipe Maze";
		public override int Year => 2023;
		public override int Day => 10;

		public override void Run()
		{
			Run("test1").Part1(8);
			Run("test2").Part2(4);
			Run("test3").Part2(8);
			Run("test4").Part2(10);			
			Run("input").Part1(6875).Part2(471);
			Run("extra").Part1(7030).Part2(285);
		}

		protected override long Part1(string[] input)
		{
			var tiles = new Tiles(input);

			// Init the tracer in some direction and then just follow the
			// trail until we reach the start again. The max distance ishalf
			// of those steps, rounded up if uneven.
			var steps = 0;
			var p = tiles.InitTracer(tiles.Start);
			do
			{
				steps++;
				p.Move();
				tiles.MaybeTurn(p);
			} while (p.Point != tiles.Start);

			return (int)Math.Ceiling(steps / 2.0);
		}



		protected override long Part2(string[] input)
		{
			var tiles = new Tiles(input);

			// Follow the trail and remember it. Unlike in part 1 we'll
			// add turns to the trail as separate steps because otherwise
			// we may miss out on checking the left/right of the trail for
			// inner tiles (see next).
			var trail = new List<Pose>();
			var tracer = tiles.InitTracer(tiles.Start);
			do
			{
				tracer.Move();
				trail.Add(tracer.Copy());
				if (tiles.MaybeTurn(tracer))
					trail.Add(tracer.Copy());
			} while (tracer.Point != tiles.Start);

			// If tracer went round clockwise (cw) then the inner tiles are
			// to the right of the trail; else if counter-clockwise (ccw) the
			// inner tiles are to the left of the trail.
			// Put the trail's points into a hashmap for quick lookup.
			// Then follow the trail and produce a set of inner tiles from
			// looking right/left at those that are "empty", ie not part of
			// the trail itself.
			var isClockwise = tracer.IsClockwise;
			var path = new HashSet<Point>(trail.Select(x => x.Point));
			var inner = new HashSet<Point>(
				trail
					.Select(p => isClockwise ? p.PeekRight : p.PeekLeft)
					.Where(p => !path.Contains(p))
				);

			// Flood-fill inner tiles because we've only reconnoitered the rim
			foreach (var p in inner.ToArray())
			{
				Fill(p);

				void Fill(Point p)
				{
					inner.Add(p);
					var neighbours = p.LookDiagonallyAround()
						.Where(x => !inner.Contains(x) && !path.Contains(x));
					foreach (var f in neighbours)
						Fill(f);
				}				
			}

			// Now we know all inner tiles
			return inner.Count;
		}

		private class Tiles : CharMap
		{
			public Tiles(string[] input) : base(input)
			{
				Start = AllPointsWhere(ch => ch  == 'S').First();
			}

			public Point Start { get; private set; }

			public class Tracer(Point p, Direction d) : Pose(p, d)
			{
				private int _rightTurns = 0;
				private int _leftTurns = 0;

				// If we've made more right turns that left turns, then
				// we've gone around clockwise; else counter-clockwise.
				public bool IsClockwise => _rightTurns > _leftTurns;

				public void TurnRightIfFacing(Direction dir)
				{
					if (Direction == dir)
					{
						TurnRight();
						_rightTurns++;
					}
					else
					{
						TurnLeft();
						_leftTurns++;
					}
				}
			}

			public Tracer InitTracer(Point p)
			{
				// F7
				// LJ
				// Find the initial direction for the tracer at this symbol
				if (this[p.Right] is '-' or '7' or 'J')
					return new Tracer(p, Direction.Right);
				if (this[p.Down] is '|' or 'J' or 'L')
					return new Tracer(p, Direction.Down);
				if (this[p.Left] is '-' or 'L' or 'F')
					return new Tracer(p, Direction.Left);
				if (this[p.Up] is '|' or 'F' or '7')
					return new Tracer(p, Direction.Left);
				throw new Exception($"Can't move from {p}");
			}

			public bool MaybeTurn(Tracer p)
			{
				// Tracer should turn when it's on a corner. The direction of
				// the turn (left or right) depends on the tracer's direction.
				switch (this[p.Point])
				{
					case '7': p.TurnRightIfFacing(Direction.Right); return true;
					case 'J': p.TurnRightIfFacing(Direction.Down); return true;
					case 'L': p.TurnRightIfFacing(Direction.Left); return true;
					case 'F': p.TurnRightIfFacing(Direction.Up); return true;
				}
				return false;
			}			
		}

	}
}
