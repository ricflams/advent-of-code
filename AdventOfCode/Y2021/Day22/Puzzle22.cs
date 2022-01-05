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
			//Run("test1").Part1(39).Part2(39);

			//Run("test2").Part1(590784).Part2(0);
			//Run("test2.5").Part1(590784).Part2(590784);
			//Run("test1").Part2(39);
			//Run("test2.5").Part2(590784);
						Run("test3").Part2(2758514936282235);

		//				Run("input").Part1(596989).Part2(0);

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
			//public long Size => (long)(Bot.X - Top.X + 1) * (Bot.Y - Top.Y + 1) * (Bot.Z - Top.Z + 1) - Cardinality(1, Holes);

			public override string ToString() => $"size={Size} #holes={Holes.Count}";

			//public long CountDots()
			//{
			//	var dots = Size - Holes.Select(h => h.Size).Sum();
			//	return dots;
			//}


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

			public Cube MaybeOverlap(Cube other)
			{
				return other.Corners.Any(p => Contains(p)) ? Overlap(other) : null;
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
		}

		internal class Mod
        {
			public Cube Chunk { get; set; }
			public bool On { get; set; }
			public int Time;
		}

		protected override long Part2(string[] input)
		{
			//var allcubes = input
			//	.Select(s =>
			//	{
			//		var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
			//		var on = set == "on";
			//		return (new Cube(x1, x2, y1, y2, z1, z2), on);
			//	})
			//	.ToArray();

			//var minx = allcubes.Select(c => c.Item1.Bot.X).Min();
			//var miny = allcubes.Select(c => c.Item1.Bot.Y).Min();
			//var minz = allcubes.Select(c => c.Item1.Bot.Z).Min();
			//var maxx = allcubes.Select(c => c.Item1.Bot.X).Max();
			//var maxy = allcubes.Select(c => c.Item1.Bot.Y).Max();
			//var maxz = allcubes.Select(c => c.Item1.Bot.Z).Max();

			//var kube = new Cube(minx, maxx, miny, maxy, minz, maxz);
			//var size = kube.Size;
			//var space = new bool[maxx-minx+1, maxy-miny+1, maxz-minz+1];

			//var sets = Enumerable.Range(0, 1000).Select(_ => new List<Cube>()).ToArray();
			//var cubes = new List<Cube>();

			//void InstallCubeOn(Cube cube, int level)
			//         {
			//	var cubes = new[] { cube };
			//	while (cubes.Any())
			//             {
			//		Console.WriteLine($"Install {cubes.Length} at level {level}");
			//		var cubes2 = sets[level]
			//			.SelectMany(c => cubes.Select(cs => cs.MaybeOverlap(c)))
			//			.Where(c => c != null)
			//			.ToArray();
			//		sets[level].AddRange(cubes);
			//		cubes = cubes2;
			//		level++;
			//	}
			//         }

			var cubes = new List<Cube>();
			var holes = new List<Cube>();
			var time = 0;
			foreach (var s in input)
			{
				time++;
				// on x=10..12,y=10..12,z=10..12
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var on = set == "on";
				var cube = new Cube(x1, x2, y1, y2, z1, z2);
				//Console.WriteLine(on ? "##### CUBE" : "----- hole");
				if (on)
                {
					cubes.Add(cube);
                }
				else
                {
					foreach (var c in cubes)//.Concat(extras))
					{
						if (cube.HasOverlap(c, out var overlap))
						{
							c.Holes.Add(overlap);
						}
					}
				}
			}

			//for (var r = 1; r < 10; r++)
			//         {
			//	var values = Enumerable.Range(1, 42);
			//	var combos = MathHelper.Combinations(values, r).Count();
			//	Console.WriteLine($"{r}: {combos}");
			//}



			//var grandtotal2 = OldCardinality(cubes);
			var grandtotal = Cardinality(cubes);
			//if (grandtotal != grandtotal2)
			//	;// throw new Exception();

			var fit = (double)grandtotal / 2758514936282235;
			Console.WriteLine($"Fit: {fit*100:F0}%");


			return grandtotal;
		}


		private static long Cardinality(IEnumerable<Cube> cubes0)
		{
			if (!cubes0.Any())
            {
				return 0;
            }

			var set0 = cubes0.Select((c, i) =>
				{
					var index = new int[] { i };
					return (c, index);
				})
				.ToArray();

			var excessHoles = 0L;

			var sum0 = 0L;
			foreach (var c in cubes0)
            {
				sum0 += c.Size;// - Puzzle.Cardinality(c.Holes);
			}
			var card = Cardinality(2, set0);
			return sum0 - card - excessHoles;

			long Cardinality(int setlengh, (Cube cube,int[] indexes)[] set)
			{
				if (set.Length == 1)
                {
					var excessHoles2 = Puzzle.Cardinality(set[0].cube.Holes);
					return set[0].cube.Size - excessHoles2;
                }

				var allIndexes = Enumerable.Range(0, set.Length);
				var unions = new Dictionary<string, (Cube, int[])>();
				var sum = 0L;
				foreach (var indexes in MathHelper.Combinations(allIndexes, 2))
				{
					var subset = indexes.Select(i => set[i]).ToArray();
					var (c1, c2) = (subset[0], subset[1]);
					var covers = c1.indexes.Concat(c2.indexes).Distinct().OrderBy(x => x).ToArray();
					if (covers.Length > setlengh)
						continue;
					var key = string.Join("-", covers);
					if (unions.ContainsKey(key))
						continue;
					if (c1.cube.HasOverlap(c2.cube, out var overlap))
                    {
						unions[key] = (overlap, covers);

                        var uholes = new List<Cube>();
                        foreach (var hole in c1.cube.Holes.Concat(c2.cube.Holes))
                        {
                            if (overlap.HasOverlap(hole, out var uhole))
                            {
                                uholes.Add(uhole);
                            }
                        }
						//			overlap.Holes = uholes;

						sum += overlap.Size;
						excessHoles += Puzzle.Cardinality(uholes);

						////sum -= Puzzle.Cardinality(uholes);
						////excessHoles += Puzzle.Cardinality(uholes);

						//var uholes1 = new List<Cube>();
      //                  foreach (var hole in c1.cube.Holes)
      //                  {
      //                      if (overlap.HasOverlap(hole, out var uhole))
      //                      {
      //                          uholes1.Add(uhole);
      //                      }
      //                  }
      //                  var uholes2 = new List<Cube>();
      //                  foreach (var hole in c2.cube.Holes)
      //                  {
      //                      if (overlap.HasOverlap(hole, out var uhole))
      //                      {
      //                          uholes2.Add(uhole);
      //                      }
      //                  }
						//sum -= Puzzle.Cardinality(uholes1);
						//sum -= Puzzle.Cardinality(uholes2);


						//                        overlap.Holes = uholes;
						//excessHoles += Puzzle.Cardinality(c1.cube.Holes);
						//excessHoles += Puzzle.Cardinality(c2.cube.Holes);

						//sum -= Puzzle.Cardinality(c1.cube.Holes);
						//sum -= Puzzle.Cardinality(c2.cube.Holes);
					}
					else
                    {
						unions[key] = (null,null); // to indicate it's been seen and has no overlaps
                    }
				}
				var nextSets = unions.Values.Where(x => x.Item1 != null).ToArray();
				return sum == 0 ? 0 : sum - Cardinality(setlengh + 1, nextSets);
			}
		}


        private static long OldCardinality(IEnumerable<Cube> cubes)
        {
            var set = cubes.ToArray();
            var allIndexes = Enumerable.Range(0, set.Length);

            var excessHoles = 0L;
            var card = OldCardinality(1);
            return card - excessHoles;

            long OldCardinality(int depth)
            {
                Console.WriteLine($"{depth}");
                var sum = 0L;
                foreach (var indexes in MathHelper.Combinations(allIndexes, depth))
                {
                    var subset = indexes.Select(i => set[i]);
                    var union = subset.First();
                    foreach (var c in subset.Skip(1))
                    {
                        if (!union.HasOverlap(c, out union))
                            break;
                    }
                    if (union == null)
                        continue;

                    sum += union.Size;
                    //var uholes = new List<Cube>();
                    //foreach (var hole in subset.SelectMany(i => i.Holes))
                    //               {
                    //	if (union.HasOverlap(hole, out var uhole))
                    //                   {
                    //		//excessHoles += uhole.Size;
                    //		uholes.Add(uhole);
                    //                   }
                    //               }
                    //excessHoles += Puzzle.Cardinality(uholes);

                    //var mods = subset
                    //	.SelectMany(i => i.Mods)
                    //	.Where(m => union.HasOverlap(m.Chunk, out var _))
                    //	.OrderBy(m => m.Time)
                    //	.ToArray();
                    //Console.WriteLine($"{subset.Count()} {mods.Count()}");
                    ////foreach (var mod in subset.SelectMany(i => i.Mods).OrderBy(m => m.Time))
                    ////               {

                    ////               }

                }
                return sum == 0 ? 0 : sum - OldCardinality(depth + 1);
            }
        }

    }
}
