using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System.Linq;

namespace AdventOfCode.Y2019.Day05
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 5;

		public void Run()
		{
			RunFor("input", 7566643, 9265694);
		}

		protected override long Part1(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithInput(1)
				.Execute()
				.Output.TakeAll()
				.SkipWhile(x => x == 0)
				.First();
			return result;
		}

		protected override long Part2(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithInput(5)
				.Execute()
				.Output.TakeAll()
				.SkipWhile(x => x == 0)
				.First();
			return result;
		}
	}
}