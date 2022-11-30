using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day20.Raw
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "A Regular Map";
		public override int Year => 2018;
		public override int Day => 20;

		public void Run()
		{
			Run("test1").Part1(3);
			Run("test2").Part1(10);
			Run("test3").Part1(18);
			Run("test4").Part1(23);
			Run("test5").Part1(31);
			Run("input").Part1(4184).Part2(8596);
		}

		protected override int Part1(string[] input)
		{
			var rx = input.Single();
			// Console.WriteLine();
			// Console.WriteLine();
			// Console.WriteLine(rx);
			var map = new CharMap(' ');

			var p = Point.Origin;

			WalkRegex(map, p, rx, 1);
			map[p] = 'X';
			//map.ConsoleWrite();

			var longest = LongestWalk(map, p) / 2;

			return longest;
		}

		private int LongestWalk(CharMap map, Point p)
		{
			// map[p] = 'v';
			// var ways = p.LookAround().Where(x => map[x] == '.').ToArray();
			// return ways.Any() ? 1 + ways.Max(x => LongestWalk(map, x, dist)) : 0;

			map[p] = 'v';
			var dist = 0;
			var edge = new HashSet<Point> { p };
			while (edge.Any())
			{
				foreach (var e in edge)
					map[e] = 'v';
				edge = new HashSet<Point>(edge.SelectMany(e => e.LookAround().Where(x => map[x] == '.')));
				dist++;
			}
			return dist;
		}

		private int WalkRegex(CharMap map, Point p, string rx, int pos)
		{
			var p0 = p;
			while (pos > 0)
			{
				// Console.WriteLine();
				// Console.WriteLine(rx);
				// Console.WriteLine($"{new string(' ', pos)}^  {pos}");
				// map.ConsoleWrite((p2,ch) => p2 == p ? 'o' : ch);

				switch (rx[pos++])
				{
					case 'N': map[p.N] = map[p.N.N] = '.'; p = p.N.N; break;
					case 'E': map[p.E] = map[p.E.E] = '.'; p = p.E.E; break;
					case 'S': map[p.S] = map[p.S.S] = '.'; p = p.S.S; break;
					case 'W': map[p.W] = map[p.W.W] = '.'; p = p.W.W; break;
					case '$': return -1;
					case ')': return pos;
					//case '|': pos = WalkRegex(map, p0, rx, pos); break;
					case '|': p = p0; break;
					case '(': pos = WalkRegex(map, p, rx, pos); break;
				}

			}
			return pos;
		}

		protected override int Part2(string[] input)
		{
			var rx = input.Single();
			// Console.WriteLine();
			// Console.WriteLine();
			// Console.WriteLine(rx);
			var map = new CharMap(' ');

			var p = Point.Origin;

			WalkRegex(map, p, rx, 1);
			map[p] = 'X';
			// map.ConsoleWrite();


			var rooms = new HashSet<Point>();

			map[p] = 'v';
			var dist = 0;
			var edge = new HashSet<Point> { p };
			while (edge.Any())
			{
				foreach (var e in edge)
					map[e] = 'v';
				edge = new HashSet<Point>(edge.SelectMany(e => e.LookAround().Where(x => map[x] == '.')));
				dist++;

				if (dist%2==0 && dist/2 >= 1000)
				{
					foreach (var e in edge)
						rooms.Add(e);
				}
			}

// 17191 too high
			return rooms.Count();
		}
	}
}
