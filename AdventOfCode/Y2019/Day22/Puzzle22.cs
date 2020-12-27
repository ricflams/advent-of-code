using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Y2019.Day22
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 22;

		public void Run()
		{
			RunFor("input", 4096, 78613970589919);
		}

		protected override long Part1(string[] input)
		{
			var shuffles = input;
			var deck = Enumerable.Range(0, 10007).ToArray();
			deck = NaiveShuffle(deck, shuffles);
			var indexOfCard2019 = deck.ToList().IndexOf(2019);
			return indexOfCard2019;
		}

		protected override long Part2(string[] input)
		{
			var shuffles = input;
			var N = 119315717514047;
			var n = 101741582076661;
			var (a, c) = FormulaForPositionFor(N, 0, shuffles);
			var card2020 = FindCardAtPositionAfterShuffles(2020, a, c, n, N);
			return card2020;
		}

		private static int[] NaiveShuffle(int[] deck, string[] shuffles)
		{
			var N = deck.Length;
			foreach (var shuffle in shuffles)
			{
				if (shuffle == "deal into new stack")
				{
					deck = deck.Reverse().ToArray();
					continue;
				}
				if (SimpleRegex.IsMatch(shuffle, "deal with increment %d", out var incval))
				{
					var inc = int.Parse(incval[0]);
					var newdeck = new int[N];
					for (var i = 0; i < N; i++)
					{
						newdeck[(i * inc) % N] = deck[i];
					}
					deck = newdeck;
					continue;
				}
				if (SimpleRegex.IsMatch(shuffle, "cut %d", out var cutval))
				{
					var cut = int.Parse(cutval[0]);
					cut = (cut + N) % N;
					deck = deck.Skip(cut).Concat(deck.Take(cut)).ToArray();
					continue;
				}
				throw new Exception($"Unknown shuffle: {shuffle}");
			}
			return deck;
		}

		private static (long, long) FormulaForPositionFor(long N, long card, string[] shuffles)
		{
			var Cut = "cut";
			var DealWithIncrement = "deal with increment";

			var a = 1L;
			var b = 0L;
			foreach (var shuffle in shuffles)
			{
				if (shuffle == "deal into new stack")
				{
					a = -a;
					b = N - 1 - b;
				}
				else if (shuffle.StartsWith(DealWithIncrement))
				{
					var inc = int.Parse(shuffle.Substring(DealWithIncrement.Length));
					a *= inc;
					b *= inc;
				}
				else if (shuffle.StartsWith(Cut))
				{
					long cut = int.Parse(shuffle.Substring(Cut.Length));
					cut = (cut + N) % N;
					b -= cut;
				}
				else
				{
					throw new Exception($"Unknown shuffle: {shuffle}");
				}

				a = (a + N) % N;
				b = (b + N) % N;
			}
			return (a, b);
		}

		private static long FindCardAtPositionAfterShuffles(long cardN, BigInteger a, BigInteger c, long n, BigInteger N)
		{
			// Card are shuffled as a geometric sequence:
			//    card(n) = a * card(n-1) + c
			// => cardN = a^n*card0 + c*(a^n - 1)/(a - 1) mod N
			// => cardN = (a^n mod N)*card0 + c*ModInverse(a - 1, N)(a^n - 1) mod N
			// => (a^n mod N)*card0 = cardN - c*ModInverse(a - 1, N)*(a^n - 1) mod N
			// => card0 = (cardN - c*ModInverse(a - 1, N)*(a^n - 1) mod N) * ModInverse((a^n mod N), N)
			var aPowN = BigInteger.ModPow(a, n, N);
			var cardNmodN = c * (a - 1).ModInverse(N) * (aPowN - 1) % N;
			var card0 = (cardN - cardNmodN + N) * aPowN.ModInverse(N) % N;
			return (long)card0;
		}
	}
}

