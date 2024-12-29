using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day25
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Code Chronicle";
		public override int Year => 2024;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1(3);
			Run("input").Part1(3439);
			Run("extra").Part1(2933);
		}

		protected override long Part1(string[] input)
		{
			var parts = input.GroupByEmptyLine();

			var keys = parts.Where(p => p[0] == "#####").Select(ParsePins).ToArray();
			var locks = parts.Where(p => p[^1] == "#####").Select(ParsePins).ToArray();

			static int[] ParsePins(string[] s) => Enumerable.Range(0, 5).Select(i => PinHeight(s, i)).ToArray();
			static int PinHeight(string[] s, int pos) => s.Count(s => s[pos] == '#') - 1;

			var fits = 0;
			foreach (var key in keys)
			{
				foreach (var lck in locks)
				{
					if (key.Zip(lck).All(x => x.First + x.Second < 6))
						fits++;
				}
			}

			return fits;
		}

		protected override long Part2(string[] input) => 0;
	}
}
