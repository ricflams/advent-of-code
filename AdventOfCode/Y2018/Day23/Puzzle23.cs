using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AdventOfCode.Y2018.Day23
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Experimental Emergency Teleportation";
		public override int Year => 2018;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(7);
			Run("test3").Part1(253).Part2(108618801);
			Run("input").Part1(613).Part2(101599540);
			Run("extra").Part1(906).Part2(121493971);
		}

		protected override int Part1(string[] input)
		{
			var bots = Nanobot.Parse(input);

			var biggest = bots.OrderByDescending(x => x.R).First();
			var nearby = bots.Count(b => biggest.ManhattanDistanceTo(b) <= biggest.R);
			return nearby;
		}

		protected override int Part2(string[] input)
		{
			var bots = Nanobot.Parse(input);

			// For each bot, find all the other bots that it overlaps with
			var N = bots.Length;
			var allOverlaps = Enumerable.Range(0, N).Select(_ => new bool[N]).ToList();
			for (var i = 0; i < N; i++)
			{
				for (var j = 0; j <= i; j++)
				{
					if (bots[i].OverlapsWith(bots[j]))
					{
						allOverlaps[i][j] = true;
						allOverlaps[j][i] = true;
					}
				}
			}

			// Find the greatest swarm of bots that all overlap. Treat all bots as
			// candidates to be part of that set and then go through each bot x's overlaps
			// one by one, removing from the swarm those bots that x doesn't overlap with
			// because if x doesn't overlap with it then it's not part of the swarm we seek.
			// For this puzzle's input this will quickly resolve to the greatest set.
			// Operate on int indexes instead of directly on the nanobots because it's faster.
			var candidates = new HashSet<int>(Enumerable.Range(0, N));
			var mostOverlapByIndex = allOverlaps
				.Select((overlap, idx) => (BotIndex: idx, Overlaps: overlap.Count(x => x)))
				.OrderByDescending(x => x.Overlaps)
				.Select(x => x.BotIndex);
			foreach (var boti in mostOverlapByIndex)
			{
				if (!candidates.Contains(boti))
					continue;
				var overlaps = allOverlaps[boti].Select((b, idx) => b ? idx : -1);
				candidates.IntersectWith(overlaps);
			}

			// Sanity-check the swarm: all bots must overlap and there can't be a greater swarm
			var swarm = candidates.Select(i => bots[i]).ToArray();
			Debug.Assert(swarm.All(a => swarm.All(b => a.OverlapsWith(b) && b.OverlapsWith(a))));
			Debug.Assert(swarm.Length > N / 2);

			// Find all close neighbor-bots, ie those bots that just barely overlap.
			// The real puzzle has plenty of those but the smaller tests doesn't. Oh well.
			var closeNeighbors = new List<(Nanobot A, Nanobot B, int Distance)>();
			for (var i = 0; i < swarm.Length; i++)
			{
				var a = swarm[i];
				for (var j = 0; j < i; j++)
				{
					var b = swarm[j];
					var overlap = a.Overlap(b);
					if (overlap <= 10)
					{
						closeNeighbors.Add((a, b, overlap));
					}
				}
			}

			// Loop through all those barely-overlapping bot-pairs and calculate the plane
			// that runs through their intersection, ie the plane where they overlap. Find
			// those planes (normal-vector and D) until we've got one in each "direction" so
			// they intersect in just one single point. Fill out a matrix for solving the
			// three planes' equations and find (x,y,z) of that point.
			// https://www.mathsisfun.com/algebra/systems-linear-equations-matrices.html
			var vals = Matrix.Build.Dense(3, 1);
			var coeff = Matrix.Build.Dense(3, 3);
			var normals = new HashSet<Point3D>();
			var row = 0;
			foreach (var (a, b, distance) in closeNeighbors.OrderBy(x => x.Distance))
			{
				var (normal, val) = a.IntersectingPlane(b);
				if (normals.Contains(normal))
					continue;
				normals.Add(normal);
				normals.Add(normal * -1);
				vals[row, 0] = val;
				coeff[row, 0] = normal.X;
				coeff[row, 1] = normal.Y;
				coeff[row, 2] = normal.Z;
				if (++row == 3)
					break;
			}

			// Find the intersection point, ip
			var xyz = coeff.Inverse() * vals;
			var ip = new Point3D((int)Math.Round(xyz[0, 0]), (int)Math.Round(xyz[1, 0]), (int)Math.Round(xyz[2, 0]));

			// The intersection point is likely not the precise closest-to-0 point, so
			// swirl around a bit looking for points that belong to all bots until we've
			// found that shortest-distance point. This takes only 10-20 rounds.
			var shortestDistance = int.MaxValue;
			var foundMax = 0;
			var seen = new HashSet<Point3D>();
			var queue = new Queue<Point3D>();
			queue.Enqueue(ip);
			while (queue.TryDequeue(out var p))
			{
				if (seen.Contains(p))
					continue;
				seen.Add(p);

				var found = swarm.Count(b => b.Contains(p));
				if (found < foundMax)
					continue;
				foundMax = found;

				if (found == swarm.Length)
				{
					var distance = p.ManhattanDistanceTo(Point3D.Origin);
					if (distance < shortestDistance)
						shortestDistance = distance;
				}

				for (var dx = -1; dx <= 1; dx++)
				{
					for (var dy = -1; dy <= 1; dy++)
					{
						for (var dz = -1; dz <= 1; dz++)
						{
							queue.Enqueue(new Point3D(p.X + dx, p.Y + dy, p.Z + dz));
						}
					}
				}
			}

			return shortestDistance;
		}


		private class Nanobot
		{
			public static Nanobot[] Parse(string[] input)
			{
				var bots = input
					.Select(s =>
					{
						var (x, y, z, r) = s.RxMatch("pos=<%d,%d,%d>, r=%d").Get<int, int, int, int>();
						return new Nanobot(x, y, z, r);
					})
					.ToArray();
				return bots;
			}

			internal Nanobot(int x, int y, int z, int r) => (O, R) = (new Point3D(x, y, z), r);

			public Point3D O;
			public int R;

			public int ManhattanDistanceTo(Nanobot o) => O.ManhattanDistanceTo(o.O);
			public int Overlap(Nanobot o) => R + o.R - (ManhattanDistanceTo(o) - 1);
			public bool OverlapsWith(Nanobot o) => Overlap(o) > 0;
			public bool Contains(Point3D p) => O.ManhattanDistanceTo(p) <= R;

			public override string ToString() => $"pos={O} r={R}";

			public (Point3D Normal, int D) IntersectingPlane(Nanobot bot)
			{
				Debug.Assert(OverlapsWith(bot));

				// Find mid point; it may be slightly off due to rounding but that's
				// okay for finding the relative position to this nanobot's origin
				var (x0, y0, z0) = (O.X, O.Y, O.Z);
				var f = (double)R / ManhattanDistanceTo(bot);
				var x = (int)Math.Round(x0 + (bot.O.X - x0) * f);
				var y = (int)Math.Round(y0 + (bot.O.Y - y0) * f);
				var z = (int)Math.Round(z0 + (bot.O.Z - z0) * f);

				// Find the normal vector and 
				//      |x-x0| + |y-y0| + |z-z0| = R
				// <=>  x+y+z = R + (x0+y0+z0), accounting for the sign of x-x0 etc
				var sx = Math.Sign(x - x0);
				var sy = Math.Sign(y - y0);
				var sz = Math.Sign(z - z0);
				return (new Point3D(sx, sy, sz), R + sx * x0 + sy * y0 + sz * z0);
			}
		}
	}
}
