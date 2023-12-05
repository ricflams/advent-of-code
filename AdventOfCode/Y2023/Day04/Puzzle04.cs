using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day04
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Scratchcards";
		public override int Year => 2023;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(13).Part2(30);
			Run("input").Part1(24848).Part2(7258152);
		}

		protected override long Part1(string[] input)
		{
			var cards = input.Select(s => new Game(s)).ToArray();

			var sum = cards.Sum(c => c.Points);

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var cards = input.Select(s => new Game(s)).ToArray();

			// The total number of instances starts out with just the originals
			var instances = Enumerable.Repeat(1, cards.Length).ToArray();

			// Run through all cards, producing more cards below for the wins
			for (var i = 0; i < cards.Length; i++)
			{
				for (var j = 0; j < cards[i].Wins; j++)
				{
					instances[i + 1 + j] += instances[i];
				}
			}

			return instances.Sum();
		}

		private record Game
		{
			public int Wins;
			public int Points;
			public Game(string data)
			{
				var raw = data
					.Split(':')[1]
					.Split('|')
					.Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
					.ToArray();
				var wins = new HashSet<int>(raw[0]);
				var nums = new HashSet<int>(raw[1]);
				Wins = wins.Intersect(nums).Count();
				Points = (1<<Wins) / 2;
			}
		}

	}
}
