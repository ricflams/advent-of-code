using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day15.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(2028);
			Run("test2").Part1(10092).Part2(9021);
			Run("input").Part1(1318523).Part2(0);
			//Run("extra").Part1(0).Part2(0);
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
			
			// map[robot] = '@';
			// map.ConsoleWrite();
			//map[robot] = '.';

				var dir = m switch
				{
					'<' => Point.From(-1, 0),
					'>' => Point.From(1, 0),
					'^' => Point.From(0, -1),
					'v' => Point.From(0, 1),
					_ => throw new Exception()
				};

				var next = robot + dir;
				if (map[next] == '#')
					continue;
				if (map[next] == '.')
				{
					//map[robot] = '.';
					robot = next;
//					map[robot] = '@';
					continue;
				}
				// can push?
				var hole = robot + dir;
				while (map[hole] == 'O')
					hole += dir;
				if (map[hole] == '.')
				{
					map[hole] = 'O';
					robot = next;
					map[robot] = '.';
//					map[robot] = '@';
					continue;
				}
			}

			var sum = 0;
			foreach (var box in map.AllPointsWhere(c => c == 'O'))
			{
				sum += box.Y * 100 + box.X;
			}

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
				var repl = c switch
				{
					'#' => "##",
					'O' => "[]",
					'.' => "..",
					'@' => "@.",
					_ => throw new Exception()
				};
				map[p.X*2, p.Y] = repl[0];
				map[p.X*2+1, p.Y] = repl[1];
			}
			map.ConsoleWrite();

			var robot = map.AllPointsWhere(c => c == '@').Single();
			map[robot] = '.';

			foreach (var m in moves)
			{

			// Console.WriteLine();
			// Console.WriteLine($"Move {m}");
			// map[robot] = '@';
			// map.ConsoleWrite();
			// map[robot] = '.';

			// map[robot] = '@';
			// map.ConsoleWrite();
			//map[robot] = '.';

				var dir = m switch
				{
					'<' => Point.From(-1, 0),
					'>' => Point.From(1, 0),
					'^' => Point.From(0, -1),
					'v' => Point.From(0, 1),
					_ => throw new Exception()
				};

				var next = robot + dir;
				if (map[next] == '#')
					continue;
				if (map[next] == '.')
				{
					robot = next;
					continue;
				}

				// can push?
				if (m is '<' or '>')
				{
					var hole = robot + dir;
					while (map[hole] is '[' or ']')
						hole += dir;
					if (map[hole] == '.')
					{
						while (hole != next)
						{
							map[hole] = map[hole - dir];
							hole -= dir;
						}
						robot = next;
						map[robot] = '.';
	//					map[robot] = '@';
					}				
					continue;
				}

				if (CanPush(next))
				{
					DoPush(next);
					robot = next;
				}

				bool CanPush(Point p)
				{
					if (map[p] == '#')
						return false;
					if (map[p] == '[')
						return CanPush(p + dir) && CanPush(p.Right + dir);
					if (map[p] == ']')
						return CanPush(p + dir) && CanPush(p.Left + dir);
					return true;
				}

				void DoPush(Point p)
				{
					if (map[p] == '[')
					{
						DoPush(p + dir);
						DoPush(p.Right + dir);
					}
					if (map[p] == ']')
					{
						DoPush(p + dir);
						DoPush(p.Left + dir);
					}
					map[p] = map[p-dir];
					map[p-dir] = '.';
				}

			}



		//	map.ConsoleWrite();

			var sum = 0;
			foreach (var box in map.AllPointsWhere(c => c == '['))
			{
				sum += box.Y * 100 + box.X;
			}

			return sum;
		}
	}
}
