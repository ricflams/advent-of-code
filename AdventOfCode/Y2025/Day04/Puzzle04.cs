using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day04
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Printing Department";
		public override int Year => 2025;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(13).Part2(43);
			Run("input").Part1(1356).Part2(8713);
			Run("extra").Part1(1419).Part2(8739);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var n = map.AllPointsWhere(p => map[p] == '@' && p.LookDiagonallyAround().Count(n => map[n] == '@') < 4);

			return n.Count();
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var removed = 0;
			while (true)
			{
				var removable = map.AllPointsWhere(p => map[p] == '@' && p.LookDiagonallyAround().Count(n => map[n] == '@') < 4);
				if (removable.Count() == 0)
					break;
				foreach (var p in removable)
				{
					map[p] = '.';
					removed++;
				}
			}

			return removed;
		}
	}
}
