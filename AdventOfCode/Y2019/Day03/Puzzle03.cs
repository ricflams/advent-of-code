using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day03
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 3;

		public void Run()
		{
			RunFor("test1", 159, 610);
			RunFor("test2", 135, 410);
			RunFor("input", 860, 9238);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var wiredefs = input;

			var map = new Dictionary<int, int[]>();
			for (var i = 0; i < wiredefs.Length; i++)
			{
				MapWire(i, wiredefs.Length, wiredefs[i]);
			}

			const int xyFactor = 100000;
			static int MakeXy(int x, int y) => x * xyFactor + y;
			static int MakeX(int xy) => xy / xyFactor;
			static int MakeY(int xy) => xy % xyFactor;

			var crossings = map
				.Where(x => x.Value.All(s => s != 0))
				.Select(x => new
				{
					X = MakeX(x.Key),
					Y = MakeY(x.Key),
					Steps = x.Value.Sum()
				})
				.ToList();

			var nearest = crossings.Min(x => Math.Abs(x.X) + Math.Abs(x.Y));
			var fewestSteps = crossings.Min(x => x.Steps);

			return (nearest, fewestSteps);

			void MapWire(int wireIndex, int wireCount, string wiredef)
			{
				int x = 0, y = 0, step = 0;
				foreach (var wire in wiredef.Split(','))
				{
					int dx = 0, dy = 0;
					var len = int.Parse(wire.Substring(1));
					switch (wire[0])
					{
						case 'R': dx = 1; dy = 0; break;
						case 'D': dx = 0; dy = -1; break;
						case 'L': dx = -1; dy = 0; break;
						case 'U': dx = 0; dy = 1; break;
					}
					for (var i = 0; i < len; i++)
					{
						step++;
						x += dx;
						y += dy;
						var xy = MakeXy(x, y);
						if (!map.ContainsKey(xy))
						{
							map[xy] = new int[wireCount];
						}
						if (map[xy][wireIndex] == 0)
						{
							map[xy][wireIndex] = step;
						}
					}
				}
			}
		}
	}
}