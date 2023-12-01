using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2018.Day14
{
	internal class Puzzle : Puzzle<long, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Chocolate Charts";
		public override int Year => 2018;
		public override int Day => 14;

		public void Run()
		{
			Run("test11").Part1(0124515891);
			Run("test12").Part2(5);
			Run("test21").Part1(5158916779);
			Run("test22").Part2(9);
			Run("test31").Part1(9251071085);
			Run("test32").Part2(18);
			Run("test41").Part1(5941429882);
			Run("test42").Part2(2018);
			Run("input").Part1(1413131339).Part2(20254833);
		}

		protected override long Part1(string[] input)
		{
			const int Recipies = 10;
			var N = int.Parse(input[0]);

			// Make room for N recipies followed by the NRecipies and 1 more
			// because the last recipe when we're just 1 short may produce 2,
			// not just one.
			// Init with fixed values 3 and 7
			var r = new int[N + Recipies + 1];
			var e1 = 0;
			var e2 = 1;
			r[e1] = 3;
			r[e2] = 7;

			// Produce required number of recipies
			for (var i = 2; i < N + Recipies; )
			{
				var n = r[e1] + r[e2];
				if (n >= 10)
				{
					r[i++] = 1;
					n -= 10;
				}
				r[i++] = n;
				e1 = (e1 + 1 + r[e1]) % i;
				e2 = (e2 + 1 + r[e2]) % i;
			}

			// The score is long so we can't use linq Aggregate
			var score = 0L;
			for (var i = 0; i < Recipies; i++)
			{
				score = score*10 + r[N+i];
			}

			return score;
		}

		protected override int Part2(string[] input)
		{
			var scorePattern = input[0];

			// Use a string as input to cater for leading 0
			var scores = scorePattern.Select(c => c - '0').ToArray();
			var N = scores.Count();

			// For simplicity simply allocated "enough" space
			// Init with fixed values 3 and 7
			var r = new int[25_000_000];
			var e1 = 0;
			var e2 = 1;
			r[e1] = 3;
			r[e2] = 7;

			// Produce recipies until we discover the desired scores
			for (var i = 2;; )
			{
				var n = r[e1] + r[e2];

				if (n >= 10)
				{
					r[i++] = 1;
					if (IsMatch(i))
						return i-N;					
					n -= 10;
				}

				r[i++] = n;
				if (IsMatch(i))
					return i-N;					

				// Modulus is a bit expensive and rarely in play, so optimize
				// by only engaging it when needed, replacing this:
				// e1 = (e1 + 1 + r[e1]) % i;
				// e2 = (e2 + 1 + r[e2]) % i;
				e1 += 1 + r[e1];
				e2 += 1 + r[e2];
				if (e1 >= i) e1 %= i;
				if (e2 >= i) e2 %= i;
			}
			throw new Exception("No solution");

			bool IsMatch(int i)
			{
				for (var n = 1; n <= N; n++)
				{
					if (r[i-n] != scores[N-n])
						return false;
				}
				return true;
			}
		}
	}
}
