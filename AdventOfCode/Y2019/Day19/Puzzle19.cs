using System;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Y2019.Intcode;

namespace AdventOfCode.Y2019.Day19
{
	internal static class Puzzle19
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var points = Enumerable.Range(0, 50)
				.SelectMany(x => Enumerable.Range(0, 50).Select(y => Point.From(x, y)));
			var beam = new Beam();
			var beampoints = points.Count(p => beam.InBeam(p));
			Console.WriteLine($"Day 19 Puzzle 1: {beampoints}");
			Debug.Assert(beampoints == 141);
		}

		private static void Puzzle2()
		{
			var beam = new Beam();

			var left = Point.From(0, 10);
			while (!beam.InBeam(left))
			{
				left = left.Right;
			}
			var right = left;
			while (beam.InBeam(right.Right))
			{
				right = right.Right;
			}

			// Refine a number of times
			for (var refine = 0; refine < 4; refine++)
			{
				const int K = 10;
				(left, right) = FindBeamEdges(Point.From(left.X * K, left.Y * K), Point.From(right.X * K, right.Y * K));
			}

			//  +----------------------------------
			//  |--\                               
			//  | \ ----\ a2 (slope)                          
			//  |  \     ----\                     
			//  |   \         ----\                
			//  |    \      +-----+----\           
			//  |  a1 \     |     |p2   ----\      
			//  |      \    |     |          ---\  
			//  |       \   |     |              --
			//  |        \  |     |                
			//  |         \ |p1   |                
			//  |y         \+-----+                
			//  |           \                      
			//  |            \                     
			//  |             \
			//
			//     p1 + (N,-N) = p2
			// =>  a1*y + N = a2*(y-N)  (x-component only)
			// =>  a1*y + N = a2*y - a2*N
			// =>  a2*N + N = a2*y - a1*y
			// =>  N(1+a2) = (a2-a1)y
			// =>  y = N(1+a2)/(a2-a1)
			var N = 100;
			var a1 = (double)left.X / left.Y;
			var a2 = (double)right.X / right.Y;
			var y = N * (1 + a2) / (a2 - a1);
			var corner = Point.From((int)(y * a1), (int)y - N);

			// Don't bother nudging correctly; just spiral for a radius of 30 points
			corner = corner.SpiralFrom().Take(30*30).Last(IsWithinBeam);

			var position = corner.X * 10000 + corner.Y;
			Console.WriteLine($"Day 19 Puzzle 2: {position}");
			Debug.Assert(position == 15641348);

			bool IsWithinBeam(Point p)
			{
				var tr = Point.From(p.X + N - 1, p.Y); // tr: top right corner
				var bl = Point.From(p.X, p.Y + N - 1); // bl: bottom left corner
				return
					beam.InBeam(tr) && !beam.InBeam(tr.Up) && !beam.InBeam(tr.Right) &&
					beam.InBeam(bl) && !beam.InBeam(bl.Left) && !beam.InBeam(bl.Down);
			}

			(Point, Point) FindBeamEdges(Point p1, Point p2)
			{
				// Find left-most ray of beam
				while (!beam.InBeam(p1))
				{
					p1 = p1.Right;
				}
				while (p1.X > 0 && beam.InBeam(p1.Left))
				{
					p1 = p1.Left;
				}

				// Find right-most ray of beam
				while (beam.InBeam(p2.Right))
				{
					p2 = p2.Right;
				}
				while (p1.X > 0 && !beam.InBeam(p2))
				{
					p2 = p2.Left;
				}

				return (p1, p2);
			}
		}

	}

	internal class Beam
	{
		private static readonly Engine _engine = new Engine();
		private static readonly long[] _memory = Engine.ReadMemoryFromFile("Y2019/Day19/input.txt");
		private readonly CharMap _cache = new CharMap();

		public bool InBeam(Point pos)
		{
			if (_cache[pos] == 0)
			{
				_cache[pos] = _engine
					.WithMemory(_memory)
					.WithInput(pos.X, pos.Y)
					.Execute()
					.Output
					.Take() == 1 ? '#' : '.';
			}
			return _cache[pos] == '#';
		}

		public int Lookups => _cache.AllPoints().Count();
	}
}

