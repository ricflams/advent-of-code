using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day15
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Warehouse Woes";
		public override int Year => 2024;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(2028);
			Run("test2").Part1(10092).Part2(9021);
			Run("input").Part1(1318523).Part2(1337648);
			Run("extra").Part1(1414416).Part2(1386070);
		}

		protected override long Part1(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();
			var map = CharMap.FromArray(parts[0]);
			var moves = string.Concat(parts[1]).ToCharArray();

			var robot = map.AllPointsWhere(c => c == '@').Single();
			map[robot] = '.';

			foreach (var m in moves)
			{
				// map.ConsoleWrite((p, c) => p == robot ? '@' : c);

				var dir = m.AsDirection();
				var next = robot + dir;

				// Can't move if there's a wall
				if (map[next] == '#')
					continue;

				// If there's a box then see if there's a hole so it can be moved
				if (map[next] != '.')
				{
					var hole = next;
					while (map[hole] == 'O')
						hole += dir;
					if (map[hole] != '.')
						continue;
					map[hole] = 'O';
					map[next] = '.';
				}

				robot = next;
			}

			var sum = map.AllPointsWhere(c => c == 'O').Sum(p => p.Y * 100 + p.X);

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();
			var map1 = CharMap.FromArray(parts[0]);
			var moves = string.Concat(parts[1]).ToCharArray();

			var map = new CharMap();
			foreach (var (p, c) in map1.All())
			{
				var wide = c switch
				{
					'#' => "##",
					'O' => "[]",
					'.' => "..",
					'@' => "@.",
					_ => throw new Exception()
				};
				map[p.X*2, p.Y] = wide[0];
				map[p.X*2+1, p.Y] = wide[1];
			}

			var robot = map.AllPointsWhere(c => c == '@').Single();
			map[robot] = '.';

			foreach (var m in moves)
			{
				//map.ConsoleWrite((p, c) => p == robot ? '@' : c);

				var dir = m.AsDirection();
				var next = robot + dir;

				// Can't move if there's a wall
				if (map[next] == '#')
					continue;

				// For left/right we look for a hole and then shift the entire set of boxes
				// For up/down we recursively check if boxes can be moved and then move them
				if (dir is Direction.Left or Direction.Right)
				{
					var hole = robot + dir;
					while (map[hole] is '[' or ']')
						hole += dir;
					if (map[hole] == '.')
					{
						// Found a hole; go backwards and move all one spot
						while (hole != next)
						{
							map[hole] = map[hole - dir];
							hole -= dir;
						}
						robot = next;
						map[robot] = '.';
					}				
				}
				else
				{
					// We can't start moving anything until we know all "branches" can be moved.
					// It could eg look like below, where one part can be moved but another can't.
					// So first we must check and then we can move.
					//
					//     #
					//     []  []
					//      [][]
					//       []
					//        @
					//
					if (CanMoveTo(next))
					{
						DoMove(next);
						robot = next;
					}

					bool CanMoveTo(Point p) => map[p] switch {
						'#' => false,
						'[' => CanMoveTo(p + dir) && CanMoveTo(p.Right + dir),
						']' => CanMoveTo(p + dir) && CanMoveTo(p.Left + dir),
						_ => true // is space
					};

					void DoMove(Point p)
					{
						if (map[p] == '[')
						{
							DoMove(p + dir);
							DoMove(p.Right + dir);
						}
						if (map[p] == ']')
						{
							DoMove(p + dir);
							DoMove(p.Left + dir);
						}
						map[p] = map[p - dir];
						map[p - dir] = '.';
					}
				}
			}

			var sum = map.AllPointsWhere(c => c == '[').Sum(p => p.Y * 100 + p.X);

			return sum;
		}
	}
}
