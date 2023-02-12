using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day10
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Monitoring Station";
		public override int Year => 2019;
		public override int Day => 10;

		public void Run()
		{
			Run("test1").Part1(210).Part2(802);
			Run("input").Part1(299).Part2(1419);
		}

		protected override int Part1(string[] input)
		{
			var asteroidmap = input;
			var (_, _, detectableAsteroids) = MaxDetectable(asteroidmap);
			return detectableAsteroids;
		}

		protected override int Part2(string[] input)
		{
			var asteroidmap = input;
			var (stationX, stationY, _) = MaxDetectable(asteroidmap);

			//var map2 = asteroidmap.Select(x => x.ToArray()).ToArray();
			//map2[stationY][stationX] = 'X';
			//int xx = 65;
			//foreach (var a in VaporizedAsteroids(asteroidmap, xloc, yloc))
			//{
			//	var x = a / 100;
			//	var y = a % 100;
			//	Console.WriteLine($">>> {x},{y}");
			//	map2[y][x] = char.ConvertFromUtf32(xx++).First();
			//	for (var line = 0; line < asteroidmap.Length; line++)
			//	{
			//		Console.WriteLine($"{line % 10}" + new string(map2[line]));
			//	}
			//	Console.ReadLine();
			//}
			var asteroid200 = VaporizedAsteroidsFrom(asteroidmap, stationX, stationY)
				.Skip(199)
				.First();
			return asteroid200;
		}


		private static (int, int, int) MaxDetectable(string[] mapinfo)
		{
			var w = mapinfo[0].Length;
			var h = mapinfo.Length;

			var asteroid = Enumerable.Range(0, w).SelectMany(x =>
				Enumerable.Range(0, h).Select(y => new
				{
					X = x,
					Y = y,
					Detectable = DetectableFrom(x, y)
				})
			)
			.OrderByDescending(x => x.Detectable)
			.First();
			return (asteroid.X, asteroid.Y, asteroid.Detectable);

			int DetectableFrom(int xpos, int ypos)
			{
				// Create a copy of the map to work on
				var map = mapinfo.Select(x => x.ToArray()).ToArray();
				map[ypos][xpos] = '@'; // welcome to nethack

				var detected = 0;
				for (var dx = 0; xpos + dx >= 0; dx--)
				{
					MapX(dx);
				}
				for (var dx = 1; xpos + dx < w; dx++)
				{
					MapX(dx);
				}
				return map.Sum(row => row.Count(ch => ch == '#'));

				void MapX(int dx)
				{
					for (var dy = 0; ypos + dy >= 0; dy--)
					{
						if (dx == 0 && dy == 0)
							continue;
						MapXY(dx, dy);
					}
					for (var dy = 1; ypos + dy < h; dy++)
					{
						MapXY(dx, dy);
					}
				}

				void MapXY(int dx, int dy)
				{
					var x = xpos;
					var y = ypos;
					var visible = true;
					while (0 <= x && x < w && 0 <= y && y < h)
					{
						if (!visible)
						{
							map[y][x] = ' ';
						}
						else if (map[y][x] == '#')
						{
							detected++;
							visible = false;
						}
						x += dx;
						y += dy;
					}
				}
			}
		}

		private static IEnumerable<int> VaporizedAsteroidsFrom(string[] mapinfo, int xpos, int ypos)
		{
			var w = mapinfo[0].Length;
			var h = mapinfo.Length;

			var agByDist =
				Enumerable.Range(0, w).SelectMany(x =>
					Enumerable.Range(0, h).Select(y =>
					{
						if (mapinfo[y][x] == '#' && (x != xpos || y != ypos))
						{
							var angle = Math.Atan2(ypos - y, xpos - x) - Math.PI / 2;
							var dx = x - xpos;
							var dy = y - ypos;
							return new
							{
								X = x,
								Y = y,
								Dist = Math.Sqrt(dx * dx + dy * dy),
								Angle = angle >= 0 ? angle : angle + 2 * Math.PI
							};
						}
						return null;
					}))
			.Where(x => x != null)
			.GroupBy(x => x.Angle)
			.OrderBy(x => x.Key)
			.Select(a => a.OrderBy(x => x.Dist));

			for (var dist = 0; agByDist.Any(x => x.Count() > dist); dist++)
			{
				foreach (var ag in agByDist)
				{
					if (ag.Count() > dist)
					{
						var asteroid = ag.ElementAt(dist);
						yield return asteroid.X * 100 + asteroid.Y;
					}
				}
			}
		}
	}
}