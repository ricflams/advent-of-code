using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers.String
{
    public static class Extensions
    {
        public static IEnumerable<string> AllReplacements(this string str, string oldValue, string newValue)
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

		public static char[] RotateRight(this char[] s, int n)
		{
			var N = s.Length;
			var temp = new char[N];
			for (var i = 0; i < N; i++)
			{
				temp[(i + n) % N] = s[i];
			}
			return temp;
		}

		private static readonly char[] IntArraySep = new char[] { ' ', '\t', ',' };
		public static int[] ToIntArray(this string s)
		{
			return s.Split(IntArraySep, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
		}
    }
}