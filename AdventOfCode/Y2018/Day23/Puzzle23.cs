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
			public bool OverlapsWith(Nanobot o) => Overlap(o) > 0; //ManhattanDistanceTo(o) <= R + o.R;
			public int Overlap(Nanobot o) => R + o.R - (ManhattanDistanceTo(o)-1);
		}

		private class Node : GraphxNode
		{
			public Nanobot Bot;
			public override string ToString() => Bot.ToString();
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
			var N = bots.Length;

			var groups = new List<List<Nanobot>>();

			var graph = new Graphx<Node>();
			foreach (var b1 in bots)
			{
				foreach (var b2 in bots)
				{
					if (b1 == b2)
						continue;
					if (b1.OverlapsWith(b2))
					{
						var n = graph.AddEdge(b1.ToString(), b2.ToString(), b1.Overlap(b2));
						n.Bot = b1;
					}
				}
			}

			var chunks = graph.Chunks().OrderByDescending(c => c.Count).ToArray();

			;

			// var overlaps = new HashSet<int>[N];

			// for (var i = 0; i < N; i++)
			// {
			// 	overlaps[i] = new();
			// 	for (var j = 0; j < N; j++)
			// 	{
			// 		if (bots[i].OverlapsWith(bots[j]))
			// 			overlaps[i].Add(j);
			// 	}
			// }

			// var overlappings = overlaps
			// 	.Select(o =>
			// 	{
			// 		var set = new HashSet<int>(o);
			// 		foreach (var j in o)
			// 		{
			// 			set = new HashSet<int>(set.Intersect(overlaps[j]));
			// 		}
			// 		return set;
			// 	})
			// 	.OrderByDescending(x => x.Count)
			// 	.ToArray();

			// var sum = overlappings
			// 	.Select((o, index) => (Index: index, Overlaps: o))
			// 	.OrderByDescending(x => x.Overlaps.Count())
			// 	.ToArray();
			// var top = sum.First().Overlaps.Count();
			// var tops = sum.TakeWhile(b => b.Overlaps.Count() == top).ToArray();
			
			// // var matches = tops
			// // 	.Select(x => overlaps x.Index)

			// var first = tops.First();
			// foreach (var t in tops.Skip(1))
			// {
			// 	if (!first.Overlaps.SetEquals(t.Overlaps))
			// 		Console.Write("x");
			// }

			// var dists = overlappings[first.Index]
			// 	.Select(i => tops.Where(j => j.Index != first.Index).Select(j => j.Overlaps.OrderBy(x => bots[i].Overlap(bots[x]))))
			// 	.OrderBy(x => x.First())
			// 	.ToArray();

			return 0;
		}
	}
}
