using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.ComponentModel;
using System.Globalization;

namespace AdventOfCode.Y2024.Day20
{
	internal class Puzzle : PuzzleWithParameter<(int, int), long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 20;

		public override void Run()
		{
			Run("test1").WithParameter((1, 50)).Part1(44).Part2(285);
			//Run("test2").Part1(0).Part2(0);
			Run("input").WithParameter((100, 100)).Part1(1395).Part2(993178);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var minSaveTime = PuzzleParameter.Item1;
			var map = CharMap.FromArray(input);

			return FindCheats(map, 2, minSaveTime);

			var savedTime = PuzzleParameter.Item1;

			//var map = CharMap.FromArray(input);
			var (w, h) = map.Size();
			var start = map.AllPointsWhere(c => c == 'S').Single();
			var goal = map.AllPointsWhere(c => c == 'E').Single();

			map[start] = map[goal] = '.';

			//var steps = new Dictionary<Point, int>();
			var path = new List<Point>();

			for (
				Point p = start, prev = null;
				p != goal;
				(p, prev) = (p.LookAround().Single(x => x != prev && map[x] == '.'), p))
			{
				path.Add(p);
			}

			var dist = new Dictionary<Point, int>();
			for (var i = 0; i < path.Count; i++)
			{
				dist[path[i]] = path.Count - i;
			}
			dist[goal] = 0;

			var cheats = 0;
			foreach (var p in path)
			{
				foreach (var dir in DirectionExtensions.LookAroundDirection())
				{
					var p1 = p.Move(dir);
					var p2 = p1.Move(dir);
					if (!dist.ContainsKey(p1) && dist.ContainsKey(p2) && dist[p2] + 2 <= dist[p] - savedTime)
					{
						cheats++;
						// Console.WriteLine($"cheats={cheats}");
						// map.ConsoleWrite((pt, c) => pt == p ? '@' : pt == p1 ? '1' : pt == p2 ? '2' : c);
					}
				}
			}

			return cheats;

			// var maze = new Maze(map)
			// 	.WithEntry(start)
			// 	.WithExit(goal);
			// var graph = Graph<char>.BuildUnitGraphFromMazeByQueue(maze);
			// var steps = graph.AStarPath(graph[maze.Entry], graph[maze.Exit], (_, _) => 0);

			// steps.Reverse();
			// var length = steps.Count - 1;

			// var cheats = new HashSet<Point>();
			// var saves = new SafeDictionary<int, int>();
			// foreach (var s in steps)
			// {
			// 	//Console.Write('.');
			// 	var p = s.Id;
			// 	foreach (var dir in DirectionExtensions.LookAroundDirection())
			// 	{
			// 		var p1 = p.Move(dir);
			// 		if (cheats.Contains(p1))
			// 			continue;
			// 		var p2 = p1.Move(dir);
			// 		if (!map.Exists(p2))
			// 			continue;
			// 		if (map[p1] == '#' && map[p2] == '.')
			// 		{
			// 			var n2 = graph.Nodes.FirstOrDefault(n => n.Id == p2);

			// 			if (n2 == null || !steps.Contains(n2) || steps.IndexOf(n2) < steps.IndexOf(s))
			// 				continue;

			// 			var saved = steps.IndexOf(n2) - steps.IndexOf(s) - 2;

			// 			// // if (n2 != null && steps.Contains(n2) && steps.IndexOf(n2) < steps.IndexOf(s))
			// 			// // 	continue;
			// 			// // break a hole
			// 			// //Console.WriteLine($"Break at {p1} {p2}");
			// 			// var dest = graph[p2];
			// 			// s.Neighbors.Add(dest, 2);
			// 			// // map[p1] = '.';
			// 			// // var graph2 = Graph<char>.BuildUnitGraphFromMazeByQueue(maze);
			// 			// var steps2 = graph.ShortestPathDijkstra(maze.Entry, maze.Exit);
			// 			// var saved = length - steps2;

			// 			if (saved >= savedTime)
			// 			{
			// 				cheats.Add(p1);
			// 				//Console.WriteLine($"  saved {saved}");
			// 				saves[saved]++;
			// 				//map.ConsoleWrite((p, c) => p == p1 ? 'O' : c);
			// 			}
			// 			//	s.Neighbors.Remove(dest);
			// 			//map[p1] = '#';
			// 		}
			// 	}
			// }

			// // foreach (var s in saves.OrderBy(x => x.Key))
			// // 	Console.WriteLine($"there are {s.Value} cheats that save {s.Key} picosecond");

			// return cheats.Count;
		}

