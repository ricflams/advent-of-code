using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day25
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Four-Dimensional Adventure";
		public override int Year => 2018;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1(4);
			Run("test2").Part1(3);
			Run("test3").Part1(8);
			Run("input").Part1(420);
		}

		protected override long Part1(string[] input)
		{
			// Store points as tuples, which seems to be faster than a record or array
			var pts = input
				.Select(s => s.Split(',').Select(int.Parse).ToArray())
				.Select(x => (A: x[0], B: x[1], C: x[2], D: x[3]))
				.ToArray();

			var constellations = new List<List<(int A,int B,int C,int D)>>();

			foreach (var p in pts)
			{
				var join = constellations
					.Where(c => c.Any(x => Math.Abs(x.A - p.A) + Math.Abs(x.B - p.B) + Math.Abs(x.C - p.C) + Math.Abs(x.D - p.D) <= 3))
					.ToArray();
				if (join.Any())
				{
					var dest = join[0];
					foreach (var c in join[1..])
					{
						constellations.Remove(c);
						dest.AddRange(c);
					}
					dest.Add(p);
				}
				else
				{
					constellations.Add(new List<(int,int,int,int)> { p });
				}
			}

			return constellations.Count;
		}

		protected override long Part2(string[] input) => 0;
	}
}
