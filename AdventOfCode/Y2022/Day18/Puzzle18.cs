using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day18
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Boiling Boulders";
		public override int Year => 2022;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").Part1(10);
			Run("test2").Part1(64).Part2(58);
			Run("test9").Part1(4308).Part2(2540);
			Run("input").Part1(3576).Part2(2066);
		}

		protected override long Part1(string[] input)
		{
			var cubes = new HashSet<Point3D>(input.Select(s => s.ToIntArray()).Select(c => new Point3D(c[0], c[1], c[2])));
			return CountSurfaces(cubes);
		}

		protected override long Part2(string[] input)
		{
			var cubes = new HashSet<Point3D>(input.Select(s => s.ToIntArray()).Select(c => new Point3D(c[0], c[1], c[2])));

			var surfaces = CountSurfaces(cubes);

			var holes = BuildInverse(cubes);
			ExtractCohesiveChunk(holes, holes.First(c => !cubes.Contains(c)));
			while (holes.Any())
			{
				var cavity = ExtractCohesiveChunk(holes, holes.First());
				surfaces -= CountSurfaces(cavity);
			}

			return surfaces;
		}


		private static int CountSurfaces(HashSet<Point3D> world)
		{
			// It may appeas as if we've counted the shared sides twice; once from each of the
			// cubes sharing the side. But that's in fact correct since each cube includes that
			// shared side in their side-count of 6 so 1 shared side does need to be subtracted
			// from each of them.
			var shared = 0;
			foreach (var p in world)
			{
				shared += p.Neighbors.Count(c => world.Contains(c));
			}
			return world.Count * 6 - shared;
		}

		private static HashSet<Point3D> ExtractCohesiveChunk(HashSet<Point3D> world, Point3D start)
		{
			var chunk = new HashSet<Point3D>();
			var removals = new Queue<Point3D>();
			removals.Enqueue(start);
			while (removals.TryDequeue(out var p))
			{
				if (chunk.Contains(p))
					continue;
				chunk.Add(p);
				world.Remove(p);
				foreach (var n in p.Neighbors.Where(c => world.Contains(c)))
				{
					removals.Enqueue(n);
				}
			}
			return chunk;
		}

		private static HashSet<Point3D> BuildInverse(HashSet<Point3D> cubes)
		{
			var (minx, miny, minz) = (int.MaxValue, int.MaxValue, int.MaxValue);
			var (maxx, maxy, maxz) = (int.MinValue, int.MinValue, int.MinValue);
			foreach (var c in cubes)
			{
				(minx, miny, minz) = (Math.Min(minx, c.X), Math.Min(miny, c.Y), Math.Min(minz, c.Z));
				(maxx, maxy, maxz) = (Math.Max(maxx, c.X), Math.Max(maxy, c.Y), Math.Max(maxz, c.Z));
			}
			
			var inverse = new HashSet<Point3D>();
			for (var x = minx-1; x <= maxx+1; x++)
			{
				for (var y = miny-1; y <= maxy+1; y++)
				{
					for (var z = minz-1; z <= maxz+1; z++)
					{
						var p = new Point3D(x, y, z);
						if (!cubes.Contains(p))
							inverse.Add(p);
					}
				}
			}
			return inverse;
		}
	}
}
