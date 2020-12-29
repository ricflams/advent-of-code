using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day01
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Not Quite Lisp";
		public override int Year => 2015;
		public override int Day => 1;

		public void Run()
		{
			RunFor("input", 280, 1797);
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
