using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day13.Raw
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Transparent Origami";
		public override int Year => 2021;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(17);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(716).Part2("RPCKFBLR");
		}

		protected override int Part1(string[] input)
		{
			var things = input
				.GroupByEmptyLine()
				.ToArray();

			var coords0 = things[0]
				.Select(s =>
				{
					var xx = s.Split(',');
					return Point.From(int.Parse(xx[0]), int.Parse(xx[1]));
				})
				.ToArray();

			var minx = coords0.Select(x => x.X).Min();
			var miny = coords0.Select(x => x.Y).Min();

			var coords = coords0.Select(p => Point.From(p.X - minx, p.Y - miny)).ToArray();

			var w = coords.Select(x => x.X).Max() + 1;
			var h = coords.Select(x => x.Y).Max() + 1;

			var map = CharMatrix.Create(w, h, '.');
			foreach (var p in coords)
			{
				map[p.X, p.Y] = '#';
			}
			//map.ConsoleWrite();

			var folds = things[1].Select(s =>
			{
				var (axis, n) = s.RxMatch("fold along %c=%d").Get<char, int>();
				return new
				{
					Axis = axis,
					N = n
				};
			})
			.ToArray();

			//foreach (var f in folds)
			//{
			//	map = Fold(map, f.Axis, f.N);
			//	Console.WriteLine();
			//	map.ConsoleWrite();
			//}

			var fold0 = folds.First();

			map = Fold(map, fold0.Axis, fold0.N);


			return map.CountChar('#');
		}

		private static char[,] Fold(char[,] map, char axis, int n)
		{
			if (axis == 'x')
			{
				var h = map.Height();
				//var w = Math.Max(map.Width() - n + 1, n);

				var folded = map.CopyPart(n + 1, 0, map.Width() - n - 1, h).FlipV();
				var remain = map.CopyPart(0, 0, n, h);
				if (folded.Width() > remain.Width())
				{
					var tmp = folded;
					folded = remain;
					remain = tmp;
				}

				var dx = remain.Width() - folded.Width();
				for (var x = 0; x < folded.Width(); x++)
				{
					for (var y = 0; y < h; y++)
					{
						remain[x + dx, y] = remain[x + dx, y] == '#' || folded[x, y] == '#' ? '#' : '.';
					}
				}
				return remain;
			}

			if (axis == 'y')
			{
				var w = map.Width();
				//var h = Math.Max(map.Height() - n, n);

				var folded = map.CopyPart(0, n + 1, w, map.Height() - n - 1).FlipH();
				var remain = map.CopyPart(0, 0, w, n);
				if (folded.Width() > remain.Width())
				{
					var tmp = folded;
					folded = remain;
					remain = tmp;
				}

				var dy = remain.Width() - folded.Width();
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < folded.Height(); y++)
					{
						remain[x, y + dy] = remain[x, y + dy] == '#' || folded[x, y] == '#' ? '#' : '.';
					}
				}
				return remain;
			}

			return null;
		}

		protected override string Part2(string[] input)
		{
			var things = input
				.GroupByEmptyLine()
				.ToArray();

			var coords0 = things[0]
				.Select(s =>
				{
					var xx = s.Split(',');
					return Point.From(int.Parse(xx[0]), int.Parse(xx[1]));
				})
				.ToArray();

			var minx = coords0.Select(x => x.X).Min();
			var miny = coords0.Select(x => x.Y).Min();

			var coords = coords0.Select(p => Point.From(p.X - minx, p.Y - miny)).ToArray();

			var w = coords.Select(x => x.X).Max() + 1;
			var h = coords.Select(x => x.Y).Max() + 1;

			var map = CharMatrix.Create(w, h, '.');
			foreach (var p in coords)
			{
				map[p.X, p.Y] = '#';
			}
			//map.ConsoleWrite();

			var folds = things[1].Select(s =>
			{
				var (axis, n) = s.RxMatch("fold along %c=%d").Get<char, int>();
				return new
				{
					Axis = axis,
					N = n
				};
			})
			.ToArray();

			foreach (var f in folds)
			{
				map = Fold(map, f.Axis, f.N);
			}

			//map.ConsoleWrite();

			var code = LetterScanner.Scan(map.ToStringArray());

			return code;
		}
	}
}