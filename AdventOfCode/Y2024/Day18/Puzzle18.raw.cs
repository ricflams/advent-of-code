using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day18.Raw
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
			Run("input").WithParameter((70, 1024)).Part1(0).Part2("46,18");
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
			var steps = graph.ShortestPathDijkstra(maze.Entry, dest);

			return steps;
		}

		protected override string Part2(string[] input)
		{
			var bytes = input.Select(Point.Parse).ToArray();
			var (width, fallen) = PuzzleParameter;

			for (var fall = fallen + 1; fall < bytes.Length; fall++)
			{
				var map = new CharMap('#');
				for (var x = 0; x <= width; x++)
					for (var y = 0; y <= width; y++)
						map[x, y] = '.';
				foreach (var b in bytes.Take(fall))
					map[b] = '#';

				var maze = new Maze(map)
					.WithEntry(Point.From(0, 0));
				var dest = Point.From(width, width);

				var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
				var steps = graph.ShortestPathDijkstra(maze.Entry, dest);

				//Console.Write($"{bytes[fall - 1]} ");
				if (steps > 100000)
					return bytes[fall - 1].ToString();
			}

			return "no";
		}
	}
}
