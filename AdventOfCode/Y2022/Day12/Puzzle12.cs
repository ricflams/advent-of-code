using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day12
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Hill Climbing Algorithm";
		public override int Year => 2022;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(31).Part2(29);
			Run("input").Part1(484).Part2(478);
			Run("extra").Part1(408).Part2(399);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var start = map.PositionsOf('S').Single();
			var dest = map.PositionsOf('E').Single();
			map.Set(start, 'a');
			map.Set(dest, 'z');

			var (w, h) = map.Dim();
			var seen = new int[w, h];
			var queue = new PriorityQueue<(Point,int), int>();
			queue.Enqueue((start, 0), 0);
			while (queue.TryDequeue(out var item, out var _))
			{
				var (pos, step) = item;
				var (x, y) = (pos.X, pos.Y);

				// Found the destination spot
				if (pos == dest)
					return step;

				// Don't revisit spots
				var shortest = seen[x, y];
				if (shortest > 0 && shortest <= step)
					continue;
				seen[x, y] = step;

				var val = map[x, y];
				foreach (var p in pos.LookAround().Where(map.InRange))
				{
					var v = map.Get(p);
					if (v <= val + 1) // Go max 1 higher
					{
						var dist = p.ManhattanDistanceTo(dest);
						queue.Enqueue((p, step + 1), step + 1 + dist);
					}
				}
			}
			throw new Exception("No path found");
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var start = map.PositionsOf('S').Single();
			var dest = map.PositionsOf('E').Single();
			map.Set(start, 'a');
			map.Set(dest, 'z');

			var (w, h) = map.Dim();
			var seen = new int[w, h];
			var queue = new PriorityQueue<(Point,int), int>();
			queue.Enqueue((dest, 0), 0);
			while (queue.TryDequeue(out var item, out var _))
			{
				var (pos, step) = item;
				var (x, y) = (pos.X, pos.Y);

				// Stop once we find any 'a'
				var val = map[x, y];
				if (val == 'a')
					return step;

				// Don't revisit spots
				var shortest = seen[x, y];
				if (shortest > 0 && shortest <= step)
					continue;
				seen[x, y] = step;

				foreach (var p in pos.LookAround().Where(map.InRange))
				{
					var v = map.Get(p);
					if (v >= val - 1) // Go max 1 lower
					{
						queue.Enqueue((p, step + 1), step + 1);
					}
				}
			}
			throw new Exception("No path found");
		}
	}
}
