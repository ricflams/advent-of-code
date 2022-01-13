using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 19";
		public override int Year => 2021;
		public override int Day => 19;

		public void Run()
		{
			//Run("test1").Part1(0).Part2(0);

			//Run("test2").Part1(79).Part2(3621);
			Run("test2").Part2(3621);

			//Run("input").Part1(403).Part2(0);
			Run("input").Part2(10569);

			// todo: clean
		}

		internal class Scanner
		{
			public Scanner(Point3D[] beacons, int number) => (Beacons, AlignedBeacons, Number) = (beacons, beacons, number);

			public int Number { get; }
			public Point3D[] Beacons { get; private set; }
			public Point3D[] AlignedBeacons { get; private set; }
			public Point3D Position { get; private set; } = new Point3D(0, 0, 0);

			public bool Match12beacons(Scanner other)
			{
				foreach (var rot in BeaconsRot())
				{
					//Console.WriteLine();
					//foreach (var p in rot)
					//{
					//	Console.WriteLine(p);
					//}
					//var rotBeacons = new HashSet<string>(rot.Select(b => b.ToString()));

					var (overlaps, moved, diff) = FindMaxOverlaps(rot, other.AlignedBeacons);
					if (overlaps >= 12)
					{
						Console.WriteLine($"  Found {Number} at offset {diff} with {overlaps} overlaps");
						AlignedBeacons = moved;
						Position = diff;
						return true;
					}
				}
				return false;
			}

			public IEnumerable<Point3D[]> BeaconsRot()
			{
				var rotPoints = Beacons.Select(b => b.Rotate().ToArray()).ToArray();
				for (var i = 0; i < 24; i++)
				{
					var beacons = rotPoints.Select(rp => rp[i]).ToArray();
					yield return beacons;
				}
			}

			private Point3D[] SortBeacons(Point3D[] b)
			{
				return b.OrderBy(b => b.X).ThenBy(b => b.Y).ThenBy(b => b.Z).ToArray();
			}

			private static (int, Point3D[], Point3D) FindMaxOverlaps(Point3D[] a, Point3D[] b)
			{
				var bs = new HashSet<string>(b.Select(b => b.ToString()));
				foreach (var aa in a)
				{
					foreach (var bb in b)
					{
						var diff = aa.Diff(bb);
						//var diff = new Point3D(1105, -1205, 1229);

						var shifted = new HashSet<string>(a.Select(p => p.Offset(diff).ToString()));
						var overlaps = shifted.Intersect(bs).Count();

						//if (diff.X == 1105 && diff.Y == -1205)
						//	Console.WriteLine($"      diff={diff}");

						if (overlaps >= 3)
						{
							//Console.WriteLine($"    (overlaps={overlaps})");
						}

						//if (overlaps > maxoverlaps)
						if (overlaps >= 12)
						{
							var aligned = a.Select(p => p.Offset(diff)).ToArray();
							return (overlaps, aligned, diff);
						}
					}
				}
				return (0, null, null);
			}

		}
		internal class Point3D
		{
			public Point3D(int x, int y, int z) => (X, Y, Z) = (x, y, z);
			public int X;
			public int Y;
			public int Z;
			public override string ToString() => $"({X},{Y},{Z})";

			public Point3D Diff(Point3D other) => new Point3D(other.X - X, other.Y - Y, other.Z - Z);
			public Point3D Offset(Point3D offset) => new Point3D(X + offset.X, Y + offset.Y, Z + offset.Z);

			public IEnumerable<Point3D> Rotate()
			{
				foreach (var p in new Point3D(Y, Z, X).RotateUp())
					yield return p;
				foreach (var p in new Point3D(-Y, Z, -X).RotateUp())
					yield return p;
				foreach (var p in new Point3D(Z, X, Y).RotateUp())
					yield return p;
				foreach (var p in new Point3D(-Z, X, -Y).RotateUp())
					yield return p;
				foreach (var p in new Point3D(X, Y, Z).RotateUp())
					yield return p;
				foreach (var p in new Point3D(-X, Y, -Z).RotateUp())
					yield return p;
			}

			public IEnumerable<Point3D> RotateUp()
			{
				yield return new Point3D(X, Y, Z);
				yield return new Point3D(-Y, X, Z);
				yield return new Point3D(-X, -Y, Z);
				yield return new Point3D(Y, -X, Z);
			}
		}

		protected override long Part1(string[] input)
		{
			var scanners = input
				.GroupByEmptyLine()
				.Select(lines =>
				{
					return lines.Skip(1)
						.Select(s =>
						{
							var ss = s.ToIntArray();
							var (x, y, z) = (ss[0], ss[1], ss[2]);
							return new Point3D(x, y, z);
						})
						.ToArray();
				})
				.Select((x,idx) => new Scanner(x, idx))
				.ToList();

			var wellknown = new List<Scanner>();
			wellknown.Add(scanners.First());
			scanners.RemoveAt(0);

			////var lastfound = wellknown.First();
			//while (scanners.Any())
			//{
			//	Console.WriteLine($"wellknown={wellknown.Count} scanners={scanners.Count}");
			//	//var match = scanners.Count == 1
			//	//	? scanners.First()
			//	//	: scanners.First(sc => wellknown.Any(x => sc.Match12beacons(x)));
			//	var match = scanners.First(sc => wellknown.Any(x => sc.Match12beacons(x)));
			//	wellknown.Add(match);
			//	scanners.Remove(match);
			////	lastfound = match;
			//}


			
			while (scanners.Any())
			{
				Console.WriteLine($"wellknown={wellknown.Count} scanners={scanners.Count}");
				//var match = scanners.Count == 1
				//	? scanners.First()
				//	: scanners.First(sc => wellknown.Any(x => sc.Match12beacons(x)));

				var matches = scanners.Where(sc => wellknown.Any(x => sc.Match12beacons(x))).ToArray();
				Console.WriteLine($"matches: {string.Join(" ", matches.Select(m=>m.Number))}");

				if (!matches.Any())
					throw new Exception();

				foreach (var m in matches)
				{
					wellknown.Add(m);
					scanners.Remove(m);
				}
			}

			var seen = new HashSet<string>();
			foreach (var sc in wellknown)
			{
				foreach (var b in sc.AlignedBeacons)
				{
					seen.Add(b.ToString());
				}
			}
			var n = seen.Count;

			return n;
		}

		protected override long Part2(string[] input)
		{

			var scanners = input
				.GroupByEmptyLine()
				.Select(lines =>
				{
					return lines.Skip(1)
						.Select(s =>
						{
							var ss = s.ToIntArray();
							var (x, y, z) = (ss[0], ss[1], ss[2]);
							return new Point3D(x, y, z);
						})
						.ToArray();
				})
				.Select((x, idx) => new Scanner(x, idx))
				.ToList();

			var wellknown = new List<Scanner>();
			wellknown.Add(scanners.First());
			scanners.RemoveAt(0);

			////var lastfound = wellknown.First();
			//while (scanners.Any())
			//{
			//	Console.WriteLine($"wellknown={wellknown.Count} scanners={scanners.Count}");
			//	//var match = scanners.Count == 1
			//	//	? scanners.First()
			//	//	: scanners.First(sc => wellknown.Any(x => sc.Match12beacons(x)));
			//	var match = scanners.First(sc => wellknown.Any(x => sc.Match12beacons(x)));
			//	wellknown.Add(match);
			//	scanners.Remove(match);
			////	lastfound = match;
			//}



			while (scanners.Any())
			{
				Console.WriteLine($"wellknown={wellknown.Count} scanners={scanners.Count}");
				//var match = scanners.Count == 1
				//	? scanners.First()
				//	: scanners.First(sc => wellknown.Any(x => sc.Match12beacons(x)));

				var matches = scanners.Where(sc => wellknown.Any(x => sc.Match12beacons(x))).ToArray();
				Console.WriteLine($"matches: {string.Join(" ", matches.Select(m => m.Number))}");

				if (!matches.Any())
					throw new Exception();

				foreach (var m in matches)
				{
					wellknown.Add(m);
					scanners.Remove(m);
				}
			}

			var maxdist = 0;
			foreach (var a in wellknown)
			{
				foreach (var b in wellknown)
				{

					var dist = a.Position.Diff(b.Position);
					var mandist = Math.Abs(dist.X) + Math.Abs(dist.Y) + Math.Abs(dist.Z);
					if (mandist > maxdist)
						maxdist = mandist;
				}
			}

			return maxdist;

		}
	}
}
