using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Security.Cryptography;

namespace AdventOfCode.Y2024.Day14.Raw
{
	internal class Puzzle : PuzzleWithParameter<(int, int), long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").WithParameter((11, 7)).Part1(12);
			Run("input").WithParameter((101, 103)).Part1(218619324).Part2(0);

			// 16849 not right
			//Run("extra").Part1(0).Part2(0);
		}

		class Robot 
		{
			public Point Pos { get; set; }
			public Point V { get; init; }
			public static Robot Parse(string s)
			{
				// p=39,7 v=77,-20
				var (px, py, vx, vy) = s.RxMatch("p=%d,%d v=%d,%d").Get<int, int, int, int>();
				return new Robot
				{
					Pos = Point.From(px, py),
					V = Point.From(vx, vy)
				};
			}
		}

		protected override long Part1(string[] input)
		{
			var robots = input.Select(Robot.Parse).ToArray();
			var (width, height) = PuzzleParameter;

			var sec = 100;
			foreach (var r in robots)
			{
				r.Pos = Point.From((((r.Pos.X + sec * r.V.X) % width) + width) % width, (((r.Pos.Y + sec * r.V.Y) % height) + height) % height);
			}

			var halfw = width / 2;
			var halfh = height/ 2;

			var f1 = robots.Count(r => r.Pos.X < halfw && r.Pos.Y < halfh);
			var f2 = robots.Count(r => r.Pos.X > halfw && r.Pos.Y < halfh);
			var f3 = robots.Count(r => r.Pos.X < halfw && r.Pos.Y > halfh);
			var f4 = robots.Count(r => r.Pos.X > halfw && r.Pos.Y > halfh);
			var factor = (long)f1*f2*f3*f4;

			return factor;
		}

		protected override long Part2(string[] input)
		{
			var robots = input.Select(Robot.Parse).ToArray();
			var (width, height) = PuzzleParameter;

// 			var sec = 16849;
// 			var map2 = new CharMap('.');
// 			foreach (var r in robots)
// 			{
// 				r.Pos = Point.From((((r.Pos.X + sec * r.V.X) % width) + width) % width, (((r.Pos.Y + sec * r.V.Y) % height) + height) % height);
// 				map2[r.Pos] = '#';
// 			}
// 			map2.ConsoleWrite();
// ;

			for (var i = 1; i < 18888; i++)
			{
				var map = new CharMap('.');
				foreach (var r in robots)
				{
					r.Pos = Point.From((((r.Pos.X + r.V.X) % width) + width) % width, (((r.Pos.Y + r.V.Y) % height) + height) % height);
					map[r.Pos] = '#';
				}
				//if (i > 13000)
				{
					Console.WriteLine();
					Console.WriteLine(i);
					map.ConsoleWrite();
				;
				}
			}
			// var sec = 100;
			// foreach (var r in robots)
			// {
			// 	r.Pos = Point.From((((r.Pos.X + sec * r.V.X) % width) + width) % width, (((r.Pos.Y + sec * r.V.Y) % height) + height) % height);
			// }

			// var halfw = width / 2;
			// var halfh = height/ 2;

			// var f1 = robots.Count(r => r.Pos.X < halfw && r.Pos.Y < halfh);
			// var f2 = robots.Count(r => r.Pos.X > halfw && r.Pos.Y < halfh);
			// var f3 = robots.Count(r => r.Pos.X < halfw && r.Pos.Y > halfh);
			// var f4 = robots.Count(r => r.Pos.X > halfw && r.Pos.Y > halfh);
			// var factor = (long)f1*f2*f3*f4;

			// return factor;

			return 0;
		}
	}
}
