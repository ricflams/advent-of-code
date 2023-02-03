using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.RegularExpressions;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day25.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Four-Dimensional Adventure";
		public override int Year => 2018;
		public override int Day => 25;

		public void Run()
		{
			Run("test1").Part1(4);
			Run("test2").Part1(3);
			Run("test3").Part1(8);
			Run("input").Part1(420);
		}

		protected override long Part1(string[] input)
		{
			var pts = input.Select(s => s.Split(',').Select(int.Parse).ToArray()).ToArray();

			var constellations = new List<List<int[]>>();

			foreach (var p in pts)
			{
				var connected = constellations.Where(c => c.Any(x => x.Dist(p) <= 3)).ToArray();
				if (connected.Any())
				{
					var combined = connected.SelectMany(x => x).Append(p).ToList();
					foreach (var c in connected)
						constellations.Remove(c);
					constellations.Add(combined);
				}
				else
				{
					constellations.Add(new List<int[]> { p });
				}
			}

			return constellations.Count;
		}

		protected override long Part2(string[] input) => 0;
	}

	static class Extensions
	{
		public static int Dist(this int[] a, int[] b)
		{
			var dist = 0;
			for (var i = 0; i < 4; i++)
			{
				dist += Math.Abs(a[i] - b[i]);
			}
			return dist;
		}
	}
}
