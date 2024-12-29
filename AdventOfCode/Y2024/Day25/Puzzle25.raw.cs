using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day25.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1(3);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var parts = input.GroupByEmptyLine();

			var keys = parts.Where(p => p[0] == "#####").Select(CountPins).ToArray();
			var locks = parts.Where(p => p[^1] == "#####").Select(CountPins).ToArray();

			int[] CountPins(string[] schematic) => Enumerable.Range(0, 5).Select(i => CountHeight(schematic, i)).ToArray();
			int CountHeight(string[] schematic, int pos) => schematic.Count(s => s[pos] == '#') - 1;

			var fits = 0;
			foreach (var key in keys)
			{
				foreach (var lck in locks)
				{
					if (key.Zip(lck).Select(x => x.First + x.Second).All(n => n < 6))
						fits++;
				}
			}

			return fits;
		}

		protected override long Part2(string[] input) => 0;
	}
}
