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
					if (pattern[i] != '%')
					{
						sb.Append(pattern[i]);
					}
					else switch (pattern[++i])
						{
							case '%': sb.Append('%'); break;
							case '*': sb.Append(@"(.+)"); break;
							case 's': sb.Append(@"(\w+)"); break;
							case 'c': sb.Append(@"(.)"); break;
							case 'd': sb.Append(@"([-+]?\d+)"); break;
							default: throw new Exception($"Regex: invalid sequence '%{pattern[i]}'");
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

		public class Captures
		{
			private readonly string[] _matches;
			private int _index;
			public Captures(string[] matches) { _matches = matches; }
			public Captures Get(out int value) { value = int.Parse(_matches[_index++]); return this; }
			public Captures Get(out string value) { value = _matches[_index++]; return this; }
			public Captures Get(out char value) { value = _matches[_index++][0]; return this; }

		}

		public static Captures RegexCapture(this string input, string pattern)
		{
			if (!IsMatch(input, pattern, out var matches))
			{
				throw new Exception($"No match for pattern {pattern} in input {input}");
			}
			return new Captures(matches);
		}
	}
}
