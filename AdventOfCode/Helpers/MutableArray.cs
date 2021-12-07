using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	internal class MutableArray<T> : IEnumerable
	{
		private readonly T[] _array;

		public MutableArray(T[] array) => _array = array;

		public IEnumerator<T> GetEnumerator() => new PurgeableArrayEnumerator(_array);
		IEnumerator IEnumerable.GetEnumerator() => throw new Exception();

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

			public void Reset()
			{
				_index = -1;
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

			public void Dispose() { }
		}
	}

	internal static class MutableArrayExtensions
	{
		public static MutableArray<T> ToMutableArray<T>(this IEnumerable<T> enumerable)
		{
			return new MutableArray<T>(enumerable.ToArray());
		}
	}

}
