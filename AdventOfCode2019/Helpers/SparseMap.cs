using System.Collections.Generic;

namespace AdventOfCode2019.Helpers
{
    internal class SparseMap<T>
    {
		private readonly Dictionary<int, SparseMapColumn> _column = new Dictionary<int, SparseMapColumn>();
		private readonly T _defaultValue;

		public SparseMap(T defaultValue = default(T))
		{
			_defaultValue = defaultValue;
		}

		public class Point
		{
			public int X { get; set; }
			public int Y { get; set; }
			public T Value { get; set; }
		}

		public IEnumerable<Point> AllPoints
		{
			get
			{
				foreach (var x in _column.Keys)
				{
					var column = _column[x];
					foreach (var y in column.Row.Keys)
					{
						yield return new Point
						{
							X = x,
							Y = y,
							Value = column.Row[y]
						};
					}
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
		}
	}
}
