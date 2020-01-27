using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day03
{
	internal class Puzzle03
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var raw = File.ReadAllText("Y2015/Day03/input.txt");
			var map = new SparseMap<int>();

			var pos = Point.From(0, 0);
			map[pos]++;
			foreach (var ch in raw)
			{
				pos = pos.Move(ch);
				map[pos]++;
			}

			var visited = map.AllPoints().Count();
			Console.WriteLine($"Day  3 Puzzle 1: {visited}");
			Debug.Assert(visited == 2081);
		}

		private static void Puzzle2()
		{
			var raw = File.ReadAllText("Y2015/Day03/input.txt");
			var map = new SparseMap<int>();

			var deliveries = new Point[] { Point.From(0, 0), Point.From(0, 0) };
			foreach (var d in deliveries)
			{
				map[d]++;
			}
			for (var i = 0; i < raw.Length; i++)
			{
				var turn = i % deliveries.Length;
				deliveries[turn] = deliveries[turn].Move(raw[i]);
				map[deliveries[turn]]++;
			}

			var visited = map.AllPoints().Count();
			Console.WriteLine($"Day  3 Puzzle 2: {visited}");
			Debug.Assert(visited == 2341);
		}
	}
}
