using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2019.Day25
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 25;

		public void Run()
		{
			RunPart1For("input", 33624080);
		}

		protected override int Part1(string[] input)
		{
			//while (true)
			//{
			//	new Game()
			//		.WithController(new NethackishController())
			//		//.WithController(new RawController())
			//		.Run();
			//}

			var intcode = input[0];
			var password = new Game(intcode)
					.WithController(new AutoplayController())
					.Run()
					.Password;
			return password;
		}

		protected override int Part2(string[] _) => 0;
	}
}

