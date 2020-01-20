using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day24
{
	internal static class Puzzle24
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var map = new CharMap();

			var lines = File.ReadAllLines("Day24/input.txt");
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					map[x][y] = line[x];
				}
			}

			var seen = new HashSet<uint>();
			while (true)
			{
				//map.ConsoleWrite(false);
				////Console.ReadKey();

				var bio = BioDiversity(map);
				if (seen.Contains(bio))
				{
					Console.WriteLine($"Day 24 Puzzle 1: {bio}");
					Debug.Assert(bio == 32573535);
					break;
				}
				seen.Add(bio);
				var nextmap = new CharMap();
				foreach (var pos in map.AllPoints().ToArray()) // ToArray should not be needed?
				{
					var n = pos.LookAround().Count(p => map[p] == '#');
					nextmap[pos] = map[pos] == '#'
						? n == 1 ? '#' : '.'
						: n == 1 || n == 2 ? '#' : '.';
				}
				map = nextmap;
			}
			//Console.WriteLine($"Day 24 Puzzle 1: {}");
			//Debug.Assert(beampoints == 141);
		}

		private static uint BioDiversity(CharMap map)
		{
			var (min, max) = map.Area();
			var width = max.Y - min.Y + 1;
			uint val = 0;
			foreach (var pos in map.AllPoints(c => c == '#'))
			{
				var position = pos.Y * width + pos.X;
				val += 1U << position;
			}
			return val;
		}

		private static void Puzzle2()
		{
			var map = new CharMap();
			var lines = File.ReadAllLines("Day24/input.txt");
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					map[x][y] = line[x];
				}
			}

			var width = 5;
			var height = 5;
			var levels = new List<CharMap>
			{
				map
			};

			var center = Point.From(2, 2);
			for (var t = 0; t < 200; t++)
			{
				if (levels.First().AllPoints(c => c == '#').Any(p => p.X == 0 || p.Y == 0 || p.X == width - 1 || p.Y == height - 1))
				{
					var outer = new CharMap('.');
					for (var x = 0; x < width; x++)
					{
						for (var y = 0; y < height; y++)
						{
							outer[x][y] = '.';
						}
					}
					levels.Insert(0, outer);
				}
				var lowestlevel = levels.Last();
				if (center.LookAround().Any(p => lowestlevel[p] == '#'))
				{
					var inner = new CharMap('.');
					for (var x = 0; x < width; x++)
					{
						for (var y = 0; y < height; y++)
						{
							inner[x][y] = '.';
						}
					}
					levels.Add(inner);
				}

				// Now go to work, one outer and its inner at a time
				var nextlevels = new List<CharMap>();
				for (var i = 0; i < levels.Count(); i++)
				{
					var outer = i > 0 ? levels[i - 1] : null;
					var level = levels[i];
					var inner = i < levels.Count() - 1 ? levels[i + 1] : null;
					var innerBugs = inner?.AllPoints(c => c == '#').ToArray();
					var nextmap = new CharMap();
					foreach (var pos in level.AllPoints().Where(p => !p.Is(center))) // ToArray should not be needed?
					{
						var n = Extensions.LookAroundDirection().Select(d => BugsInDirection(outer, level, innerBugs, pos, d)).Sum();
						var isOnBug = level[pos] == '#';
						nextmap[pos] = isOnBug
							? n == 1 ? '#' : '.'
							: n == 1 || n == 2 ? '#' : '.';
					}
					nextlevels.Add(nextmap);
				}
				levels = nextlevels;

				//Console.WriteLine();
				//Console.WriteLine($"After {t} minutes");
				//var leveldepth = lowest;
				//foreach (var l in levels)
				//{
				//	Console.WriteLine($"Depth {leveldepth++}:");
				//	l.ConsoleWrite(false);
				//	Console.WriteLine();
				//}
				//Console.WriteLine($"{levels.Count} levels, lowest={lowest}");
				//Console.ReadLine();

			}


			var bugs = levels.Sum(l => l.AllPoints(c => c == '#').Count());
			Console.WriteLine($"Day 24 Puzzle 2: {bugs}");
			Debug.Assert(bugs == 1951);

			int BugsInDirection(CharMap outer, CharMap level, Point[] innerBugs, Point pos0, Direction direction)
			{
				var pos = pos0.Move(direction);
				if (pos.X < 0)
				{
					return outer?[center.Left] == '#' ? 1 : 0;
				}
				if (pos.X >= width)
				{
					return outer?[center.Right] == '#' ? 1 : 0;
				}
				if (pos.Y < 0)
				{
					return outer?[center.Up] == '#' ? 1 : 0;
				}
				if (pos.Y >= height)
				{
					return outer?[center.Down] == '#' ? 1 : 0;
				}
				if (pos.Is(center))
				{
					switch (direction)
					{
						case Direction.Down: return innerBugs?.Count(p => p.Y == 0) ?? 0;
						case Direction.Left: return innerBugs?.Count(p => p.X == width - 1) ?? 0;
						case Direction.Up: return innerBugs?.Count(p => p.Y == height - 1) ?? 0;
						case Direction.Right: return innerBugs?.Count(p => p.X == 0) ?? 0;
					}
				}
				return level[pos] == '#' ? 1 : 0;
			}




			//IEnumerable<CharMap> PaddedLevels(IEnumerable<CharMap> maps)
			//{
			//	yield return new CharMap('.');
			//	foreach (var m in maps)
			//	{
			//		yield return m;
			//	}
			//	yield return new CharMap('.');
			//}


			//Console.WriteLine($"Day 24 Puzzle 2: {}");
			//Debug.Assert(beampoints == 141);
		}

	}

}
