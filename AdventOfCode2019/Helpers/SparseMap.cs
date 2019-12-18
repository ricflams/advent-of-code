using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Helpers
{
	internal class SparseMap
	{
		private readonly Dictionary<int, SparseMapColumn> _column = new Dictionary<int, SparseMapColumn>();
		private readonly char _defaultValue;

		public SparseMap(char defaultValue = default(char))
		{
			_defaultValue = defaultValue;
		}

		public string[] Render(Func<int, int, char, char> rendering = null)
		{
			var points = AllPoints().ToList();
			var xMin = points.Min(z => z.X);
			var xMax = points.Max(z => z.X);
			var yMin = points.Min(z => z.Y);
			var yMax = points.Max(z => z.Y);
			return Enumerable.Range(yMin, yMax - yMin + 1)
				.Select(y => Enumerable.Range(xMin, xMax - xMin + 1)
					.Select(x => rendering != null ? rendering(x, y, this[x][y]) : this[x][y])
					.ToArray()
				)
				.Select(ch => new string(ch))
				.ToArray();
		}

		public void ConsoleWrite(params string[] headers)
		{
			Console.Clear();
			foreach (var header in headers)
			{
				Console.WriteLine(header);
			}
			foreach (var line in Render())
			{
				Console.WriteLine(line);
			}
		}

		public IEnumerable<Point> AllPoints(Func<char, bool> predicate = null)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					if (predicate == null || predicate(column.Row[y]))
					{
						yield return Point.From(x, y);
					}
				}
			}
		}

		public char this[Point pos]
		{
			get => this[pos.X][pos.Y];
			set => this[pos.X][pos.Y] = value;
		}

		public SparseMapColumn this[int x]
		{
			get
			{
				if (!_column.ContainsKey(x))
				{
					_column[x] = new SparseMapColumn(_defaultValue);
				}
				return _column[x];
			}
		}

		public class SparseMapColumn
		{
			internal readonly Dictionary<int, char> Row = new Dictionary<int, char>();
			private readonly char _defaultValue;

			public SparseMapColumn(char defaultValue)
			{
				_defaultValue = defaultValue;
			}

			public char this[int y]
			{
				get => Row.ContainsKey(y) ? Row[y] : _defaultValue;
				set => Row[y] = value;
			}
		}
	}
}
