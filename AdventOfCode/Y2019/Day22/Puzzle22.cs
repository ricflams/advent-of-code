﻿using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Y2019.Day22
{
	internal class Puzzle : Puzzle<int, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Slam Shuffle";
		public override int Year => 2019;
		public override int Day => 22;

		public override void Run()
		{
			Run("input").Part1(4096).Part2(78613970589919);
			Run("extra").Part1(2519).Part2(58966729050483);
		}

		protected override int Part1(string[] input)
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
				}
				else if (shuffle.IsRxMatch("deal with increment %d", out var captures))
				{
					var inc = captures.Get<int>();
					var newdeck = new int[N];
					for (var i = 0; i < N; i++)
					{
						newdeck[(i * inc) % N] = deck[i];
					}
					deck = newdeck;
				}
				else if (shuffle.IsRxMatch("cut %d", out captures))
				{
					var cut = captures.Get<int>();
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

