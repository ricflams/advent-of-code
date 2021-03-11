using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day03
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2018;
		public override int Day => 3;

		public void Run()
		{
			RunFor("test1", 4, 3);
			//RunFor("test2", 0, 0);
			RunFor("input", 116489, 0);
		}

		protected override int Part1(string[] input)
		{
			var once = new SimpleMemo<int>();
			var twice = new SimpleMemo<int>();
			var overlaps = 0;
			foreach (var c in input)
			{
				// #1 @ 1,3: 4x4
				var (id, x0, y0, w, h) = c.RxMatch("#%d @ %d,%d: %dx%d").Get<int, int, int, int, int>();

				for (var x = x0; x < x0 + w; x++)
				{
					for (var y = y0; y < y0 + h; y++)
					{
						var coord = x*10000 + y;
						if (once.IsSeenBefore(coord) && !twice.IsSeenBefore(coord))
						{
							overlaps++;
						}
					}
				}
			}
			//var overlaps = map.Values.Count(x => x > 1);
			return overlaps;
		}

		protected override int Part2(string[] input)
		{
			var map = new SafeDictionary<int, int>();
			foreach (var c in input)
			{
				// #1 @ 1,3: 4x4
				var (id, x0, y0, w, h) = c.RxMatch("#%d @ %d,%d: %dx%d").Get<int, int, int, int, int>();

				for (var x = x0; x < x0 + w; x++)
				{
					for (var y = y0; y < y0 + h; y++)
					{
						var coord = x*10000 + y;
						map[coord]++;
					}
				}
			}
			foreach (var c in input)
			{
				// #1 @ 1,3: 4x4
				var (id, x0, y0, w, h) = c.RxMatch("#%d @ %d,%d: %dx%d").Get<int, int, int, int, int>();

				var nooverlap = true;
				for (var x = x0; x < x0 + w; x++)
				{
					for (var y = y0; y < y0 + h; y++)
					{
						var coord = x*10000 + y;
						if (map[coord] > 1)
						{
							nooverlap = false;
							break;
						}
					}
				}
				if (nooverlap)
					return id;
			}
			throw new Exception();
		}
	}
}
