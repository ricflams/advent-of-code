using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day22
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Crab Combat";
		public override int Year => 2020;
		public override int Day => 22;

		public void Run()
		{
			RunFor("test1", 306, 291);
			RunFor("input", 32179, 30498);
		}

		protected override int Part1(string[] input)
		{
			var rawdecks = input.GroupByEmptyLine().ToArray();
			var deck1 = Deck.ParseFrom(rawdecks[0]);
			var deck2 = Deck.ParseFrom(rawdecks[1]);

			var winner = PlayCombat(deck1, deck2);
			return winner.Score;

			static Deck PlayCombat(Deck d1, Deck d2)
			{
				while (d1.HasCards && d2.HasCards)
				{
					var card1 = d1.DrawTopCard();
					var card2 = d2.DrawTopCard();
					if (card1 > card2)
					{
						d1.AddToBottom(card1, card2);
					}
					else
					{
						d2.AddToBottom(card2, card1);
					}
				}
				return d1.HasCards ? d1 : d2;
			}
		}

		protected override int Part2(string[] input)
		{
			var rawdecks = input.GroupByEmptyLine().ToArray();
			var deck1 = Deck.ParseFrom(rawdecks[0]);
			var deck2 = Deck.ParseFrom(rawdecks[1]);

			var winner = PlayRecursiveCombat(deck1, deck2);
			return winner == Player.One ? deck1.Score : deck2.Score;

			static Player PlayRecursiveCombat(Deck d1, Deck d2)
			{
				var seen = new HashSet<uint>();
				while (d1.HasCards && d2.HasCards)
				{
					var hand = d1.Hand; // One hand is unique enough
					if (seen.Contains(hand))
					{
						return Player.One;
					}
					seen.Add(hand);

					var card1 = d1.DrawTopCard();
					var card2 = d2.DrawTopCard();
					var winner = d1.Count >= card1 && d2.Count >= card2
						? PlayRecursiveCombat(d1.CopyOf(card1), d2.CopyOf(card2))
						: card1 > card2 ? Player.One : Player.Two;

					if (winner == Player.One)
					{
						d1.AddToBottom(card1, card2);
					}
					else
					{
						d2.AddToBottom(card2, card1);
					}
				}
				return d1.HasCards ? Player.One : Player.Two;
			}
		}

		internal enum Player { One, Two };

		internal class Deck : Queue<byte>
		{
			private Deck(IEnumerable<byte> cards) : base(cards) { }
			public static Deck ParseFrom(string[] s) => new Deck(s.Skip(1).Select(byte.Parse));
			public byte DrawTopCard() => Dequeue();
			public void AddToBottom(byte card1, byte card2) { Enqueue(card1); Enqueue(card2); }
			public bool HasCards => this.Any();
			public int Score => this.Select((card, index) => card * (this.Count() - index)).Sum();
			public uint Hand => Hashing.JenkinsHash(this);
			public Deck CopyOf(int n) => new Deck(this.Take(n));
		}
	}
}
