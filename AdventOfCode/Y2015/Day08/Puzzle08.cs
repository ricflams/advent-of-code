using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day08
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2015;
		protected override int Day => 8;

		public void Run()
		{
			RunFor("test1", 12, 19);
			RunFor("input", 1333, 2046);
		}

		protected override int Part1(string[] input)
		{
			var rawlen = 0;
			var uescaped = 0;
			foreach (var line in input)
			{
				rawlen += line.Length;
				uescaped += UnescapeLength(line);
			}

			var result = rawlen - uescaped;
			return result;

			static int UnescapeLength(string s)
			{
				// \\ represents a single backslash
				// \" represents a lone double-quote character
				// \x plus two hexadecimal characters represents a single character with that ASCII code
				var length = 0;
				for (var i = 1; i < s.Length - 1; i++)
				{
					if (s[i] == '\\')
					{
						switch (s[i + 1])
						{
							case '\\': i++; break;
							case '\"': i++; break;
							case 'x': i += 3; break;
						}
					}
					length++;
				}
				return length;
			}
		}

		protected override int Part2(string[] input)
		{
			var rawlen = 0;
			var escaped = 0;
			foreach (var line in input)
			{
				rawlen += line.Length;
				escaped += EscapeLength(line);
			}

			var result = escaped - rawlen;
			return result;

			static int EscapeLength(string s)
			{
				// \\ represents a single backslash
				// \" represents a lone double-quote character
				// \x plus two hexadecimal characters represents a single character with that ASCII code
				var length = s.Length + s.Count(ch => ch == '\\' || ch == '\"') + 2;
				return length;
			}
		}
	}
}
