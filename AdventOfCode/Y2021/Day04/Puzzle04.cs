using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers.String;
using System.Collections;

namespace AdventOfCode.Y2021.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Giant Squid";
		public override int Year => 2021;
		public override int Day => 4;

		public void Run()
		{
			Run("test1").Part1(4512).Part2(1924);
			Run("input").Part1(50008).Part2(17408);
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

		private static (int[], PurgeableArray<Board>) ReadBoards(string[] input)
		{
			var numbers = input.First().ToIntArray();
			var boards = input
				.Skip(1)
				.GroupByEmptyLine()
				.Select(lines => new Board(lines))
				.ToPurgeableArray();
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

			public int SumUnmarked => +_grid.Sum(row => row.Where(x => x != -1).Sum());

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

	public static class Extensions
	{
		public static PurgeableArray<T> ToPurgeableArray<T>(this IEnumerable<T> enumerable)
		{
			return new PurgeableArray<T>(enumerable.ToArray());
		}
	}


	public class PurgeableArray<T> : IEnumerable
	{
		private readonly T[] _array;

		public PurgeableArray(T[] array) => _array = array;

		public IEnumerator<T> GetEnumerator() => new PurgeableArrayEnumerator(_array);
		IEnumerator IEnumerable.GetEnumerator() => 
			throw new Exception();
//			_enumerator;

		public void Purge(T item)
		{
			var index = Array.FindIndex(_array, x => item.Equals(x));
			if (index != -1)
			{
				_array[index] = default;
			}
		}

		public int Length => _array.Count(x => x != null);

		public class PurgeableArrayEnumerator : IEnumerator<T>
		{
			private readonly T[] _array;
			private int _index = -1;

			public PurgeableArrayEnumerator(T[] array) => _array = array;

			public T Current => _array[_index];

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				//throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				do
				{
					_index++;
					if (_index >= _array.Length)
						return false;
				} while (_array[_index] == null);
				return true;
			}

			public void Reset()
			{
				_index = -1;
			}
		}
	}

}
