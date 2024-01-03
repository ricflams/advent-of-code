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

namespace AdventOfCode.Y2023.Day24.Raw
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
			Run("test1").Part2(47);
	//		Run("test2").Part1(0).Part2(0);
	//Run("input").WithParameter((new BigInteger(200000000000000), new BigInteger(400000000000000))).Part1(15593);
			Run("input").WithParameter((new BigInteger(200000000000000), new BigInteger(400000000000000))).Part1(15593).Part2(757031940316991);
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

		private bool IsPrime(BigInteger p)
		{
			p = BigInteger.Abs(p);
			if (p == 0 || p == 1)
				return false;
			var sqrt = Math.Sqrt((double)p);
			if (new BigInteger(sqrt+1)*new BigInteger(sqrt+1) < p)
				throw new Exception();
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
					var p = s.Replace(" ", "").Replace("@", ",").Split(',').Select(BigInteger.Parse).ToArray();
					return new
					{
						P = (X: p[0], Y: p[1], Z: p[2]),
						V = (X: p[3], Y: p[4], Z: p[5])
					};
				})
				.ToArray();

			// var geo = new Geogebra();
			// foreach (var h in hails0)//.Append(new { P = new Point3D(24, 13, 10), V = new Point3D(-3, 1, 2) }))
			// {
			// 	for (var i = 0; i < 7; i++)
			// 	{
			// 		//geo.Add(new Helpers.Vector((int)h.P.X + i * (int)h.V.X, i, (int)h.V.X, 1));
			// 		geo.Add(new Helpers.Vector((int)h.P.Y + i * (int)h.V.Y, i, (int)h.V.Y, 1));
			// 	}
			// }
			// Console.WriteLine(geo.AsExecuteCommands());				


			//var fact = 2000000000000;
			//var geo = new Geogebra();
			//foreach (var h in hails0)
			//{
			//	geo.Add(new Helpers.Vector(h.P.X / fact, 0, h.V.X, 200));
			//}
			//Console.WriteLine(geo.AsExecuteCommands());

			var x = SolveIt(hails0.Select(h => h.P.X).ToList(), hails0.Select(h => h.V.X).ToList());
			var y = SolveIt(hails0.Select(h => h.P.Y).ToList(), hails0.Select(h => h.V.Y).ToList());
			var z = SolveIt(hails0.Select(h => h.P.Z).ToList(), hails0.Select(h => h.V.Z).ToList());
			Console.WriteLine($"x={x} y={y} z={z}");

			return (long)x + (long)y + (long)z;

		}

		private static BigInteger SolveIt(List<BigInteger> pos, List<BigInteger> vel)
		{
			var matches = new List<int>();
			var matches2 = new List<int>();

			var result = BigInteger.MinusOne;

			var mirror = new BigInteger(1000000000000000);
			var posinv = pos.Select(p => mirror - p).ToList();
			var velinv = vel.Select(v => -v).ToList();
			var dx = 1;
			for (; dx < 5000; dx++)
			{
				if (Solve(false, dx, pos, vel, out result)) break;
				if (Solve(true, dx, posinv, velinv, out result))
				{
					result = mirror - result;
					dx = -dx;
					break; 
				}
			}

			Console.WriteLine($"{result} {dx}");

			return result;

			bool Solve(bool neg, int v, List<BigInteger> pos, List<BigInteger> vel, out BigInteger result)
			{
				result = -1;
				var parts = new List<(BigInteger Moduli, BigInteger Rem)>();
				
				var parallels = new List<BigInteger>();
				for (var i = pos.Count; i-- > 0; )
				{
					if (vel[i] == v)
					{
						parallels.Add(pos[i]);
						vel.RemoveAt(i);
						pos.RemoveAt(i);
					}
				}

				var N = pos.Count;

				//if (vel.Any(vv => vv == v))
				//	return false;
				for (var i = 0; i < N; i++)
				{
					var pi = pos[i];
					var vi = vel[i];

					var mod = BigInteger.Abs(vel[i] - v);
					var remainder = pi;
					parts.Add((mod, pi));
				}
				
//				parts = parts.Distinct().ToList();
				var moduli = parts.Select(x => x.Moduli).ToArray();
				var residues = parts.Select(x => x.Rem).ToArray();

				//bool areCoprime = AreCoprime(moduli);

				//if (!areCoprime)
				//{
				//	Console.Write("-");
				//	return false;
				//}
				//Console.Write("+");


				// if (factors.Any(f => parts.Count(x => x.Fac == f) > 1))
				// 	return false;

				//				var p = MathHelper.LeastCommonMultiple(factors.Select(f => (long)f).Distinct().ToArray());

				try
				{
					//var remainder = MathHelper.SolveChineseRemainderTheorem(factors, remainders);
					//var remainder = SolveCRT(moduli, remainders);

					/** Works for non-coprime moduli.
					 Returns {-1,-1} if solution does not exist or input is invalid.
					 Otherwise, returns {x,L}, where x is the solution unique to mod L
*/
					var solutions = ChineseRemainderTheorem(residues, moduli);
					var p = solutions.Item1;

					if (p == -1)
						return false;

					if (parallels.Any(x => x != p))
						return false;
					
					while (pos.Any(x => x == p))
						p += solutions.Item2 * (neg ? -1 : 1);

					// var factorprod = BigInteger.One;
					// foreach (var r in factors)
					// {
					// 	factorprod *= r;
					// }			
					var match = 0;
					for (var i = 0; i < N; i++)
					{
						var pi = pos[i];
						var dv = BigInteger.Abs(vel[i] - v);
						var dist = p - pi;
						var rem = dist % dv;
						if (rem == 0)
						{
							match++;
							//break;
						}
					}
					matches2.Add(match);
					if (match >= 296)
						;
					if (match == N)
					{
						Console.WriteLine($"################### v={v}: {p} at C={solutions.Item2}");
						result = p;
						return true;
					}
					if (match >= 296)
						;
					// Console.Write($"{match}/{factors.Count} ");
					matches.Add(match);
					// return match == N;
					//p += factorprod;
				}
				catch
				{
					return false;
				}
				return false;
			}

		// 	var hailprimex = hails0.Where(h => IsPrime(h.V.X)).ToArray();

		// 	var maxx = hails0.Max(h => Math.Abs(h.P.X));
		// 	var maxv = hails0.Max(h => Math.Abs(h.V.X));
		// 	var f = maxx / maxv;

		// 	//var geo = new Geogebra();
		// 	//foreach (var h in hails0.Append(new Hail { P = new Point3D(24, 13, 10), V = new Point3D(-3, 1, 2) }))
		// 	//{
		// 	//	//for (var i = 0; i < 7; i++)
		// 	//	//{
		// 	//	//	geo.Add(new Helpers.Vector(h.P.X + i * h.V.X, i, h.V.X, 1));
		// 	//	//}
		// 	//	geo.Add(new Helpers.Vector(h.P.X/f, 0, h.V.X*f, f));
		// 	//}
		// 	//Console.WriteLine(geo.AsExecuteCommands());


		// 	var hailCache = new Dictionary<int, Point3D[]>();
		// 	Point3D[] HailsAt(int t)
		// 	{
		// 		if (!hailCache.TryGetValue(t, out Point3D[] hails))
		// 			hails = hailCache[t] = Hail.HailsAt(hails0, t);
		// 		return hails;
		// 	}

		// 	var minDx = hails0.Min(h => h.V.Y); // except the max's one
		// 	var hails = hails0.Select(h => h.P.Y).ToArray();
		// 	var vx = hails0.Select(h => h.V.Y).ToArray();

		// 	for (var t0 = 1; ; t0++)
		// 	{
		// 		if (t0 % 1000 == 0)
		// 			Console.Write('.');

		// 		var xMax = decimal.MinValue;
		// 		for (var i = 0; i < N; i++)
		// 		{
		// 			var max = hails[i] + t0*vx[i];
		// 			if (max > xMax)
		// 				xMax = max;
		// 		}
		// 		//var xMax = hails.Max();

		// 		//var tEnd = t0 + N - 1;
		// 		//var hailsAtEnd = HailsAt(tEnd);
		// //		var xMin = hailsAtEnd.Min(p => p.X);

		// 		for (var dx = minDx-1; dx > minDx*10; dx--)
		// 		{
		// 			//if (xMax + dx*N < xMin) // slope (dx) can't ever touch all hails
		// 			//	break;
		// 			var rockX0 = xMax - dx * t0;
		// 			var i = 0;
		// 			for (; i < N; i++)
		// 			{
		// 				var dv = Math.Abs(dx - vx[i]); // hack
		// 				if (dv == 0)
		// 					break;
		// 				if ((rockX0 - hails[i]) % dv != 0)
		// 					break;
		// 			}
		// 			if (i == N)
		// 			{
		// 				Console.WriteLine($"##### bingo at {t0} {dx} {rockX0}!");
		// 				return 0;
		// 			}
		// 		}

		// 	}



		// 	for (var t0 = 1;; t0++)
		// 	{
		// 		//Console.Write('+');
		// 		hails0 = Hail.Move(hails0, 1);
		// 		//var p0s = hails0.Select(h => h.P).ToArray();
		// 		for (var i = 0; i < hails0.Length; i++)
		// 		{
		// 			var hi = hails0[i];
		// 			for (var j = 0; j < hails0.Length; j++)
		// 			{
		// 				if (j == i)
		// 					continue;
		// 				for (var dt = 1; dt < 1000; dt++)
		// 				{
		// 					var hj = hails0[j].Move(dt);
		// 					var dx = Math.Abs(hj.P.X - hi.P.X);
		// 					var dy = Math.Abs(hj.P.Y - hi.P.Y);
		// 					var dz = Math.Abs(hj.P.Z - hi.P.Z);
		// 					if (dx % dt != 0 || dy % dt != 0 || dz % dt != 0)
		// 						continue;
		// 					var v = new Point3D((hj.P.X - hi.P.X)/dt, (hj.P.Y - hi.P.Y)/dt, (hj.P.Z - hi.P.Z)/dt);
		// 					var rock2 = new Hail
		// 					{
		// 						P = hi.P,
		// 						V = v
		// 					};
		// 					// Now we have start-point p0 and a pnext
		// 					// Try the remaining hails on for fit
		// 					// var remain = Hail.Copy(hails);
		// 					// remain[i].P = null;
		// 					// remain[j].P = null;
		// 					var hitall = hails0.All(h => h.HitXy(rock2) != null);
		// 					if (hitall)
		// 					{
		// 						var rock0 = rock2.Move(-t0);
		// 						var result = (long)(rock0.P.X + rock0.P.Y + rock0.P.Z);
		// 						//Console.WriteLine($"{t0+dt} {rock0} {result}");
		// 						Console.Write($".");
		// 						var ok = FullCheck(rock0, rock2, hails0);
		// 						if (ok)
		// 						{
		// 							Console.WriteLine($"##### found {rock0}");
		// 							return result;
		// 						}
		// 						break;
		// 					}
		// 				}
		// 			}

		// 		}
		// 	}



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

		static Tuple<BigInteger, BigInteger> ChineseRemainderTheorem(BigInteger[] A, BigInteger[] M)
		{
			if (A.Length != M.Length)
				return Tuple.Create(BigInteger.MinusOne, BigInteger.MinusOne); // Invalid input

			int n = A.Length;

			BigInteger a1 = A[0];
			BigInteger m1 = M[0];
			// Initially x = a_0 (mod m_0)

			// Merge the solution with remaining equations
			for (int i = 1; i < n; i++)
			{
				BigInteger a2 = A[i];
				BigInteger m2 = M[i];

				BigInteger g = GCD(m1, m2);
				if (a1 % g != a2 % g)
					return Tuple.Create(BigInteger.MinusOne, BigInteger.MinusOne); // No solution exists

				// Merge the two equations
				BigInteger p, q;
				ExtGCD(m1 / g, m2 / g, out p, out q);

				BigInteger mod = m1 / g * m2; // LCM of m1 and m2

				// We need to be careful about overflow, but I did not bother about overflow here to keep the code simple.
				BigInteger x = (a1 * (m2 / g) * q + a2 * (m1 / g) * p) % mod;

				// Merged equation
				a1 = x;
				if (a1 < 0) a1 += mod; // Result is not supposed to be negative
				m1 = mod;
			}

			return Tuple.Create(a1, m1);
		}

		static BigInteger GCD(BigInteger a, BigInteger b)
		{
			while (b != 0)
			{
				BigInteger temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}

		static void ExtGCD(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
		{
			BigInteger x0 = 1, x1 = 0, y0 = 0, y1 = 1;

			while (b != 0)
			{
				BigInteger q = a / b;
				BigInteger temp = b;
				b = a % b;
				a = temp;

				temp = x0;
				x0 = x1;
				x1 = temp - q * x1;

				temp = y0;
				y0 = y1;
				y1 = temp - q * y1;
			}

			x = x0;
			y = y0;
		}
	}
}
