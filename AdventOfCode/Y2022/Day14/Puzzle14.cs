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

		public override void Run()
		{
			Run("test1").Part1(24).Part2(93);
			Run("input").Part1(618).Part2(26358);
			Run("extra").Part1(888).Part2(26461);
		}

		protected override long Part1(string[] input)
		{
			var (map, abyss) = ReadMap(input);
			var start = (500, 0);

			for (var unit = 1; ; unit++)
			{
				if (PourIntoAbyss())
					return unit - 1; // last one overfloweth
			}

			bool PourIntoAbyss()
			{
				var (x, y) = start;
				while (y < abyss)
				{
					if (!map.IsSet(x, y+1))
						y++;
					else if (!map.IsSet(x-1, y+1))
						{x--; y++; }
					else if (!map.IsSet(x+1, y+1))
						{x++; y++; }
					else
					{
						map.Set(x, y);
						return false;
					}
				}
				return true;
			}			
		}

		protected override long Part2(string[] input)
		{
			var (map, abyss) = ReadMap(input);
			var start = (500, 0);
			var floor = abyss + 1;

			for (var unit = 1; ; unit++)
			{
				if (PourReachesStart())
					return unit;
			}

			bool PourReachesStart()
			{
				var (x, y) = start;
				while (true)
				{
					if (y == floor)
					{
						map.Set(x, y);
						return false;
					}
					if (!map.IsSet(x, y+1))
						y++;
					else if (!map.IsSet(x-1, y+1))
						{x--; y++; }
					else if (!map.IsSet(x+1, y+1))
						{x++; y++; }
					else
					{
						map.Set(x, y);
						return y == 0;
					}					
				}
			}
		}

		private (BoolMap, int) ReadMap(string[] input)
		{
			var map = new BoolMap();
			var maxy = 0;
			foreach (var line in input)
			{
				var points = line.Split("->").Select(Point.Parse);
				foreach (var (a, b) in points.Windowed2())
				{
					foreach (var (x, y) in a.LineTo(b))
					{
						map.Set(x, y);
						if (y > maxy)
							maxy = y;
					}
				}
			}
			return (map, maxy);
		}
	}
}
