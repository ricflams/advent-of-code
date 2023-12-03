using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day22.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 22";
		public override int Year => 2021;
		public override int Day => 22;

		public override void Run()
		{
			Run("test4").Part2(1000);

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

			public readonly Point3D Bot;
			public readonly Point3D Top;
			public readonly Point3D[] Corners;
			public long Size => (long)(Top.X - Bot.X + 1) * (Top.Y - Bot.Y + 1) * (Top.Z - Bot.Z + 1);

			public int Tick;

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

			//public bool IsCongruent(Cube c) => Bot == c.Bot && Top == c.Top;
			public bool IsCongruent(Cube c) => Bot.Equals(c.Bot) && Top.Equals(c.Top);

			public Cube Copy() => new Cube(Bot.X, Top.X, Bot.Y, Top.Y, Bot.Z, Top.Z) { Tick = Tick };

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

				Debug.Assert(exploded.Select(x => x.Size).Sum() + union.Size == Size);
				Debug.Assert(exploded.All(e => Contains(e)));

				for (var i = 0; i < exploded.Count; i++)
				{
					var ci = exploded[i];
					for (var j = i + 1; j < exploded.Count; j++)
					{
						var cj = exploded[j];
						var o1 = ci.HasOverlap(cj, out var _);
						var o2 = cj.HasOverlap(ci, out var _);
						Debug.Assert(!o1);
						Debug.Assert(!o2);
					}
				}


				return exploded;
			}


			//public static bool SplitCube(Cube c1, Cube c2, out List<Cube> parts1, out List<Cube> parts2, out Cube union)
			//{
			//	parts1 = parts2 = null;
			//	union = null;
			//	if (!c1.Corners.Any(p => c2.Contains(p)))
			//		return false;

			//	parts1 = new List<Cube>();
			//	parts2 = new List<Cube>();

			//	if (c1.Bot == c2.Bot && c1.Top == c2.Top)
			//	{
			//		parts1.Add(c1);
			//		parts2.Add(c2);
			//		union = c2.Copy().Invert();
			//		return true;
			//	}

			//	var x2 = Split((c1.Bot.X, c1.Top.X), (c2.Bot.X, c2.Top.X)).ToArray();
			//	var y2 = Split((c1.Bot.Y, c1.Top.Y), (c2.Bot.Y, c2.Top.Y)).ToArray();
			//	var z2 = Split((c1.Bot.Z, c1.Top.Z), (c2.Bot.Z, c2.Top.Z)).ToArray();

			//	foreach (var x in Split((c1.Bot.X, c1.Top.X), (c2.Bot.X, c2.Top.X)))
			//	{
			//		foreach (var y in Split((c1.Bot.Y, c1.Top.Y), (c2.Bot.Y, c2.Top.Y)))
			//		{
			//			foreach (var z in Split((c1.Bot.Z, c1.Top.Z), (c2.Bot.Z, c2.Top.Z)))
			//			{
			//				var part = new Cube(c1.On, x.min, x.max, y.min, y.max, z.min, z.max);
			//				var in1 = c1.Contains(part);
			//				var in2 = c2.Contains(part);
			//				if (!in1 && !in2)
			//					continue;
			//				if (in1 && in2)
			//				{
			//					//var on = c1 == c2;
			//					var on = !c2.On;
			//					union = part.Copy().SetOn(on);
			//				}
			//				if (in1)
			//				{
			//					var part1 = part.Copy().SetOn(c1.On);
			//					parts1.Add(part1);
			//				}
			//				if (in2)
			//				{
			//					var part2 = part.Copy().SetOn(c2.On);
			//					parts2.Add(part2);
			//				}
			//			}
			//		}
			//	}

			//	var sizereal = c1.Size + c2.Size - c1.Overlap(c2).Size;
			//	var sizeparts = parts1.Concat(parts2).Select(x => x.Size).Sum() - union.Size;
			//	Debug.Assert(sizereal == sizeparts);
			//	Debug.Assert(parts1.Select(x => x.Size).Sum() == c1.Size);
			//	Debug.Assert(parts2.Select(x => x.Size).Sum() == c2.Size);
			//	Debug.Assert(parts1.All(x => x.On == c1.On));
			//	Debug.Assert(parts2.All(x => x.On == c2.On));
			//	Debug.Assert(parts1.All(c => c.Bot.X <= c.Top.X && c.Bot.Y <= c.Top.Y && c.Bot.Z <= c.Top.Z));
			//	Debug.Assert(parts2.All(c => c.Bot.X <= c.Top.X && c.Bot.Y <= c.Top.Y && c.Bot.Z <= c.Top.Z));

			//	return true;

			//	static IEnumerable<(int min,int max)> Split((int min, int max) a, (int min, int max) b)
			//	{
			//		if (a.min == b.min && a.max == b.max)
			//		{
			//			// completely identical
			//			// +-----a-----+
			//			// +-----b-----+
			//			yield return a; // either will do
			//		}
			//		else if (a.min == b.min || a.max == b.max)
			//		{
			//			// not identical, but anchored in one end
			//			if (a.min == b.min)
			//			{
			//				if (a.max < b.max)
			//				{
			//					// +---a---+
			//					// +-----b-----+
			//					yield return a;
			//					yield return (a.max + 1, b.max);
			//				}
			//				else
			//				{
			//					// +-----a-----+
			//					// +---b---+
			//					yield return b;
			//					yield return (b.max + 1, a.max);
			//				}
			//			}
			//			else
			//			{
			//				if (a.min > b.min)
			//				{
			//					//     +---a---+
			//					// +-----b-----+
			//					yield return a;
			//					yield return (b.min, a.min - 1);
			//				}
			//				else
			//				{
			//					// +-----a-----+
			//					//     +---b---+
			//					yield return b;
			//					yield return (a.min, b.min - 1);
			//				}
			//			}
			//		}
			//		else
			//		{
			//			// true overlap
			//			if (a.min > b.min)
			//			{
			//				if (a.max < b.max)
			//				{
			//					//    +--a--+
			//					// +-----b-----+
			//					yield return (b.min, a.min - 1);
			//					yield return a;
			//					yield return (a.max + 1, b.max);
			//				}
			//				else
			//				{
			//					//    +-----a-----+
			//					// +-----b-----+
			//					yield return (b.min, a.min - 1);
			//					yield return (a.min, b.max);
			//					yield return (b.max + 1, a.max);
			//				}
			//			}
			//			else
			//			{
			//				if (a.max > b.max)
			//				{
			//					// +-----a-----+
			//					//    +--b--+
			//					yield return (a.min, b.min - 1);
			//					yield return b;
			//					yield return (b.max + 1, a.max);
			//				}
			//				else
			//				{
			//					// +-----a-----+
			//					//    +-----b-----+
			//					yield return (a.min, b.min - 1);
			//					yield return (b.min, a.max);
			//					yield return (a.max + 1, b.max);
			//				}
			//			}
			//		}
			//	}
			//}

			//public Cube MaybeOverlap(Cube other)
			//{
			//	return other.Corners.Any(p => Contains(p)) ? Overlap(other) : null;
			//}

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

		//private static List<Cube> OverlayCubes(List<Cube> allcubes, bool toplevel)
		//{
		//	Console.WriteLine($"OverlayCubes: N={allcubes.Count} toplevel={toplevel}");
		//	Cube span = null;
		//	bool[,,] kube = null;

		//	if (toplevel)
		//	{
		//		span = new Cube(false,
		//			allcubes.Select(c => c.Bot.X).Min(),
		//			allcubes.Select(c => c.Top.X).Max(),
		//			allcubes.Select(c => c.Bot.Y).Min(),
		//			allcubes.Select(c => c.Top.Y).Max(),
		//			allcubes.Select(c => c.Bot.Z).Min(),
		//			allcubes.Select(c => c.Top.Z).Max()
		//		);
		//		Console.WriteLine($"Size: {span.Size}");
		//		kube = new bool[span.Top.X - span.Bot.X + 1, span.Top.Y - span.Bot.Y + 1, span.Top.Z - span.Bot.Z + 1];
		//	}

		//	var cubes = new List<Cube>();
		//	foreach (var cube in allcubes)
		//	{
		//		var added = new List<Cube>();
		//		foreach (var c in cubes)
		//		{
		//			if (cube.HasOverlap(c, out var overlap))
		//			{



		//				var c1 = cube.On;
		//				var c2 = c.On;

		//				if (toplevel && !c1 && !c2)
		//					continue;
		//				//if (c1 != c2)
		//				//    continue;
		//				overlap.On = !c2;
		//				added.Add(overlap);
		//			}
		//		}

		//		if (added.Count > 1)
		//		{
		//			added = OverlayCubes(added, false);
		//		}
		//		cubes.AddRange(added);
		//		if (toplevel)
		//		{
		//			if (cube.On)
		//				cubes.Add(cube);
		//		}
		//		else
		//		{
		//			cubes.Add(cube);
		//		}

		//		if (toplevel)
		//		{
		//			for (var x = cube.Bot.X; x <= cube.Top.X; x++)
		//			{
		//				for (var y = cube.Bot.Y; y <= cube.Top.Y; y++)
		//				{
		//					for (var z = cube.Bot.Z; z <= cube.Top.Z; z++)
		//					{
		//						kube[x - span.Bot.X, y - span.Bot.Y, z - span.Bot.Z] = cube.On;
		//					}
		//				}
		//			}
		//			var realOn = 0;
		//			foreach (var b in kube)
		//			{
		//				if (b) realOn++;
		//			}
		//			var plus0 = cubes.Where(c => c.On).Select(c => c.Size).Sum();
		//			var minus0 = cubes.Where(c => !c.On).Select(c => c.Size).Sum();
		//			var calcOn = plus0 - minus0;
		//			Console.WriteLine($"RealOn: {realOn} On: {calcOn}  Diff={calcOn - realOn}   {(realOn == calcOn ? "ok" : "####### not ok")}");
		//			//foreach (var c in cubes.Skip(seenindex))
		//			//	Console.WriteLine($"\t\t{c}");
		//		}
		//	}
		//	return cubes;
		//}

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

			var ticks = 0;

			while (steps.Any())
			{
				var (step, on) = steps.Pop();
				ticks++;

				if (ticks > 2386)
				{
					CheckOverlappingCubes(cubes);
				}
				//Console.Write($"[{steps.Count}]");

				//while (true)
				//{
				//	bool done = true;
				//	for (var i = 0; i < cubes.Count; i++)
				//	{
				//		for (var j = i + 1; j < cubes.Count; j++)
				//		{
				//			if (cubes[i].IsCongruent(cubes[j]))
				//			{
				//				cubes.RemoveAt(j);
				//				Console.WriteLine("KILL");
				//				done = false;
				//				j = i = cubes.Count;
				//			}
				//		}
				//	}
				//	if (done)
				//		break;
				//}

				var match = cubes.FirstOrDefault(c => step.IsCongruent(c));
				if (match != null)
				{
					if (!on)
					{
						cubes.Remove(match);
					}
					//match.On = step.On; // this is your life now
					//Console.WriteLine($"  Cancelled out by {matches[0]}");
					continue; // we're done
				}

				//foreach (var c in cubes.Where(c => step.Contains(c)).ToArray())
				//{
				//	cubes.Remove(c);
				//}

				//if (on)
				//{
				//	var containedIn = cubes.FirstOrDefault(c => c.Contains(step));
				//	if (containedIn != null)
				//	{
				//		continue;
				//	}
				//}

				//if (step.Size == 43605)
				//	;

				////CheckOverlappingCubes(cubes);
				//if (IsOverlapping(step, out var result))
				//{
				//	var (other, union) = result;

				//	if (other.Contains(step) && !step.Contains(other) && on)
				//		continue;

				//	cubes.Remove(other);
				//	var otherExploded = other.Explode(union).ToArray();
				//	cubes.AddRange(otherExploded);
				//	//if (step.On)
				//		cubes.Add(union);

				//	var exploded = step.Explode(union).ToArray();
				//	foreach (var part in exploded)
				//	{
				//		steps.Push((part, on));
				//	}
				//	steps.Push((union, on));

				//	if (IsOverlapping(step, out var _))
				//		Console.Write("#");

				//	continue;
				//}


				List<Cube> BreakdownSteps(Cube step)
				{
					var breaksteps = new List<Cube>();
					if (IsOverlapping(step, out var result))
					{
						var (other, union) = result;

						cubes.Remove(other);
						//if (IsOverlapping(other, out var xxx))
						//{
						//	Console.WriteLine($"Other {other} has overlap={xxx.overlapping}");
						//}
						//if (ticks > 2386)
						//{
						//	CheckOverlappingCubes(cubes);
						//}

						var otherExploded = other.Explode(union).ToArray();
						//if (ticks > 2386)
						//{
						//	Console.WriteLine($"Explode {other}  hasoverlap={IsOverlapping(other, out var _)}");
						//	foreach (var e in otherExploded)
						//	{
						//		Console.WriteLine($"Explode into {e}  hasoverlap={IsOverlapping(e, out var _)}");
						//	}
						//}

						//var en = 0;
						foreach (var e in otherExploded)
						{
							cubes.Add(e.Copy());

							//en++;
							//e.Tick = ticks;
							//if (!IsOverlapping(e, out var eoverlap))
							//{
							//	if (ticks > 2386)
							//	{
							//		CheckOverlappingCubes(cubes);
							//	}
							//	cubes.Add(e.Copy());
							//	if (ticks > 2386)
							//	{
							//		CheckOverlappingCubes(cubes);
							//	}
							//}
							//else
							//{
							//	var (overlapped, union2) = eoverlap;
							//	Console.WriteLine($"Dup at #cubes={cubes.Count} n={ticks} for exploded {e} at pos {en}");
							//	Console.WriteLine($"  other {other}");
							//	foreach (var e2 in otherExploded)
							//	{
							//		var hasoverlap = IsOverlapping(e2, out var o);
							//		var index = hasoverlap ? cubes.IndexOf(o.overlapping) : -1;
							//		Console.WriteLine($"    explode into {e2}  hasoverlap={hasoverlap} at {index}");
							//		if (hasoverlap)
							//		{
							//			var hasoverlap2 = other.HasOverlap(o.overlapping, out var _);
							//			Console.WriteLine($"      other hasoverlap2={hasoverlap2}");
							//		}
							//	}
							//	Console.WriteLine($"  union {union}");
							//	Console.WriteLine($"e {e} overlaps with {overlapped} by {union2}");
							//	CheckOverlappingCubes(cubes);
							//}
						}
						//cubes.AddRange(otherExploded);

						//union.Tick = ticks;
						//if (!IsOverlapping(union, out var _))
						//{
						//	if (ticks > 2386)
						//	{
						//		CheckOverlappingCubes(cubes);
						//	}
						//	cubes.Add(union.Copy());
						//	if (ticks > 2386)
						//	{
						//		CheckOverlappingCubes(cubes);
						//	}
						//}
						//else
						//	Console.WriteLine($"Dup at #cubes={cubes.Count} n={ticks} for union {union}");
						cubes.Add(union);

						var exploded = step.Explode(union).ToArray();
						foreach (var part in exploded)
						{
							breaksteps.AddRange(BreakdownSteps(part));
						}
						breaksteps.Add(union);
					}
					else
					{
						breaksteps.Add(step);
					}
					return breaksteps;
				}

				var stepsteps = BreakdownSteps(step);
				if (stepsteps.Count > 1)
				{
					foreach (var s in stepsteps)
					{
						steps.Push((s, on));
					}
					continue;
				}

				if (on)
				{
					step.Tick = ticks;
					//if (!IsOverlapping(step, out var _))
					//	cubes.Add(step.Copy());
					//else
					//	Console.WriteLine($"Dup at #cubes={cubes.Count} n={ticks} for new {step}");
					//CheckOverlappingCubes(cubes);
					cubes.Add(step);
				}
			}




			var total = cubes.Select(c => c.Size).Sum();

			var fit = (double)total / 2758514936282235;
			Console.WriteLine($"Fit: {fit * 100:F0}%  {total}");// {f1} {f2} {f3} {f4}");

			return total;
		}

		private void CheckOverlappingCubes(List<Cube> cubes)
		{
			//for (var i = 0; i < cubes.Count; i++)
			//{
			//	var ci = cubes[i];
			//	for (var j = i + 1; j < cubes.Count; j++)
			//	{
			//		var cj = cubes[j];
			//		if (ci.HasOverlap(cj, out var _) || cj.HasOverlap(ci, out var _))
			//			;
			//	}
			//}
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
