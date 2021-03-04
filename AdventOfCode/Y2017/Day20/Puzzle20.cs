using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2017.Day20
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Particle Swarm";
		public override int Year => 2017;
		public override int Day => 20;

		public void Run()
		{
			RunPart1For("test1", 0);
			RunPart2For("test2", 1);
			RunFor("input", 119, 471);
		}

		protected override int Part1(string[] input)
		{
			var particles = ReadParticles(input);

			// Just pick a large number of steps;
			// above 1000 seems to do it, so we pick 10000
			var step = 10000;
			var closest = particles.OrderBy(p => p.Step(step).Dist).First();

			return closest.Id;
		}

		protected override int Part2(string[] input)
		{
			var particles = ReadParticles(input);

			// Loop all pairs of particles and find out at what step, if any,
			// they collide. This will give a list of collidin particles at every
			// step where there are any collisions at all.
			var collisionsAtStep = new SafeDictionary<int, HashSet<int>>(() => new HashSet<int>());
			for (var i = 0; i < particles.Length; i++)
			{
				for (var j = i + 1; j < particles.Length; j++)
				{
					var n = particles[i].CollisionAt(particles[j]);
					if (n > 0)
					{
						collisionsAtStep[n].Add(i);
						collisionsAtStep[n].Add(j);
					}
				}
			}

			// Now go through the steps for which there are collisions, starting
			// from the smallest as a real simulation would do. At every such step
			// we look at what particles have not yet been destroyed; if two or more
			// particles are still present at this step then they are both destroyed,
			// byt if only one is still undestroyed at this step it will remain. At
			// the end of this, all destroyed particles will have been placed in the
			// destroyed-collection.
			var destroyed = new HashSet<int>();
			foreach (var n in collisionsAtStep.Keys.OrderBy(x => x))
			{
				var remains = collisionsAtStep[n].Except(destroyed).ToArray();
				if (remains.Count() > 1)
				{
					destroyed.UnionWith(remains);
				}
			}

			// The number of particles left are those that weren't destroyed
			var uncollided = particles.Length - destroyed.Count();

			return uncollided;
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

		internal class Particle
		{
			internal class Pos
			{
				 public Pos(long x, long y, long z) => (X, Y, Z) = (x, y, z);
				 public long X, Y, Z;
				 public long Dist => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
				 public static Pos operator +(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
				 public static Pos operator *(int n, Pos a) => new Pos(a.X * n, a.Y * n, a.Z * n);
			}
			public int Id { get; set; }
			public Pos P { get; set; }
			public Pos V { get; set; }
			public Pos A { get; set; }

			public Pos Step(int n) => P + n * V + n*(n+1)/2 * A;

			public int CollisionAt(Particle p)
			{
				// Find all collisions for all three x,y,z dimensions
				var nx = DimensionCollisions(P.X, V.X, A.X, p.P.X, p.V.X, p.A.X);
				var ny = DimensionCollisions(P.Y, V.Y, A.Y, p.P.Y, p.V.Y, p.A.Y);
				var nz = DimensionCollisions(P.Z, V.Z, A.Z, p.P.Z, p.V.Z, p.A.Z);

				// Find out which collisions happens at the same time. A null result
				// means "is always the same" (ie the particles follow the exact same
				// path in that dimension; yes, that happens) ie no restrictions.
				var ps = nx ?? ny ?? nz;
				if (ps == null)
				{
					// No collisions in ANY dimension? This will only happen for particles
					// that are exactly the same in all 3 dimensions and there are none of
					// those in the input.
					throw new Exception("No collisions in any dimension");
				}
				if (nx != null) ps.IntersectWith(nx);
				if (ny != null) ps.IntersectWith(ny);
				if (nz != null) ps.IntersectWith(nz);

				// Get earliest collision in all dimensions, if any; 0 means none
				return ps.Any() ? ps.Min() : 0;

				static HashSet<int> DimensionCollisions(long p0, long v0, long a0, long p1, long v1, long a1)
				{
					// If tow two particles follow the exact same path in this
					// dimension then they will match at any step; return null
					if (a1 == a0 && v1 == v0 && p1 == p0)
						return null;

					// Find all solutions to the collision-equation that are positive, whole steps
					return SolveCollisionEquation()
						.Where(x => x > 0 && x == (int)x)
						.Select(x => (int)x)
						.ToHashSet();

					IEnumerable<double> SolveCollisionEquation()
					{
						// The step-equation for a particle is:
						//    Pn = P + V*n + n*(n+1)/2 * A
						//       = P + V*n + A/2*n^2 + A/2*n
						//       = A/2*n^2 + V*n + A/2*n + P
						//       = (A*n^2 + (2*V+A)*n + 2*P) / 2    // divide by 2 outside is easier
						//
						// For two particles P0,P1 to collide, their positions P0n,P1n must be the same.
						// This leads to a 2nd degree equation for solving n:
						//      P0n = P1n
						//  <=> P1n - P0n == 0
						//  <=> (A1-A0) * n^2 + (2*(V1-V0)+(A1-A0)) * n + 2*(P1-P0) == 0
						//  <=> an^2 + bn + c == 0, where
						//          a = (A1-A0)
						//          b = 2*(V1-V0)+(A1-A0)
						//          c = 2*(P1-P0)
						//
						// In the case where a==0 we're just dealing with a 1st degree equation:
						//      2*(V1-V0) * n + 2*(P1-P0) == 0
						//  <=> n = -2*(P1-P0) / 2*(V1-V0)
						//  <=> n = (P0-P1) / (V1-V0)
						if (a1 == a0)
						{
							// 1st degree equation, unsolveable if v1 == v0
							if (v1 != v0)
							{
								yield return (double)(p0 - p1) / (v1 - v0);
							}
						}
						else
						{
							// 2nd degree equation; see above
							var a = a1 - a0;
							var b = 2*(v1 - v0) + (a1 - a0);
							var c = 2*(p1 - p0);
							var d = b*b - 4*a*c;
							if (d == 0) // only 1 solution
							{
								yield return (double)-b / (2*a);
							}
							else // 2 solutions
							{
								var droot = Math.Sqrt(d);
								if (droot == (int)droot) // only integer discriminants can yield a whole number
								{
									var dint = (int)droot;
									yield return (double)(-b + dint) / (2*a);
									yield return (double)(-b - dint) / (2*a);
								}
							}
						}
					}
				}
			}
		}
	}
}
