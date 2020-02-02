using System;
using System.Collections.Generic;

namespace AdventOfCode.Helpers
{
	public static class StringExtensions
	{
		public static IEnumerable<string> Replacements(this string str, string oldValue, string newValue)
		{
			// The length of a replaced string is fixed
			var replacedLength = str.Length - oldValue.Length + newValue.Length;

			var offset = 0;
			while (true)
			{
				var found = str.IndexOf(oldValue, offset);
				if (found < 0)
				{
					break;
				}
				offset = found + oldValue.Length;

				var newString = string.Create(replacedLength, 0, (chars, _) =>
				{
					str.AsSpan(0, found).CopyTo(chars);
					newValue.AsSpan().CopyTo(chars.Slice(found));
					str.AsSpan(found + oldValue.Length).CopyTo(chars.Slice(found + newValue.Length));
				});

				yield return newString;
			}
		}
	}
}
