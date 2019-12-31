using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using AdventOfCode2019.Helpers;

namespace AdventOfCode2019.Day22
{
	internal static class Puzzle
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var shuffles = File.ReadAllLines("Day22/input.txt");
			var deck = Enumerable.Range(0, 10007).ToArray();
			deck = NaiveShuffle(deck, shuffles);
			var indexOfCard2019 = deck.ToList().IndexOf(2019);
			Console.WriteLine($"Day 22 Puzzle 1: {indexOfCard2019}");
			Debug.Assert(indexOfCard2019 == 4096);
		}

		private static void Puzzle2()
		{
			var shuffles = File.ReadAllLines("Day22/input.txt");
			var N = 119315717514047;
			var n = 101741582076661;
			var (a, c) = FormulaForPositionFor(N, 0, shuffles);
			var card2020 = FindCardAtPositionAfterShuffles(2020, a, c, n, N);
			Console.WriteLine($"Day 22 Puzzle 2: {card2020}");
			Debug.Assert(card2020 == 78613970589919);
		}

		private static int[] NaiveShuffle(int[] deck, string[] shuffles)
		{
			var Cut = "cut";
			var DealWithIncrement = "deal with increment";

			var N = deck.Length;
			foreach (var shuffle in shuffles)
			{
				if (shuffle == "deal into new stack")
				{
					deck = deck.Reverse().ToArray();
				}
				else if (shuffle.StartsWith(DealWithIncrement))
				{
					var inc = int.Parse(shuffle.Substring(DealWithIncrement.Length));
					var newdeck = new int[N];
					for (var i = 0; i < N; i++)
					{
						newdeck[(i * inc) % N] = deck[i];
					}
					deck = newdeck;
				}
				else if (shuffle.StartsWith(Cut))
				{
					var cut = int.Parse(shuffle.Substring(Cut.Length));
					cut = (cut + N) % N;
					deck = deck.Skip(cut).Concat(deck.Take(cut)).ToArray();
				}
				else
				{
					throw new Exception($"Unknown shuffle: {shuffle}");
				}
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
					a = a * inc;
					b = b * inc;
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

