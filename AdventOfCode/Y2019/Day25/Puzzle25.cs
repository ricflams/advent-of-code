using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2019.Day25
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Cryostasis";
		public override int Year => 2019;
		public override int Day => 25;

		public void Run()
		{
			Run("input").Part1(33624080);
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

