using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;

namespace AdventOfCode.Y2020.Day13
{
	internal class Puzzle : PuzzleRunner<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 13;

		public void Run()
		{
			//RunPuzzles("test1.txt", );
			//RunPuzzles("test2.txt", );
			//RunPuzzles("test3.txt", );
			RunPuzzles("input.txt", 0, 0);
		}

		protected override int Puzzle1(string[] input)
		{


			return 0;
		}

		protected override int Puzzle2(string[] input)
		{


			return 0;
		}
	}




	internal class Puzzle2 : PuzzleRunner<int,int>
	{
		public static Puzzle2 Instance = new Puzzle2();
		protected override int Year => 2020;
		protected override int Day => 13;

		public void Run()
		{
			//RunPuzzles("test1.txt", );
			//RunPuzzles("test2.txt", );
			//RunPuzzles("test3.txt", );
			RunPuzzles("input.txt", 0, 0);
		}

		protected override (int, int) Puzzle1And2(string[] input)
		{


			return (0, 0);
		}
	}
}
