using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Experimental Emergency Teleportation";
		public override int Year => 2018;
		public override int Day => 23;

		public void Run()
		{
			// Run("test1").Part1(7);
	//		Run("test2").Part2(36);
			Run("input").Part1(613).Part2(0);
			 // 102411278 to high
			 // 102411078
			 // 102410678
			 // 102409478
			 // 102407482
			 // 102405482 too high
			 // 102410482

			 // https://www.geogebra.org/3d
		}

		// #
		// ##
		// ###
		// ####
		// #####
		// ######
		// #######
		// ########
		// #########
		// ##########
		// ###########
		// ############
		// #############
		// ##############
		// ###############
		// ################
		// #################
		// ##################
		// ###################
		// ####################
		// #####################
		// ######################
		// #######################
		// ########################
		// --##---##---------------#
		// ########################-#
		// ########################-##
		// ########################-###
		// ########################-####

		private record Point3D(long X, long Y, long Z)
		{
			public override string ToString() => $"<{X},{Y},{Z}>";
			public long ManhattanDistanceTo(Point3D o) => Math.Abs(X - o.X) + Math.Abs(Y - o.Y) + Math.Abs(Z - o.Z);
			public static readonly Point3D Origin = new Point3D(0, 0, 0);
		}

		private class Nanobot
		{
			public Nanobot(int x, int y, int z, int r, int idx) => (O, R, Index) = (new Point3D(x, y, z), r, idx);
			public Point3D O;
			public long R;
			public int Index;
			public override string ToString() => $"pos={O}, r={R}";
			public long ManhattanDistanceTo(Nanobot o) => O.ManhattanDistanceTo(o.O);
			public bool OverlapsWith(Nanobot o) => Overlap(o) > 0; //ManhattanDistanceTo(o) <= R + o.R;
			public long Overlap(Nanobot o) => R + o.R - (ManhattanDistanceTo(o)-3);
			public bool Contains(Point3D p) => O.ManhattanDistanceTo(p) <= R;
			public long ManhattanDistanceTo(Point3D p) => O.ManhattanDistanceTo(p);
		}

		private class Node : GraphxNode
		{
			public Nanobot Bot;
			public override string ToString() => Bot.ToString();
		}

		protected override long Part1(string[] input)
		{
			var bots = input
				.Select((s,idx) =>
				{
					var (x, y, z, r) = s.RxMatch("pos=<%d,%d,%d>, r=%d").Get<int, int, int, int>();
					return new Nanobot(x, y, z, r, idx);
				})
				.ToArray();

			var strongest = bots.OrderByDescending(x => x.R).First();

			var near = bots
				.Count(b => strongest.ManhattanDistanceTo(b) <= strongest.R);
			return near;
		}

		protected override long Part2(string[] input)
		{
			var bots = input
				.Select((s,idx) =>
				{
					var (x, y, z, r) = s.RxMatch("pos=<%d,%d,%d>, r=%d").Get<int, int, int, int>();
					return new Nanobot(x, y, z, r, idx);
				})
				.ToArray();

			var span = FindGreatestSet(bots);


			static Nanobot[] FindGreatestSet(Nanobot[] bots)
			{
				var set = bots.ToList();
				while (true)
				{
					var N = set.Count;

					var overlaps = new List<bool[]>();

					for (var i = 0; i < N; i++)
					{
						overlaps.Add(new bool[i+1]);
						for (var j = 0; j < i+1; j++)
						{
							if (set[i].OverlapsWith(set[j]))
							{
								overlaps[i][j] = true;
							}
						}
					}

					var span = 0;
					while (span < N && overlaps[span].All(x=>x))
						span++;

					if (span == N)
					{
						// for (var i = 0; i < N; i++)
						// {
						// 	for (var j = 0; j < i+1; j++)
						// 	{
						// 		Console.Write(overlaps[i][j] ? '#' : '-');
						// 	}
						// 	Console.WriteLine();
						// }
						break;
					}

					for (var i = N; i-- > span; )
					{
						if (overlaps[i].TakeWhile(x=>x).Count() <= span)
							set.RemoveAt(i);
					}
				}
				return set.ToArray();
			}


			var includesOrigin = span.Count(s => s.Contains(new Point3D(0,0,0)));
			var aboveOrigin = span.Select(s => (s, s.ManhattanDistanceTo(Point3D.Origin) - s.R)).OrderByDescending(x => x.Item2).ToArray();
			var (bfar, bdist) = aboveOrigin.First();
			var b1b2x = (double)(bdist - bfar.R) / bdist;
			var p0x = new Point3D(
				(long)(bfar.O.X*b1b2x),
				(long)(bfar.O.Y*b1b2x),
				(long)(bfar.O.Z*b1b2x)
				// X: (b1.X + b
				// Y: (b1.Y + b2.Y)/2,
				// Z: (b1.Z + b2.Z)/2
			);
			var incount = span.Count(s => s.Contains(p0x));

			// Execute[{"A=Sphere((413982,33018475,42272511),81533371)","B=Sphere((62049508,14737551,109848523),89608487)"}]
			var spheres = span.Select((a,idx) => $"\"Bot{idx}=Sphere(({a.O.X},{a.O.Y},{a.O.Z}),{a.R})\"");
			var s = $"Execute[{{{string.Join(",", spheres)}}}]";
			Console.WriteLine(s);

			// foreach (var a in span)
			// 	Console.WriteLine($"Sphere(({a.O.X},{a.O.Y},{a.O.Z}),{a.R})");

			var N = span.Length;
			var close0 = new List<(Nanobot A, Nanobot B, long Dist)>();
			var minOverlap = long.MaxValue;
			for (var i = 0; i < N; i++)
			{
				var bota = span[i];
				for (var j = 0; j < i; j++)
				{
					var botb = span[j];
					if (bota.ManhattanDistanceTo(botb)+1 == bota.R+botb.R)
						;
					var overlap0 = bota.Overlap(botb);
					if (overlap0 <= minOverlap)
					{
						minOverlap = overlap0;
						close0.Add((bota, botb, overlap0));
					}
				}
			}

			var close = close0.OrderBy(x => x.Dist).ThenBy(x => Math.Min(x.A.R, x.B.R)).ToList();



			var shortest = new List<(Nanobot A, Nanobot B, int cas, long shortest, long otherdist)>();
			for (var i = 0; i < N; i++)
			{
				var bota = span[i];
				for (var j = 0; j < i; j++)
				{
					var botb = span[j];
					var radii = bota.R+botb.R;
					shortest.Add((bota, botb, 1, Math.Abs(bota.O.Y-botb.O.Y)+Math.Abs(bota.O.Z-botb.O.Z), radii - Math.Abs(bota.O.X-botb.O.X)));
					shortest.Add((bota, botb, 2, Math.Abs(bota.O.X-botb.O.X)+Math.Abs(bota.O.Z-botb.O.Z), radii - Math.Abs(bota.O.Y-botb.O.Y)));
					shortest.Add((bota, botb, 3, Math.Abs(bota.O.X-botb.O.X)+Math.Abs(bota.O.Y-botb.O.Y), radii - Math.Abs(bota.O.Z-botb.O.Z)));
				}
			}
			var shortest2 = shortest
				.OrderBy(x => x.shortest)
				.ThenBy(x => x.otherdist)
				.ToArray();
			var shortest3 = shortest
				.OrderBy(x => x.shortest+x.otherdist)
				.ToArray();


			// if (close.Count > 18)
			// {
			// 	close.RemoveAt(18);
			// 	close.RemoveAt(10);
			// }

			var smalldist = close.First().Dist;
			var closest = close.Where(x => x.Dist == smalldist).ToList();


			var middles = closest
				.Select(s =>
				{
					var (b1, b2, dist) = s;
					var b1b2 = (double)b1.R / (b1.R+b2.R);
					return new Point3D(
						(long)(b1.O.X+(b2.O.X-b1.O.X)*b1b2),
						(long)(b1.O.Y+(b2.O.Y-b1.O.Y)*b1b2),
						(long)(b1.O.Z+(b2.O.Z-b1.O.Z)*b1b2)
						// X: (b1.X + b2.X)/2,
						// Y: (b1.Y + b2.Y)/2,
						// Z: (b1.Z + b2.Z)/2
						);
				})
				.ToArray();

			// for (var i = 0; i < closest.Count; i++)
			// {
			// 	Debug.Assert(closest[i].A.Contains(middles[i]));
			// 	Debug.Assert(closest[i].B.Contains(middles[i]));
			// }
			
			// foreach (var m1 in middles)
			// 	foreach (var m2 in middles)
			// 		Console.WriteLine($"  mhdist={m1.ManhattanDistanceTo(m2)}");

			var bot21 = span.Single(s=>s.Index==21);
			var bot270 = span.Single(s=>s.Index==270);
			Debug.Assert(span.All(s => s.OverlapsWith(bot21)));
			Debug.Assert(span.All(s => s.OverlapsWith(bot270)));

			var p0 = middles.First();
			var dists0 = span.Select(s => (s.Index, s.R - s.ManhattanDistanceTo(p0))).OrderByDescending(x => x.Item2).ToArray();
			var dist21 = bot21.ManhattanDistanceTo(p0);
			var overlap21 = dist21 - bot21.R;


			// {
			// 	// var xb1b2 = (double)xb1.R / (xb1.R+xb2.R);
			// 	// var xp0 = (
			// 	// 	X: (int)(xb1.X+(xb2.X-xb1.X)*xb1b2),
			// 	// 	Y: (int)(xb1.Y+(xb2.Y-xb1.Y)*xb1b2),
			// 	// 	Z: (int)(xb1.Z+(xb2.Z-xb1.Z)*xb1b2)
			// 	// 	// X: (b1.X + b2.X)/2,
			// 	// 	// Y: (b1.Y + b2.Y)/2,
			// 	// 	// Z: (b1.Z + b2.Z)/2
			// 	// 	);
			// 	var xd = p.ManhattanDistanceTo(p0.X, p0.Y, p0.Z)-p.R;
			// 	Console.WriteLine($"d={xd}");
			// }

			span = span.Where(s=>s.Index!=21 && s.Index!=270).ToArray();

			var inAll = new List<long>();
			var inAllP = new List<Point3D>();

			span = new [] { closest.First().A, closest.First().B };

			var queue = Quack<(Point3D, int)>.Create(QuackType.Stack);
			var containedIn0 = span.Count(b => b.Contains(p0));
			queue.Put((p0, containedIn0));
			var seen = new HashSet<Point3D>();
			var round = 0;
			while (queue.TryGet(out var item))
			{
				var (p, n) = item;

				if (seen.Contains(p))
					continue;
				seen.Add(p);

				if (++round % 10000 == 0)
				{
					var dists = span.Select(s => (s.Index, s.ManhattanDistanceTo(p) - s.R)).OrderByDescending(x => x.Item2).Take(10).ToArray();
					Console.WriteLine($"round={round} q={queue.Count} n={n} p={p} farthest={string.Join(" ", dists)}");
				}

				if (n == span.Length)
				{
					inAll.Add(Math.Abs(p.X) + Math.Abs(p.Y) + Math.Abs(p.Z));
					inAllP.Add(p);
				}

				for (var dx = -1; dx <= 1; dx++)
				{
					for (var dy = -1; dy <= 1; dy++)
					{
						for (var dz = -1; dz <= 1; dz++)
						{
							var next = new Point3D(p.X + dx, p.Y + dy, p.Z + dz);
							var n2 = span.Count(b => b.Contains(next));
							if (n2 < n)
								continue;
							if (seen.Contains(next))
								continue;								
							queue.Put((next, n2));
						}
					}
				}
			}

			inAllP = inAllP.Distinct().ToList();
			foreach (var p in inAllP)
			{
				Debug.Assert(span.All(s => s.Contains(p)));
			}

			var closestDist = inAll.Min();
			return closestDist;


			//var inAll = new List<long>();
			// //for (var d = 0; d <= dist+10; d++)
			// var dt1 = smalldist - 10;
			// var dt2 = smalldist + 10;
			// {
			// 	for (var x = p0.X-dt1; x <= p0.X+dt2; x++)
			// 	{
			// 		for (var y = p0.Y-dt1; y <= p0.Y+dt2; y++)
			// 		{
			// 			for (var z = p0.Z-dt1; z <= p0.Z+dt2; z++)
			// 			{
			// 				// var m1 = b1.ManhattanDistanceTo(x, y, z)-b1.R;
			// 				// var m2 = b2.ManhattanDistanceTo(x, y, z)-b2.R;
			// 			//	Console.WriteLine($"da={m1} db={m2}");
			// 				//if (close.All(pair => pair.A.Contains(x, y, z) && pair.B.Contains(x, y, z)))
			// 				var p = new Point3D(x, y, z);
			// 				if (span.All(b => b.Contains(p)))
			// 				{
			// 					inAll.Add(Math.Abs(x) + Math.Abs(y) + Math.Abs(z));
			// 				}
			// 			}
			// 		}
			// 	}
			// }



;


//			var group = overlaps.Where(x => x.Overlaps.Take(x.Index+1).All(x=>x)).OrderBy(x => x.Index).ToArray();


			;
			// var dimx = FindOverlaps(b => b.X);
			// var dimy = FindOverlaps(b => b.Y);
			// var dimz = FindOverlaps(b => b.Z);

			// (int BotIndex, int Count, long Hash)[] FindOverlaps(Func<Nanobot, int> dim)
			// {
			// 	var overlaps = new List<(int BotIndex, int Count, long Hash)>();
			// 	for (var i = 0; i < bots.Length; i++)
			// 	{
			// 		var b1 = bots[i];
			// 		var p1 = dim(b1);

			// 		var hash = 0L;
			// 		var count = 0;
			// 		foreach (var b2 in bots)
			// 		{
			// 			var p2 = dim(b2);
			// 			var start1 = p1 - b1.R;
			// 			var end1 = p1 + b1.R;
			// 			var start2 = p2 - b2.R;
			// 			var end2 = p2 + b2.R;
			// 			if (start1 <= end2 && start2 <= end1)
			// 			{
			// 				hash += b2.Index;
			// 				hash *= 3074457345618258799L;
			// 				count++;
			// 			}
			// 		}
			// 		overlaps.Add((b1.Index, count, hash));
			// 	}
			// 	return overlaps.OrderByDescending(x => x.Count).ToArray();
			// }

			// var maxes = new HashSet<long>(dimx.Select(x => x.Hash));
			// maxes.IntersectWith(dimy.Select(x => x.Hash));
			// maxes.IntersectWith(dimz.Select(x => x.Hash));
			
			// var maxcounts = maxes
			// 	.Where(h => dimx.Any(x=>x.Hash==h) && dimy.Any(x=>x.Hash==h) && dimz.Any(x=>x.Hash==h))
			// 	.Select(h => (Hash:h, Nx: dimx.Where(x=>x.Hash==h).Max(x=>x.Count), Ny: dimy.Where(x=>x.Hash==h).Max(x=>x.Count), Nz: dimz.Where(x=>x.Hash==h).Max(x=>x.Count)))
			// 	.OrderByDescending(x => x.Nx+x.Ny+x.Nz)
			// 	.ToArray();


			return span.Length;
		}
	}
}
