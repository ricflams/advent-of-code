using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day20
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "A Regular Map";
		public override int Year => 2018;
		public override int Day => 20;

		public void Run()
		{
			Run("test1").Part1(3);
			Run("test2").Part1(10);
			Run("test3").Part1(18);
			Run("test4").Part1(23);
			Run("test5").Part1(31);
			Run("input").Part1(4184).Part2(8596);
		}

		protected override int Part1(string[] input)
		{
			var rx = input.Single();

			var map = new CharMap(' ');
			var p = Point.Origin;
			WalkRegex(map, p, rx, 1);

			var dist = 0;
			var fill = new HashSet<Point> { p };
			while (fill.Any())
			{
				foreach (var x in fill)
					map[x] = 'v';
				fill = new HashSet<Point>(fill.SelectMany(x => x.LookAround().Where(x => map[x] == '.')));
				dist++;
			}
			return dist / 2;
		}

		protected override int Part2(string[] input)
		{
			var rx = input.Single();

			var map = new CharMap(' ');
			var p = Point.Origin;
			WalkRegex(map, p, rx, 1);

			var rooms = 0;
			var dist = 0;
			var fill = new HashSet<Point> { p };
			while (fill.Any())
			{
				foreach (var x in fill)
					map[x] = 'v';
				fill = new HashSet<Point>(fill.SelectMany(x => x.LookAround().Where(x => map[x] == '.')));
				dist++;
				if (dist % 2 == 0 && dist / 2 >= 1000)
				{
					rooms += fill.Count();
				}
			}
			return rooms;
		}

		private static int WalkRegex(CharMap map, Point p, string rx, int pos)
		{
			var p0 = p;
			while (pos > 0)
			{
				switch (rx[pos++])
				{
					case 'N': map[p=p.N] = '.'; map[p=p.N] = '.'; break;
					case 'E': map[p=p.E] = '.'; map[p=p.E] = '.'; break;
					case 'S': map[p=p.S] = '.'; map[p=p.S] = '.'; break;
					case 'W': map[p=p.W] = '.'; map[p=p.W] = '.'; break;
					case '|': p = p0; break;
					case '(': pos = WalkRegex(map, p, rx, pos); break;
					case ')': return pos;
					case '$': return -1;
				}
			}
			return pos;
		}
	}
}
