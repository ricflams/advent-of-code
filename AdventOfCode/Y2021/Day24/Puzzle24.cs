using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day24
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Arithmetic Logic Unit";
		public override int Year => 2021;
		public override int Day => 24;

		public void Run()
		{
			Run("input").Part1(53999995829399).Part2(11721151118175);
		}

		protected override long Part1(string[] input)
		{
			return FindModelNumber(input, true);
		}

		protected override long Part2(string[] input)
		{
			return FindModelNumber(input, false);
		}

		private static long FindModelNumber(string[] input, bool largest)
		{
			//  0:  W = d;         // inp w
			//  1:  X = 0;         // mul x 0
			//  2:  X += Z;        // add x z
			//  3:  X %= 26;       // mod x 26  ---> Z is always positive, >0
			//  4:  Z /= f[0];     // div z f0
			//  5:  X -= f[1];     // add x f1
			//  6:  X = X==W?1:0;  // eql x w
			//  7:  X = X==0?1:0;  // eql x 0
			//  8:  Y = 0;         // mul y 0
			//  9:  Y += 25;       // add y 25
			// 10:  Y *= X;        // mul y x
			// 11:  Y += 1;        // add y 1
			// 12:  Z *= Y;        // mul z y
			// 13:  Y = 0;         // mul y 0
			// 14:  Y += W;        // add y w
			// 15:  Y += f[2];     // add y f2
			// 16:  Y *= X;        // mul y x
			// 17:  Z += Y;        // add z y
			//
			// Can be rewritten as:
			// z = z % 26 + f[1] == d
			//     ? z / f[0]
			//     : z / f[0] * 26 + f[2] + d

			var N = 14;
			var factors = Enumerable.Range(0, N)
				.Select(i =>
				{
					var part = input.Skip(i * input.Length / N).ToArray();
					return new int[]
					{
						int.Parse(part[4].Split(' ')[2]),
						int.Parse(part[5].Split(' ')[2]),
						int.Parse(part[15].Split(' ')[2])
					};
				})
				.ToArray();

			var digitOrder = largest
				? new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }
				: new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var memo = new Dictionary<long, bool>();

			// Find model-number
			var digits = new int[N];
			if (!IsAcceptable(0, 0))
				throw new Exception("No solution");
			var modelNumber = digits.Aggregate(0L, (s, v) => s * 10 + v);

			return modelNumber;

			bool IsAcceptable(int pos, int z)
			{
				// After testing all digits z must be 0
				if (pos == N)
					return z == 0;

				// Impose an arbitrary high limit on max z to avoid running amok
				if (z > 10_000_000)
					return false;

				// Use long as memo-key because it's twice as fast for lookup
				// than a formatted string of eg "pos-z". Shift pos out of z's range.
				var key = z + pos * 1_000_000_000;
				if (memo.TryGetValue(key, out var ok))
					return ok;

				// Now find the first digit that produce a z-value that satisfies
				// the rest of the model-number digits too; that's the digit we want.
				// (note: I though it would be necessary to also check for z1<0 to
				// guard against negative modulus but turns out z1 is never negative)
				var f = factors[pos];
				foreach (var d in digitOrder)
                {
                    var z1 = z % 26 + f[1] == d
                        ? z / f[0]
                        : z / f[0] * 26 + f[2] + d;
                    if (IsAcceptable(pos + 1, z1))
                    {
						memo[key] = true;
						digits[pos] = d;
						return true;
					}
				}
				memo[key] = false;
				return false;
			}
		}
	}
}
