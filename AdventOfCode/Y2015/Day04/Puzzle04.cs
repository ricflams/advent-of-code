using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Ideal Stocking Stuffer";
		public override int Year => 2015;
		public override int Day => 4;

		public override void Run()
		{
			Run("input").Part1(254575).Part2(1038736);
			Run("extra").Part1(346386).Part2(9958218);
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
