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

			////Run("test2").Part1(590784).Part2(0);
			Run("test2.5").Part1(590784).Part2(590784);
			////Run("test1").Part2(39);
			////Run("test2.5").Part2(590784);
			Run("test3").Part2(2758514936282235);

			Run("input").Part1(596989).Part2(1160011199157381);
			// 1182157305438760 too high
			// 1181015561989639 too high
			// 1181015561989639
			// 1180954560331460 too high

		}

		protected override long Part1(string[] input)
		{
			var space = new bool[101, 101, 101];
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

			public override bool Equals(object p) => Equals(p as Point3D);
			public bool Equals(Point3D p) => X == p?.X && Y == p?.Y && Z == p?.Z;
			public override int GetHashCode() => X * 397 ^ Y ^ Z;
			public override string ToString() => $"({X},{Y},{Z})";

			public Point3D Diff(Point3D other) => new Point3D(other.X - X, other.Y - Y, other.Z - Z);
			public Point3D Offset(Point3D offset) => new Point3D(X + offset.X, Y + offset.Y, Z + offset.Z);
		}

		internal class Cube
		{
			public Cube(int x1, int x2, int y1, int y2, int z1, int z2)
			{
				Bot = new Point3D(x1, y1, z1);
				Top = new Point3D(x2, y2, z2);
				//Corners = new Point3D[]
				//{
				//	new Point3D(x1, y1, z1),
				//	new Point3D(x1, y1, z2),
				//	new Point3D(x1, y2, z1),
				//	new Point3D(x1, y2, z2),
				//	new Point3D(x2, y1, z1),
				//	new Point3D(x2, y1, z2),
				//	new Point3D(x2, y2, z1),
				//	new Point3D(x2, y2, z2)
				//};
			}

			public readonly Point3D Bot;
			public readonly Point3D Top;
			//public readonly Point3D[] Corners;
			public long Size => (long)(Top.X - Bot.X + 1) * (Top.Y - Bot.Y + 1) * (Top.Z - Bot.Z + 1);

			//public int Tick;

			public override string ToString() => $"[x={Bot.X}..{Top.X},y={Bot.Y}..{Top.Y},z={Bot.Z}..{Top.Z}:{Size}]";

			public bool Contains(Point3D p)
			{
				if (p.X < Bot.X || p.X > Top.X) return false;
				if (p.Y < Bot.Y || p.Y > Top.Y) return false;
				if (p.Z < Bot.Z || p.Z > Top.Z) return false;
				return true;
			}

			public bool Contains(Cube c)
			{
				if (c.Bot.X < Bot.X || c.Top.X > Top.X) return false;
				if (c.Bot.Y < Bot.Y || c.Top.Y > Top.Y) return false;
				if (c.Bot.Z < Bot.Z || c.Top.Z > Top.Z) return false;
				return true;
			}

			public bool IsCongruent(Cube c) => Bot == c.Bot && Top == c.Top;
			//public bool IsCongruent(Cube c) => Bot.Equals(c.Bot) && Top.Equals(c.Top);

			public Cube Copy() => new Cube(Bot.X, Top.X, Bot.Y, Top.Y, Bot.Z, Top.Z);

			public static bool Intersects(Cube a, Cube b)
			{
				return (a.Bot.X <= b.Top.X && a.Top.X >= b.Bot.X) &&
						(a.Bot.Y <= b.Top.Y && a.Top.Y >= b.Bot.Y) &&
						(a.Bot.Z <= b.Top.Z && a.Top.Z >= b.Bot.Z);
			}

			public bool HasOverlap(Cube other, out Cube overlap)
			{
				overlap = Intersects(this, other) ? Overlap(other) : null;
				return overlap != null;
			}


			public List<Cube> Explode(Cube union)
			{
				var exploded = new List<Cube>();
				if (union.Top.X < Top.X)
				{
					exploded.Add(new Cube(union.Top.X + 1, Top.X, Bot.Y, Top.Y, Bot.Z, Top.Z));
				}
				if (union.Bot.X > Bot.X)
				{
					exploded.Add(new Cube(Bot.X, union.Bot.X - 1, Bot.Y, Top.Y, Bot.Z, Top.Z));
				}

				if (union.Top.Y < Top.Y)
				{
					exploded.Add(new Cube(union.Bot.X, union.Top.X, union.Top.Y + 1, Top.Y, Bot.Z, Top.Z));
				}
				if (union.Bot.Y > Bot.Y)
				{
					exploded.Add(new Cube(union.Bot.X, union.Top.X, Bot.Y, union.Bot.Y - 1, Bot.Z, Top.Z));
				}

				if (union.Top.Z < Top.Z)
				{
					exploded.Add(new Cube(union.Bot.X, union.Top.X, union.Bot.Y, union.Top.Y, union.Top.Z + 1, Top.Z));
				}
				if (union.Bot.Z > Bot.Z)
				{
					exploded.Add(new Cube(union.Bot.X, union.Top.X, union.Bot.Y, union.Top.Y, Bot.Z, union.Bot.Z - 1));
				}

				//Debug.Assert(exploded.Select(x => x.Size).Sum() + union.Size == Size);
				//Debug.Assert(exploded.All(e => Contains(e)));

				//for (var i = 0; i < exploded.Count; i++)
				//{
				//	var ci = exploded[i];
				//	for (var j = i + 1; j < exploded.Count; j++)
				//	{
				//		var cj = exploded[j];
				//		var o1 = ci.HasOverlap(cj, out var _);
				//		var o2 = cj.HasOverlap(ci, out var _);
				//		Debug.Assert(!o1);
				//		Debug.Assert(!o2);
				//	}
				//}

				return exploded;
			}

			public Cube Overlap(Cube other)
			{
				var x1 = Math.Max(Bot.X, other.Bot.X);
				var x2 = Math.Min(Top.X, other.Top.X);
				var y1 = Math.Max(Bot.Y, other.Bot.Y);
				var y2 = Math.Min(Top.Y, other.Top.Y);
				var z1 = Math.Max(Bot.Z, other.Bot.Z);
				var z2 = Math.Min(Top.Z, other.Top.Z);
				return new Cube(x1, x2, y1, y2, z1, z2);
			}
		}

		protected override long Part2(string[] input)
		{
			var allcubes = input
				.Select(s =>
				{
					// on x=10..12,y=10..12,z=10..12
					var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
					var on = set == "on";
					return (new Cube(x1, x2, y1, y2, z1, z2), on);
				})
				.ToList();
			allcubes.Reverse();
			var steps = new Stack<(Cube, bool)>(allcubes);

			var cubes = new List<Cube>();

			bool IsOverlapping(Cube cube, out (Cube overlapping, Cube union) result)
			{
				foreach (var c in cubes)
				{
					if (cube.HasOverlap(c, out var union))
					{
						result = (c, union);
						return true;
					}
				}
				result = (null, null);
				return false;
			}

			while (steps.Any())
			{
				var (step, on) = steps.Pop();

				var match = cubes.FirstOrDefault(c => step.IsCongruent(c));
				if (match != null)
				{
					if (!on)
					{
						cubes.Remove(match);
					}
					continue; // we're done
				}

				//List<Cube> BreakdownSteps(Cube step)
				//{
				//	var breaksteps = new List<Cube>();
				//	if (IsOverlapping(step, out var result))
				//	{
				//		var (other, union) = result;
				//		cubes.Remove(other);
				//		foreach (var e in other.Explode(union))
				//		{
				//			cubes.Add(e);
				//		}
				//		cubes.Add(union);

				//		foreach (var part in step.Explode(union))
				//		{
				//			breaksteps.AddRange(BreakdownSteps(part));
				//		}
				//		breaksteps.Add(union);
				//	}
				//	else
				//	{
				//		breaksteps.Add(step);
				//	}
				//	return breaksteps;
				//}

				bool abort = false;
				void BreakdownSteps(Cube step)
				{
					if (IsOverlapping(step, out var result))
					{
						var (other, union) = result;
						cubes.Remove(other);
						foreach (var e in other.Explode(union))
						{
							cubes.Add(e);
						}
						cubes.Add(union);

						foreach (var part in step.Explode(union))
						{
							abort = true;
							BreakdownSteps(part);
						}
						steps.Push((union, on));
					}
					else
					{
						steps.Push((step, on));
					}
				}
				if (abort)
					continue;

				if (on)
				{
					cubes.Add(step);
				}
			}

			var total = cubes.Select(c => c.Size).Sum();
			return total;
		}

		//private static long Cardinality(IEnumerable<Cube> cubes0)
		//{
		//	if (!cubes0.Any())
		//          {
		//		return 0;
		//          }

		//	var set0 = cubes0.Select((c, i) =>
		//		{
		//			var index = new int[] { i };
		//			return (c, index);
		//		})
		//		.ToArray();

		//	var excessHoles = 0L;

		//	var sum0 = 0L;
		//	foreach (var c in cubes0)
		//          {
		//		sum0 += c.Size;// - Puzzle.Cardinality(c.Holes);
		//	}
		//	var card = Cardinality(2, set0);
		//	return sum0 - card - excessHoles;

		//	long Cardinality(int setlengh, (Cube cube,int[] indexes)[] set)
		//	{
		//		if (set.Length == 1)
		//              {
		//			//var excessHoles2 = Puzzle.Cardinality(set[0].cube.Holes);
		//			return set[0].cube.Size;// - excessHoles2;
		//              }

		//		var allIndexes = Enumerable.Range(0, set.Length);
		//		var unions = new Dictionary<string, (Cube, int[])>();
		//		var sum = 0L;
		//		foreach (var indexes in MathHelper.Combinations(allIndexes, 2))
		//		{
		//			var subset = indexes.Select(i => set[i]).ToArray();
		//			var (c1, c2) = (subset[0], subset[1]);
		//			var covers = c1.indexes.Concat(c2.indexes).Distinct().OrderBy(x => x).ToArray();
		//			if (covers.Length > setlengh)
		//				continue;
		//			var key = string.Join("-", covers);
		//			if (unions.ContainsKey(key))
		//				continue;
		//			if (c1.cube.HasOverlap(c2.cube, out var overlap))
		//                  {
		//				unions[key] = (overlap, covers);

		//                      var uholes = new List<Cube>();
		//                      //foreach (var hole in c1.cube.Holes.Concat(c2.cube.Holes))
		//                      //{
		//                      //    if (overlap.HasOverlap(hole, out var uhole))
		//                      //    {
		//                      //        uholes.Add(uhole);
		//                      //    }
		//                      //}
		//				//			overlap.Holes = uholes;

		//				sum += overlap.Size;
		//				excessHoles += Puzzle.Cardinality(uholes);

		//				////sum -= Puzzle.Cardinality(uholes);
		//				////excessHoles += Puzzle.Cardinality(uholes);

		//				//var uholes1 = new List<Cube>();
		//    //                  foreach (var hole in c1.cube.Holes)
		//    //                  {
		//    //                      if (overlap.HasOverlap(hole, out var uhole))
		//    //                      {
		//    //                          uholes1.Add(uhole);
		//    //                      }
		//    //                  }
		//    //                  var uholes2 = new List<Cube>();
		//    //                  foreach (var hole in c2.cube.Holes)
		//    //                  {
		//    //                      if (overlap.HasOverlap(hole, out var uhole))
		//    //                      {
		//    //                          uholes2.Add(uhole);
		//    //                      }
		//    //                  }
		//				//sum -= Puzzle.Cardinality(uholes1);
		//				//sum -= Puzzle.Cardinality(uholes2);


		//				//                        overlap.Holes = uholes;
		//				//excessHoles += Puzzle.Cardinality(c1.cube.Holes);
		//				//excessHoles += Puzzle.Cardinality(c2.cube.Holes);

		//				//sum -= Puzzle.Cardinality(c1.cube.Holes);
		//				//sum -= Puzzle.Cardinality(c2.cube.Holes);
		//			}
		//			else
		//                  {
		//				unions[key] = (null,null); // to indicate it's been seen and has no overlaps
		//                  }
		//		}
		//		var nextSets = unions.Values.Where(x => x.Item1 != null).ToArray();
		//		return sum == 0 ? 0 : sum - Cardinality(setlengh + 1, nextSets);
		//	}
		//}


		//      private static long OldCardinality(IEnumerable<Cube> cubes)
		//      {
		//          var set = cubes.ToArray();
		//          var allIndexes = Enumerable.Range(0, set.Length);

		//          var excessHoles = 0L;
		//          var card = OldCardinality(1);
		//          return card - excessHoles;

		//          long OldCardinality(int depth)
		//          {
		//              Console.WriteLine($"{depth}");
		//              var sum = 0L;
		//              foreach (var indexes in MathHelper.Combinations(allIndexes, depth))
		//              {
		//                  var subset = indexes.Select(i => set[i]);
		//                  var union = subset.First();
		//                  foreach (var c in subset.Skip(1))
		//                  {
		//                      if (!union.HasOverlap(c, out union))
		//                          break;
		//                  }
		//                  if (union == null)
		//                      continue;

		//                  sum += union.Size;
		//                  //var uholes = new List<Cube>();
		//                  //foreach (var hole in subset.SelectMany(i => i.Holes))
		//                  //               {
		//                  //	if (union.HasOverlap(hole, out var uhole))
		//                  //                   {
		//                  //		//excessHoles += uhole.Size;
		//                  //		uholes.Add(uhole);
		//                  //                   }
		//                  //               }
		//                  //excessHoles += Puzzle.Cardinality(uholes);

		//                  //var mods = subset
		//                  //	.SelectMany(i => i.Mods)
		//                  //	.Where(m => union.HasOverlap(m.Chunk, out var _))
		//                  //	.OrderBy(m => m.Time)
		//                  //	.ToArray();
		//                  //Console.WriteLine($"{subset.Count()} {mods.Count()}");
		//                  ////foreach (var mod in subset.SelectMany(i => i.Mods).OrderBy(m => m.Time))
		//                  ////               {

		//                  ////               }

		//              }
		//              return sum == 0 ? 0 : sum - OldCardinality(depth + 1);
		//          }
		//      }

	}
}
