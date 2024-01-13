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

namespace AdventOfCode.Y2023.Day14.Raw
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
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			TiltNorth(map);

			var weight = map.AllPoints(c => c == 'O')
				.Sum(p => input.Length - p.Y);


			return weight;
		}

		protected override long Part2(string[] input)
		{
			var map0 = CharMap.FromArray(input);
			var pos0 = map0.AllPoints(c => c == 'O').OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

			var map = CharMap.FromArray(input);

			var N = 1000000000;
			var loads = new Dictionary<ulong, (int Round, int Load)>();
			while (true)
			{
				var hash = HashMap(map);
				var load = map.AllPoints(c => c == 'O').Sum(p => input.Length - p.Y);

				if (loads.TryGetValue(hash, out var seen))
				{
					var (init, loadseen) = seen;
					var cycle = loads.Count - init;
					var xx = ((N - init) % cycle) + init;
					return loads.First(x => x.Value.Round == xx).Value.Load;
				}
				loads[hash] = (loads.Count, load);
				
				TiltCycle(map);
				if ((loads.Count()%10000000)==0)
					Console.Write(".");

				// map.ConsoleWrite();
				// Console.WriteLine();

				// var pos = map.AllPoints(c => c == 'O').OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();
				// if (pos.SequenceEqual(pos0))
				// {
				// 	break;
				// }
			}

			// var idx = N % loads.Count;
			// var result = loads.val;
		}

		private ulong HashMap(CharMap map)
		{
			var rocks = map.AllPoints(c => c == 'O').OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();
			var h1 = Hashing.LongHash(rocks.Select(x => x.X*10000+x.Y).ToArray());
			return h1;
		}


		private void TiltCycle(CharMap map)
		{
			TiltNorth(map);
			TiltWest(map);
			TiltSouth(map);
			TiltEast(map);
		}

		private void TiltNorth(CharMap map)
		{
			foreach (var p in map.AllPoints(c => c == 'O').OrderBy(p => p.Y))
			{
				var pp = p;
				while (map[pp.N] == '.')
				{
					map[pp] = '.';
					pp = pp.N;
					map[pp] = 'O';
				}
			}			
		}

		private void TiltSouth(CharMap map)
		{
			foreach (var p in map.AllPoints(c => c == 'O').OrderByDescending(p => p.Y))
			{
				var pp = p;
				while (map[pp.S] == '.')
				{
					map[pp] = '.';
					pp = pp.S;
					map[pp] = 'O';
				}
			}			
		}

		private void TiltEast(CharMap map)
		{
			foreach (var p in map.AllPoints(c => c == 'O').OrderByDescending(p => p.X))
			{
				var pp = p;
				while (map[pp.E] == '.')
				{
					map[pp] = '.';
					pp = pp.E;
					map[pp] = 'O';
				}
			}			
		}

		private void TiltWest(CharMap map)
		{
			foreach (var p in map.AllPoints(c => c == 'O').OrderBy(p => p.X))
			{
				var pp = p;
				while (map[pp.W] == '.')
				{
					map[pp] = '.';
					pp = pp.W;
					map[pp] = 'O';
				}
			}			
		}

	}
}
