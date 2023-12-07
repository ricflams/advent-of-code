using System;
using System.Collections.Generic;
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
			Run("test2").Part1(0).Part2(0);
			Run("input").Part1(253933213).Part2(253473930);
			Run("extra").Part1(253313241).Part2(253362743);
		}

		private int Value(char[] strength, string hand)
		{
			foreach (var s in strength)
			{
				if (hand.All(x => x == s))
					return 1;
			}
			foreach (var s in strength)
			{
				if (hand.Count(x => x == s) == 4)
					return 2;
			}
			foreach (var s1 in strength)
			{
				foreach (var s2 in strength)
				{
					if (s1 == s2)
						continue;
					if (hand.Count(x => x == s1) == 3 && hand.Count(x => x == s2) == 2)
						return 3;
				}						
			}
			foreach (var s in strength)
			{
				if (hand.Count(x => x == s) == 3)
					return 4;
			}
			foreach (var s1 in strength)
			{
				foreach (var s2 in strength)
				{
					if (s1 == s2)
						continue;
					if (hand.Count(x => x == s1) == 2 && hand.Count(x => x == s2) == 2)
						return 5;
				}						
			}
			foreach (var s in strength)
			{
				if (hand.Count(x => x == s) == 2)
					return 6;
			}
			return 7;
		}

		protected override long Part1(string[] input)
		{
			var hands = input
				.Select(x => x.Split(' ').ToArray())
				.Select(x => (Hand: x[0], Bid: int.Parse(x[1])))
				.ToArray();
			var strength = new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

			var orderedHands = hands
				.Select(hand => (hand, Value(strength, hand.Hand)))
				.OrderByDescending(x => x.Item2)
				.ThenBy(x => Array.IndexOf(strength, x.hand.Hand[0]))
				.ThenBy(x => Array.IndexOf(strength, x.hand.Hand[1]))
				.ThenBy(x => Array.IndexOf(strength, x.hand.Hand[2]))
				.ThenBy(x => Array.IndexOf(strength, x.hand.Hand[3]))
				.ThenBy(x => Array.IndexOf(strength, x.hand.Hand[4]))
				.ToArray();

			// Console.WriteLine();
			// foreach (var h in orderedHands.Take(20))
			// 	Console.WriteLine(h.hand);

			var wins = orderedHands.Select((x, i) => (long)x.hand.Bid * (i+1))
				.Sum();

			return wins;
		}

		protected override long Part2(string[] input)
		{
			var hands = input
				.Select(x => x.Split(' ').ToArray())
				.Select(x => (Hand: x[0], Bid: int.Parse(x[1])))
				.ToArray();
			var strength = new char[] { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };

			IEnumerable<string> Jokerize(string hand, int[] pos)
			{
				if (pos.Length == 0)
					yield return hand;
				else
				{
					foreach (var s in strength)
					{
						if (s == 'J') continue;
						var jokered = hand.ToCharArray();
						jokered[pos[0]] = s;
						var hand2 = new string(jokered);
						foreach (var j in Jokerize(hand2, pos[1..]))
							yield return j;
					}
				}
			}

			var orderedHands = hands
				.Select(hand =>
				{
					var jokers = hand.Hand.Select((h, i) => (h,i)).Where(x => x.h == 'J').Select(x => x.i).ToArray();
					// if (jokers.Length >= 4)
					// 	return (hand.Hand, Value: 1, hand.Bid);
					// if (jokers.Length >= 3)
					// 	Console.Write('#');
					// var jokered = Jokerize(hand.Hand, jokers)
					// 	.ToArray()
					// 	.Select(x => (Hand: x, Value: Value(strength, x)))
					// 	.OrderByDescending(x => x.Value)
					// 	.ThenBy(x => Array.IndexOf(strength, x.Hand[0]))
					// 	.ThenBy(x => Array.IndexOf(strength, x.Hand[1]))
					// 	.ThenBy(x => Array.IndexOf(strength, x.Hand[2]))
					// 	.ThenBy(x => Array.IndexOf(strength, x.Hand[3]))
					// 	.ThenBy(x => Array.IndexOf(strength, x.Hand[4]))						
					// 	.ToArray();
					// var jokervalue = jokered.Last().Value;
					


// 1  Five of a kind, where all five cards have the same label: AAAAA
// 2  Four of a kind, where four cards have the same label and one card has a different label: AA8AA
// 3  Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
// 4  Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
// 5  Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
// 6  One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
// 7  High card, where all cards' labels are distinct: 23456					

// JJJJJ _ -> 1

// JJJJa _ -> 1

// JJJaa 3 -> 1
// JJJab 4 -> 2

// JJaaa 3 -> 1
// JJaab 5 -> 2
// JJabc 6 -> 4

// Jaaaa 2 -> 1 
// Jaaab 4 -> 2
// Jaabb 5 -> 3
// Jaabc 6 -> 4
// Jabcd 7 -> 6

// abcde -> value

					var cards = hand.Hand;
					var njokers = hand.Hand.Count(c => c == 'J');
					if (njokers > 0 && njokers < 5)
					{
						var most = cards
							.ToArray()
							.Where(c => c != 'J')
							.GroupBy(c => c)
							.OrderByDescending(x => x.Count())
							.Select(x => x.Key)
							.First();
						cards = cards.Replace('J', most);
					}
					var jokervalue2 = Value(strength, cards);
					// var njokers = hand.Hand.Count(c => c == 'J');
					// var value = Value(strength, hand.Hand);
					// var jokervalue2 = njokers switch
					// {
					// 	5 => 1,
					// 	4 => 1,
					// 	3 => value switch
					// 		{
					// 			3 => 1,
					// 			4 => 2,
					// 			_ => throw new Exception()
					// 		},
					// 	2 => value switch
					// 		{
					// 			3 => 1,
					// 			5 => 2,
					// 			6 => 4,
					// 			_ => throw new Exception()
					// 		},
					// 	1 => value switch
					// 		{
					// 			2 => 1,
					// 			4 => 2,
					// 			5 => 3,
					// 			6 => 4,
					// 			7 => 6,
					// 			_ => throw new Exception()
					// 		},
					// 	_ => value
					// };

					// if (jokervalue != jokervalue2)
					// 	Console.WriteLine($"{hand.Hand}: {jokervalue2} should be {jokervalue}");

					return (hand.Hand, Value: jokervalue2, hand.Bid);
				})
				.OrderByDescending(x => x.Value)
				.ThenBy(x => Array.IndexOf(strength, x.Hand[0]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[1]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[2]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[3]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[4]))
				.ToArray();

			// Console.WriteLine();
			// foreach (var h in orderedHands.Take(5))
			// 	Console.WriteLine(h.Hand);

			var wins = orderedHands.Select((x, i) => (long)x.Bid * (i+1))
				.Sum();

			return wins;
		}
	}
}
