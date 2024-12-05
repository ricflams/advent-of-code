using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using MathNet.Numerics;
using System.Reflection.PortableExecutable;

namespace AdventOfCode.Y2024.Day04.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(18).Part2(9);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			//map.ExpandBy(1, '.');

			var seen = new HashSet<Point>();
			var starts = map.AllPoints(ch => ch == 'X');
			var n = starts.Sum(p => CountFromX(map, seen, p));

			// foreach (var p in map.AllPoints())
			// {
			// 	if (!seen.Contains(p))
			// 		map.Set(p, '.');
			// }
			// map.ConsoleWrite();

			return n;
		}

		static int CountFromX(char[,] map, HashSet<Point> seen, Point p0)
		{
			var n = 0;
			if (CountMas(map, seen, p0, (1,0), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (1,1), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (0,1), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (-1,1), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (-1,0), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (-1,-1), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (0,-1), "MAS", 0)) n++;
			if (CountMas(map, seen, p0, (1,-1), "MAS", 0)) n++;
			return n;
		}

		static bool CountMas(char[,] map, HashSet<Point> seen, Point p0, (int,int) delta, string word, int pos)
		{
			if (pos == word.Length)
				return true;
			var p = Point.From(p0.X + delta.Item1, p0.Y + delta.Item2);
			if (map.InRange(p) && map.Get(p) == word[pos])
				return CountMas(map, seen, p, delta, word, pos+1);
			return false;
			// if (n > 0)
			// 	seen.Add(p0);
			// return n;
		}

		// static int Count(char[,] map, HashSet<Point> seen, Point p0, string word, int pos)
		// {
		// 	if (seen.Contains(p0))
		// 		return 0;
		// 	if (pos == word.Length)
		// 		return 1;
		// 	var n = 0;
		// 	foreach (var p in p0.LookDiagonallyAround().Where(p => !seen.Contains(p) && map.InRange(p) && map.Get(p) == word[pos]))
		// 	{
		// 		// for (var i = 0; i < pos; i++)
		// 		// 	Console.Write("    ");
		// 		// Console.WriteLine($"examine {p} for pos {pos}");
		// 		n += Count(map, seen, p, word, pos+1);
		// 		// for (var i = 0; i < pos; i++)
		// 		// 	Console.Write("    ");
		// 		// Console.WriteLine($"Found {n} at {p} for pos {pos}");
		// 		if (n > 0)
		// 		{
		// 			seen.Add(p);
		// 		}
		// 	}
		// 	if (n > 0)
		// 	{
		// 		seen.Add(p0);
		// 	}
		// 	return n;
		// }

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			map.ExpandBy(1, '.');

			var seen = new HashSet<Point>();
			var starts = map.AllPoints(ch => ch == 'A');
			var n = 0;
			foreach (var p in starts)
			{
				var diag = 0;
				if (CharAt(p.DiagonalUpLeft) == 'M' && CharAt(p.DiagonalDownRight) == 'S') diag++;
				if (CharAt(p.DiagonalUpRight) == 'M' && CharAt(p.DiagonalDownLeft) == 'S') diag++;
				if (CharAt(p.DiagonalDownLeft) == 'M' && CharAt(p.DiagonalUpRight) == 'S') diag++;
				if (CharAt(p.DiagonalDownRight) == 'M' && CharAt(p.DiagonalUpLeft) == 'S') diag++;
				if (diag >= 2)
					n++;
			}

			char CharAt(Point p) => map.InRange(p) ? map.Get(p) : '.';

			return n;
		}
	}
}
