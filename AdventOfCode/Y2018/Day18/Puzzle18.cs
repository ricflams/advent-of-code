using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day18
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Settlers of The North Pole";
		public override int Year => 2018;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").Part1(1147);
			Run("input").Part1(737800).Part2(212040);
			Run("extra").Part1(646437).Part2(208080);
		}

		protected override int Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			// Simulate 10 sec, then calculate the resource value
			for (var i = 0; i < 10; i++)
			{
				map = map.Transform(LandscapeCycle);
			}
			return ResourceValue(map);
		}

		protected override int Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			return CalculateResourceValue(1000000000);

			int CalculateResourceValue(int n)
			{
				// Look for a cycle and figure out from there what the resource-value
				// of the n'th second must be
				var seen = new Dictionary<ulong, (int Index, int ResourceValue)>();
				for (var i = 0; ; i++)
				{
					map = map.Transform(LandscapeCycle);
					var hash = map.Hash(Hashing.KnuthHash);
					if (seen.TryGetValue(hash, out var lastSeen))
					{
						var cycleIndex = lastSeen.Index;
						var cycleLength = i - cycleIndex;
						var targetIndex = cycleIndex + (n - cycleIndex - 1) % cycleLength; // -1 because the n'th second is at the (n-1)'th index
						var value = seen.First(x => x.Value.Index == targetIndex).Value.ResourceValue;
						return value;
					}
					seen[hash] = (i, ResourceValue(map));
				}
			}
		}

		private static int ResourceValue(char[,] map) => map.CountChar('|') * map.CountChar('#');

		private static char LandscapeCycle(char ch, char[] adjacents)
		{
			// open ground (.), trees (|), or a lumberyard (#)
			// An open acre will become filled with trees if three or more adjacent acres contained trees. Otherwise, nothing happens.
			// An acre filled with trees will become a lumberyard if three or more adjacent acres were lumberyards. Otherwise, nothing happens.
			// An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
			switch (ch)
			{
				case '.':
					{
						var n = 0;
						foreach (var c in adjacents)
						{
							if (c == '|' && ++n >= 3)
								return '|';
						}
						return ch;
					}
				case '|':
					{
						var n = 0;
						foreach (var c in adjacents)
						{
							if (c == '#' && ++n >= 3)
								return '#';
						}
						return ch;
					}
				case '#':
					var (tree, lumb) = (false, false);
					foreach (var c in adjacents)
					{
						if (c == '|') tree = true;
						else if (c == '#') lumb = true;
						if (tree && lumb)
							return ch;
					}
					return '.';
				}
			throw new Exception($"Unhandled state {ch}");
		}
	}
}
