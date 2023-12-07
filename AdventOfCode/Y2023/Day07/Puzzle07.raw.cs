using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.ComponentModel;

namespace AdventOfCode.Y2023.Day07.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 7;

		public override void Run()
		{
			Run("test1").Part1(6440).Part2(5905);
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
					var jokered = Jokerize(hand.Hand, jokers)
						.ToArray()
						.Select(x => (Hand: x, Value: Value(strength, x)))
						.OrderByDescending(x => x.Value)
						.ThenBy(x => Array.IndexOf(strength, x.Hand[0]))
						.ThenBy(x => Array.IndexOf(strength, x.Hand[1]))
						.ThenBy(x => Array.IndexOf(strength, x.Hand[2]))
						.ThenBy(x => Array.IndexOf(strength, x.Hand[3]))
						.ThenBy(x => Array.IndexOf(strength, x.Hand[4]))						
						.ToArray();
					return (hand.Hand, jokered.Last().Value, hand.Bid);
				})
				.OrderByDescending(x => x.Value)
				.ThenBy(x => Array.IndexOf(strength, x.Hand[0]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[1]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[2]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[3]))
				.ThenBy(x => Array.IndexOf(strength, x.Hand[4]))
				.ToArray();

			Console.WriteLine();
			foreach (var h in orderedHands.Take(5))
				Console.WriteLine(h.Hand);

			var wins = orderedHands.Select((x, i) => (long)x.Bid * (i+1))
				.Sum();

			return wins;
		}
	}
}
