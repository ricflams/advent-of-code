using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(94).Part2(0);
		//	Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);

			// 1346 too low

			//	Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (w, h) = map.Size();

			map[1, -1] = '#';
//			map[w - 2, h] = '#';

			//var maze = new Maze(map)
			//	.WithEntry(Point.From(1, 0));



			//var dest = Point.From(w-2, h-1);
			//maze.ExternalMapPoints = [maze.Entry, dest];
			//var graph = Graph<char>.BuildUnitGraphFromMaze(maze);

			var seenall = new Dictionary<string, int>();
			var queue = Quack<(Point, int, HashSet<Point>)>.Create(QuackType.PriorityQueue);
			var start = Point.From(1, 0);
			var dest = Point.From(w-2, h-1);
			var maxSteps = 0;

			queue.Put((start, 0, new HashSet<Point>([start])), 0);

			while (queue.TryGet(out var item))
			{
				var (pos, steps, seen) = item;

				if (pos == dest)
				{
					if (steps > maxSteps)
						maxSteps = steps;
					continue;
				}

				//var key = $"{pos}-{slide}";
				//if (seenall.TryGetValue(key, out var s))
				//{
				//	if (s < steps)
				//		continue;
				//}
				//seenall[key] = steps;

				var ch = map[pos];
				if ("<>^v".Contains(ch))
				{
					var next = ch switch
					{
						'<' => pos.Left,
						'>' => pos.Right,
						'^' => pos.Up,
						'v' => pos.Down,
						_ => throw new Exception()
					};
					Debug.Assert(map[next] != '#');
					if (seen.Contains(next))
						continue;
					var seen2 = new HashSet<Point>(seen)
					{
						next
					};
					queue.Put((next, steps + 1, seen2), next.ManhattanDistanceTo(dest));
				}

				foreach (var n in pos.LookAround().Where(p => map[p] != '#'))
				{
					if (seen.Contains(n))
						continue;
					var seen2 = new HashSet<Point>(seen)
					{
						n
					};
					queue.Put((n, steps + 1, seen2), n.ManhattanDistanceTo(dest));
				}

				//if (map[pos.Left] != '#' && map[pos.Left] != '>')
				//	queue.Put((pos.Left, steps + 1, slide), pos.Left.ManhattanDistanceTo(dest));
				//if (map[pos.Right] != '#' && map[pos.Right] != '<')
				//	queue.Put((pos.Right, steps + 1, slide), pos.Right.ManhattanDistanceTo(dest));
				//if (map[pos.Up] != '#' && map[pos.Up] != 'v')
				//	queue.Put((pos.Up, steps + 1, slide), pos.Up.ManhattanDistanceTo(dest));
				//if (map[pos.Down] != '#' && map[pos.Down] != '^')
				//	queue.Put((pos.Down, steps + 1, slide), pos.Down.ManhattanDistanceTo(dest));
			}

			return maxSteps;
		}


		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
