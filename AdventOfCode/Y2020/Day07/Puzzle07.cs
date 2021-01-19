using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day07
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Handy Haversacks";
		public override int Year => 2020;
		public override int Day => 7;

		public void Run()
		{
			RunFor("test1", 4, 32);
			RunPart2For("test2", 126);
			RunFor("input", 289, 30055);
		}

		protected override int Part1(string[] input)
		{
			var bags = GetBags(input);

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
			
			var result = bags.Keys.Count(ContainsShinyGold) - 1;
			return result;
		}

		protected override int Part2(string[] input)
		{
			var bags = GetBags(input);

			// Recursive count of bag plus bags inside
			// Note: Don't count the "shiny gold" bag itself
			int TotalBagsInside(string color)
			{
				return 1 + bags[color].InnerBags.Sum(kvp => kvp.Value * TotalBagsInside(kvp.Key));
			}

			var result = TotalBagsInside("shiny gold") - 1;
			return result;
		}

		private class Bag
		{
			public string Color { get; set; }
			public Dictionary<string, int> InnerBags { get; set; }
		}

		private static Dictionary<string, Bag> GetBags(string[] input)
		{
			var bags = input
				.AsParallel()
				.Select(line =>
				{
					// clear chartreuse bags contain 3 mirrored olive bags, 1 posh yellow bag, 1 faded salmon bag, 5 drab salmon bags.
					var (color, content) = line
						.RxMatch("%* bags contain %*.")
						.Get<string, string>();
					return new Bag
					{
						Color = color,
						InnerBags = (content.Contains("no other bags") ? "" : content)
							.Split(",", StringSplitOptions.RemoveEmptyEntries)
							.Select(x =>
							{
								var (n, color) = x.RxMatch("%d %* bag").Get<int, string>();
								return (color, n);
							})
							.ToDictionary(x => x.color, x => x.n)
					};
				})
				.AsEnumerable()
				.ToDictionary(x => x.Color, x => x);
			return bags;
		}
	}
}
