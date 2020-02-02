using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Helpers
{
	public class SimpleRegex
	{
		private static readonly Dictionary<string, string> _regexCache = new Dictionary<string, string>();

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
							case 's': sb.Append(@"(\w+)"); break;
							case 'd': sb.Append(@"(-?\d+)"); break;
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
	}
}
