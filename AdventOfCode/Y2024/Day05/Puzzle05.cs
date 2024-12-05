using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day05
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Print Queue";
		public override int Year => 2024;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(143).Part2(123);
			Run("input").Part1(7074).Part2(4828);
			Run("extra").Part1(5747).Part2(5502);
		}

		protected override long Part1(string[] input)
		{
			var rules = new Rules(input);

			var sum = rules.PageOrders.Where(rules.IsOkay).Sum(x => x[x.Length/2]);

			return sum;
		}


		protected override long Part2(string[] input)
		{
			var rules = new Rules(input);

			var sum = rules.PageOrders.Where(rules.IsNotOkay).Select(rules.FixPages).Sum(x=> x[x.Length/2]);

			return sum;

		}

		private class Rules
		{
			private readonly SafeDictionary<int, HashSet<int>> _before = new(() => []);
			private readonly SafeDictionary<int, HashSet<int>> _after = new(() => []);

			public int[][] PageOrders { get; init; }

			public Rules(string[] input)
			{
				var parts = input
					.GroupByEmptyLine()
					.ToArray();

				var rules = parts[0].Select(x => x.Split('|')).Select(x => new {Before = int.Parse(x[0]), After = int.Parse(x[1])}).ToArray();
				PageOrders = parts[1].Select(x => x.Split(',').Select(int.Parse).ToArray()).ToArray();

				foreach (var rule in rules)
				{
					_before[rule.After].Add(rule.Before);
					_after[rule.Before].Add(rule.After);
				}
			}

			public bool IsOkay(int[] pages)
			{
				for (var i = 0; i < pages.Length; i++)
				{
					if (i > 0 && !_before[pages[i]].Contains(pages[i-1]))
						return false;
					if (i < pages.Length-1 && !_after[pages[i]].Contains(pages[i+1]))
						return false;
				}
				return true;
			}

			public bool IsNotOkay(int[] pages) => !IsOkay(pages);

			public int[] FixPages(int[] pages)
			{
				while (!FixOneSweep(pages))
					;
				return pages;

				bool FixOneSweep(int[] pages)
				{
					bool isOkay = true;
					for (var i = 0; i < pages.Length; i++)
					{
						var page = pages[i];
						if (i > 0)
						{
							if (!_before[page].Contains(pages[i-1]))
							{
								(pages[i], pages[i+1]) = (pages[i+1], pages[i]);
								isOkay = false;
							}
						}
						if (i < pages.Length-1)
						{
							if (!_after[page].Contains(pages[i+1]))
							{
								(pages[i], pages[i+1]) = (pages[i+1], pages[i]);
								isOkay = false;
							}
						}
					}
					return isOkay;
				}					
			}
		}
	}
}
