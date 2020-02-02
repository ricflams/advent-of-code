using System;
using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	internal class SafeDictionary<TK,TV> : Dictionary<TK,TV>
	{
		private readonly Func<TV> _factory;

		public SafeDictionary(Func<TV> factory = null)
		{
			_factory = factory ?? (() => default);
		}

		public SafeDictionary(TV value)
		{
			_factory = () => value;
		}

		public new TV this[TK key]
		{
			get
			{
				if (!TryGetValue(key, out var value))
				{
					value = _factory();
					this[key] = value;
				}
				return value;
			}
			set => base[key] = value;
		}
	}
}
