using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

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

		internal class Game
		{
			public const char TileEmpty = ' ';
			public const char TileWall = Graphics.FullBlock;
			public const char TileBlock = 'o';
			public const char TilePaddle = '_';
			public const char TileBall = '*';

			public readonly SparseMap Map = new SparseMap();
			private readonly Engine _engine;
			private Func<Game, int> _paddleControl;

			public Game()
			{
				var outputBuffer = new List<int>();
				_engine = new Engine()
					.WithMemoryFromFile("Day13/input.txt")
					.OnOutput(engine =>
					{
						outputBuffer.Add((int)engine.Output.Take());
						if (outputBuffer.Count == 3)
						{
							var x = outputBuffer[0];
							var y = outputBuffer[1];
							var val = outputBuffer[2];
							outputBuffer.Clear();
							if (x == -1 && y == 0)
							{
								Score = val;
							}
							else
							{
								switch (val)
								{
									case 0: Map[x][y] = TileEmpty; break;
									case 1: Map[x][y] = TileWall; break;
									case 2: Map[x][y] = TileBlock; break;
									case 3: Map[x][y] = TilePaddle; Paddle = Point.From(x, y); break;
									case 4: Map[x][y] = TileBall; Ball = Point.From(x, y); break;
								}
							}
						}
					})
					.OnInput(engine =>
					{
						var direction = _paddleControl(this);
						engine.WithInput(direction);
					});
			}

			public Game WithFreePlay(Func<Game, int> paddleControl)
			{
				_paddleControl = paddleControl;
				_engine.WithMemoryValueAt(0, 2);
				return this;
			}

			public Game Run()
			{
				_engine.Execute();
				return this;
			}

			public Point Ball { get; internal set; }
			public Point Paddle { get; internal set; }
			public int Score { get; internal set; }

			public void Render()
			{
				Console.Clear();
				Console.WriteLine($"Score: {Score}");
				foreach (var line in Map.Render())
				{
					Console.WriteLine(line);
				}
			}
		}
	}
}
