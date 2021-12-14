using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day09.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Smoke Basin";
		public override int Year => 2021;
		public override int Day => 9;

		public void Run()
		{
			Run("test1").Part1(15).Part2(1134);
			Run("input").Part1(585).Part2(827904);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var n = 0;
			var (min, max) = map.Area();
			foreach (var p in map.AllPoints())
			{
				var islow = true;
				foreach (var pp in p.LookAround().Within(min, max))
				{
					if (map[pp] <= map[p])
						islow = false;
				}
				if (islow)
				{
					//Console.WriteLine(p);
					n += map[p] - '0' + 1;
				}
			}

			return n;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var (min, max) = map.Area();
			var basins = new List<int>();
			var all = new HashSet<Point>();
			foreach (var p in map.AllPoints())
			{
				var islow = true;
				foreach (var pp in p.LookAround().Within(min, max))
				{
					if (map[pp] <= map[p])
						islow = false;
				}
				if (islow)
				{
					//Console.WriteLine(p);
					//n += map[p] - '0' + 1;
					var seen = new HashSet<Point>();
					seen.Add(p);
					//Console.WriteLine(p);
					CalcBasin(seen, p);
					var basin = seen.Count;
					basins.Add(basin);

					all.UnionWith(seen);
				}
			}

			foreach (var p in map.AllPoints())
			{
				if (!all.Contains(p))
				{
					if (map[p] != '9')
						Console.WriteLine(p);
				}
			}

			var biggest = basins.OrderByDescending(x => x).Take(3).ToArray();
			var n = biggest.Prod();

			return n;

			void CalcBasin(HashSet<Point> seen, Point low)
			{
				//var n = 1;
				foreach (var pp in low.LookAround().Within(min, max))
				{
					if (map[pp] == '9')
						continue;
					if (map[pp] > map[low])
					{
						//Console.WriteLine(pp);
						seen.Add(pp);
						CalcBasin(seen, pp);
					}
				}
				//return n;
			}

		}

	}
}