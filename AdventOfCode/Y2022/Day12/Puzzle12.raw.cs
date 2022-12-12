using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day12.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 12";
		public override int Year => 2022;
		public override int Day => 12;

		public void Run()
		{
			Run("test1").Part1(31).Part2(29);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(484).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var dest = map.PositionsOf('E').Single();

		//	map.ConsoleWrite();

			return ShortestPath(map);
		}


		public int ShortestPath(char[,] map)
		{
			// Fast dictonary-key
			int Key(Point p) => p.X * 10000 + p.Y;

			var (w, h) = map.Dim();

			var curr = map.PositionsOf('S').Single();
			var dest = map.PositionsOf('E').Single();
			//Console.WriteLine($"{dest}   {w} x {h}");

			map[dest.X, dest.Y] = 'z';
			map[curr.X, curr.Y] = 'a';

			var queue = new PriorityQueue<(Point,int), int>();

			var times = new int[w,h];
			for (var x = 0; x < w; x++)
				for (var y = 0; y < h; y++)
					times[x,y] = int.MaxValue;
			//var times = new Dictionary<int, int>();

			queue.Enqueue((curr, 0), 0);

			while (queue.TryDequeue(out var item, out var _))
			{
				var (pos, time) = item;

				//Console.WriteLine($"{pos} {time}");
				if (pos == dest)
				{
					return time;
				}

				// Skip if we've visited this place w/ equip before in at least as short time

				if (times[pos.X, pos.Y] <= time)
				{
					continue;
				}
				times[pos.X, pos.Y] = time;

				// var key = Key(pos);
				// if (times.TryGetValue(key, out var fastest))
				// {
				// 	if (fastest <= time)
				// 		continue;
				// }
				// times[key] = time;

				// If target is found then ensure torch is being used
				var val = map[pos.X,pos.Y];
				// Explore
				foreach (var p in pos.LookAround().Where(p => p.X >= 0 && p.Y >= 0 && p.X < w && p.Y < h))
				{
					// // Optimization: skip those that have already been visited quicker
					// if (times.TryGetValue(Key(p), out fastest))
					// {
					// 	if (fastest <= time + 1) // +1 because it takes at least one step to go there
					// 		continue;
					// }

					var v = map[p.X, p.Y];
					if (v <= val+1)// || ((val == 'y' || val == 'z') && v == 'E'))
					{
						var dist = p.ManhattanDistanceTo(dest);
						queue.Enqueue((p, time + 1), time + 1 + dist); // Move takes 1 steps
					}
				}
			}
			//throw new Exception("No path found");

			// 504 not right
			// 505 not right
			return int.MaxValue;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var dest = map.PositionsOf('E').Single();

			var starts = map.AllPoints(ch => ch == 'a' || ch == 'S').ToArray();

			var dists = starts
				.Select(p =>
				{
					var map2 = CharMatrix.FromArray(input);
					var ss = map2.PositionsOf('S').Single();
					map2[ss.X, ss.Y] = 'a';
					map2[p.X, p.Y] = 'S';
					return ShortestPath(map2);
				})
				.OrderBy(x => x)
				.ToArray();
			return dists				
				.First();
		}
	}
}
