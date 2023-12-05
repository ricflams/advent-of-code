using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Runtime.Serialization;

namespace AdventOfCode.Y2023.Day04.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(13).Part2(30);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(24848).Part2(0);
			// not 448
		}

		protected override long Part1(string[] input)
		{
			var cards = input
				.Select(s => s.Split(":"))
				.Select(s => s[1].Split("|"))
				.Select(x => (Wins: x[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(), Numbers: x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()))
				.ToArray();

			var sum = cards
				.Sum(c => {
					var n = 0;
					foreach (var v in c.Numbers)
					{

						if (c.Wins.Contains(v))
							n++;
					}
					// var wins = new HashSet<int>(); wins.Add(c.Wins);
					// var has =  new HashSet<int>(c.Numbers);
					// var n = wins.Intersect(has).Count();
					var p = (int)Math.Pow(2, n-1);
					return p;
				});

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var cards = input
				.Select(s => s.Split(":"))
				.Select(s => s[1].Split("|"))
				.Select((x,i) => (Id: i+1, Wins: x[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(), Numbers: x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()))
				.ToList();

			var counts = Enumerable.Repeat(1, cards.Count).ToArray();
			// while (true)
			// {
			// 	var won = false;
				for (var i = 0; i < cards.Count; i++)
				{
					var c = cards[i];
					var n = 0;
					foreach (var v in c.Numbers)
					{

						if (c.Wins.Contains(v))
							n++;
					}
					for (var j = 0; j < n; j++)
					{
						counts[i + 1 + j] += counts[i];
						// if (counts[i] > 1)
						// 	won = true;
					}
				}
			// 	if (!won)
			// 		break;
			// }

			return counts.Sum();
		}
	}
}
