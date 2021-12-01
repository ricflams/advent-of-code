using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	internal class SafeArray<T>
	{
		private readonly T[] _values;

		public SafeArray(IEnumerable<T> values)
		{
			_values = values.ToArray();
		}

		public T this[int i]
		{
			get
			{
				return i >= 0 && i < _values.Length
					? _values[i]
					: default;
			}
			set
			{
				if (i >= 0 && i < _values.Length)
				{
					_values[i] = value;
				}
			}
		}

		public int Length => _values.Length;

		public T[] Values => _values;
	}
}
