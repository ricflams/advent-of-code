﻿using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Not Quite Lisp";
		public override int Year => 2015;
		public override int Day => 1;

		public override void Run()
		{
			Run("input").Part1(280).Part2(1797);
			Run("extra").Part1(280).Part2(1797);
		}

		protected override int Part1(string[] input)
		{
			var line = input[0];
			var floor = line.Count(c => c == '(') - line.Count(c => c == ')');
			return floor;
		}

		protected override int Part2(string[] input)
		{
			var line = input[0];
			var moves = 0;
			for (var level = 0; level >= 0; level += line[moves++] == '(' ? 1 : -1)
			{
			}
			return moves;
		}
	}
}
