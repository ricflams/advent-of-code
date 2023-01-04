using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Experimental Emergency Teleportation";
		public override int Year => 2018;
		public override int Day => 23;

		public void Run()
		{
			Run("test1").Part1(7);
			Run("test2").Part2(36);
			Run("input").Part1(613).Part2(0);
		}

		private class Nanobot
		{
			public Nanobot(int x, int y, int z, int r) => (X, Y, Z, R) = (x, y, z, r);
			public int X, Y, Z, R;
			public override string ToString() => $"pos=<{X},{Y},{Z}>, r={R}";
			public int ManhattanDistanceTo(Nanobot o) => Math.Abs(X - o.X) + Math.Abs(Y - o.Y) + Math.Abs(Z - o.Z);
		}

		protected override long Part1(string[] input)
		{
			var bots = input
				.Select(s =>
				{
					var (x, y, z, r) = s.RxMatch("pos=<%d,%d,%d>, r=%d").Get<int, int, int, int>();
					return new Nanobot(x, y, z, r);
				})
				.ToArray();

			var strongest = bots.OrderByDescending(x => x.R).First();

			var near = bots
				.Count(b => strongest.ManhattanDistanceTo(b) <= strongest.R);



			return near;
		}

		protected override long Part2(string[] input)
		{
			var bots = input
				.Select(s =>
				{
					var (x, y, z, r) = s.RxMatch("pos=<%d,%d,%d>, r=%d").Get<int, int, int, int>();
					return new Nanobot(x, y, z, r);
				})
				.ToArray();

			var overlaps = bots
				.Select(b =>
					bots
						.Where(x => x != b)
						.Count(o => b.R + o.R >= b.ManhattanDistanceTo(o))
				)
				.OrderBy(x => x)
				.ToArray();
			foreach (var x in overlaps)
			{
				Console.WriteLine(x);
			}

			return 0;
		}
	}
}
