using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2016.Assembunny;

namespace AdventOfCode.Y2016.Day12
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Leonardo's Monorail";
		public override int Year => 2016;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(42);
			Run("input").Part1(318003).Part2(9227657);
		}

		protected override int Part1(string[] input)
		{
			var comp = new Computer(input);
			comp.Run();
			return comp.Regs[0];
		}

		protected override int Part2(string[] input)
		{
			var comp = new Computer(input);
			comp.Regs[2] = 1;
			comp.Run();
			return comp.Regs[0];
		}
	}
}
