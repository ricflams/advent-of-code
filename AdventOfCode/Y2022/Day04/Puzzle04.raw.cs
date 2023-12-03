using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day04.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 4";
		public override int Year => 2022;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(2).Part2(4);
			Run("input").Part1(453).Part2(919);
		}

		protected override long Part1(string[] input)
		{
			var pairs = input
				.Select(line => 
				{
					var (a, b, c, d) = line.RxMatch("%d-%d,%d-%d").Get<int, int, int, int>();
					return (a <= c && b >= d || c <= a && d >= b) ? 1 : 0;
				}) 
				.Sum();


			return pairs;
		}

		protected override long Part2(string[] input)
		{
			var pairs = input
				.Select(line => 
				{
					var (a1, a2, b1, b2) = line.RxMatch("%d-%d,%d-%d").Get<int, int, int, int>();
					if (a1 <= b1 && a2 >= b2 || b1 <= a1 && b2 >= a2)
						return 1;
					if (a1 <= b1)
						return (a2 >= b1 && a2 <= b2) ? 1 : 0;
					if (b1 <= a1)
						return (b2 >= a1 && b2 <= a2) ? 1 : 0;
					return 0;
				}) 
				.Sum();


			return pairs;
		}
	}
}
