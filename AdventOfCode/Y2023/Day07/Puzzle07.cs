using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day07
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Camel Cards";
		public override int Year => 2023;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(6440).Part2(5905);
			Run("input").Part1(253933213).Part2(253473930);
			Run("extra").Part1(253313241).Part2(253362743);
		}

		protected override long Part1(string[] input)
		{
			var strengths = new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

			var wins = CalculateWins(input, (cards, bid) =>
			{
				var strength = Hand.CalculateStrength(strengths, cards);
				var value = Hand.CalculateValue(strengths, cards);
				return new Hand(strength, value, bid);
			});

			return wins;
		}

		protected override long Part2(string[] input)
		{
			var strengths = new char[] { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };

			var wins = CalculateWins(input, (cards, bid) =>
			{
				// Replace any joker J with the most card that yields the highest value.
				// Seems it will always be the most frequent card but calculating the
				// max-value is about just as fast as finding the most frequent card.
				var njokers = cards.Count(c => c == 'J');
				var strength = njokers == 0 && njokers == 5
					? Hand.CalculateStrength(strengths, cards)
					: cards.Select(c => Hand.CalculateStrength(strengths, cards.Replace('J', c))).Max();

				var value = Hand.CalculateValue(strengths, cards);
				return new Hand(strength, value, bid);
			});

			return wins;
		}

		private static long CalculateWins(string[] input, Func<string, int, Hand> MakeHand) =>
			input
				.Select(x => x.Split(' ').ToArray())
				.Select(x =>
				{
 					var (cards, bid) = (x[0], int.Parse(x[1]));
					return MakeHand(cards, bid);
				})
				.OrderBy(h => h.Strength)
				.ThenBy(h => h.Value)
				.Select((h, i) => (long)(i+1) * h.Bid)
				.Sum();	

		private class Hand(int strength, int value, int bid)
		{
			public int Strength { get; private set; } = strength;
			public int Value { get; private set; } = value;
			public int Bid { get; private set; } = bid;

			public static int CalculateStrength(char[] strengths, string cards)
			{
				// Find out how many there is of all the cards;
				// eg cardCount 'Q' == 2 means there are two Qs
				var cardCount = new int[strengths.Length+1];
				foreach (var c in cards)
				{
					var v = Array.IndexOf(strengths, c);
					cardCount[v]++;
				}

				// Now find out how many of the kinds there are;
				// Eg kinds[3] == 1 means that there is one set of threes, ie three of a kind
				var kinds = new int[6];
				foreach (var v in cardCount)
				{
					kinds[v]++;
				}

				// Give cards strengths from 7 (5 of a kind, strongest) to 1 (weakest)
				var strength =
					kinds[5] == 1 ? 7 :
					kinds[4] == 1 ? 6 :
					kinds[3] == 1 ? (kinds[2] == 1 ? 5 : 4) :
					kinds[2] == 2 ? 3 :
					kinds[2] == 1 ? 2 :
					1;

				return strength;
			}

			public static int CalculateValue(char[] strengths, string cards)
			{
				// Calculate the value of the cards as one single number
				// which is optimal for sorting
				var value = 0;
				foreach (var c in cards)
				{
					var v =  Array.IndexOf(strengths, c);
					value = value * strengths.Length + v;
				}

				return value;
			}
		}

	}
}
