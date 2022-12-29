using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day17
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Pyroclastic Flow";
		public override int Year => 2022;
		public override int Day => 17;

		public void Run()
		{
			Run("test1").Part1(3068).Part2(1514285714288);
			Run("test9").Part1(3227).Part2(1597714285698);
			Run("input").Part1(3133).Part2(1547953216393);
		}

		protected override long Part1(string[] input)
		{
			var cave = new Cave(input);
			for (var j = 0; j < 2022; j++)
			{
				cave.DropRock();
			}
			return cave.Height;
		}

		protected override long Part2(string[] input)
		{
			var cave = new Cave(input);

			var heights = new List<int>();
			var cycles = new Dictionary<string, (int,int)>();

			var fullHeight = 0L;
			while (fullHeight == 0)
			{
				// Remember the height at each drop for easy calculation of remaining height at the end
				heights.Add(cave.Height);

				// Drop a rock and check if we end up in a cycle, looking at the exact same
				// place in jets and rocks and with a cycle-time (delta) exactly like last time
				cave.DropRock((drop, jet, rock) =>
				{
					var delta = -1;
					var key = $"{jet}-{rock}";
					if (cycles.TryGetValue(key, out var lastseen))
					{
						var (drop0, delta0) = lastseen;
						delta = heights.Last() - heights[drop0];
						if (delta == delta0) // it's a cycle
						{
							var rocks = 1000000000000 - drop0;
							var rocksPerCycle = drop - drop0;
							var fullCycles = rocks / rocksPerCycle;
							var remainingRocks = (int)(rocks % rocksPerCycle);
							var remainingHeight = heights[drop0 + remainingRocks];
							fullHeight = fullCycles * delta + remainingHeight;
							return true;	
						}
					}
					cycles[key] = (drop, delta);
					return false;
				});
			}
			return fullHeight;
		}

		private class Cave : CharMap
		{
			public Cave(string[] input) : base('.')
			{
				Jets = input[0];

				for (var x = 1; x < Width+1; x++)
					this[x][0] = '-';
			}

			public const int Width = 7;
			public readonly string Jets;
			public int Height => -_top;

			private int _top = 0;
			private int _jet = 0;
			private int _drop = 0;

			public void FillEmptyRow(int y)
			{
				for (var x = 1; x < Width+1; x++)
					this[x][y] = '.';
				this[0][y] = this[Width+1][y] = '|';
			}

			public void DropRock(Func<int, int, int, bool> EavesDrop = null)
			{
				var rock = Rocks[_drop % Rocks.Length];
				for (var i = 0; i < 3 + rock.Length; i++)
				{
					FillEmptyRow(_top - 1 - i);
				}

				var w = rock[0].Length;
				var x = 2 + 1; // [0] is wall
				var y = _top - (3 + rock.Length);

				while (true)
				{
					if (EavesDrop?.Invoke(_drop, _jet % Jets.Length, _drop % Rocks.Length) ?? false)
						return;
					var jet = Jets[_jet++ % Jets.Length];
					if (jet == '<')
					{
						if (CanMove(rock, x-1, y))
							x--;
					}
					else
					{
						if (CanMove(rock, x+1, y))
							x++;
					}
					if (!CanMove(rock, x, y+1))
						break;
					y++;
				}
				Draw(rock, x, y);
				if (y < _top)
					_top = y;

				_drop++;
			}

			public bool CanMove(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#' && this[x0+x][y0+y] != '.')
							return false;
					}
				}
				return true;
			}

			public void Draw(string[] rock, int x0, int y0)
			{
				var w = rock[0].Length;
				var h = rock.Length;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						if (rock[y][x] == '#')
							this[x0+x][y0+y] = '#';
					}
				}
			}			

			private readonly string[][] Rocks = new string[][]
			{
				new []
				{
					"####"
				},
				new []
				{
					".#.",
					"###",
					".#."
				},
				new []
				{
					"..#",
					"..#",
					"###"
				},
				new []
				{
					"#",
					"#",
					"#",
					"#"
				},
				new []
				{
					"##",
					"##"
				}
			};
		}
	}
}