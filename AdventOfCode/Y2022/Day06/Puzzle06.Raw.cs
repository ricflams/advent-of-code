using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day06.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 6";
		public override int Year => 2022;
		public override int Day => 6;

		public void Run()
		{
			Run("test1").Part1(7).Part2(19);
			Run("test2").Part1(5).Part2(23);
			Run("test3").Part1(6).Part2(23);
			Run("test4").Part1(10).Part2(29);
			Run("test5").Part1(11).Part2(26);
			Run("input").Part1(1109).Part2(3965);
		}

		protected override long Part1(string[] input)
		{
			var pos = 0;
			foreach (var w in input[0].Windowed(4))
			{
				pos++;
				if (!w.GroupBy(x => x).Any(x => x.Count() > 1))
				{
					break;
				}
			}

			return pos + 3;
		}

		protected override long Part2(string[] input)
		{
			var pos = 0;
			foreach (var w in input[0].Windowed(14))
			{
				pos++;
				if (!w.GroupBy(x => x).Any(x => x.Count() > 1))
				{
					break;
				}
			}

			return pos + 13;
		}
	}
}