		private int FindCheats(CharMap map, int cheatLength, int minSaveTime)
		{
			//var minSaveTime = PuzzleParameter.Item2;
			var start = map.AllPointsWhere(c => c == 'S').Single();
			var goal = map.AllPointsWhere(c => c == 'E').Single();

			map[start] = map[goal] = '.';

			//var steps = new Dictionary<Point, int>();
			var path = new List<Point>();

			for (
				Point p = start, prev = null;
				p != goal;
				(p, prev) = (p.LookAround().Single(x => x != prev && map[x] == '.'), p))
			{
				path.Add(p);
			}
			path.Add(goal);

			var dist = new Dictionary<Point, int>();
			for (var i = 0; i < path.Count; i++)
			{
				dist[path[i]] = path.Count - i;
			}
			dist[goal] = 0;

			var cheats = 0;
			for (var i = 0; i < path.Count; i++)
			{
				var p = path[i];

				for (var j = i + 4; j < path.Count; j++)
				{
					var p2 = path[j];
					var mdist = p.ManhattanDistanceTo(p2);
					if (mdist <= cheatLength)
					{
						var delta = dist[p] - dist[p2];
						var saved = delta - mdist;
						if (saved >= minSaveTime)
							cheats++;
					}
				}
				//var vicinity = dist.Keys.Where(n => n != p && p.ManhattanDistanceTo(n) <= cheatLength).ToArray();
				// // Console.WriteLine();
				// // map.ConsoleWrite((p, c) => vicinity.Any(x => x.Id == p) ? 'V' : c);

				// foreach (var v in vicinity)
				// {
				// 	var delta = dist[p] - dist[v];
				// 	if (delta > 0)
				// 	{
				// 		var saved = delta - v.ManhattanDistanceTo(p);
				// 		if (saved >= minSaveTime)
				// 			cheats++;
				// 	}
				// }
			}
			return cheats;
		}

