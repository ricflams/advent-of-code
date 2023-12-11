using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day11
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Cosmic Expansion";
		public override int Year => 2023;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").Part1(374);
			Run("input").Part1(9233514).Part2(363293506944);
			Run("extra").Part1(9795148).Part2(650672493820);
		}

		protected override long Part1(string[] input)
		{
			return SumByExpansion(input, 2);
		}

		protected override long Part2(string[] input)
		{
			return SumByExpansion(input, 1_000_000);
		}

		private static long SumByExpansion(string[] input, long expandTo)
		{
			// Find all galaxies
			var map = CharMap.FromArray(input);
			var galaxies = map.AllPoints(c => c == '#').ToArray();

			// Find x,y coordinates of all galaxies
			var xCoords = new HashSet<int>(galaxies.Select(g => g.X));
			var yCoords = new HashSet<int>(galaxies.Select(g => g.Y));
			var (w, h) = map.Size();

			// Find all the empty rows/columns
			var xEmpty = Enumerable.Range(0, w).Where(x => !xCoords.Contains(x)).ToArray();
			var yEmpty = Enumerable.Range(0, h).Where(y => !yCoords.Contains(y)).ToArray();

			// Map all x and y coordinates just once for use in mapping all
			// the galaxies' x,y-coordinates below.
			var xMapping =  Enumerable.Range(0, w).Select(x => x + xEmpty.Count(p => p < x)*(expandTo-1)).ToArray();
			var yMapping =  Enumerable.Range(0, h).Select(y => y + yEmpty.Count(p => p < y)*(expandTo-1)).ToArray();

			// Expand all the galaxies, using the one-time mapping we found above
			var expanded = galaxies
				.Select(p => (X: xMapping[p.X], Y: yMapping[p.Y]))
				.ToArray();

			// Sum all the distances between every pair of galaxies.
			// Just use two simple loops because it's faster than linq etc
			var sum = 0L;
			for (var i = 0; i < expanded.Length; i++)
			{
				var (xi, yi) = expanded[i];
				for (var j = i+1; j < expanded.Length; j++)
				{
					var (xj, yj) = expanded[j];
					sum += Math.Abs(xi - xj) + Math.Abs(yi - yj);
				}
			}

			return sum;
		}
	}
}
