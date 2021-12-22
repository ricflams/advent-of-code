using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 22";
		public override int Year => 2021;
		public override int Day => 22;

		public void Run()
		{
			Run("test1").Part1(39).Part2(39);

			//Run("test2").Part1(590784).Part2(0);
			Run("test2.5").Part1(590784).Part2(590784);
						Run("test3").Part1(0590784).Part2(2758514936282235);

			//			Run("input").Part1(596989).Part2(0);

		}

		protected override long Part1(string[] input)
		{
			var space = new bool[101,101,101];
			foreach (var s in input)
			{
				// on x=10..12,y=10..12,z=10..12
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var xx1 = Math.Max(x1, -50);
				var xx2 = Math.Min(x2, 50);
				var yy1 = Math.Max(y1, -50);
				var yy2 = Math.Min(y2, 50);
				var zz1 = Math.Max(z1, -50);
				var zz2 = Math.Min(z2, 50);
				var on = set == "on";
				for (var x = xx1; x <= xx2; x++)
				{
					for (var y = yy1; y <= yy2; y++)
					{
						for (var z = zz1; z <= zz2; z++)
						{
							space[x + 50, y + 50, z + 50] = on;
						}
					}
				}
			}

			var n = 0;
			foreach (var b in space)
			{
				if (b) n++;
			}

			return n;
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
		}

		internal class Cube
		{
			public Cube(int x1, int x2, int y1, int y2, int z1, int z2)
			{
				Top = new Point3D(x1, y1, z1);
				Bot = new Point3D(x2, y2, z2);
				Corners = new Point3D[]
				{
					new Point3D(x1, y1, z1),
					new Point3D(x1, y1, z2),
					new Point3D(x1, y2, z1),
					new Point3D(x1, y2, z2),
					new Point3D(x2, y1, z1),
					new Point3D(x2, y1, z2),
					new Point3D(x2, y2, z1),
					new Point3D(x2, y2, z2)
				};
			}
			public readonly Point3D Top;
			public readonly Point3D Bot;
			public readonly Point3D[] Corners;
			public List<Cube> Holes = new List<Cube>();
			public long Size => (long)(Bot.X - Top.X + 1) * (Bot.Y - Top.Y + 1) * (Bot.Z - Top.Z + 1);

			public override string ToString() => $"size={Size} #holes={Holes.Count}";

			public long CountDots()
			{
				var dots = Size - Holes.Select(h => h.Size).Sum();
				return dots;
			}


			public bool Contains(Point3D p)
			{
				if (p.X < Top.X || p.X > Bot.X) return false;
				if (p.Y < Top.Y || p.Y > Bot.Y) return false;
				if (p.Z < Top.Z || p.Z > Bot.Z) return false;
				return true;
			}

			public bool HasOverlap(Cube other, out Cube overlap)
			{
				overlap = other.Corners.Any(p => Contains(p)) ? Overlap(other) : null;
				return overlap != null;
			}

			public Cube Overlap(Cube other)
			{
				var x1 = Math.Max(Top.X, other.Top.X);
				var x2 = Math.Min(Bot.X, other.Bot.X);
				var y1 = Math.Max(Top.Y, other.Top.Y);
				var y2 = Math.Min(Bot.Y, other.Bot.Y);
				var z1 = Math.Max(Top.Z, other.Top.Z);
				var z2 = Math.Min(Bot.Z, other.Bot.Z);
				return new Cube(x1, x2, y1, y2, z1, z2);
			}
			//public void OverlapWith(Cube other, bool on)
			//{
			//	if (HasOverlap(other, out var overlap))
			//	{
			//		foreach (var (o, on2) in Overlaps)
			//		{
			//			if (on != on2)
			//				o.OverlapWith(overlap, !on);
			//		}
			//		Overlaps.Add((overlap, on));
			//	}
			//}
		}

		protected override long Part2(string[] input)
		{
			var cubes = new List<Cube>();
			var unions = new List<Cube>();

			foreach (var s in input)
			{
				// on x=10..12,y=10..12,z=10..12
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var on = set == "on";
				var cube = new Cube(x1, x2, y1, y2, z1, z2);

				foreach (var c in cubes.Concat(unions).ToArray())
				{
					if (c.HasOverlap(cube, out var overlap))
					{
						if (on)
						{
							unions.Add(overlap);
							foreach (var hc in c.Holes)
							{
								if (hc.HasOverlap(overlap, out var holeoverlap))
								{
									overlap.Holes.Add(holeoverlap);
								}
							}


						}
						else
						{
							c.Holes.Add(overlap);
						}
					}
				}
				if (on)
				{
					cubes.Add(cube);
				}
			}

			var dots = CountDots(cubes);
			var doubles = CountDots(unions);

			return dots - doubles;
		}

		private static long CountDots(List<Cube> cubes)
		{
			var dots = 0L;
			foreach (var cube in cubes)//.OrderByDescending(x => x.Holes.Count))
			{
				Console.Write($"holes={cube.Holes.Count} size={cube.Size}");

				var holes = new List<Cube>();
				var unions = 0L;
				foreach (var hole in cube.Holes)
				{
					foreach (var h in holes)
					{
						if (h.HasOverlap(hole, out var overlap))
						{
							unions += overlap.Size;
						}
					}
					holes.Add(hole);
				}

				var holecount = holes.Sum(h => h.Size) - unions;
				var dotcount = cube.Size - holecount;
				Console.WriteLine($"  #holes={holecount} dots={dotcount}");
				dots += dotcount;
			}
			return dots;
		}

		//for (var i = 0; i < cube.Overlaps.Count; i++)
		//{
		//	var (overlap, on) = cube.Overlaps[i];
		//	if (on)
		//	{
		//		overlappedDots += overlap.Size;
		//	}
		//	else
		//	{
		//		dots -= overlap.Size;
		//		for (var j = 0; j < i; j++)
		//		{
		//			var (o2, on2) = cube.Overlaps[j];
		//			if (o2.OverlapsWith(overlap))
		//			{
		//				if (on2)
		//				{
		//					overlappedDots -= o2.Overlap(overlap).Size;
		//				}
		//				else
		//				{
		//					for (var k = 0; k < j; k++)
		//					{
		//						var (o3, on3) = cube.Overlaps[k];
		//						if (o3.OverlapsWith(o2))
		//						{
		//							if (!on2)
		//							{
		//								overlappedDots -= o3.Overlap(o2).Size;
		//							}
		//							else
		//							{

		//								//overlappedDots += o2.Overlap(overlap).Size;
		//							}
		//						}
		//					}

		//					//overlappedDots += o2.Overlap(overlap).Size;
		//				}
		//			}
		//		}
		//	}
		//}
	}
}
