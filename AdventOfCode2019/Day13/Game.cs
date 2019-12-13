using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019
{
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
					var direction = _paddleControl.Invoke(this);
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
