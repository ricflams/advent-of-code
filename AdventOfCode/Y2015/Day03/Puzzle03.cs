using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day03
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2015;
		protected override int Day => 3;

		public void Run()
		{
			RunFor("input", 2081, 2341);
		}

		protected override int Part1(string[] input)
		{
			var raw = input[0];
			var map = new SparseMap<int>();

			var pos = Point.From(0, 0);
			map[pos]++;
			foreach (var ch in raw)
			{
				pos = pos.Move(ch);
				map[pos]++;
			}

			var visited = map.AllPoints().Count();
			return visited;
		}

		protected override int Part2(string[] input)
		{
			var raw = input[0];
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
			return visited;
		}
	}
}
