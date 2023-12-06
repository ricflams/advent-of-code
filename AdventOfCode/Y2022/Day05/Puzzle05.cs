using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day05
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Supply Stacks";
		public override int Year => 2022;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1("CMZ").Part2("MCD");
			Run("input").Part1("QGTHFZBHV").Part2("MGDMPSZTM");
			Run("extra").Part1("TLNGFGMFN").Part2("FGLQJCMBD");
		}

		protected override string Part1(string[] input)
		{
			return Run(input, true);
		}

		protected override string Part2(string[] input)
		{
			return Run(input, false);
		}

		private string Run(string[] input, bool moveOneByOne)
		{
			var parts = input.GroupByEmptyLine().ToArray();

			// Create and init the crates
			var crates = Enumerable.Range(0, parts[0].Last().Length/ 4 + 1)
				.Select(_ => new Stack<char>())
				.ToArray();
			foreach (var s in parts[0][..^1].Reverse())
			{
				for (var (i, pos) = (0, 1); pos < s.Length; pos += 4, i++)
				{
					if (s[pos] != ' ')
						crates[i].Push(s[pos]);
				}
			}

			// Do the moves
			foreach (var move in parts[1])
			{
				var (n, srcIndex, dstIndex) = move.RxMatch("move %d from %d to %d").Get<int, int, int>();
				var src = crates[srcIndex - 1];
				var dst = crates[dstIndex - 1];
				
				if (moveOneByOne)
				{
					for (var i = 0; i < n; i++)
					{
						dst.Push(src.Pop());
					}					
				}
				else
				{
					var temp = new Stack<char>();
					for (var i = 0; i < n; i++)
					{
						temp.Push(src.Pop());
					}
					for (var i = 0; i < n; i++)
					{
						dst.Push(temp.Pop());
					}
				}
			}

			var top = new string(crates.Select(c => c.Pop()).ToArray());
			return top;
		}
	}
}
