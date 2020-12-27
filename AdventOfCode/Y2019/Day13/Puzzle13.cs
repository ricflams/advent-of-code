using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day13
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 13;

		public void Run()
		{
			RunFor("input", 291, 14204);
		}

		protected override int Part1(string[] input)
		{
			var intcode = input[0];
			var blocks = new Game(intcode)
				.Run()
				.Map.AllPoints(value => value == Game.TileBlock)
				.Count();
			return blocks;
		}

		protected override int Part2(string[] input)
		{
			var intcode = input[0];
			var score = new Game(intcode)
				.WithFreePlay(RobotPaddleControl)
				//.WithFreePlay(UserPaddleControl)
				.Run()
				.Score;

			return score;

			static int RobotPaddleControl(Game game)
			{
				// The naivest of strategies: just follow the ball
				if (game.Ball.X < game.Paddle.X)
					return -1;
				if (game.Ball.X > game.Paddle.X)
					return 1;
				return 0;
			}

			//static int UserPaddleControl(Game game)
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

			public readonly CharMap Map = new CharMap();
			private readonly Engine _engine;
			private Func<Game, int> _paddleControl;

			public Game(string intcode)
			{
				var outputBuffer = new List<int>();
				_engine = new Engine()
					.WithMemory(intcode)
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
