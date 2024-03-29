using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day08
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Matchsticks";
		public override int Year => 2015;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(12).Part2(19);
			Run("input").Part1(1333).Part2(2046);
			Run("extra").Part1(1342).Part2(2074);
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
