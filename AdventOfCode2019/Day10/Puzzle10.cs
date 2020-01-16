using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2019.Day10
{
	internal class Puzzle10
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var asteroidmap = File.ReadAllLines("Day10/input.txt");
			var (_, _, detectableAsteroids) = MaxDetectable(asteroidmap);
			Console.WriteLine($"Day 10 Puzzle 1: {detectableAsteroids}");
			Debug.Assert(detectableAsteroids == 299);
		}

		private static void Puzzle2()
		{
			var asteroidmap = File.ReadAllLines("Day10/input.txt");
			var (stationX, stationY, detectableAsteroids) = MaxDetectable(asteroidmap);

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
			Console.WriteLine($"Day 10 Puzzle 2: {asteroid200}");
			Debug.Assert(asteroid200 == 1419);
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
						//Console.WriteLine($"{asteroid.X},{asteroid.Y} at angle {asteroid.Angle}");
						//if (astoroids.Count() > dist + 1)
						//{
						//	Console.WriteLine($"--- skipping {string.Join(" ", astoroids.Skip(dist + 1).Select(x => $"{x.X},{x.Y}"))}");
						//}
						yield return asteroid.X * 100 + asteroid.Y;
					}
				}
			}
		}
	}
}