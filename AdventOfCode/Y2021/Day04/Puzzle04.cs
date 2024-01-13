using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Giant Squid";
		public override int Year => 2021;
		public override int Day => 4;

		public override void Run()
		{
			Run("test1").Part1(4512).Part2(1924);
			Run("input").Part1(50008).Part2(17408);
			Run("extra").Part1(58838).Part2(6256);
		}

		protected override int Part1(string[] input)
		{
			var (numbers, boards) = ReadBoards(input);
			foreach (var n in numbers)
			{
				foreach (var b in boards)
				{
					if (b.MarkAndCheckForWin(n))
					{
						return n * b.SumUnmarked;
					}
				}
			}
			throw new Exception("No winning board");
		}

		protected override int Part2(string[] input)
		{
			var (numbers, boards) = ReadBoards(input);

			foreach (var n in numbers)
			{
				foreach (var b in boards)
				{
					if (b.MarkAndCheckForWin(n))
					{
						if (boards.Length == 1)
						{
							return n * b.SumUnmarked;
						}
						boards.Purge(b);
					}
				}
			}
			throw new Exception("No winning board");
		}

		private static (int[], MutableArray<Board>) ReadBoards(string[] input)
		{
			var numbers = input.First().ToIntArray();
			var boards = input
				.Skip(1)
				.GroupByEmptyLine()
				.Select(lines => new Board(lines))
				.ToMutableArray();
			return (numbers, boards);
		}

		internal class Board
		{
			private readonly int[][] _grid;
			private readonly int _cols, _rows;

			public Board(string[] lines)
			{
				_grid = lines.Select(x => x.ToIntArray()).ToArray();
				_cols = _grid.First().Length;
				_rows = _grid.Length;
			}

			public int SumUnmarked => _grid.Sum(row => row.Where(x => x != -1).Sum());

			public bool MarkAndCheckForWin(int n)
			{
				for (var x = 0; x < _cols; x++)
				{
					for (var y = 0; y < _rows; y++)
					{
						if (_grid[y][x] == n)
						{
							_grid[y][x] = -1;
							return Enumerable.Range(0, _rows).All(yy => _grid[yy][x] == -1)
								|| _grid[y].All(v => v == -1);
						}
					}
				}
				return false;
			}
		}
	}
}
