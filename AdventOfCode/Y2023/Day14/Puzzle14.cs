using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day14
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 14;

		public void Run()
		{
			Run("test1").Part1(0).Part2(0);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{


			return 0;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
