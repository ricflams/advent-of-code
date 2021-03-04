using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2017.Day20
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2017;
		public override int Day => 20;

		public void Run()
		{
			//RunFor("test1", 0, 0);
			//RunPart2For("test2", 3);
			RunFor("input", 0, 0);
		}

		protected override int Part1(string[] input)
		{
			var ps = ReadParticles(input);
			// for (var n = 1; n < 1000; n++)
			// {
			// 	var p = ps.OrderBy(p => p.Step(n).Dist).First();
			// 	Console.WriteLine($"{n}: {p.Id}");
			// }
			// for (var n = 1; n < int.MaxValue/10; n *= 10)
			// {
			// 	var p = ps.OrderBy(p => p.Step(n).Dist).First();
			// 	Console.WriteLine(p.Id);
			// }


			return 0;
		}

		protected override int Part2(string[] input)
		{
			var ps = ReadParticles(input);
			var collisions = new bool[ps.Length];

			for (var i = 0; i < ps.Length; i++)
			{
				var cols = ps.Skip(i+1)
					.Select(p2 =>
					{
						var n = ps[i].WillCollideWith(p2);
						return (p2, n);
					})
					.Where(x => x.n.HasValue)
					.ToArray();

				if (cols.Any())
				{
					Console.WriteLine($"{i}: will colllide with:");
					foreach (var c in cols)
					{
						Console.WriteLine($"   {c.p2.Id} after step={c.n.Value}");
					}
				}
			}

			var uncollided = collisions.Count(x => !x);




			return uncollided;
		}

		internal class Particle
		{
			internal class Pos
			{
				 public Pos(long x, long y, long z) => (X, Y, Z) = (x, y, z);
				 public long X, Y, Z;
				 public long Dist => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
				 public static Pos operator +(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
				 public static Pos operator -(Pos a, Pos b) => new Pos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
				 public static Pos operator *(int n, Pos a) => new Pos(a.X * n, a.Y * n, a.Z * n);
			}
			public int Id { get; set; }
			public Pos P { get; set; }
			public Pos V { get; set; }
			public Pos A { get; set; }

			public Pos Step(int n) => P + n*V + n*(n+1)/2 * A;

			public int? WillCollideWith(Particle p)
			{
				var nx = Collisions(P.X, V.X, A.X, p.P.X, p.V.X, p.A.X);
				var ny = Collisions(P.Y, V.Y, A.Y, p.P.Y, p.V.Y, p.A.Y);
				var nz = Collisions(P.Z, V.Z, A.Z, p.P.Z, p.V.Z, p.A.Z);

				if ((nx?.Any()??true) && (ny?.Any()??true) && (nz?.Any()??true))
				{

				}

				var ps = nx ?? ny ?? nz;
				if (ps == null)
				{
					// Same position!
					throw new Exception("Same position");
					return null;
				}
				if (nx != null) ps = ps.Intersect(nx).ToArray();
				if (ny != null) ps = ps.Intersect(ny).ToArray();
				if (nz != null) ps = ps.Intersect(nz).ToArray();

				var collisions = ps;

				if (collisions.Any())
				{
					// Got it! Check
					foreach (var n in collisions)
					{
						var p1 = Step(n);
						var p2 = p.Step(n);
						var collide = p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
						if (!collide)
							throw new Exception("No collision");
					}
					return collisions.Min();
				}
				return null;

				static int[] Collisions(long ap, long av, long aa, long bp, long bv, long ba)
				{
					if (aa==ba && av==bv && ap==bp)
						return null;
					return PotentialCollisions().Where(x => x > 0).Select(x => (int)x).ToArray();
					IEnumerable<long> PotentialCollisions()
					{
						if (aa == ba)
						{
							// 1st degree equation, only solveable for av!=bv
							if (av != bv)
							{
								if ((bp - ap) % (av - bv) == 0)
									yield return (bp - ap) / (av - bv);
							}
						}
						else
						{
							// 2nd degree equation
							var a = aa - ba;
							var b = 2*(av - bv) + (aa - ba);
							var c = 2*(ap - bp) + (aa - ba);
							var d = b*b - 4*a*c;
							if (d == 0)
							{
								if (b % (2*a) == 0)
									yield return -b / (2*a);
							}
							else
							{
								var droot = Math.Sqrt(d);
								if (droot == (int)droot) // non-integer discriminants can never yield a whole number
								{
									var d2 = (int)droot;
									if ((-b + d2) % (2*a) == 0)
										yield return (-b + d2) / (2*a);
									if ((-b - d2) % (2*a) == 0)
										yield return (-b - d2) / (2*a);
								}
							}
						}
					}
				}

			}
		}

		public static Particle[] ReadParticles(string[] input)
		{
			var id = 0;
			return input
				.Select(line =>
				{
					// p=<-1027,-979,-188>, v=<7,60,66>, a=<9,1,-7>
					var (px, py, pz, vx, vy, vz, ax, ay, az) = line
						.RxMatch("p=<%D,%D,%D>, v=<%D,%D,%D>, a=<%D,%D,%D>")
						.Get<int,int,int,int,int,int,int,int,int>();
					var myid = id++;
					return new Particle
					{
						Id = myid,
						P = new Particle.Pos(px, py, pz),
						V = new Particle.Pos(vx, vy, vz),
						A = new Particle.Pos(ax, ay, az),
					};
				})
				.ToArray();

		}
	}
}
