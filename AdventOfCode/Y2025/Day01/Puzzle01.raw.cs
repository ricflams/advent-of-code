using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day01.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Secret Entrance";
		public override int Year => 2025;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(3).Part2(6);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(980).Part2(5961);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var pos = 50;
			var pwd = 0;
			foreach (var p in input)
			{
				if (p[0] == 'R')
					pos += int.Parse(p[1..]);
				else
					pos -= int.Parse(p[1..]);
				pos = pos %= 100;
				//Console.WriteLine($"Pos: {pos}");
				if (pos == 0)
					pwd++;
			}

			return pwd;
		}

		protected override long Part2(string[] input)
		{
			var pos = 50;
			var pwd = 0;
			foreach (var p in input)
			{
				var moves = int.Parse(p[1..]);
				var dir = p[0] == 'R' ? 1 : -1;
				while (moves-- > 0)
				{
					pos += dir;
					pos %= 100;
					if (pos == 0)
						pwd++;
				}
				//Console.WriteLine($"Pos: {pos}");
			}

			return pwd;
		}
	}
}
