using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System.Linq;

namespace AdventOfCode.Y2019.Day19
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Tractor Beam";
		public override int Year => 2019;
		public override int Day => 19;

		public override void Run()
		{
			Run("input").Part1(141).Part2(15641348);
		}

		protected override int Part1(string[] input)
		{
			var intcode = input[0];
			var points = Enumerable.Range(0, 50)
				.SelectMany(x => Enumerable.Range(0, 50).Select(y => Point.From(x, y)));
			var beam = new Beam(intcode);
			var beampoints = points.Count(p => beam.InBeam(p));
			return beampoints;
		}

		protected override int Part2(string[] input)
		{
			var intcode = input[0];
			var beam = new Beam(intcode);

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
			corner = corner.SpiralFrom().Take(30 * 30).Last(IsWithinBeam);

			var position = corner.X * 10000 + corner.Y;
			return position;

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
		private readonly Engine _engine = new Engine();
		private readonly long[] _memory;
		private readonly CharMap _cache = new CharMap();

		public Beam(string intcode)
		{
			_memory = Engine.ReadAsMemory(intcode);
		}

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

