using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day07
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Handy Haversacks";
		protected override int Year => 2020;
		protected override int Day => 7;

		public void Run()
		{
			RunFor("test1", 4, 32);
			//RunFor("test2", null, 126);
			RunFor("input", 289, 30055);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var bags = input
				.AsParallel()
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
				.AsEnumerable()
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

			// Recursive count of bag plus bags inside
			// Note: Don't count the "shiny gold" bag itself
			int TotalBagsInside(string color)
			{
				return 1 + bags[color].InnerBags.Sum(kvp => kvp.Value * TotalBagsInside(kvp.Key));
			}
			var result2 = TotalBagsInside("shiny gold") - 1;

			return (result1, result2);
		}
	}
}
