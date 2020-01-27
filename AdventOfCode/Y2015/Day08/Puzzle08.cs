using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day08
{
	internal class Puzzle08
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2015/Day08/input.txt");

			var rawlen = 0;
			var uescaped = 0;
			foreach (var line in input)
			{
				rawlen += line.Length;
				uescaped += UnescapeLength(line);
			}

			var result = rawlen - uescaped;
			Console.WriteLine($"Day  8 Puzzle 1: {result}");
			Debug.Assert(result == 1333);

			int UnescapeLength(string s)
			{
				// \\ represents a single backslash
				// \" represents a lone double-quote character
				// \x plus two hexadecimal characters represents a single character with that ASCII code
				var length = 0;
				for (var i = 1; i < s.Length - 1; i++)
				{
					if (s[i] == '\\')
					{
						switch (s[i+1])
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

		private static void Puzzle2()
		{
			var input = File.ReadAllLines("Y2015/Day08/input.txt");

			var rawlen = 0;
			var escaped = 0;
			foreach (var line in input)
			{
				rawlen += line.Length;
				escaped += EscapeLength(line);
			}

			var result = escaped - rawlen;
			Console.WriteLine($"Day  8 Puzzle 2: {result}");
			Debug.Assert(result == 2046);

			int EscapeLength(string s)
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
