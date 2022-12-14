using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day14
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Regolith Reservoir";
		public override int Year => 2022;
		public override int Day => 14;

		public void Run()
		{
			Run("test1").Part1(24).Part2(93);
			Run("input").Part1(618).Part2(26358);
		}

		protected override long Part1(string[] input)
		{
			var map = ReadMap(input);
			var abyss = map.Max().Y;
			var start = Point.From(500, 0);

			for (var unit = 1; ; unit++)
			{
				if (PourIntoAbyss())
					return unit - 1; // last one overfloweth
			}

			bool PourIntoAbyss()
			{
				var p = start;
				while (p.Y < abyss)
				{
					if (map[p.Down] == '.')
						p = p.Down;
					else if (map[p.DiagonalDownLeft] == '.')
						p = p.DiagonalDownLeft;
					else if (map[p.DiagonalDownRight] == '.')
						p = p.DiagonalDownRight;
					else
					{
						map[p] = 'o';
						return false;
					}
				}
				return true;
			}			
		}

		protected override long Part2(string[] input)
		{
			var map = ReadMap(input);
			var start = Point.From(500, 0);
			var floor = map.Max().Y + 2;

			for (var unit = 1; ; unit++)
			{
				if (PourReachesStart())
					return unit;
			}

			bool PourReachesStart()
			{
				var p = start;
				while (true)
				{
					if (p.Y == floor-1)
					{
						map[p] = 'o';
						return false;
					}
					if (map[p.Down] == '.')
						p = p.Down;
					else if (map[p.DiagonalDownLeft] == '.')
						p = p.DiagonalDownLeft;
					else if (map[p.DiagonalDownRight] == '.')
						p = p.DiagonalDownRight;
					else
					{
						map[p] = 'o';
						return p.Y == 0;
					}
				}
			}
		}

		private CharMap ReadMap(string[] input)
		{
			var map = new CharMap('.');
			foreach (var line in input)
			{
				var points = line.Split("->").Select(Point.Parse);
				foreach (var (a, b) in points.Windowed2())
				{
					foreach (var p in a.LineTo(b))
						map[p] = '#';
				}
			}
			return map;
		}
	}
}
