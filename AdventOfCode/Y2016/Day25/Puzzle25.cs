using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2016.Assembunny;

namespace AdventOfCode.Y2016.Day25
{
	internal class Puzzle : Puzzle<int, bool>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Clock Signal";
		public override int Year => 2016;
		public override int Day => 25;

		public override void Run()
		{
			Run("input").Part1(180);
			Run("extra").Part1(189);
		}

		protected override int Part1(string[] input)
		{
			var clock = new [] { 0, 1, 0, 1, 0, 1, 0, 1 }; // this ought to do it
			var comp = new Computer(input);
			var result = 0;
			// Try out seeds, comparing the output against a 01010101-sequence
			// for every seed. If it matches fully then set the result and halt
			// both the bunny-running program and the seed-loop.
			for (var seed = 1; result == 0; seed++)
			{
				// Clear the registers completely before each run
				comp.Regs[0] = seed;
				comp.Regs[1] = comp.Regs[2] = comp.Regs[3] = 0;
				var n = 0;
				comp.OnOutShouldHalt = v =>
				{
					if (n == clock.Length)
					{
						result = seed;
						return true;
					}
					return v != clock[n++];
				};
				comp.Run();
			}

			return result;
		}

		protected override bool Part2(string[] input) => true;
	}
}
