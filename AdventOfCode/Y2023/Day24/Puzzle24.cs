using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Runtime.Intrinsics.X86;
using System.Numerics;
using MathNet.Numerics;
using AdventOfCode.Helpers.Arrays;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace AdventOfCode.Y2023.Day24
{
	internal class Puzzle : PuzzleWithParameter<(BigInteger, BigInteger), long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 24;

		public override void Run()
		{
			//Run("test1").WithParameter((7, 27)).Part1(2).Part2(47);
//			Run("test1").Part2(47);
	//		Run("test2").Part1(0).Part2(0);
	//Run("input").WithParameter((new BigInteger(200000000000000), new BigInteger(400000000000000))).Part1(15593);
			Run("input").WithParameter((new BigInteger(200000000000000), new BigInteger(400000000000000))).Part1(15593).Part2(0);
	//		Run("extra").Part1(0).Part2(0);
		}

		class Point3D
		{
            public Point3D(decimal x, decimal y, decimal z) => (X, Y, Z) = (x, y, z);
            public decimal X;
			public decimal Y;
			public decimal Z;
            public override string ToString() => $"({X},{Y},{Z})";
        }
		class Hail
		{
			public Point3D P;
			public Point3D V;

			public Hail Move(int t)
			{
				return new Hail
				{
					P = new Point3D(P.X + t*V.X, P.Y + t*V.Y, P.Z + t*V.Z),
					V = V
				};
			}

            public override string ToString() => $"{P} @ {V}";

			public static Hail[] Move(Hail[] hails, int t) => hails.Select(h => h.Move(t)).ToArray();
			public static Point3D[] HailsAt(Hail[] hails, int t) => hails.Select(h => h.Move(t).P).ToArray();

			public static Hail[] Copy(Hail[] hails) => Move(hails, 0);

			public Point3D HitXy(Hail o)
			{
				var h1 = this;
				var h2 = o;

				var adx = new BigInteger(h2.P.X - h1.P.X);
				var ady = new BigInteger(h2.P.Y - h1.P.Y);
				var bdx = -adx;
				var bdy = -ady;

				var adenominator = new BigInteger(h1.V.Y * h2.V.X) - new BigInteger(h1.V.X * h2.V.Y);
				var bdenominator = new BigInteger(h2.V.Y * h1.V.X) - new BigInteger(h2.V.X * h1.V.Y);

				if (BigInteger.Abs(adenominator) == 0 || BigInteger.Abs(bdenominator) == 0)
					return null;

				var at = (ady * new BigInteger(h2.V.X) - adx * new BigInteger(h2.V.Y)) / adenominator;
				var bt = (bdy * new BigInteger(h1.V.X) - bdx * new BigInteger(h1.V.Y)) / bdenominator;

				if (at < 0)
					return null;
				if (bt < 0)
					return null;

				// Calculate the intersection point
				var x = new BigInteger(h1.P.X) + at * new BigInteger(h1.V.X);
				var y = new BigInteger(h1.P.Y) + at * new BigInteger(h1.V.Y);

				Debug.Assert(x > new BigInteger(decimal.MinValue) && x < new BigInteger(decimal.MaxValue));
				Debug.Assert(y > new BigInteger(decimal.MinValue) && y < new BigInteger(decimal.MaxValue));

				return new Point3D((decimal)x, (decimal)y, 0);
			}

			// public Point3D HitXyz(Hail o)
			// {
			// 	var h1 = this;
			// 	var h2 = o;

			// 	var adx = new BigInteger(h2.P.X - h1.P.X);
			// 	var ady = new BigInteger(h2.P.Y - h1.P.Y);
			// 	var bdx = -adx;
			// 	var bdy = -ady;

			// 	var adenominator = new BigInteger(h1.V.Y * h2.V.X) - new BigInteger(h1.V.X * h2.V.Y);
			// 	var bdenominator = new BigInteger(h2.V.Y * h1.V.X) - new BigInteger(h2.V.X * h1.V.Y);

			// 	if (BigInteger.Abs(adenominator) == 0 || BigInteger.Abs(bdenominator) == 0)
			// 		return null;

			// 	var at = (ady * new BigInteger(h2.V.X) - adx * new BigInteger(h2.V.Y)) / adenominator;
			// 	var bt = (bdy * new BigInteger(h1.V.X) - bdx * new BigInteger(h1.V.Y)) / bdenominator;

			// 	if (at < 0)
			// 		return null;
			// 	if (bt < 0)
			// 		return null;

			// 		return h1.P;

			// 	// // Calculate the intersection point
			// 	// var x = h1.P.X + at * h1.V.X;
			// 	// var y = h1.P.Y + at * h1.V.Y;
			// 	// var z1 = h1.P.Z + at * h1.V.Z;
			// 	// var z2 = h2.P.Z + at * h2.V.Z;
			// 	// if (Math.Abs(z1 - z2) > (decimal)0.01)
			// 	// 	return null;

			// 	// return new Point3D(x, y, z1);
			// }			
		}

		protected override long Part1(string[] input)
		{
			var (min, max) = PuzzleParameter;
			var (minP, maxP) = ((decimal)min, (decimal)max);

			// Console.WriteLine(minP);
			// Console.WriteLine(maxP);

			var hails = input
				.Select(s =>
				{
					var p = s.Replace(" ", "").Replace("@", ",").Split(',').Select(decimal.Parse).ToArray();
					return new Hail
					{
						P = new Point3D(p[0], p[1], p[2]),
						V = new Point3D(p[3], p[4], p[5])
					};
				})
				.ToArray();

			//
			// A + Bx = C + Dy
			// E + Fx = G + Hy
			//
			// <=>
			//
			// HA + HBx = HC + HDy
			// DE + DFx = DG + DHy
			//
			// <=>
			//
			// HA + HBx - (DE + DFx) = HC + HDy - (DG + DHy)
			// <=>
			//
			// (HB - DF)x + (HA - DE) = HC - DG
			// <=>
			//
			// x = ((HC-DG) - (HA-DE)) / (HB - DF)
			// y = (A + Bx - C) / D

			var sum = 0;
			for (var i = 0; i < hails.Length; i++)
			{
				var h1 = hails[i];
				for (var j = i+1; j < hails.Length; j++)
				{
					var h2 = hails[j];
					var p = h1.HitXy(h2);
					if (p == null)
						continue;
					if (p.X >= minP && p.Y >= minP && p.X <= maxP && p.Y <= maxP)
						sum++;
				}
			}

			return sum;
		}

		private bool IsPrime(decimal p)
		{
			p = Math.Abs(p);
			if (p == 0 || p == 1)
				return false;
			var sqrt = Math.Sqrt((double)p);
			for (var i = 2; i <= sqrt; i++)
			{
				if ((p % i) == 0)
					return false;
			}
			return true;
		}

		protected override long Part2(string[] input)
		{
			var (minp, maxp) = PuzzleParameter;
			var (minP, maxP) = ((decimal)minp, (decimal)maxp);

			var hails0 = input
				.Select(s =>
				{
					var p = s.Replace(" ", "").Replace("@", ",").Split(',').Select(decimal.Parse).ToArray();
					return new Hail
					{
						P = new Point3D(p[0], p[1], p[2]),
						V = new Point3D(p[3], p[4], p[5])
					};
				})
				.ToArray();


			//var fact = 2000000000000;
			//var geo = new Geogebra();
			//foreach (var h in hails0)
			//{
			//	geo.Add(new Helpers.Vector(h.P.X / fact, 0, h.V.X, 200));
			//}
			//Console.WriteLine(geo.AsExecuteCommands());




			var matches = new List<int>();
			var N = hails0.Length;

			var pos = hails0.Select(h => h.P.X).ToArray();
			var vel = hails0.Select(h => h.V.X).ToArray();
			for (var dx = 1; dx < 10000; dx++)
			{
				if (Solve(dx) || Solve(-dx))
					break;
			}

			foreach (var m in matches.OrderDescending().Take(100))
			{
				Console.Write($"{m} ");
			}
			Console.WriteLine();

			bool Solve(int v)
			{
				var factors = new List<BigInteger>();
				var remainders = new List<BigInteger>();
				if (vel.Any(vv => vv == v))
					return false;
				for (var i = 0; i < N; i++)
				{
					var pi = pos[i];
					var vi = vel[i];
					if (Math.Sign(vi) == Math.Sign(v))
					{
						if (v > 0)
						{
							if (vi > v)
							{
								var dv = v - vi;
								if (pi % dv != 0) continue;
								factors.Add(new BigInteger(dv));
							}
							else
							{
								var dv = vi - v;
								if (pi % dv != 0) continue;
								factors.Add(new BigInteger(dv));
							}
						}
						else
						{
							if (vi > v)
							{
								var dv = v - vi;
								if (!IsPrime(dv)) continue;
							}
							else
							{
								var dv = vi - v;
								if (!IsPrime(dv)) continue;
							}
						}
					}
					else
					{

					}

					vel[i] - v;
					if (!IsPrime(dv)) continue;
					//if (dv < 0) continue;
					if ((pos[i] % dv) != 0) continue;
					factors.Add(new BigInteger(dv));
					//remainders.Add(new BigInteger(pos[i] % dv));
					//if (factors.Count == 5)
					//	break;
				}
				if (factors.Count == 0)
					return false;
				//			var remainder = MathHelper.SolveChineseRemainderTheorem(factors.ToArray(), remainders.ToArray());
				var p = MathHelper.LeastCommonMultiple(factors.Select(f => (long)f).Distinct().ToArray());
				//var p = (decimal)0;
				//try
				//{
				//	var pp = BigInteger.One;
				//	foreach (var ff in factors)
				//		pp *= ff;
				//	p = (decimal)pp;
				//}
				//catch (OverflowException)
				//{
				//	Console.Write(".");
				//	return false;
				//}

				//var factorprod = (decimal)1;
				//foreach (var r in factors)
				//{
				//	factorprod *= (decimal)r;
				//}

				//p /= remainprod;
				//var prod = remainders.ToArray().Prod();
				//p += prod;

				//for (var j = 0; j < 10000000; j++)
				//{
					var match = 0;
					for (var i = 0; i < N; i++)
					{
						// if (vel[i] == v)
						// 	continue;
						var dist = p - pos[i];
						var dv = vel[i] - v;
						var rem = dist % dv;
						if (rem != 0)
						{
							continue;
						}
						match++;
					}
					if (match == N)
						return true;
					// Console.Write($"{match}/{factors.Count} ");
					matches.Add(match);
					// return match == N;
					//p += factorprod;
				//}
				return false;
			}

			return 0;

			var hailprimex = hails0.Where(h => IsPrime(h.V.X)).ToArray();

			var maxx = hails0.Max(h => Math.Abs(h.P.X));
			var maxv = hails0.Max(h => Math.Abs(h.V.X));
			var f = maxx / maxv;

			//var geo = new Geogebra();
			//foreach (var h in hails0.Append(new Hail { P = new Point3D(24, 13, 10), V = new Point3D(-3, 1, 2) }))
			//{
			//	//for (var i = 0; i < 7; i++)
			//	//{
			//	//	geo.Add(new Helpers.Vector(h.P.X + i * h.V.X, i, h.V.X, 1));
			//	//}
			//	geo.Add(new Helpers.Vector(h.P.X/f, 0, h.V.X*f, f));
			//}
			//Console.WriteLine(geo.AsExecuteCommands());


			var hailCache = new Dictionary<int, Point3D[]>();
			Point3D[] HailsAt(int t)
			{
				if (!hailCache.TryGetValue(t, out Point3D[] hails))
					hails = hailCache[t] = Hail.HailsAt(hails0, t);
				return hails;
			}

			var minDx = hails0.Min(h => h.V.Y); // except the max's one
			var hails = hails0.Select(h => h.P.Y).ToArray();
			var vx = hails0.Select(h => h.V.Y).ToArray();

			for (var t0 = 1; ; t0++)
			{
				if (t0 % 1000 == 0)
					Console.Write('.');

				var xMax = decimal.MinValue;
				for (var i = 0; i < N; i++)
				{
					var max = hails[i] + t0*vx[i];
					if (max > xMax)
						xMax = max;
				}
				//var xMax = hails.Max();

				//var tEnd = t0 + N - 1;
				//var hailsAtEnd = HailsAt(tEnd);
		//		var xMin = hailsAtEnd.Min(p => p.X);

				for (var dx = minDx-1; dx > minDx*10; dx--)
				{
					//if (xMax + dx*N < xMin) // slope (dx) can't ever touch all hails
					//	break;
					var rockX0 = xMax - dx * t0;
					var i = 0;
					for (; i < N; i++)
					{
						var dv = Math.Abs(dx - vx[i]); // hack
						if (dv == 0)
							break;
						if ((rockX0 - hails[i]) % dv != 0)
							break;
					}
					if (i == N)
					{
						Console.WriteLine($"##### bingo at {t0} {dx} {rockX0}!");
						return 0;
					}
				}

			}



			for (var t0 = 1;; t0++)
			{
				//Console.Write('+');
				hails0 = Hail.Move(hails0, 1);
				//var p0s = hails0.Select(h => h.P).ToArray();
				for (var i = 0; i < hails0.Length; i++)
				{
					var hi = hails0[i];
					for (var j = 0; j < hails0.Length; j++)
					{
						if (j == i)
							continue;
						for (var dt = 1; dt < 1000; dt++)
						{
							var hj = hails0[j].Move(dt);
							var dx = Math.Abs(hj.P.X - hi.P.X);
							var dy = Math.Abs(hj.P.Y - hi.P.Y);
							var dz = Math.Abs(hj.P.Z - hi.P.Z);
							if (dx % dt != 0 || dy % dt != 0 || dz % dt != 0)
								continue;
							var v = new Point3D((hj.P.X - hi.P.X)/dt, (hj.P.Y - hi.P.Y)/dt, (hj.P.Z - hi.P.Z)/dt);
							var rock2 = new Hail
							{
								P = hi.P,
								V = v
							};
							// Now we have start-point p0 and a pnext
							// Try the remaining hails on for fit
							// var remain = Hail.Copy(hails);
							// remain[i].P = null;
							// remain[j].P = null;
							var hitall = hails0.All(h => h.HitXy(rock2) != null);
							if (hitall)
							{
								var rock0 = rock2.Move(-t0);
								var result = (long)(rock0.P.X + rock0.P.Y + rock0.P.Z);
								//Console.WriteLine($"{t0+dt} {rock0} {result}");
								Console.Write($".");
								var ok = FullCheck(rock0, rock2, hails0);
								if (ok)
								{
									Console.WriteLine($"##### found {rock0}");
									return result;
								}
								break;
							}
						}
					}

				}
			}



			// 	var hails = Hail.Copy(hails0);
			// 	for (var t = t0+1; t < t0+10; t++)
			// 	{
			// 		if (t%1000 == 0)
			// 			Console.Write('.');
			// 		Hail.Move(hails);
			// 		// Try on first hit after t steps
			// 		for (var i = 0; i < hails.Length; i++)
			// 		{
			// 			var p0 = p0s[i];
			// 			for (var j = 0; j < hails.Length; j++)
			// 			{
			// 				if (j == i)
			// 					continue;
			// 				var pnext = hails[j].P;
			// 				var dx = Math.Abs(p0.X - pnext.X);
			// 				var dy = Math.Abs(p0.Y - pnext.Y);
			// 				var dz = Math.Abs(p0.Z - pnext.Z);
			// 				var tp = t-1;
			// 				if (dx % tp != 0 || dy % tp != 0 || dz % tp != 0)
			// 					continue;
			// 				var v = new Point3D((pnext.X - p0.X) / tp, (pnext.Y - p0.Y) / tp, (pnext.Z - p0.Z) / tp);
			// 				var rock0 = new Hail
			// 				{
			// 					P = new Point3D(p0.X - v.X, p0.Y - v.Y, p0.Z - v.Z),
			// 					V = v
			// 				};
			// 				// Now we have start-point p0 and a pnext
			// 				// Try the remaining hails on for fit
			// 				var remain = Hail.Copy(hails);
			// 				remain[i].P = null;
			// 				remain[j].P = null;
			// 				var hitall = remain.All(h => h.P == null || h.HitXy(rock0) != null);
			// 				if (hitall)
			// 				{
			// 					var result = (long)(rock0.P.X + rock0.P.Y + rock0.P.Z);
			// 					Console.WriteLine($"{t} {rock0} {result}");
			// 					var rock2 = rock0.Move(t);
			// 					var ok = FullCheck(rock0, rock2, remain);
			// 					if (ok)
			// 					{
			// 						Console.WriteLine($"##### found {t} {rock0}");
			// 						return result;
			// 					}
			// 				}
			// 			}
			// 		}
			// 	}
			// }



			// for (var t0 = 1;; t0++)
			// {
			// 	Console.Write('+');
			// 	Hail.Move(hails0);
			// 	var p0s = hails0.Select(h => h.P).ToArray();
			// 	var hails = Hail.Copy(hails0);
			// 	for (var t = t0+1; t < t0+10; t++)
			// 	{
			// 		if (t%1000 == 0)
			// 			Console.Write('.');
			// 		Hail.Move(hails);
			// 		// Try on first hit after t steps
			// 		for (var i = 0; i < hails.Length; i++)
			// 		{
			// 			var p0 = p0s[i];
			// 			for (var j = 0; j < hails.Length; j++)
			// 			{
			// 				if (j == i)
			// 					continue;
			// 				var pnext = hails[j].P;
			// 				var dx = Math.Abs(p0.X - pnext.X);
			// 				var dy = Math.Abs(p0.Y - pnext.Y);
			// 				var dz = Math.Abs(p0.Z - pnext.Z);
			// 				var tp = t-1;
			// 				if (dx % tp != 0 || dy % tp != 0 || dz % tp != 0)
			// 					continue;
			// 				var v = new Point3D((pnext.X - p0.X) / tp, (pnext.Y - p0.Y) / tp, (pnext.Z - p0.Z) / tp);
			// 				var rock0 = new Hail
			// 				{
			// 					P = new Point3D(p0.X - v.X, p0.Y - v.Y, p0.Z - v.Z),
			// 					V = v
			// 				};
			// 				// Now we have start-point p0 and a pnext
			// 				// Try the remaining hails on for fit
			// 				var remain = Hail.Copy(hails);
			// 				remain[i].P = null;
			// 				remain[j].P = null;
			// 				var hitall = remain.All(h => h.P == null || h.HitXy(rock0) != null);
			// 				if (hitall)
			// 				{
			// 					var result = (long)(rock0.P.X + rock0.P.Y + rock0.P.Z);
			// 					Console.WriteLine($"{t} {rock0} {result}");
			// 					var rock2 = rock0.Move(t);
			// 					var ok = FullCheck(rock0, rock2, remain);
			// 					if (ok)
			// 					{
			// 						Console.WriteLine($"##### found {t} {rock0}");
			// 						return result;
			// 					}
			// 				}
			// 			}
			// 		}
			// 	}
			// }


			bool FullCheck(Hail rock0, Hail rocknow, Hail[] hails)
			{
				var hailcount = hails.Count(h => h.P != null);
				var matches = hails.Sum(h =>
				{
					if (h.P == null)
						return 0;
					if (h.P.X == rock0.P.X && h.P.Y == rock0.P.Y && h.P.Z == rock0.P.Z)
						return 1;
					var hit = h.HitXy(rock0);
					var t = (hit.X - h.P.X)/h.V.X;
					var z1 = h.P.Z + t*h.V.Z;
					var z2 = rocknow.P.Z + t*rocknow.V.Z;
					if (z1 != z2)
						return 0;
					return 1;
				});
				//Console.WriteLine($"hailcount={hailcount} matches={matches}");
				return matches == hailcount;
			}

			// void FindFit(int t0, Hail[] hails)
			// {
			// 	var p0s = hails.Select(h => h.P).ToArray();
			// 	for (var t = t0+1; t < 1000; t++)
			// 	{
			// 		for (var i = 0; i < hails.Length; i++)
			// 		{

			// 			if (FindFit)
			// 		}
			// 	}
			// }

			return 0;

			// bool LineFitxxx(decimal x, decimal dx, int t, List<(decimal P, decimal V)> remain)
			// {
			// 	while (true)
			// 	{
			// 		var dists = remain.Select(r => Math.Abs(x - r.P)).ToArray();
			// 		x += dx;
			// 		remain.Select(r => (P: r.P+r.V, r.V));
			// 		var hit = remain.FirstOrDefault(r => r.P == x);
			// 		if (hit.P == x)
			// 		{
			// 			remain.Remove(hit);
			// 			if (remain.Count == 0)
			// 				return true;
			// 			continue;
			// 		}
			// 		for (var i = 0; i < remain.Count; i++)
			// 		{
			// 			if (Math.Abs(remain[i].P - x) >= dists[i])
			// 				return false;
			// 		}
			// 	}
			// }
		}
	}
}
