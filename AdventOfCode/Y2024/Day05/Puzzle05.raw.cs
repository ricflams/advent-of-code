using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day05.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 5;

		public override void Run()
		{
			// not 11902
			Run("test1").Part1(143).Part2(123);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var things = input
				.GroupByEmptyLine()
				.ToArray();
			var rules = things[0].Select(x => x.Split('|')).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToArray();

			var before = new SafeDictionary<int, HashSet<int>>(() => new HashSet<int>());
			var after = new SafeDictionary<int, HashSet<int>>(() => new HashSet<int>());
			foreach (var rule in rules)
			{
				var beforeSet = before[rule.Item2];
				beforeSet.Add(rule.Item1);
				var afterSet = after[rule.Item1];
				afterSet.Add(rule.Item2);
			}

			var pageorders = things[1].Select(x => x.Split(',').Select(int.Parse).ToArray()).ToArray();

			var ok = pageorders.Where(IsOkay).Sum(x=> x[x.Length/2]);

			bool IsOkay(int[] pages)
			{
				for (var i = 0; i < pages.Length; i++)
				{
					var page = pages[i];
					if (i > 0)
					{
						if (!before[page].Contains(pages[i-1]))
							return false;
					}
					if (i < pages.Length-1)
					{
						if (!after[page].Contains(pages[i+1]))
							return false;
					}
				}
				return true;
			}

			return ok;
		}

		protected override long Part2(string[] input)
		{
			var things = input
				.GroupByEmptyLine()
				.ToArray();
			var rules = things[0].Select(x => x.Split('|')).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToArray();

			var before = new SafeDictionary<int, HashSet<int>>(() => new HashSet<int>());
			var after = new SafeDictionary<int, HashSet<int>>(() => new HashSet<int>());
			foreach (var rule in rules)
			{
				var beforeSet = before[rule.Item2];
				beforeSet.Add(rule.Item1);
				var afterSet = after[rule.Item1];
				afterSet.Add(rule.Item2);
			}

			var pageorders = things[1].Select(x => x.Split(',').Select(int.Parse).ToArray()).ToArray();

			var sum = 0;
			var bad = 0;
			var fixes = 0;
			foreach (var pages in pageorders)
			{
				if (IsOkay(pages))
					continue;
					bad++;
				while (!FixOne(pages))
				{
					fixes++;
				}
				sum += pages[pages.Length/2];
			}
			Console.WriteLine($"bad:{bad} fixes={fixes} fixes/bad={fixes*1.0/bad}");

		//	var ok = pageorders.Sum(x=> x[x.Length/2]);

			bool IsOkay(int[] pages)
			{
				for (var i = 0; i < pages.Length; i++)
				{
					var page = pages[i];
					if (i > 0)
					{
						if (!before[page].Contains(pages[i-1]))
							return false;
					}
					if (i < pages.Length-1)
					{
						if (!after[page].Contains(pages[i+1]))
							return false;
					}
				}
				return true;
			}			

			bool FixOne(int[] pages)
			{
				for (var i = 0; i < pages.Length; i++)
				{
					var page = pages[i];
					if (i > 0)
					{
						if (!before[page].Contains(pages[i-1]))
						{
							(pages[i], pages[i+1]) = (pages[i+1], pages[i]);
							return false;
						}
					}
					if (i < pages.Length-1)
					{
						if (!after[page].Contains(pages[i+1]))
						{
							(pages[i], pages[i+1]) = (pages[i+1], pages[i]);
							return false;
						}
					}
				}
				return true;
			}

			return sum;
		}
	}
}
