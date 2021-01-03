using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day04
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Ideal Stocking Stuffer";
		public override int Year => 2015;
		public override int Day => 4;

		public void Run()
		{
			RunFor("input", 254575, 1038736);
		}

		protected override int Part1(string[] input)
		{
			var secret = input[0];
			var finder = new Md5HashFinder(Md5HashFinder.Condition5x0);
			var iterations = finder.FindMatches(secret, 0).First().Iterations;
			return iterations;
		}

		protected override int Part2(string[] input)
		{
			var secret = input[0];
			var finder = new Md5HashFinder(Md5HashFinder.Condition6x0);
			var iterations = finder.FindMatches(secret, 0).First().Iterations;
			return iterations;
		}
	}
}
