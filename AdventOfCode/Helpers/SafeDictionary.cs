using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	internal class SafeDictionary<TK,TV> : Dictionary<TK,TV>
	{
		public new TV this[TK key]
		{
			get => TryGetValue(key, out var value) ? value : default;
			set => base[key] = value;
		}
	}
}
