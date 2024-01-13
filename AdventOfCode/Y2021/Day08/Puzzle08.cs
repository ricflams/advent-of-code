using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day08
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Seven Segment Search";
		public override int Year => 2021;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(0).Part2(5353);
			Run("test2").Part1(26).Part2(61229);
			Run("input").Part1(548).Part2(1074888);
			Run("extra").Part1(397).Part2(1027422);
		}

		protected override int Part1(string[] input)
		{
			// Count number of times digits 1, 4, 7, or 8 appears;
			// they are uniquely identified by lengths 2, 3, 4, and 7
			var n = SanitizeInput(input)
				.Select(s => s.Split('|')[1]) // pick output-part
				.SelectMany(s => s.SplitSpace()) // split into digits
				.Select(x => x.Length) // count the digit's length
				.Count(len => len is 2 or 3 or 4 or 7);
			return n;
		}

		protected override int Part2(string[] input)
		{
			// Deduce each segment's digits and sum them
			var n = SanitizeInput(input)
				.Select(s => s.Split('|'))
				.Select(x => new Note
				{
					Signal = x[0].SplitSpace().Select(x => new Note.Set(x)).ToArray(),
					Digits = x[1].SplitSpace().Select(x => new Note.Set(x)).ToArray()
				})
				.Sum(CalculateOutputValue);

			return n;
		}

		internal class Note
		{
			public Set[] Signal;
			public Set[] Digits;

			public class Set : HashSet<char>
			{
				public Set(string set) : base(set.ToCharArray()) { }
			}
		}

		private static int CalculateOutputValue(Note input)
		{
			//  0:      1:      2:      3:      4:      5:      6:      7:      8:      9:
			// aaaa    ....    aaaa    aaaa    ....	   aaaa    aaaa    aaaa    aaaa    aaaa
			//b    c  .    c  .    c  .    c  b    c  b    .  b    .  .    c  b    c  b    c
			//b    c  .    c  .    c  .    c  b    c  b    .  b    .  .    c  b    c  b    c
			// ....    ....    dddd    dddd    dddd	   dddd    dddd    ....    dddd    dddd
			//e    f  .    f  e    .  .    f  .    f  .    f  e    f  .    f  e    f  .    f
			//e    f  .    f  e    .  .    f  .    f  .    f  e    f  .    f  e    f  .    f
			// gggg    ....    gggg    gggg    ....	   gggg    gggg    ....    gggg    gggg

			// Deduce the 10 segments
			var seg = new Note.Set[10];

			// Segments 1, 4, 7, and 8 are uniquely identifiable
			seg[1] = input.Signal.First(x => x.Count == 2);
			seg[4] = input.Signal.First(x => x.Count == 4);
			seg[7] = input.Signal.First(x => x.Count == 3);
			seg[8] = input.Signal.First(x => x.Count == 7);

			// Sixers: 9 contains 4, 0 contains 1, 6 is last sixer
			var sixes = input.Signal.Where(x => x.Count == 6).ToList();
			seg[9] = Extract(sixes, x => x.IsSupersetOf(seg[4]));
			seg[0] = Extract(sixes, x => x.IsSupersetOf(seg[1]));
			seg[6] = sixes.Single();

			// Fivers: 3 contains 1, 6 contains 5, 2 is last fiver
			var fives = input.Signal.Where(x => x.Count == 5).ToList();
			seg[3] = Extract(fives, x => x.IsSupersetOf(seg[1]));
			seg[5] = Extract(fives, x => seg[6].IsSupersetOf(x));
			seg[2] = fives.Single();

			var val = input.Digits.Aggregate(0, (v, digit) => v * 10 + seg.IndexOf(s => s.SetEquals(digit)));
			return val;

			static Note.Set Extract(List<Note.Set> sets, Func<Note.Set, bool> predicate)
			{
				var set = sets.Single(predicate);
				sets.Remove(set);
				return set;
			}
		}

		// Collapse multi-lines from the examples
		private static IEnumerable<string> SanitizeInput(string[] input)
		{
			for (var i = 0; i < input.Length; i++)
			{
				var s = input[i];
				yield return s.EndsWith('|')
					? s + input[++i]
					: s;
			}
		}
	}
}
