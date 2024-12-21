using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Net.Security;

namespace AdventOfCode.Y2024.Day18
{
	internal class Puzzle : PuzzleWithParameter<(int, int), long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").WithParameter((6, 12)).Part1(22).Part2("6,1");
			//Run("test2").Part1(0).Part2(0);
			Run("input").WithParameter((70, 1024)).Part1(344).Part2("46,18");
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var bytes = input.Select(Point.Parse).ToArray();
			var (width, fallen) = PuzzleParameter;

			var map = new CharMap('#');
			for (var x = 0; x <= width; x++)
				for (var y = 0; y <= width; y++)
					map[x, y] = '.';
			foreach (var b in bytes.Take(fallen))
				map[b] = '#';

			var maze = new Maze(map)
				.WithEntry(Point.From(0, 0));
			var dest = Point.From(width, width);

			var graph = Graph<char>.BuildUnitGraphFromMaze(maze);

			//var steps = graph.ShortestPathDijkstra(maze.Entry, dest);
			var steps = graph.AStarPath(graph[maze.Entry], graph[dest], Heuristics).Count() - 1;

			return steps;
		}

		static int Heuristics(Graph<Point, bool>.Node a, Graph<Point, bool>.Node b)
		{
			return a.Id.ManhattanDistanceTo(b.Id);
		}

		protected override string Part2(string[] input)
		{
			var bytes = input.Select(Point.Parse).ToArray();
			var (width, fallen) = PuzzleParameter;

			var map = new CharMap('#');
			for (var x = 0; x <= width; x++)
				for (var y = 0; y <= width; y++)
					map[x, y] = '.';
			foreach (var b in bytes.Take(fallen + 1))
				map[b] = '#';

			var maze = new Maze(map)
				.WithEntry(Point.From(0, 0))
				.WithExit(Point.From(width, width));

			var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
			var steps = new HashSet<Point>(graph.AStarPath(graph[maze.Entry], graph[maze.Exit], Heuristics).Select(n => n.Id));

			foreach (var b in bytes.Skip(fallen + 1))
			{
				map[b] = '#';
				if (!steps.Contains(b))
					continue;
				Console.WriteLine("regraph");
				graph = Graph<char>.BuildUnitGraphFromMaze(maze);
				if (graph[maze.Exit] == null)
					return $"{b.X},{b.Y}";
				var path = graph.AStarPath(graph[maze.Entry], graph[maze.Exit], Heuristics)?.Select(n => n.Id);
				if (path == null)
					return $"{b.X},{b.Y}";
				steps = [.. path];
			}
			return null;



			// var maze = new Maze(map)
			// 	.WithEntry(Point.From(0, 0));
			// var dest = Point.From(width, width);

			// var graph = Graph<char>.BuildUnitGraphFromMaze(maze);

			// //var steps = graph.ShortestPathDijkstra(maze.Entry, dest);
			// var steps = graph.AStarPath(graph[maze.Entry], graph[dest]).Count() - 1;

			// var min = fallen + 1;
			// var max = bytes.Length;
			// while (min < max - 1)
			// {
			// 	var guess = (max + min) / 2;
			// 	var isBlocked = IsBlocked(guess);
			// 	if (!isBlocked)
			// 		min = guess;
			// 	else
			// 		max = guess;
			// }

			// var last = bytes[min];
			// return $"{last.X},{last.Y}";

			// bool IsBlocked(int fall)
			// {
			// 	var map = new CharMap('#');
			// 	for (var x = 0; x <= width; x++)
			// 		for (var y = 0; y <= width; y++)
			// 			map[x, y] = '.';
			// 	foreach (var b in bytes.Take(fall))
			// 		map[b] = '#';

			// 	var maze = new Maze(map)
			// 		.WithEntry(Point.From(0, 0));
			// 	var dest = Point.From(width, width);

			// 	var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
			// 	var steps = graph.ShortestPathDijkstra(maze.Entry, dest);

			// 	//Console.Write($"{bytes[fall - 1]} ");
			// 	return steps > 100000;
			// }
		}
	}
}
