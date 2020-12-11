using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2020.Day11
{
	internal class Puzzle11
	{
		public static void Run()
		{
			//var input = File.ReadAllLines("Y2020/Day11/input.txt");


			//var result1 = 0;

			//int occupied = 0;
			//var m1 = CharMap.FromFile("Y2020/Day11/input.txt");
			//while (true)
			//{
			//	var tofill = m1.AllPoints(c => c == 'L').Where(p => p.LookDiagonallyAround().All(c => m1[c] != '#')).ToList();
			//	var toclear = m1.AllPoints(c => c == '#').Where(p => {

			//		var adj = p.LookDiagonallyAround();
			//		var nnn = adj.Count(c => m1[c] == '#');
			//		var gt4 = nnn >= 4;
			//		return gt4;
			//	}).ToList();

			//	foreach (var tf in tofill)
			//	{
			//		m1[tf] = '#';
			//	}
			//	foreach (var x2 in toclear)
			//	{
			//		m1[x2] = 'L';
			//	}

			//	//m1.ConsoleWrite(false);
			//	//Console.WriteLine();

			//	var nn = m1.AllValues(c => c == '#').Count();
			//	if (nn == occupied)
			//	{
			//		Console.WriteLine($"Day 11 Puzzle 1: {nn}");
			//		break;
			//	}
			//	occupied = nn;
			//}






			//Console.WriteLine($"Day 11 Puzzle 1: {result1}");
			//Debug.Assert(result == );


			var result2 = 0;

			var result1 = 0;

			int occupied = 0;
			var m1 = CharMap.FromFile("Y2020/Day11/input.txt");
			while (true)
			{
				var info = m1.AllPoints().Select(p =>
				{
					var n =
					(FirstOf(m1, p, Direction.Up) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Up, Direction.Right) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Right) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Right, Direction.Down) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Down) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Down, Direction.Left) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Left) == '#' ? 1 : 0) +
					(FirstOf(m1, p, Direction.Left, Direction.Up) == '#' ? 1 : 0);
					return new { Pointxx = p, Occupied = n };
				}
				).ToList();
				var tofill = info.Where(x => m1[x.Pointxx] == 'L' && x.Occupied == 0).ToList();
				var toclear = info.Where(x => m1[x.Pointxx] == '#' && x.Occupied >= 5).ToList();


				//var tofill = m1.AllPoints(c => c == 'L').Where(p =>
				//{
				//	var n =
				//	(FirstOf(m1, p, Direction.Up) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Up, Direction.Right) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Right) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Right, Direction.Down) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Down) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Down, Direction.Left) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Left) == '#' ? 1 : 0) +
				//	(FirstOf(m1, p, Direction.Left, Direction.Up) == '#' ? 1 : 0);
				//	return new bool { Pointxx = p, Occupied = n };
				//}
				//).ToList();


				//var toclear = m1.AllPoints(c => c == '#').Where(p => {

				//	var adj = p.LookDiagonallyAround();
				//	var nnn = adj.Count(c => m1[c] == '#');
				//	var gt4 = nnn >= 4;
				//	return gt4;
				//}).ToList();

				foreach (var tf in tofill)
				{
					m1[tf.Pointxx] = '#';
				}
				foreach (var x2 in toclear)
				{
					m1[x2.Pointxx] = 'L';
				}

				//m1.ConsoleWrite(false);
				//Console.WriteLine();

				var nn = m1.AllValues(c => c == '#').Count();
				if (nn == occupied)
				{
					Console.WriteLine($"Day 11 Puzzle 1: {nn}");
					break;
				}
				occupied = nn;
			}




			Console.WriteLine($"Day 11 Puzzle 2: {result2}");
			//Debug.Assert(result == );
		}

		static char FirstOf(CharMap map, Point p, params Direction[] dir)
		{
			while (true)
			{
				foreach (var d in dir)
				{
					p = p.Move(d);
				}
				if (map[p] != '.')
					return map[p];
			}
		}
	}
}
