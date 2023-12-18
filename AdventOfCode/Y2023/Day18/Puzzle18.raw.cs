using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using MathNet.Numerics.Optimization.LineSearch;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode.Y2023.Day18.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").Part1(62).Part2(952408144115);
		//	Run("test2").Part1(0).Part2(0);
			Run("input").Part1(26857).Part2(129373230496292);
		//	Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var plan = input
				.Select(s => s.RxMatch("%c %d (#%s)").Get<char, int, string>())
				.ToArray();

			var p = Pose.From(0, 0, Direction.Up);

			var pts = new List<Point>();
			var pathlength = 0L;

			foreach (var dig in plan)
			{
				var (dir, n, rgb) = dig;
				p = Pose.From(p.Point, DirFromChar(dir));
				p.Move(n);
				pathlength += n;
				pts.Add(p.Point);
			}

			//var innerpoint = plan[0].Item1 == 'R' ? Point.From(1, 1) : Point.From(-1, 1);
			var innerpoint = Point.From(1, 1);

			//map.ConsoleWrite((p, v) => p==innerpoint ? 'S' : v > 0 ? '#' : '.');

			return AreaByShoelace(pts.ToArray(), pathlength);

			// Flood-fill inner tiles because we've only reconnoitered the rim

			//var inner = new HashSet<Point>(map.AllPoints());


			////		Fill(innerpoint);

			//var fill = new Queue<Point>();
			//fill.Enqueue(innerpoint);
			//while (fill.TryDequeue(out var pp))
			//{
			//	if (map[pp] > 0)
			//		continue;
			//	map[pp] = 1;
			//	var nexts = pp.LookDiagonallyAround().Where(x => map[x] == 0).ToArray();
			//	foreach (var ppp in nexts)
			//	{
			//		fill.Enqueue(ppp);
			//	}

			//	//map.ConsoleWrite((p, v) => p == innerpoint ? 'S' : v > 0 ? '#' : '.');

			//	//if (map.AllPoints().Count() % 1000 == 0)
			//	//{
			//	//	map.ConsoleWrite((p, v) => p == innerpoint ? 'S' : v > 0 ? '#' : '.');
			//	//}
			//}


			//return map.AllPoints().Count();

		}

		private long AreaByShoelace(Point[] points, long extra)
		{
			var pts = points.Prepend(points.Last()).Append(points[0]).ToArray();
			var area = 0L;
			for (var i = 1; i <= points.Length; i++)
			{
				area += (long)pts[i].X * (pts[i + 1].Y - pts[i - 1].Y);
				//Console.WriteLine(area / 2);
			}
			area = area / 2 + extra/2 + 1;
			return area;
		}

		private Direction DirFromChar(char ch) => ch switch
		{
			'R' => Direction.Right,
			'D' => Direction.Down,
			'L' => Direction.Left,
			'U' => Direction.Up,
			_ => throw new Exception()
		};

		protected override long Part2(string[] input)
		{
			var plan = input
				.Select(s => s.RxMatch("%c %d (#%s)").Get<char, int, string>())
				.ToArray();

			var p = Pose.From(0, 0, Direction.Up);

			var pts = new List<Point>();
			var pathlength = 0L;

			foreach (var dig in plan)
			{
				var (_, _, rgb) = dig;

				var direction = rgb[^1] switch
				{
					'0' => Direction.Right,
					'1' => Direction.Down,
					'2' => Direction.Left,
					'3' => Direction.Up,
					_ => throw new Exception()
				};
				var n = Convert.ToInt32(rgb[..^1], 16);

				p = Pose.From(p.Point, direction);
				p.Move(n);
				pathlength += n;
				pts.Add(p.Point);
			}

			return AreaByShoelace(pts.ToArray(), pathlength);
		}

	}
}
