using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day08.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 8";
		public override int Year => 2022;
		public override int Day => 8;

		public void Run()
		{
			Run("test1").Part1(21).Part2(8);
			Run("input").Part1(1711).Part2(301392);
		}

		private void MarkVisibleRowsFromLeft(char[,] map, char[,]  vis)
		{
			var (w, h) = map.Dim();

			//Console.WriteLine();
			//map.ConsoleWrite();

			for (var y = 1; y < h-1; y++)
			{
				var maxx = map[0,y];
				for (var x = 1; x < w-1; x++)
				{
					var t = map[x,y];
					if (t > maxx)
					{
						//map[x,y] = t2;
						vis[x,y] = '#';
						maxx = t;
					//	Console.WriteLine($"1  {x},{y}");
					}
				}
			}
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var (w, h) = map.Dim();
			var vis = CharMatrix.Create(w, h, '.');

			var n = w*2 + h*2 - 4;

			MarkVisibleRowsFromLeft(map, vis);
			for (var i = 0; i < 3; i++)
			{
				map = map.RotateClockwise(90);
				vis = vis.RotateClockwise(90);
				MarkVisibleRowsFromLeft(map, vis);
			}



			foreach (var b in vis)
			{
				if (b == '#')
					n++;
			}

			return n;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var (w, h) = map.Dim();

			var maxscore = 0;
			for (var y = 0; y < h; y++)
			{
				for (var x = 0; x < w; x++)
				{
					var score = ScenicScore(map, x, y);
					if (score > maxscore)
						maxscore = score;
				}
			}
			return maxscore;
		}

		public int ScenicScore(char[,] map, int x0, int y0)
		{
			var (w, h) = map.Dim();
			var t = map[x0, y0];

			var n1 = 0;
			for (var x = x0 - 1; x >= 0; x--)
			{
				n1++;
				if (map[x, y0] >= t)
					break;
			}

			var n2 = 0;
			for (var x = x0 + 1; x < w; x++)
			{
				n2++;
				if (map[x, y0] >= t)
					break;
			}

			var n3 = 0;
			for (var y = y0 - 1; y >= 0; y--)
			{
				n3++;
				if (map[x0, y] >= t)
					break;

			}

			var n4 = 0;
			for (var y = y0 + 1; y < h; y++)
			{
				n4++;
				if (map[x0, y] >= t)
					break;
			}

			return n1*n2*n3*n4;
		}

	}
}
