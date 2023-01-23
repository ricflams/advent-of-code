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
			 //Run("test1").Part1(7);
			//Run("test2").Part2(36);
			Run("input").Part1(613).Part2(0);
			 // 102411278 to high
			 // 102411078
			 // 102410678
			 // 102409478
			 // 102407482
			 // 102405482 too high
			 // 102410482
			 // 101599534 too low


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

		private record Point3D(long X, long Y, long Z) : IGeogebraObject
		{
			public override string ToString() => $"<{X},{Y},{Z}>";
			public long ManhattanDistanceTo(Point3D o) => Math.Abs(X - o.X) + Math.Abs(Y - o.Y) + Math.Abs(Z - o.Z);
			public static readonly Point3D Origin = new Point3D(0, 0, 0);

			public IEnumerable<string> Command(Func<int> NextId)
			{
				yield return $"P{NextId()}=({X},{Y},{Z})";
			}

			public static Point3D operator +(Point3D lhs, Point3D rhs) {
				return new Point3D(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
			}

			public static Point3D operator -(Point3D lhs, Point3D rhs) {
				return new Point3D(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
			}

			public static Point3D operator *(Point3D lhs, double rhs) {
				return new Point3D((long)Math.Round(lhs.X * rhs), (long)Math.Round(lhs.Y * rhs), (long)Math.Round(lhs.Z * rhs));
			}

			public long Dot(Point3D rhs) {
				return checked (X * rhs.X + Y * rhs.Y + Z * rhs.Z);
			}

			public static Point3D IntersectPoint(Point3D rayVector, Point3D rayPoint, Point3D planeNormal, Point3D planePoint) {
				var diff = rayPoint - planePoint;
				var prod1 = diff.Dot(planeNormal);
				var prod2 = rayVector.Dot(planeNormal);
				if (prod2 == 0)
					return null;
				var prod3 = (double)prod1 / prod2;
				return rayPoint - rayVector * prod3;
			}			

		}

		private class Nanobot : IGeogebraObject
		{
			public enum Line
			{
				Zig, // For x-y = ...
				Zag  // For x+y = ...
			};

			public Nanobot(int x, int y, int z, int r, int idx)
			{
				(O, R, Index) = (new Point3D(x, y, z), r, idx);
				Corners = new Point3D[]
				{
 					O with { X = O.X+R },
					O with { Y = O.Y+R },
					O with { Z = O.Z+R },
					O with { X = O.X-R },
					O with { Y = O.Y-R },
					O with { Z = O.Z-R },
				};



				// Edges, seen as x,y from above with origin=x0,y0
				//      /\ 
				//   A /  \ B
				//    /  . \
				//    \    /
				//   D \  / C
				//      \/
				// 
				//  A: y = (x-x0) + y0    <=>  x-y = x0-y0  zig
				//  B: y = (x0-x) + y0    <=>  x+y = x0+y0  zag
				//  C: y = (x-x0) - y0    <=>  x-y = x0+y0  zig
				//  D: y = (x0-x) - y0    <=>  x+y = x0-y0  zag
				// Edges = new (Line,long)[]
				// {
				// 	(Line.Zig, x - y),
				// 	(Line.Zag, x + y),
				// 	(Line.Zig, x + y),
				// 	(Line.Zag, x - y),
				// };

				// Planes, seen as x,y from above
				//      /\ 
				//     /AB\
				//    /  . \
				//    \    /
				//     \DC/
				//      \/
				//
				// A: x0-x + y-y0 + z-z0 = R   <=>  x-y = -R +x0 -y0 -z0 +z   zig
				// B: x-x0 + y-y0 + z-z0 = R   <=>  x+y =  R +x0 +y0 +z0 -z   zag
				// C: x-x0 + y0-y + z-z0 = R   <=>  x-y =  R +x0 +y0 +z0 -z   zig
				// D: x0-x + y0-y + z-z0 = R   <=>  x+y = -R +x0 +y0 -z0 +z   zag
				// 
				// Downwards A-D: same, but reversed z,z0 sign
				// Planes = new (Line,long,int)[]
				// {
				// 	(Line.Zig, -R +x -y -z, 1),
				// 	(Line.Zag,  R +x +y +z, -1),
				// 	(Line.Zig,  R +x +y +z, -1),
				// 	(Line.Zag, -R +x +y -z, 1),
				// 	(Line.Zig, -R +x -y +z, -1),
				// 	(Line.Zag,  R +x +y -z, 1),
				// 	(Line.Zig,  R +x +y -z, 1),
				// 	(Line.Zag, -R +x +y +z, -1),
				// };
				var (a, b, c, d, top, bot) =
				(
					new Point3D(x-R, y  , z),
					new Point3D(x  , y+R, z),
					new Point3D(x+R, y  , z),
					new Point3D(x,   y-R, z),
					new Point3D(x,   y,   z+R),
					new Point3D(x,   y,   z-R)
				);
				Edges = new (Point3D A, Point3D B)[]
				{
					(a, b),
					(b, c),
					(c, d),
					(d, a),
					(a, top),
					(b, top),
					(c, top),
					(d, top),
					(a, bot),
					(b, bot),
					(c, bot),
					(d, bot),
				};
				Planes = new (Point3D Point, Point3D Normal)[]
				{
					(top, new Point3D(-1,  1, 1)),
					(top, new Point3D( 1,  1, 1)),
					(top, new Point3D( 1, -1, 1)),
					(top, new Point3D(-1, -1, 1)),
					(bot, new Point3D(-1,  1, -1)),
					(bot, new Point3D( 1,  1, -1)),
					(bot, new Point3D( 1, -1, -1)),
					(bot, new Point3D(-1, -1, -1)),
				};

			}
			public Point3D O;
			public long R;
			public int Index;
			public override string ToString() => $"pos={O}, r={R}";
			public long ManhattanDistanceTo(Nanobot o) => O.ManhattanDistanceTo(o.O);
			public bool OverlapsWith(Nanobot o) => Overlap(o) > 0; //ManhattanDistanceTo(o) <= R + o.R;
			public long Overlap(Nanobot o) => R + o.R - (ManhattanDistanceTo(o)-1);
			public bool Contains(Point3D p) => O.ManhattanDistanceTo(p) <= R;
			public long ManhattanDistanceTo(Point3D p) => O.ManhattanDistanceTo(p);

			public readonly Point3D[] Corners;
			//public readonly (Line Line,long)[] Edges;
			//public readonly (Line Line,long,int)[] Planes;

			public static HashSet<string> GotMore = new();

			public readonly (Point3D A, Point3D B)[] Edges;
			public readonly (Point3D Point, Point3D Normal)[] Planes;

			public IEnumerable<Point3D> Intersections(Nanobot o)
			{
				foreach (var edge in o.Edges)
				{
					foreach (var plane in Planes)
					{
						var edgeVector = edge.B - edge.A;
						var p = Point3D.IntersectPoint(edgeVector, edge.A, plane.Normal, plane.Point);
						if (p == null)
							continue;
						var found = false;
						if (Contains(p) && o.Contains(p))// && !Corners.Contains(p) && !o.Corners.Contains(p))
						{
							yield return p;
							found = true;
						}
						// var dist = 5;
						// var gotsome = false;
						// for (var dx = -dist; dx <= dist; dx += dist)
						// {
						// 	for (var dy = -dist; dy <= dist; dy += dist)
						// 	{
						// 		for (var dz = -dist; dz <= dist; dz += dist)
						// 		{
						// 			var next = new Point3D(p.X + dx, p.Y + dy, p.Z + dz);
						// 			if (Contains(next) && o.Contains(next))
						// 			{
						// 				gotsome = true;
						// 				yield return next;
						// 			}
						// 		}
						// 	}
						// }
						// if (!found && gotsome)
						// {
						// 	var (aa,bb) = Index<o.Index ? (this,o) : (o,this);
						// 	GotMore.Add($"{Index} and {o.Index}");
						// 	//Console.WriteLine($"Got some more at intersect between {Index} and {o.Index}");
						// }
					}
				}
			}

//			private EdgePlusFactors

			// Pyramid(Polygon((0,-1,0),(1,0,0),(0,1,0),(-1,0,0)),1)
			// Pyramid(Polygon((0,-1,0),(1,0,0),(0,1,0),(-1,0,0)),-1)
			public IEnumerable<string> Command(Func<int> NextId)
			{
				// https://wiki.geogebra.org/en/Naming_Objects
				yield return $"p{NextId()}=Pyramid(Polygon(({O.X+R},{O.Y},{O.Z}),({O.X},{O.Y+R},{O.Z}),({O.X-R},{O.Y},{O.Z}),({O.X},{O.Y-R},{O.Z})),{R})";
				yield return $"p{NextId()}=Pyramid(Polygon(({O.X+R},{O.Y},{O.Z}),({O.X},{O.Y+R},{O.Z}),({O.X-R},{O.Y},{O.Z}),({O.X},{O.Y-R},{O.Z})),{-R})";
				// foreach (var p in Planes)
				// {
				// 	var pt = p.Point;
				// 	var end = p.Point + p.Normal * R;
				// 	yield return $"v{NextId()}=Vector(({pt.X},{pt.Y},{pt.Z}),({end.X},{end.Y},{end.Z}))";
				// }
				// foreach (var p in Edges)
				// {
				// 	yield return $"v{NextId()}=Vector(({p.A.X},{p.A.Y},{p.A.Z}),({p.B.X},{p.B.Y},{p.B.Z}))";
				// }
			}
		}

		private interface IGeogebraObject
		{
			IEnumerable<string> Command(Func<int> NextId);
		}

		private class Polygon : IGeogebraObject
		{
			public List<Point3D> _points = new();
			public IEnumerable<string> Command(Func<int> NextId)
			{
				var pts = string.Join(",", _points.Select(p => $"({p.X},{p.Y},{p.Z})"));
				yield return $"p{NextId()}=Plane(Polygon({pts}))";
			}
		}

		private class Visualize3D
		{
			private int _index;
			private readonly List<IGeogebraObject> _objects = new();
			public void Add(IGeogebraObject obj) => _objects.Add(obj);

			// public void AddPoint(Point3D p) => _commands.Add($"P{_index++}={p.AsGeogebra}");
			// public void AddPyramid(Point3D a, Point3D b, Point3D c, Point3D d, int h) =>
			// 	_commands.Add($"v{_index++}=Pyramid(Polygon({a.AsGeogebra},({b.AsGeogebra}),({c.AsGeogebra}),({d.AsGeogebra})),{h})");

			internal int NextId() => _index++;

			public string AsExecuteCommands()
			{
				var rawobjs = _objects.SelectMany(o => o.Command(NextId)).Select(c => '"' + c + '"');
				var objs = string.Join(",", rawobjs);
				var exe = $"Execute({{{objs}}})";
				return exe;
			}

			public void Print()
			{
				Console.WriteLine(AsExecuteCommands());
			}
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


			var set = bots.ToList();
			var N = set.Count;
			var overlaps = new List<bool[]>();
			for (var i = 0; i < N; i++)
			{
				overlaps.Add(new bool[N]);
				for (var j = 0; j < N; j++)
				{
					if (set[i].OverlapsWith(set[j]))
					{
						overlaps[i][j] = true;
					}
				}
			}



			var sortedoverlaps = overlaps
				.Select((o, idx) => (idx, o.Count(x => x)))
				.OrderByDescending(x => x.Item2)
				.ToArray();
			var chosen = new HashSet<int>(bots.Select(x=>x.Index));
			var firstindex = sortedoverlaps.First().idx;
			var bot432index = sortedoverlaps.Single(x => x.idx==432);
			foreach (var botidx in sortedoverlaps)
			{
				var (idx, n) = botidx;
				if (!chosen.Contains(idx))
					continue;
				var chosen2 = new HashSet<int>(overlaps[idx].Select((b,idx) => (b,idx)).Where(x => x.b && chosen.Contains(x.idx)).Select(x => x.idx));
				chosen = new HashSet<int>(chosen.Intersect(chosen2));
			}

			var span = chosen.Select(i => bots[i]).ToArray();

			// var overlapcount = 0;
			// for (var i = 0; i < N; i++)
			// {
			// 	if (sortedoverlaps[i].Item2 < i)
			// 		break;
			// 	overlapcount++;
			// }
			// Console.WriteLine(sortedoverlaps.Count(x=>x.Item2==981));
			// Console.WriteLine(sortedoverlaps.Count(x=>x.Item2==982));
			// Console.WriteLine(sortedoverlaps.Count(x=>x.Item2==983));
			// Console.WriteLine(sortedoverlaps.Count(x=>x.Item2==984));
			// Console.WriteLine(sortedoverlaps.Count(x=>x.Item2==985));
			// var maxoverlaps = 981;//sortedoverlaps.Max(x => x.Item2);
			// var span = sortedoverlaps.Where(x => x.Item2 == maxoverlaps).Select(x => bots[x.idx]).ToArray();


			//var span = FindGreatestSet(bots);
;

            // var rv = new Point3D(0, -1, -1);
            // var rp = new Point3D(0, 0, 10);
            // var pn = new Point3D(0, 0, 1);
            // var pp = new Point3D(0, 0, 5);
            // var ip = Point3D.IntersectPoint(rv, rp, pn, pp);
            // Console.WriteLine("The ray intersects the plane at {0}", ip);			


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

			Debug.Assert(span.All(a => span.All(b => a.OverlapsWith(b) && b.OverlapsWith(a))));
			var troubles = 0;
			foreach (var b in bots.Where(b => !span.Contains(b)))
			{
				if (span.All(s => s.OverlapsWith(b)))
				{
					Console.WriteLine($"trouble at {b.Index}");
					troubles++;
				}
			}

			var close0 = new List<(Nanobot A, Nanobot B, long Dist)>();
			var minOverlap = long.MaxValue;
			for (var i = 0; i < span.Length; i++)
			{
				var bota = span[i];
				for (var j = 0; j < i; j++)
				{
					var botb = span[j];
					var overlap0 = bota.Overlap(botb);
					if (overlap0 <= minOverlap)
					{
						minOverlap = overlap0;
						close0.Add((bota, botb, overlap0));
					}
				}
			}
			var close = close0.Where(x=>x.Dist<=5).OrderBy(x => x.Dist).ThenBy(x => Math.Min(x.A.R, x.B.R)).ToList();
			foreach (var x in close)
			{
				Console.WriteLine();
				Console.WriteLine($"{x.A.Index} vs {x.B.Index} dist={x.Dist}");
				var geox = new Visualize3D();
				geox.Add(x.A);
				geox.Add(x.B);
				geox.Print();
			}
	
			var geoz = new Visualize3D();
			geoz.Add(bots[218]);
			geoz.Add(bots[152]);
			geoz.Add(bots[432]);
			geoz.Add(bots[975]);
			geoz.Print();

			var b218 = bots[218];
			for (var dx = 0; dx < b218.R; dx += 1000)
			{
				var p = new Point3D(b218.O.X + dx, b218.O.Y, b218.O.X - dx);
				var incl = span.Count(s => s.Contains(p));
				if (incl == span.Length)
				{
					Console.WriteLine($"bingo! at {p}");
					var dist = p.ManhattanDistanceTo(Point3D.Origin);
					return dist;
				}				
				Console.Write($"{incl} ");
			}


			foreach (var x in close.TakeWhile(x=>x.Dist<=5))
			{
				//if (x.Dist==1)
				{
					geoz.Add(x.A);
					geoz.Add(x.B);
					var i1 = x.A.Intersections(x.B).Concat(x.B.Intersections(x.A)).Distinct().ToArray();
					if (i1.Length < 3)
						continue;
					var pol = new Polygon();
					foreach (var p in i1)
						pol._points.Add(p);
					pol._points.Add(i1.First());
					geoz.Add(pol);
					Console.WriteLine(i1.Length);
				}
			}
			geoz.Print();

			var span2 = close.Where(x => x.Dist==1).ToArray();
			var candidates = new List<Point3D>();

			var vis3 = new Visualize3D();
			vis3.Add(bots[432]);
			foreach (var x in span2)
			{
				if (x.A.Index != 432)
					vis3.Add(x.A);
				if (x.B.Index != 432)
					vis3.Add(x.B);
			}
			vis3.Print();

			var bot432 = bots[432];
			var dt = 1;
			for (var dz = 27635000; dz < bot432.R; dz += dt)
			{
				var maxincl = 0;
				var xy = bot432.R-dz;
				for (var dx = 0; dx < xy; dx += dt)
				{
					var dy = xy-dx;
					Debug.Assert(dx+dy+dz == bot432.R);
					var p = new Point3D(bot432.O.X - dx, bot432.O.Y - dy, bot432.O.Z + dz);
					Debug.Assert(bot432.Contains(p));
					var incl = span.Count(s => s.Contains(p));
					if (incl == span.Length)
					{
						Console.WriteLine($"bingo! at {p}");
						var dist = p.ManhattanDistanceTo(Point3D.Origin);
						return dist;
					}
					if (incl > maxincl)
						maxincl = incl;
				}
				Console.Write($"{maxincl} ");
			}
			var dist0 = bot432.O.ManhattanDistanceTo(Point3D.Origin) - bot432.R;
			return dist0;

			foreach (var x in span2)
			{
				// if (b.Index!=683)
				// 	continue;						
				Console.WriteLine();
				Console.WriteLine(x.A.Index);
				var geo2 = new Visualize3D();
				geo2.Add(x.A);
				geo2.Add(x.B);
				foreach (var p in x.A.Intersections(x.B))
				{
					geo2.Add(p);
					if (span.All(s => s.Contains(p)))
						candidates.Add(p);
				}
				foreach (var p in x.B.Intersections(x.A))
				{
					geo2.Add(p);
					if (span.All(s => s.Contains(p)))
						candidates.Add(p);
				}
				geo2.Print();
			}

			foreach (var s in span)
			{
				foreach (var c in s.Corners)
					candidates.Add(c);
			}

			var gotsome = Nanobot.GotMore.OrderBy(x=>x).ToArray();

			var dedup = candidates.Distinct().ToArray();
			var most = dedup.Select(p => (p, span.Count(s => s.Contains(p)))).OrderByDescending(x=>x.Item2).ToArray();
			var cand2 = dedup.Where(p => span.All(s => s.Contains(p))).ToArray();
			Point3D p0 = null;
			if (cand2.Any())
			{
				Console.WriteLine("bingo!");
				var dists2 = cand2.Select(c => (c, c.ManhattanDistanceTo(Point3D.Origin))).OrderBy(x=>x.Item2);
				Console.WriteLine(dists2.First());
				p0 = dists2.First().c;
			}

			// var p1 = most.First().p;
			// var not1 = span.First(s => !s.Contains(p1));
			// var dist1 = p1.ManhattanDistanceTo(not1.O)-not1.R;
			// var geo = new Visualize3D();
			// geo.Add(span.Single(s => s.Index==12));
			// geo.Add(not1);
			// geo.Add(p1);
			// geo.Print();

;
			// var includesOrigin = span.Count(s => s.Contains(new Point3D(0,0,0)));
			// var aboveOrigin = span.Select(s => (s, s.ManhattanDistanceTo(Point3D.Origin) - s.R)).OrderByDescending(x => x.Item2).ToArray();
			// var (bfar, bdist) = aboveOrigin.First();
			// var b1b2x = (double)(bdist - bfar.R) / bdist;
			// var p0 = new Point3D(
			// 	(long)(bfar.O.X*b1b2x),
			// 	(long)(bfar.O.Y*b1b2x),
			// 	(long)(bfar.O.Z*b1b2x)
			// 	// X: (b1.X + b
			// 	// Y: (b1.Y + b2.Y)/2,
			// 	// Z: (b1.Z + b2.Z)/2
			// );
			// var incount = span.Count(s => s.Contains(p0));

			// // Execute[{"A=Sphere((413982,33018475,42272511),81533371)","B=Sphere((62049508,14737551,109848523),89608487)"}]
			// var spheres = span.Select((a,idx) => $"\"Bot{idx}=Sphere(({a.O.X},{a.O.Y},{a.O.Z}),{a.R})\"");
			// var s = $"Execute[{{{string.Join(",", spheres)}}}]";
			// Console.WriteLine(s);

			// // foreach (var a in span)
			// // 	Console.WriteLine($"Sphere(({a.O.X},{a.O.Y},{a.O.Z}),{a.R})");

			// var cornerDist = span
			// 	.SelectMany(s => s.Corners)
			// 	.Select(s => (s, span.Count(b => b.Contains(s))))
			// 	.OrderByDescending(x => x.Item2)
			// 	.ToArray();
			// // foreach (var x in cornerDist)
			// // 	Console.WriteLine($"{x.s} : {x.Item2}");
			// // foreach (var s in span)
			// // 	s.WriteAsPyramid();



			// // var corner219 = new Point3D(82886322,-4450974,37731510);
			// // var botw219 = span.Single(s => s.Corners.Contains(corner219));
			// // var misses = span.Where(s => !s.Contains(corner219)).ToArray();
			// // botw219.WriteAsPyramid();
			// // foreach (var miss in misses)
			// // 	miss.WriteAsPyramid();





			// // // var shortest = new List<(Nanobot A, Nanobot B, int cas, long shortest, long otherdist)>();
			// // // for (var i = 0; i < N; i++)
			// // // {
			// // // 	var bota = span[i];
			// // // 	for (var j = 0; j < i; j++)
			// // // 	{
			// // // 		var botb = span[j];
			// // // 		var radii = bota.R+botb.R;
			// // // 		shortest.Add((bota, botb, 1, Math.Abs(bota.O.Y-botb.O.Y)+Math.Abs(bota.O.Z-botb.O.Z), radii - Math.Abs(bota.O.X-botb.O.X)));
			// // // 		shortest.Add((bota, botb, 2, Math.Abs(bota.O.X-botb.O.X)+Math.Abs(bota.O.Z-botb.O.Z), radii - Math.Abs(bota.O.Y-botb.O.Y)));
			// // // 		shortest.Add((bota, botb, 3, Math.Abs(bota.O.X-botb.O.X)+Math.Abs(bota.O.Y-botb.O.Y), radii - Math.Abs(bota.O.Z-botb.O.Z)));
			// // // 	}
			// // // }
			// // // var shortest2 = shortest
			// // // 	.OrderBy(x => x.shortest)
			// // // 	.ThenBy(x => x.otherdist)
			// // // 	.ToArray();
			// // // var shortest3 = shortest
			// // // 	.OrderBy(x => x.shortest+x.otherdist)
			// // // 	.ToArray();


			// // if (close.Count > 18)
			// // {
			// // 	close.RemoveAt(18);
			// // 	close.RemoveAt(10);
			// // }

			// var smalldist = close.First().Dist;
			// var closest = close.Where(x => x.Dist == smalldist).ToList();


			// var middles = closest
			// 	.Select(s =>
			// 	{
			// 		var (b1, b2, dist) = s;
			// 		var b1b2 = (double)b1.R / (b1.ManhattanDistanceTo(b2));
			// 		return new Point3D(
			// 			(long)(b1.O.X+(b2.O.X-b1.O.X)*b1b2),
			// 			(long)(b1.O.Y+(b2.O.Y-b1.O.Y)*b1b2),
			// 			(long)(b1.O.Z+(b2.O.Z-b1.O.Z)*b1b2)
			// 			// X: (b1.X + b2.X)/2,
			// 			// Y: (b1.Y + b2.Y)/2,
			// 			// Z: (b1.Z + b2.Z)/2
			// 			);
			// 	})
			// 	.ToArray();

			// // for (var i = 0; i < closest.Count; i++)
			// // {
			// // 	Debug.Assert(closest[i].A.Contains(middles[i]));
			// // 	Debug.Assert(closest[i].B.Contains(middles[i]));
			// // }
			
			// // foreach (var m1 in middles)
			// // 	foreach (var m2 in middles)
			// // 		Console.WriteLine($"  mhdist={m1.ManhattanDistanceTo(m2)}");

			// // var bot21 = span.Single(s=>s.Index==21);
			// // var bot270 = span.Single(s=>s.Index==270);
			// // Debug.Assert(span.All(s => s.OverlapsWith(bot21)));
			// // Debug.Assert(span.All(s => s.OverlapsWith(bot270)));

			// var p0 = middles.First();
			// // var dists0 = span.Select(s => (s.Index, s.R - s.ManhattanDistanceTo(p0))).OrderByDescending(x => x.Item2).ToArray();
			// // var dist21 = bot21.ManhattanDistanceTo(p0);
			// // var overlap21 = dist21 - bot21.R;


			// // {
			// // 	// var xb1b2 = (double)xb1.R / (xb1.R+xb2.R);
			// // 	// var xp0 = (
			// // 	// 	X: (int)(xb1.X+(xb2.X-xb1.X)*xb1b2),
			// // 	// 	Y: (int)(xb1.Y+(xb2.Y-xb1.Y)*xb1b2),
			// // 	// 	Z: (int)(xb1.Z+(xb2.Z-xb1.Z)*xb1b2)
			// // 	// 	// X: (b1.X + b2.X)/2,
			// // 	// 	// Y: (b1.Y + b2.Y)/2,
			// // 	// 	// Z: (b1.Z + b2.Z)/2
			// // 	// 	);
			// // 	var xd = p.ManhattanDistanceTo(p0.X, p0.Y, p0.Z)-p.R;
			// // 	Console.WriteLine($"d={xd}");
			// // }

			// // span = span.Where(s=>s.Index!=21 && s.Index!=270).ToArray();

			// var inAll = new List<long>();
			// var inAllP = new List<Point3D>();

			// span = new [] { closest.First().A, closest.First().B };

			// // span[0].WriteAsPyramid();
			// span[1].WriteAsPyramid();

			var queue = Quack<Point3D>.Create(QuackType.PriorityQueue);
			queue.Put(p0, 0);
			//var seen = new HashSet<Point3D>();
			var mindist = (long)int.MaxValue;
			var seen = new HashSet<string>();
			var round = 0;
			while (queue.TryGet(out var p))
			{
				//var (p, n) = item;
	//			Console.WriteLine($"round={round} distp={p.ManhattanDistanceTo(p0)}");

				// if (seen.Contains(p))
				// 	continue;
				// seen.Add(p);
				if (seen.Contains(p.ToString()))
					continue;
				seen.Add(p.ToString());
				

				// if (++round % 10000 == 0)
				// {
				// 	// var dists = span.Select(s => (s.Index, s.ManhattanDistanceTo(p) - s.R)).OrderByDescending(x => x.Item2).Take(10).ToArray();
				// 	Console.WriteLine($"round={round} q={queue.Count} p={p} mindist={mindist}");
				// }

				//if (n == span.Length)
				{
					var d = p.ManhattanDistanceTo(Point3D.Origin);
					if (d >= mindist)
						continue;
					mindist = d;	
					Console.WriteLine($"round={round} q={queue.Count} p={p} mindist={mindist}");
					// inAll.Add(d);
					// inAllP.Add(p);
				}

				for (var dx = -1; dx <= 1; dx++)
				{
					for (var dy = -1; dy <= 1; dy++)
					{
						for (var dz = -1; dz <= 1; dz++)
						{
							var next = new Point3D(p.X + dx, p.Y + dy, p.Z + dz);
							if (!span.All(b => b.Contains(next)))
								continue;
							var xdist = next.ManhattanDistanceTo(Point3D.Origin);																
							if (xdist >= mindist)
								continue;
							if (seen.Contains(next.ToString()))
								continue;
							queue.Put(next, (int)xdist);
						}
					}
				}
			}

			// inAllP = inAllP.Distinct().ToList();
			// foreach (var p in inAllP)
			// {
			// 	Debug.Assert(span.All(s => s.Contains(p)));
			// }

			// var closestDist = inAll.Min();
			return mindist;


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
