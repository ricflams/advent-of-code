using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2017.Day05
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "A Maze of Twisty Trampolines, All Alike";
		public override int Year => 2017;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(5).Part2(10);
			Run("input").Part1(391540).Part2(30513679);
			Run("extra").Part1(358309).Part2(28178177);
		}

		protected override int Part1(string[] input)
		{
			var jumps = input.Select(int.Parse).ToArray();

			var steps = 0;
			for (var i = 0; i >= 0 && i < jumps.Length; i += jumps[i]++)
			{
				steps++;
			}

			return steps;
		}

		protected override int Part2(string[] input)
		{
			var jumps = input.Select(int.Parse).ToArray();

			var steps = 0;
			for (var i = 0; i >= 0 && i < jumps.Length; )
			{
				var jmp = jumps[i];
				jumps[i] += jmp >= 3 ? -1 : 1;
				i += jmp;
				steps++;
			}

			return steps;
		}
	}
}
