using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public class SparseMap<T>
	{
		protected readonly Dictionary<int, SparseMapColumn> _column = new Dictionary<int, SparseMapColumn>();
		protected readonly T _defaultValue;

		public SparseMap(T defaultValue = default)
		{
			_defaultValue = defaultValue;
		}

		public IEnumerable<(Point Point, T Value)> All()
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					yield return (Point.From(x, y), column.Row[y]);
				}
			}
		}

		public IEnumerable<(Point Point, T Value)> AllWhere(Func<T, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];
					if (predicate(value))
					{
						yield return (Point.From(x, y), value);
					}
				}
			}
		}

		public IEnumerable<(Point Point, T Value)> AllWhere(Func<Point, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];
					var p = Point.From(x, y);
					if (predicate(p))
					{
						yield return (p, value);
					}
				}
			}
		}

		public IEnumerable<(Point Point, T Value)> AllWhere(Func<Point, T, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];
					var p = Point.From(x, y);
					if (predicate(p, value))
					{
						yield return (p, value);
					}
				}
			}
		}

		public IEnumerable<Point> AllPoints()
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					yield return Point.From(x, y);
				}
			}
		}

		public IEnumerable<Point> AllPointsWhere(Func<T, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					if (predicate(column.Row[y]))
					{
						yield return Point.From(x, y);
					}
				}
			}
		}

		public IEnumerable<Point> AllPointsWhere(Func<Point, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var p = Point.From(x, y);
					if (predicate(p))
					{
						yield return p;
					}
				}
			}
		}

		public IEnumerable<Point> AllPointsWhere(Func<Point, T, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var p = Point.From(x, y);
					if (predicate(p, column.Row[y]))
					{
						yield return p;
					}
				}
			}
		}

		public IEnumerable<T> AllValues()
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];
					yield return value;
				}
			}
		}

		public IEnumerable<T> AllValuesWhere(Func<T, bool> predicate)
		{
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];
					if (predicate(value))
					{
						yield return value;
					}
				}
			}
		}

		// public IEnumerable<T> AllValuesWhere(Func<Point, T, bool> predicate) ...
		// public IEnumerable<T> AllValuesWhere(Func<Point, bool> predicate) ...


		public int Count() => _column.Keys.Sum(key => _column[key].Row.Keys.Count);

		public int Count(Func<T, bool> predicate)
		{
			var n = 0;
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];
					if (predicate(value))
					{
						n++;
					}
				}
			}
			return n;
		}

		public int Count(Func<Point, bool> predicate)
		{
			var n = 0;
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var p = Point.From(x, y);
					if (predicate(p))
					{
						n++;
					}
				}
			}
			return n;
		}		

		public int Count(Func<Point, T, bool> predicate)
		{
			var n = 0;
			foreach (var x in _column.Keys)
			{
				var column = _column[x];
				foreach (var y in column.Row.Keys)
				{
					var value = column.Row[y];					
					var p = Point.From(x, y);
					if (predicate(p, value))
					{
						n++;
					}
				}
			}
			return n;
		}		

		public IEnumerable<Point> AllArea()
		{
			var (min, max) = MinMax();
			for (var x = min.X; x <= max.X; x++)
			{
				for (var y = min.Y; y <= max.Y; y++)
				{
					yield return Point.From(x, y);
				}
			}
		}

		public T this[Point pos]
		{
			get => _column.TryGetValue(pos.X, out var col) ? col[pos.Y] : _defaultValue;
			set => this[pos.X][pos.Y] = value;
		}

		public T this[int x, int y]
		{
			get => _column.TryGetValue(x, out var col) ? col[y] : _defaultValue;
			set => this[x][y] = value;
		}

		public bool Exists(Point pos)
		{
			return _column.TryGetValue(pos.X, out var col) && col.Exists(pos.Y);
		}

		public void Remove(Point pos)
		{
			if (_column.TryGetValue(pos.X, out var col))
			{
				col.Remove(pos.Y);
				if (col.Count == 0)
				{
					_column.Remove(pos.X);
				}
			}
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

		public Point Min()
		{
			var points = AllPoints().ToArray();
			if (points.Length == 0)
			{
				return Point.Origin;
			}
			return Point.From(points.Min(z => z.X), points.Min(z => z.Y));
		}

		public Point Max()
		{
			var points = AllPoints().ToArray();
			if (points.Length == 0)
			{
				return Point.Origin;
			}
			return Point.From(points.Max(z => z.X), points.Max(z => z.Y));
		}

		public (Point, Point) MinMax()
		{
			var points = AllPoints().ToArray();
			if (points.Length == 0)
			{
				return (Point.Origin, Point.Origin);
			}
			var min = Point.From(points.Min(z => z.X), points.Min(z => z.Y));
			var max = Point.From(points.Max(z => z.X), points.Max(z => z.Y));
			return (min, max);
		}

		public (Point, Point) Range()
		{
			var (min, max) = MinMax();
			return (min, max.DiagonalDownRight);
		}

		public (int, int) Size()
		{
			var (top, bot) = Range();
			return (bot.X - top.X, bot.Y - top.Y);
		}

		public IEnumerable<Point> Corners()
		{
			var (min, max) = MinMax();
			yield return min;
			yield return Point.From(max.X, min.Y);
			yield return max;
			yield return Point.From(min.X, max.Y);
		}

		public void WalkLine(Point p1, Point p2, Action<Point> action)
		{
			WalkLine(p1.X, p1.Y, p2.X, p2.Y, action);
		}

		public void WalkLine(int x1, int y1, int x2, int y2, Action<Point> action)
		{
			var dx = x2 - x1;
			var dy = y2 - y1;
			var lenx = Math.Abs(dx);
			var leny = Math.Abs(dy);
			var steps = Math.Max(lenx, leny);

			for (var i = 0; i <= steps; i++)
			{
				var x = x1 + i * dx / steps;
				var y = y1 + i * dy / steps;
				var p = Point.From(x, y);
				action(p);
			}
		}

		public string[] Render(Func<Point, T, char> rendering)
		{
			var (min, max) = MinMax();
			return Enumerable.Range(min.Y, max.Y- min.Y + 1)
				.Select(y => Enumerable.Range(min.X, max.X - min.X + 1)
					.Select(x => rendering(Point.From(x, y), this[x][y]))
					.ToArray()
				)
				.Select(ch => new string(ch))
				.ToArray();
		}

		public void ConsoleWrite(Func<Point, T, char> rendering)
		{
			foreach (var line in Render(rendering))
			{
				Console.WriteLine(line);
			}
		}

		public class SparseMapColumn
		{
			internal readonly Dictionary<int, T> Row = new Dictionary<int, T>();
			private readonly T _defaultValue;

			public SparseMapColumn(T defaultValue)
			{
				_defaultValue = defaultValue;
			}

			public T this[int y]
			{
				get => Row.ContainsKey(y) ? Row[y] : _defaultValue;
				set => Row[y] = value;
			}

			public bool Exists(int y)
			{
				return Row.ContainsKey(y);
			}

			public void Remove(int y)
			{
				Row.Remove(y);
			}

			public int Count => Row.Count;
		}
	}
}
