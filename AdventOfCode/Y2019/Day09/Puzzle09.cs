using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;

namespace AdventOfCode.Y2019.Day09
{
	internal class Puzzle : Puzzle<long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Sensor Boost";
		public override int Year => 2019;
		public override int Day => 9;

		public void Run()
		{
			RunFor("input", 2682107844, 34738);
		}

		protected override long Part1(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithInput(1)
				.Execute()
				.Output.Take();
			return result;
		}

		protected override long Part2(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithInput(2)
				.Execute()
				.Output.Take();
			return result;
		}
	}
}