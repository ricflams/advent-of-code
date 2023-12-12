using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day12
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Hot Springs";
		public override int Year => 2023;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(1).Part2(1);
			Run("test2").Part1(4).Part2(16384);
			Run("test3").Part1(1).Part2(1);
			Run("test4").Part1(1).Part2(16);
			Run("test5").Part1(4).Part2(2500);
			Run("test6").Part1(10).Part2(506250);
			Run("input").Part1(7771).Part2(10861030975833);
			Run("extra").Part1(7490).Part2(65607131946466);
		}

		protected override long Part1(string[] input)
		{
			var sum = input
				.Sum(line =>
				{
					var part = line.Split(' ').ToArray();
					var (springs, groups) = (part[0], part[1].ToIntArray());
					var n = CountCombos(springs, groups);
					return n;
				});

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var sum = input
				.Sum(line =>
				{
					var part = line.Split(' ').ToArray();
					var (s, g) = (part[0], part[1].ToIntArray());
					var springs = $"{s}?{s}?{s}?{s}?{s}";
					var groups = g.Concat(g).Concat(g).Concat(g).Concat(g).ToArray();
					var n = CountCombos(springs, groups);
					return n;
				});

			return sum;
		}		

		private readonly Dictionary<string, long> _memo = [];

		private long CountCombos(string spring, int[] groups)
		{
			// Clear the memo for each new search for efficiency.
			// Also pad spring with as much extra dots as as the longest
			// group could possibly cause us to look-ahead to avoid having
			// to do checks for reading past the end of the spring.
			_memo.Clear();
			var paddedSpring = spring + new string('.', groups.Max()+1);
			return ComboMemo(paddedSpring, groups);
		}

		private long ComboMemo(ReadOnlySpan<char> s, int[] groups)
		{
			// Memoize the findings
			var key = $"{s}/{string.Join('-', groups)}";
			if (!_memo.TryGetValue(key, out var n))
				n = _memo[key] = ComboCalculate(s, groups);
			return n;
		}

		private long ComboCalculate(ReadOnlySpan<char> s, int[] groups)
		{
			// Always skip leading dots
			while (s.Length > 0 && s[0] == '.')
				s = s[1..];

			// If groups are exhausted there it's a match if only ?. are left; else not a match
			if (groups.Length == 0)
				return s.IndexOf('#') < 0 ? 1 : 0;

			// If there are groups left but s is exhausted then it's not a match
			if (s.Length == 0)
				return 0;

			// We know now that we're looking at # or ?
			// If group can't match here (because s contains a . or has a trailing #)
			// then skip wildcard and continue matching; if looking at # then it's not a match
			var siz = groups[0];
			if (s[..siz].Contains('.') || s[siz] == '#')
				return s[0] == '?' ? ComboMemo(s[1..], groups) : 0;

			// It's a match so far, huzzah.
			// Keep on matching the sequence following this group, ie move ahead the
			// length of the group and move to the next group.
			// If looking at a wildcard then also count any matches that we would find
			// by skipping that wildcard.
			var n = ComboMemo(s[(siz+1)..], groups[1..]);
			if (s[0] == '?') // may skip
				n += ComboMemo(s[1..], groups);
			return n;
		}
	}
}
