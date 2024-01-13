using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day14
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Parabolic Reflector Dish";
		public override int Year => 2023;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").Part1(136).Part2(64);
			Run("input").Part1(105003).Part2(93742);
			Run("extra").Part1(108889).Part2(104671);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			// Tilt north
			var (w, h) = map.Dim();
			for (var y = 1; y < h; y++)
			{
				for (var x = 0; x < w; x++)
				{
					if (map[x, y] == 'O')
					{
						var yy = y;
						while (yy > 0 && map[x, yy - 1] == '.')
							yy--;
						if (yy != y)
						{
							map[x, y] = '.';
							map[x, yy] = 'O';
						}
					}
				}
			}

			var load = map.AllPoints(c => c == 'O').Sum(p => h - p.Y);

			return load;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var (w, h) = map.Dim();
			map = map.ExpandBy(1, '#');

			// We'll be tilting a lot of times so create up front all the x,y positions in the
			// order they should be visited, instead of having 4 different nested for-loops for the tilts.
			var tiltN = Enumerable.Range(1, h).SelectMany(y => Enumerable.Range(1, w).Select(x => (x, y))).ToArray();
			var tiltS = Enumerable.Range(1, h).SelectMany(y => Enumerable.Range(1, w).Select(x => (x, 1+h-y))).ToArray();
			var tiltW = Enumerable.Range(1, w).SelectMany(x => Enumerable.Range(1, h).Select(y => (x, y))).ToArray();
			var tiltE = Enumerable.Range(1, w).SelectMany(x => Enumerable.Range(1, h).Select(y => (1+w-x, y))).ToArray();

			// Keep tilting until we encounter a map-state we've seen before. Then do the
			// math to retrieve the load as it will be at round N.
			var N = 1000000000;
			var seen = new Dictionary<uint, (int Round, int Load)>();
			while (true)
			{
				var hash = MapStateHash();

				if (seen.TryGetValue(hash, out var match))
				{
					var (cycleStart, _) = match;
					var cycle = seen.Count - cycleStart;
					var roundModuloN = ((N - cycleStart) % cycle) + cycleStart;
					return seen.Single(x => x.Value.Round == roundModuloN).Value.Load;
				}

				var load = CalculateLoad();
				seen[hash] = (seen.Count, load);

				Tilt(tiltN, 0, -1);
				Tilt(tiltW, -1, 0);
				Tilt(tiltS, 0, 1);
				Tilt(tiltE, 1, 0);
			}


			void Tilt((int,int)[] steps, int dx, int dy)
			{
				foreach (var (x, y) in steps)
				{
					if (map[x, y] == 'O')
					{
						if (map[x+dx, y+dy] != '.')
							continue;
						var (xx, yy) = (x, y);
						while (true)
						{
							if (map[xx+dx, yy+dy] != '.')
								break;
						 	xx += dx;
						 	yy += dy;
						}
						map[x, y] = '.';
						map[xx, yy] = 'O';
					}
				}
			}
			

			uint MapStateHash()
			{
				var hash = 0u;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						hash = hash * 377 ^ map[x+1, y+1];
					}
				}
				return hash;
			}

			int CalculateLoad()
			{
				var load = 0;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (map[x + 1, y + 1] == 'O')
							load += h - y;
					}
				}
				return load;
			}
		}		
	}
}
