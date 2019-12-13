using System;
using System.Linq;

namespace AdventOfCode2019.Day13
{
    internal static class Puzzle
    {
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var blocks = new Game()
				.Run()
				.Map.AllPoints(value => value == Game.TileBlock)
				.Count();
			Console.WriteLine($"Day 13 Puzzle 1: {blocks}");
			System.Diagnostics.Debug.Assert(blocks == 291);
		}

		private static void Puzzle2()
		{
			var score = new Game()
				.WithFreePlay(RobotPaddleControl)
				//.WithFreePlay(UserPaddleControl)
				.Run()
				.Score;
			Console.WriteLine($"Day 13 Puzzle 2: {score}");
			System.Diagnostics.Debug.Assert(score == 14204);

			int RobotPaddleControl(Game game)
			{
				// The naivest of strategies: just follow the ball
				if (game.Ball.X < game.Paddle.X)
					return -1;
				if (game.Ball.X > game.Paddle.X)
					return 1;
				return 0;
			}

			//int UserPaddleControl(Game game)
			//{
			//	game.Render();
			//	switch (Console.ReadKey().Key)
			//	{
			//		case ConsoleKey.LeftArrow: return -1;
			//		case ConsoleKey.RightArrow: return 1;
			//		default: return 0;
			//	}
			//}
		}
	}
}
