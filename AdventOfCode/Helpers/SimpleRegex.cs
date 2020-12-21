using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
		public static string[] Match(string input, string pattern)
		{
			if (IsMatch(input, pattern, out var val))
			{
				return val;
			}
			throw new Exception($"Regex mismatch: pattern '{pattern}' does not match {input}");
		}

		public static string MatchString(string input, string pattern)
		{
			return Match(input, pattern).First();
		}

		public static int MatchInt(string input, string pattern)
		{
			return int.Parse(MatchString(input, pattern));
		}

		public static bool IsMatch(string input, string pattern, out string[] val)
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

		public interface ICaptures
		{
			ICaptures Get(out int value);
			ICaptures Get(out uint value);
			ICaptures Get(out long value);
			ICaptures Get(out ulong value);
			ICaptures Get(out string value);
			ICaptures Get(out char value);
			bool IsMatch { get; }
		}

		public class Captures : ICaptures
		{
			private readonly string[] _matches;
			private int _index;
			public Captures(string[] matches) { _matches = matches; }
			public ICaptures Get(out int value) { value = int.Parse(_matches[_index++]); return this; }
			public ICaptures Get(out uint value) { value = uint.Parse(_matches[_index++]); return this; }
			public ICaptures Get(out long value) { value = long.Parse(_matches[_index++]); return this; }
			public ICaptures Get(out ulong value) { value = ulong.Parse(_matches[_index++]); return this; }
			public ICaptures Get(out string value) { value = _matches[_index++]; return this; }
			public ICaptures Get(out char value) { value = _matches[_index++][0]; return this; }
			public bool IsMatch => true;
		}

		public class NoCaptures : ICaptures
		{
			public static readonly ICaptures Instance = new NoCaptures();
			public ICaptures Get(out int value) { value = 0; return this; }
			public ICaptures Get(out uint value) { value = 0; return this; }
			public ICaptures Get(out long value) { value = 0; return this; }
			public ICaptures Get(out ulong value) { value = 0; return this; }
			public ICaptures Get(out string value) { value = null; return this; }
			public ICaptures Get(out char value) { value = '\0'; return this; }
			public bool IsMatch => false;
		}

		public static Captures RegexCapture(this string input, string pattern)
		{
			if (!IsMatch(input, pattern, out var matches))
			{
				throw new Exception($"No match for pattern {pattern} in input {input}");
			}
			return new Captures(matches);
		}

		public static ICaptures MaybeRegexCapture(this string input, string pattern)
		{
			return IsMatch(input, pattern, out var matches)
				? new Captures(matches)
				: NoCaptures.Instance;
		}

	}
}
