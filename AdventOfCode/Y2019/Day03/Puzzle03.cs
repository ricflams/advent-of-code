using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Y2019.Day03
{
	internal class Puzzle03
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var wiredefs = File.ReadAllLines("Y2019/Day03/input.txt").ToArray();

			var map = new Dictionary<int, int[]>();
			for (var i = 0; i < wiredefs.Length; i++)
			{
				MapWire(i, wiredefs.Length, wiredefs[i]);
			}

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
			Console.WriteLine($"Day  3 Puzzle 1: {nearest}");
			Debug.Assert(nearest == 860);

			var fewestSteps = crossings.Min(x => x.Steps);
			Console.WriteLine($"Day  3 Puzzle 2: {fewestSteps}");
			Debug.Assert(fewestSteps == 9238);

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

			const int xyFactor = 100000;
			int MakeXy(int x, int y) => x * xyFactor + y;
			int MakeX(int xy) => xy / xyFactor;
			int MakeY(int xy) => xy % xyFactor;
		}
	}
}