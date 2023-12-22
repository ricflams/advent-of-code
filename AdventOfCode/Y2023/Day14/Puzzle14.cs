using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Globalization;
using System.Xml.Schema;

namespace AdventOfCode.Y2023.Day14
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").Part1(136).Part2(64);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(105003).Part2(93742);
			Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			TiltNorth(map);

			var weight = map.AllPoints(c => c == 'O')
				.Sum(p => input.Length - p.Y);


			return weight;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var (w, h) = map.Dim();
			map = map.ExpandBy(1, '#');

			var tiltN = Enumerable.Range(1, h).SelectMany(y => Enumerable.Range(1, w).Select(x => (x, y))).ToArray();
			var tiltS = Enumerable.Range(1, h).SelectMany(y => Enumerable.Range(1, w).Select(x => (x, 1+h-y))).ToArray();
			var tiltW = Enumerable.Range(1, w).SelectMany(x => Enumerable.Range(1, h).Select(y => (x, y))).ToArray();
			var tiltE = Enumerable.Range(1, w).SelectMany(x => Enumerable.Range(1, h).Select(y => (1+w-x, y))).ToArray();

			var N = 1000000000;
			var loads = new Dictionary<uint, (int Round, int Load)>();
			while (true)
			{
				var hash = HashMap();
				//var hash2 = HashMap();
				var load = CalculateLoad(map);//map.AllPoints(c => c == 'O').Sum(p => input.Length - p.Y);

				if (loads.TryGetValue(hash, out var seen))
				{
					var (init, loadseen) = seen;
					var cycle = loads.Count - init;
					var xx = ((N - init) % cycle) + init;
					return loads.First(x => x.Value.Round == xx).Value.Load;
				}
				loads[hash] = (loads.Count, load);
				
				// var map2 = map.Copy();
				// Tilt(map2, tiltN, 0, -1);
				// Tilt(map2, tiltW, -1, 0);
				// Tilt(map2, tiltS, 0, 1);
				// Tilt(map2, tiltE, 1, 0);
				

				Tilt(tiltN, 0, -1);
				Tilt(tiltW, -1, 0);
				Tilt(tiltS, 0, 1);
				Tilt(tiltE, 1, 0);

//				TiltCycle(ref map);

			}

			// var idx = N % loads.Count;
			// var result = loads.val;


			void Tilt((int,int)[] steps, int dx, int dy)
			{
				//var (w, h) = map.Dim();			
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
			

			uint HashMap()
			{
				var hash = 0u;
				var (w, h) = map.Dim();
				for (var x = 1; x < w-1; x++)
				{
					for (var y = 1; y < h-1; y++)
					{
						hash = (uint)(hash * 377 ^ map[x, y]);
					}
				}
				return hash;

				// var rocks = map.AllPoints(c => c == 'O').OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();
				// var h1 = Hashing.LongHash(rocks.Select(x => x.X*10000+x.Y).ToArray());
				// return h1;
			}					

			// ulong HashMap()
			// {
			// 	var hashedValue = 3074457345618258791UL;
			// 	var (w, h) = map.Dim();
			// 	for (var x = 1; x < w-1; x++)
			// 	{
			// 		for (var y = 1; y < h-1; y++)
			// 		{
			// 			hashedValue += map[x, y];
			// 			hashedValue *= 3074457345618258799UL;	

			// 			// if (map[x, y] == 'O')
			// 			// {
			// 			// 	hashedValue += (ulong)(x*w + y);
			// 			// 	hashedValue *= 3074457345618258799UL;					
			// 			// }
			// 		}
			// 	}
			// 	return hashedValue;

			// 	// var rocks = map.AllPoints(c => c == 'O').OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();
			// 	// var h1 = Hashing.LongHash(rocks.Select(x => x.X*10000+x.Y).ToArray());
			// 	// return h1;
			// }								
		}



		private int CalculateLoad(char[,] map)
		{
			var (w, h) = map.Dim();
			var load = 0;
			for (var x = 1; x < w-1; x++)
			{
				for (var y = 1; y < h-1; y++)
				{
					if (map[x, y] == 'O')
						load += h-y-1;
				}
			}
			return load;
		}		


// 		private void TiltCycle(ref char[,] map)
// 		{
// 			for (var i = 0; i < 4; i++)
// 			{
// 				TiltNorth(map);
// 				map = map.RotateClockwise(90);
// 				// map = map.RotateClockwise(270);
// 				// map = map.RotateClockwise(90);
// 			}

// //			TiltNorth(map);
// 			// TiltWest(map);
// 			// TiltSouth(map);
// 			// TiltEast(map);
// 		}




		private void TiltNorth(char[,] map)
		{
			var (w, h) = map.Dim();

			for (var y = 1; y < h; y++)//
			{
				for (var x = 0; x < w; x++)
				{
					if (map[x, y] == 'O')
					{
						var yy = y;
						while (yy > 0 && map[x, yy-1] == '.')//
							yy--;//
						if (yy != y)
						{
							map[x, y] = '.';
							map[x, yy] = 'O';
						}						
					}
				}
			}
		}

		private void TiltSouth(char[,] map)
		{
			var (w, h) = map.Dim();

			for (var y = h-1; y >= 0; y--)//
			{
				for (var x = 0; x < w; x++)
				{
					if (map[x, y] == 'O')
					{
						var yy = y;
						while (yy < h-1 && map[x, yy+1] == '.')//
							yy++;//
						if (yy != y)
						{
							map[x, y] = '.';
							map[x, yy] = 'O';
						}						
					}
				}
			}	
		}

		private void TiltEast(char[,] map)
		{
			var (w, h) = map.Dim();
			foreach (var p in map.AllPoints(c => c == 'O').OrderByDescending(p => p.X))
			{
				var pp = p;
				while (pp.X < w-1 && map.Get(pp.E) == '.')
					pp = pp.E;
				if (pp != p)
				{
					map.Set(p, '.');
					map.Set(pp, 'O');
				}
			}			
		}

		private void TiltWest(char[,] map)
		{
			foreach (var p in map.AllPoints(c => c == 'O').OrderBy(p => p.X))
			{
				var pp = p;
				while (pp.X > 0 && map.Get(pp.W) == '.')
					pp = pp.W;
				if (pp != p)
				{
					map.Set(p, '.');
					map.Set(pp, 'O');
				}
			}			
		}

	}
}
