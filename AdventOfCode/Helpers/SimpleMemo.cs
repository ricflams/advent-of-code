using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	public class SimpleMemo<T>
	{
		private readonly HashSet<T> _memo = new HashSet<T>();

		public bool IsSeenBefore(T value)
		{
			if (_memo.Contains(value))
			{
				return true;
			}
			_memo.Add(value);
			return false;
		}
	}
}
