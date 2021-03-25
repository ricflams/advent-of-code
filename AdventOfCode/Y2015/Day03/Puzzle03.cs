using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day03
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Perfectly Spherical Houses in a Vacuum";
		public override int Year => 2015;
		public override int Day => 3;

		public void Run()
		{
			Run("input").Part1(2081).Part2(2341);
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
