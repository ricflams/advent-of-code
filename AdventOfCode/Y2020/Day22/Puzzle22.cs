using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2020.Day22
{
	interface IDeck
	{
		byte DrawTopCard();
		void AddToDeck(byte card1, byte card2);
		bool HasCards { get; }
		int Score { get; }
		int Count { get; }
		//string Hand { get; }
		uint Hand { get; }
		IDeck CopyOf(int n);
	}

	[DebuggerDisplay("{ToString()}")]
	internal class Deck0 : List<byte>, IDeck
	{
		public static Deck0 ParseFrom(string[] s)
		{
			return new Deck0(s.Skip(1).Select(byte.Parse));
		}

		private Deck0(IEnumerable<byte> cards)
			: base(cards)
		{
		}

		public byte DrawTopCard()
		{
			var card = this.First();
			RemoveAt(0);
			return card;
		}

		public void AddToDeck(byte card1, byte card2)
		{
			Add(card1);
			Add(card2);
		}

		public bool HasCards => this.Any();

		public int Score => this.Select((card, index) => card * (this.Count() - index)).Sum();

		public int Count => this.Count();

		//public ulong Hand => 0;// string.Join("-", this);
		public uint Hand => Hashing.JenkinsHash(this);

		public IDeck CopyOf(int n) => new Deck0(this.Take(n));

		public override string ToString()
		{
			return $"[{string.Join(" ", this)}] count={Count}";
		}
	};




	[DebuggerDisplay("{ToString()}")]
	internal class Deck : IDeck
	{
		private readonly byte[] _deck;
		private int _size;
		private int _head;
		private int _tail;

		public Deck(string[] s)
			: this(s.Skip(1).Select(byte.Parse).ToArray(), (s.Length-1)*2 + 1)
		{
		}

		public Deck(IEnumerable<byte> values, int size)
		{
			_size = size;
			_head = 0;
			_tail = 0;
			_deck = new byte[_size];
			foreach (var v in values)
			{
				_deck[_tail++] = v;
			}
		}

		public byte DrawTopCard()
		{
			var card = _deck[_head];
			_head = (_head + 1) % _deck.Length;
			return card;
		}

		public void AddToDeck(byte card1, byte card2)
		{
			_deck[_tail % _size] = card1;
			_deck[(_tail + 1) % _size] = card2;
			_tail = (_tail + 2) % _size;
		}

		public bool HasCards => _head != _tail;

		public int Count => (_tail - _head + _size) % _size;

		public int Score
		{
			get
			{
				var cards = Cards.ToArray();
				var score = cards.Select((card, index) => card * (cards.Length - index)).Sum();
				return score;
			}
		}
			
		private IEnumerable<byte> Cards
		{
			get
			{
				for (var pos = _head; pos != _tail; pos = (pos + 1 + _size) % _size)
				{
					yield return _deck[pos];
				}
			}
		}

		//public string Hand => Hashing.Hash(Cards.ToArray()).ToString();
		public uint Hand => Hashing.JenkinsHash(Cards);

		public IDeck CopyOf(int n)
		{
			return new Deck(Cards.Take(n), _size);
		}

		public override string ToString()
		{
			return $"[{string.Join(" ", Cards)}] count={Count} head={_head} tail={_tail} size={_size}";
		}
	};



	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 22;

		public void Run()
		{
			//RunFor("test1", 306, 291);
			//RunFor("input", 32179, 30498);
			RunPart2For("input", 30498);
		}

		protected override int Part1(string[] input)
		{
			var rawdecks = input.GroupByEmptyLine().ToArray();
			//var deck1 = Deck.ParseFrom(rawdecks[0]);
			//var deck2 = Deck.ParseFrom(rawdecks[1]);
			var deck1 = new Deck(rawdecks[0]);
			var deck2 = new Deck(rawdecks[1]);

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
						d1.AddToDeck(card1, card2);
					}
					else
					{
						d2.AddToDeck(card2, card1);
					}
				}
				return d1.HasCards ? d1 : d2;
			}
		}

		internal enum Player { One, Two };

		protected override int Part2(string[] input)
		{
			var rawdecks = input.GroupByEmptyLine().ToArray();
			var deck1 = Deck0.ParseFrom(rawdecks[0]);
			var deck2 = Deck0.ParseFrom(rawdecks[1]);
			//var deck1 = new Deck(rawdecks[0]);
			//var deck2 = new Deck(rawdecks[1]);


			var memo = new Dictionary<ulong, Player>();
			var winner = PlayRecursiveCombat(deck1, deck2);
			Console.WriteLine($"memo={memo.Count}");
			return winner == Player.One ? deck1.Score : deck2.Score;

			Player PlayRecursiveCombat(IDeck d1, IDeck d2)
			{
				var seen = new HashSet<ulong>();
				while (d1.HasCards && d2.HasCards)
				{
					var hands = (ulong)d1.Hand << 32 | d2.Hand;
					if (memo.TryGetValue(hands, out var subgamewinner))
					{
						return subgamewinner;
					}

					var seenit = seen.Contains(hands);
					if (seen.Contains(hands))
					{
						return Player.One;
					}
					seen.Add(hands);

					var card1 = d1.DrawTopCard();
					var card2 = d2.DrawTopCard();
					var winner = d1.Count >= card1 && d2.Count >= card2
						? PlayRecursiveCombat(d1.CopyOf(card1), d2.CopyOf(card2))
						: card1 > card2 ? Player.One : Player.Two;

					if (winner == Player.One)
					{
						d1.AddToDeck(card1, card2);
					}
					else
					{
						d2.AddToDeck(card2, card1);
					}
				}

				var gamewinner = d1.HasCards ? Player.One : Player.Two;
				memo[(ulong)d1.Hand << 32 | d2.Hand] = gamewinner;
				return gamewinner;
			}



			//Player PlayRecursiveCombat(Deck d1, Deck d2)//, Deck0 d01, Deck0 d02)
			//{
			//	var seen = new HashSet<ulong>();
			//	//var seen0 = new HashSet<string>();
			//	while (/*d1.HasCards && d2.HasCards*/true)
			//	{
			//		var hascards = d1.HasCards && d2.HasCards;
			//		//var hascards0 = d01.HasCards && d02.HasCards;
			//		//if (hascards != hascards0) throw new Exception();
			//		if (!hascards)
			//			break;

			//		//if (d1.Score != d01.Score) throw new Exception();
			//		//if (d2.Score != d02.Score) throw new Exception();

			//		//var hands = d1.Hand + "/" + d2.Hand;
			//		var hands = ((ulong)d1.Hand << 32) | d2.Hand;

			//		//var hands0 = d01.Hand + "/" + d02.Hand;
			//		//if (memo.TryGetValue(hands, out var subgamewinner))
			//		//{
			//		//	return subgamewinner;
			//		//}

			//		var seenit = seen.Contains(hands);
			//		//var seenit0 = seen0.Contains(hands0);
			//		//if (seenit != seenit0) throw new Exception();
			//		if (seenit)
			//		{
			//			return Player.One;
			//		}
			//		seen.Add(hands);
			//		//seen0.Add(hands0);
			//		//if (seen.Contains(hands))
			//		//{
			//		//	return Player.One;
			//		//}
			//		//seen.Add(hands);


			//		var card1 = d1.DrawTopCard();
			//		var card2 = d2.DrawTopCard();
			//		//var card01 = d01.DrawTopCard();
			//		//var card02 = d02.DrawTopCard();
			//		//if (card1 != card01) throw new Exception();
			//		//if (card2 != card02) throw new Exception();

			//		//if (d1.Score != d01.Score) throw new Exception();
			//		//if (d2.Score != d02.Score) throw new Exception();
			//		var winner = d1.Count >= card1 && d2.Count >= card2
			//			? PlayRecursiveCombat(d1.CopyOf(card1), d2.CopyOf(card2))//, d01.CopyOf(card1), d02.CopyOf(card2))
			//			: card1 > card2 ? Player.One : Player.Two;
			//		//if (d1.Score != d01.Score) throw new Exception();
			//		//if (d2.Score != d02.Score) throw new Exception();

			//		if (winner == Player.One)
			//		{
			//			d1.AddToDeck(card1, card2);
			//			//d01.AddToDeck(card1, card2);
			//		}
			//		else
			//		{
			//			d2.AddToDeck(card2, card1);
			//			//d02.AddToDeck(card2, card1);
			//		}

			//		//if (d1.Score != d01.Score) throw new Exception();
			//		//if (d2.Score != d02.Score) throw new Exception();
			//	}

			//	var gamewinner = d1.HasCards ? Player.One : Player.Two;

			//	//var gamewinner0 = d01.HasCards ? Player.One : Player.Two;
			//	//if (gamewinner != gamewinner0) throw new Exception();

			//	//memo[d1.Hand + "/" + d2.Hand] = gamewinner;
			//	return gamewinner;
			//}


		}
	}
}
