using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Y2023.Day03.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 3;

		public override void Run()
		{
			Run("test1").Part1(4361).Part2(467835);
			Run("test2").Part1(0).Part2(0);
			Run("input").Part1(533775).Part2(0);

			// 329312 not right
		}

		protected override long Part1(string[] input)
		{
				var map = CharMap.FromArray(input);
			var digp = map.AllPointsWhere(ch => ch != '.' && !char.IsDigit(ch))
				.SelectMany(p => p.LookDiagonallyAround())
				.Distinct()
				.Where(p => char.IsDigit(map[p]))
				.OrderBy(p => p.Y)
				.ThenBy(p => p.X)
				.ToArray();
			// foreach (var p in map.AllPoints(ch => ch != '.' && char.IsDigit(ch))
			// {
			// 	p.LookDiagonallyAround()
			// }

			// //map.ConsoleWrite();
			// foreach (var pp in digp)
			// {
			// 	map[pp] = 'X';
			// }
			// map.ConsoleWrite();

			var answers = 0;
			var seen = new HashSet<Point>();
			foreach (var p in digp)
			{
				var p0 = p;
				while (char.IsDigit(map[p0.Left]))
					p0 = p0.Left;
				if (seen.Contains(p0))
					continue;
				seen.Add(p0);
				var v = 0;
				while (char.IsDigit(map[p0]))
				{
					v = v*10 + map[p0] - '0';
					p0 = p0.Right;
				}
				answers += v;
			}


			return answers;
		}

		protected override long Part2(string[] input)
		{
				var map = CharMap.FromArray(input);
			var digp = map.AllPointsWhere(ch => ch == '*');

			var gearsum = 0;

			foreach (var dp in digp)
			{
				var ns = dp.LookDiagonallyAround().ToArray();

				var gearp = new List<Point>();

				if (char.IsDigit(map[dp.Left])) gearp.Add(dp.Left);
				if (char.IsDigit(map[dp.Right])) gearp.Add(dp.Right);

				if (char.IsDigit(map[dp.Up]))
				{
					gearp.Add(dp.Up);
				}
				else
				{
					if (char.IsDigit(map[dp.DiagonalUpLeft]))
						gearp.Add(dp.DiagonalUpLeft);
					if (char.IsDigit(map[dp.DiagonalUpRight]))
						gearp.Add(dp.DiagonalUpRight);
				}

				if (char.IsDigit(map[dp.Down]))
				{
					gearp.Add(dp.Down);
				}
				else
				{
					if (char.IsDigit(map[dp.DiagonalDownLeft]))
						gearp.Add(dp.DiagonalDownLeft);
					if (char.IsDigit(map[dp.DiagonalDownRight]))
						gearp.Add(dp.DiagonalDownRight);
				}

				if (gearp.Count == 2)
				{
					var v1 = GearValue(gearp[0]);
					var v2 = GearValue(gearp[1]);
					gearsum += v1*v2;
				}
			}

			int GearValue(Point p)
			{
				while (char.IsDigit(map[p.Left]))
					p = p.Left;
				var v = 0;
				while (char.IsDigit(map[p]))
				{
					v = v*10 + map[p] - '0';
					p = p.Right;
				}
				return v;
			}

			return gearsum;
		}
	}
}
