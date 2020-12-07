using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day07
{
	internal class Puzzle07
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2020/Day07/input.txt");

			var bags = input
				.Select(line =>
				{
					// clear chartreuse bags contain 3 mirrored olive bags, 1 posh yellow bag, 1 faded salmon bag, 5 drab salmon bags.
					line.RegexCapture("%* bags contain %*.")
						.Get(out string color)
						.Get(out string content);
					return new 
					{
						Color = color,
						InnerBags = (content.Contains("no other bags") ? "" : content)
							.Split(",", StringSplitOptions.RemoveEmptyEntries)
							.Select(x =>
							{
								x.RegexCapture("%d %* bag")
									.Get(out int n)
									.Get(out string color);
								return (color, n);
							})
							.ToDictionary(x => x.color, x => x.n)
					};
				})
				.ToDictionary(x => x.Color, x => x);

			// Memo results so we only calculate when needed
			// Note: Don't count the "shiny gold" bag itself
			// (150000 calls become just 1700 calculations; rest is memo-lookup)
			var memo = new Dictionary<string, bool>();
			bool ContainsShinyGold(string color)
			{
				if (!memo.ContainsKey(color))
				{
					memo[color] = color == "shiny gold" || bags[color].InnerBags.Keys.Any(ContainsShinyGold);
				}
				return memo[color];
			}
			var result1 = bags.Keys.Count(ContainsShinyGold) - 1;
			Console.WriteLine($"Day 07 Puzzle 1: {result1}");
			Debug.Assert(result1 == 289);

			// Recursive count of bag plus bags inside
			// Note: Don't count the "shiny gold" bag itself
			int TotalBagsInside(string color)
			{
				return 1 + bags[color].InnerBags.Sum(kvp => kvp.Value * TotalBagsInside(kvp.Key));
			}
			var result2 = TotalBagsInside("shiny gold") - 1;
			Console.WriteLine($"Day 07 Puzzle 2: {result2}");
			Debug.Assert(result2 == 30055);
		}
	}
}