		protected override long Part2(string[] input)
		{
			var minSaveTime = PuzzleParameter.Item2;
			var map = CharMap.FromArray(input);

			return FindCheats(map, 20, minSaveTime);

			var (w, h) = map.Size();
			var start = map.AllPointsWhere(c => c == 'S').Single();
			var goal = map.AllPointsWhere(c => c == 'E').Single();

			map[start] = map[goal] = '.';

			//var steps = new Dictionary<Point, int>();
			var path = new List<Point>();

			for (
				Point p = start, prev = null;
				p != goal;
				(p, prev) = (p.LookAround().Single(x => x != prev && map[x] == '.'), p))
			{
				path.Add(p);
			}

			var dist = new Dictionary<Point, int>();
			for (var i = 0; i < path.Count; i++)
			{
				dist[path[i]] = path.Count - i;
			}
			dist[goal] = 0;

			var cheats = 0;
			foreach (var p in path)
			{
				var vicinity = dist.Keys.Where(n => p.ManhattanDistanceTo(n) <= 20).ToArray();
				// Console.WriteLine();
				// map.ConsoleWrite((p, c) => vicinity.Any(x => x.Id == p) ? 'V' : c);

				foreach (var v in vicinity)
				{
					var delta = dist[p] - dist[v];
					if (delta > 0)
					{
						var saved = delta - v.ManhattanDistanceTo(p);
						if (saved >= minSaveTime)
							cheats++;
					}
				}
				// var allshortcuts = vicinity.Where(n => dist[p] > dist[n] && dist[p] - dist[n] - n.ManhattanDistanceTo(p) <= savedTime).ToArray();

				// Console.WriteLine();
				// map.ConsoleWrite((p, c) => allshortcuts.Any(x => x.Id == p) ? 'O' : c);

				// var savingshortcuts = allshortcuts.Where(n => steps.IndexOf(n) - step >= savedTime);
				// // Console.WriteLine();
				// // map.ConsoleWrite((p, c) => savingshortcuts.Any(x => x.Id == p) ? 'O' : c);

				// var shortcuts = savingshortcuts.Where(p2 =>
				// {
				// 	if (p.X < p2.Id.X && map[p2.Id.Left] == '.') return false;
				// 	if (p.X > p2.Id.X && map[p2.Id.Right] == '.') return false;
				// 	if (p.Y < p2.Id.Y && map[p2.Id.Up] == '.') return false;
				// 	if (p.Y > p2.Id.Y && map[p2.Id.Down] == '.') return false;
				// 	return true;
				// }).ToArray();
				// Console.WriteLine();
				// map.ConsoleWrite((p, c) => shortcuts.Any(x => x.Id == p) ? 'O' : c);

				// foreach (var s in shortcuts)
				// 	cheats2.Add((p, s.Id));
				//cheats += allshortcuts.Count();
			}

			return cheats;

			// var savedTime = PuzzleParameter.Item2;

			// var map = CharMap.FromArray(input);
			// var (w, h) = map.Size();
			// var start = map.AllPointsWhere(c => c == 'S').Single();
			// var goal = map.AllPointsWhere(c => c == 'E').Single();
			// map[start] = map[goal] = '.';
			// var maze = new Maze(map)
			// 	.WithEntry(start)
			// 	.WithExit(goal);
			// var graph = Graph<char>.BuildUnitGraphFromMazeByQueue(maze);
			// var steps = graph.AStarPath(graph[maze.Entry], graph[maze.Exit], (_, _) => 0);

			// steps.Reverse();
			// var length = steps.Count - 1;

			// var cheats = new HashSet<Point>();
			// var cheats2 = new HashSet<(Point, Point)>();
			// var saves = new SafeDictionary<int, int>();

			// for (var step = 0; step < steps.Count; step++)
			// {
			// 	//Console.Write('.');
			// 	var p = steps[step].Id;

			// 	var vicinity = graph.Nodes.Where(n => n.Id.ManhattanDistanceTo(p) <= 20).ToArray();
			// 	// Console.WriteLine();
			// 	// map.ConsoleWrite((p, c) => vicinity.Any(x => x.Id == p) ? 'V' : c);

			// 	var allshortcuts = vicinity.Where(n => steps.IndexOf(n) > step).ToArray();
			// 	// Console.WriteLine();
			// 	// map.ConsoleWrite((p, c) => allshortcuts.Any(x => x.Id == p) ? 'O' : c);

			// 	var savingshortcuts = allshortcuts.Where(n => steps.IndexOf(n) - step >= savedTime);
			// 	// Console.WriteLine();
			// 	// map.ConsoleWrite((p, c) => savingshortcuts.Any(x => x.Id == p) ? 'O' : c);

			// 	var shortcuts = savingshortcuts.Where(p2 =>
			// 	{
			// 		if (p.X < p2.Id.X && map[p2.Id.Left] == '.') return false;
			// 		if (p.X > p2.Id.X && map[p2.Id.Right] == '.') return false;
			// 		if (p.Y < p2.Id.Y && map[p2.Id.Up] == '.') return false;
			// 		if (p.Y > p2.Id.Y && map[p2.Id.Down] == '.') return false;
			// 		return true;
			// 	}).ToArray();
			// 	Console.WriteLine();
			// 	map.ConsoleWrite((p, c) => shortcuts.Any(x => x.Id == p) ? 'O' : c);

			// 	foreach (var s in shortcuts)
			// 		cheats2.Add((p, s.Id));
			// }

			// // foreach (var s in saves.OrderBy(x => x.Key))
			// // 	Console.WriteLine($"there are {s.Value} cheats that save {s.Key} picosecond");

			// return cheats2.Count;
		}
	}
}
