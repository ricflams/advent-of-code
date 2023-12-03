using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Byte;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2016.Day13
{
	internal class Puzzle : PuzzleWithParameter<(int, int),int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "A Maze of Twisty Little Cubicles";
		public override int Year => 2016;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").WithParameter((7, 4)).Part1(11);
			Run("input").WithParameter((31, 39)).Part1(86).Part2(127);
		}

		protected override int Part1(string[] input)
		{
			var favorite = int.Parse(input[0]);
			var dest = Point.From(PuzzleParameter.Item1, PuzzleParameter.Item2);

			// Search in a map extending twice past the destination point
			var map = BuildMap(favorite, dest.X * 2, dest.Y * 2);
			var maze = new Maze(map);
			maze.Entry = Point.From(1, 1);
			var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
			var steps = graph.ShortestPathDijkstra(maze.Entry, dest);

			return steps;
		}

		protected override int Part2(string[] input)
		{
			var favorite = int.Parse(input[0]);
			var dest = Point.From(PuzzleParameter.Item1, PuzzleParameter.Item2);

			// We can at most move 50 steps away from (1,1)
			var map = BuildMap(favorite, 50+1, 50+1);

			var unvisitedBefore = map.Count('.');
			var visit = new Point[] {Point.From(1, 1)};
			for (var i = 0; i < 50+1; i++) // +1 because we want 50 steps *after* step 0
			{
				foreach (var v in visit)
				{
					map[v] = 'O';
				}
				visit = visit.SelectMany(v => v.LookAround()).Distinct().Where(p => map[p] == '.').ToArray();
			}

			var unvisitedAfter = map.Count('.');
			var visited = unvisitedBefore - unvisitedAfter;
			return visited;
		}

		private static CharMap BuildMap(int favorite, int width, int height)
		{
			var map = new CharMap('#');
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					// x*x + 3*x + 2*x*y + y + y*y
					var n = x*x + 3*x + 2*x*y + y + y*y;
					var bits = ((uint)(n + favorite)).NumberOfSetBits();
					map[x][y] = bits%2 == 1 ? '#' : '.';
				}
			}
			return map;
		}
	}
}
