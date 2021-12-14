using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day11.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Dumbo Octopus";
		public override int Year => 2021;
		public override int Day => 11;

		public void Run()
		{
			Run("test1").Part1(1656).Part2(195);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(1735).Part2(400);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);


			var flashes = 0;
			var (min, max) = map.MinMax();
			//var w = area.Item2.X;
			//var h = area.Item2.Y;

			for (var i = 0; i < 100; i++)
			{
				//map.ConsoleWrite();
				//Console.WriteLine();

				map = map.Transform((p, ch) => (char)(ch + 1));

				var seen = new SparseMap<bool>();

				var nines = map.AllPoints(ch => ch > '9').ToArray();
				foreach (var p in nines)
				{
					//Flash(p);
					flashes++;
					seen[p] = true;
				}

				foreach (var p in nines)
				{
					Flash(p);
				}

				void Flash(Point p)
				{
					if (!seen[p])
						flashes++;
					seen[p] = true;
					foreach (var n in p.LookDiagonallyAround().Within(min, max).Where(p => !seen[p]))
					{
						map[n] = (char)(map[n] + 1);
						if (map[n] > '9')
							Flash(n);
					}
				}

				foreach (var p in map.AllPoints(ch => ch > '9'))
				{
					map[p] = '0';
				}

				//map = map.Transform((ch, adjacents) =>
				//{
				//	if (ch == '9')
				//	{
				//		flashes++;
				//		foreach (var c in adjacents)
				//		{
				//			if (c == '|' && ++n >= 3)
				//				return '|';
				//		}
				//	}

				//	return ch+1;
				//});

			}

			return flashes;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);


			var flashes = 0;
			var (min, max) = map.MinMax();
			//var w = area.Item2.X;
			//var h = area.Item2.Y;
			var N = (max.X + 1) * (max.Y + 1);

			var step = 0;
			while (true)
			{
				//map.ConsoleWrite();
				//Console.WriteLine();

				map = map.Transform((p, ch) => (char)(ch + 1));

				var seen = new SparseMap<bool>();

				var flashes0 = flashes;
				var nines = map.AllPoints(ch => ch > '9').ToArray();
				foreach (var p in nines)
				{
					//Flash(p);
					flashes++;
					seen[p] = true;
				}

				foreach (var p in nines)
				{
					Flash(p);
				}

				void Flash(Point p)
				{
					if (!seen[p])
						flashes++;
					seen[p] = true;
					foreach (var n in p.LookDiagonallyAround().Within(min, max).Where(p => !seen[p]))
					{
						map[n] = (char)(map[n] + 1);
						if (map[n] > '9')
							Flash(n);
					}
				}

				foreach (var p in map.AllPoints(ch => ch > '9'))
				{
					map[p] = '0';
				}

				step++;
				if (flashes0 + N == flashes)
				{
					return step;
				}

				//map = map.Transform((ch, adjacents) =>
				//{
				//	if (ch == '9')
				//	{
				//		flashes++;
				//		foreach (var c in adjacents)
				//		{
				//			if (c == '|' && ++n >= 3)
				//				return '|';
				//		}
				//	}

				//	return ch+1;
				//});

			}

			return flashes;
		}
	}
}