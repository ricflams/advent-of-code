using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Helpers
{
	public static class SimpleRegex
	{
		private static readonly ConcurrentDictionary<string, string> _regexCache = new ConcurrentDictionary<string, string>();

		/// <summary>
		/// Parse according to simple C-like format-specs, %s and %d
		/// </summary>
		public static Captures RxMatch(this string input, string pattern)
		{
			if (!IsMatch(input, pattern, out var matches))
			{
				throw new Exception($"No match for pattern {pattern} in input {input}");
			}
			return new Captures(matches);
		}

		/// <summary>
		/// Parse according to simple C-like format-specs, %s and %d
		/// </summary>
		public static bool IsRxMatch(this string input, string pattern, out Captures matches)
		{
			if (IsMatch(input, pattern, out var values))
			{
				matches = new Captures(values);
				return true;
			}
			else
			{
				matches = null;
				return false;
			}
		}

		public class Captures
		{
			private readonly string[] _matches;
			private int _index;
			public Captures(string[] matches) { _matches = matches; }
			public T Get<T>()
			{
				switch (default(T))
				{
					case int _: return (T)(object)int.Parse(_matches[_index++]);
					case uint _: return (T)(object)uint.Parse(_matches[_index++]);
					case long _: return (T)(object)long.Parse(_matches[_index++]);
					case ulong _: return (T)(object)ulong.Parse(_matches[_index++]);
					case char _: return (T)(object)_matches[_index++][0];
				}
				if (typeof(T) == typeof(string))
				{
					return (T)(object)_matches[_index++];
				}
				throw new Exception();
			}
			public (T1, T2) Get<T1, T2>() => (Get<T1>(), Get<T2>());
			public (T1, T2, T3) Get<T1, T2, T3>() => (Get<T1>(), Get<T2>(), Get<T3>());
			public (T1, T2, T3, T4) Get<T1, T2, T3, T4>() => (Get<T1>(), Get<T2>(), Get<T3>(), Get<T4>());
			public (T1, T2, T3, T4, T5) Get<T1, T2, T3, T4, T5>() => (Get<T1>(), Get<T2>(), Get<T3>(), Get<T4>(), Get<T5>());
			public (T1, T2, T3, T4, T5, T6) Get<T1, T2, T3, T4, T5, T6>() => (Get<T1>(), Get<T2>(), Get<T3>(), Get<T4>(), Get<T5>(), Get<T6>());
		}

		private static bool IsMatch(string input, string pattern, out string[] val)
		{
			if (!_regexCache.TryGetValue(pattern, out var regex))
			{
				var sb = new StringBuilder();
				for (var i = 0; i < pattern.Length; i++)
				{
					var ch = pattern[i];
					if (@"\.|?+()[{".Contains(ch))
					{
						sb.Append('\\');
						sb.Append(ch);
					}
					else if (ch != '%')
					{
						sb.Append(ch);
					}
					else
					{
						ch = pattern[++i];
						switch (ch)
						{
							case '%': sb.Append('%'); break;
							case '*': sb.Append(@"(.+)"); break;
							case 's': sb.Append(@"(\w+)"); break;
							case 'c': sb.Append(@"(.)"); break;
							case 'd': sb.Append(@"([-+]?\d+)"); break;
							case 'D': sb.Append(@"\s*([-+]?\d+)"); break;
							default: throw new Exception($"Regex: invalid sequence '%{ch}'");
						}
					}
				}
				regex = sb.ToString();
				_regexCache[pattern] = regex;
			}

			var match = Regex.Match(input, regex);
			if (!match.Success)
			{
				val = null;
				return false;
			}

			val = match.Groups.Values.Skip(1).Select(g => g.Value).ToArray();
			return true;
		}
	}
}
