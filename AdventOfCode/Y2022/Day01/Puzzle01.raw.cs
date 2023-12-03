using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day01.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 1";
		public override int Year => 2022;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(24000).Part2(45000);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(66487).Part2(197301);
		}

		protected override long Part1(string[] input)
		{
			var v = input
				.GroupByEmptyLine()
				.Select(x => x.Select(int.Parse).ToArray())
				.ToArray();

			var most = v.Max(x => x.Sum());
			//var v = input[0].ToIntArray();

			return most;
		}

		protected override long Part2(string[] input)
		{
			var v = input
				.GroupByEmptyLine()
				.Select(x => x.Select(int.Parse).ToArray().Sum())
				.OrderByDescending(x => x)
				.Take(3)
				.Sum();

			//var v = input[0].ToIntArray();

			return v;
		}

	}
}
