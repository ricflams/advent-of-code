using System;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Y2015.Day10
{
	internal class Puzzle10
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = "3113322113";

			var length = LookAndSayLengthAfter(input, 40);
			Console.WriteLine($"Day 10 Puzzle 1: {length}");
			Debug.Assert(length == 329356);
		}

		private static void Puzzle2()
		{
			var input = "3113322113";

			var length = LookAndSayLengthAfter(input, 50);
			Console.WriteLine($"Day 10 Puzzle 2: {length}");
			Debug.Assert(length == 4666278);
		}

		private static int LookAndSayLengthAfter(string input, int iterations)
		{
			var sequence = input.Select(ch => (byte)(ch - '0')).ToArray();
			var length = sequence.Length;
			for (var i = 0; i < iterations; i++)
			{
				(sequence, length) = LookAndSay(sequence, length);
			}
			return length;
		}

		private static (byte[], int) LookAndSay(byte[] input, int inputlen)
		{
			// 1 becomes 11 (1 copy of digit 1).
			// 11 becomes 21 (2 copies of digit 1).
			// 21 becomes 1211 (one 2 followed by one 1).
			// 1211 becomes 111221 (one 1, one 2, and two 1s).
			// 111221 becomes 312211 (three 1s, two 2s, and one 1).
			var output = new byte[inputlen * 2];
			var outputlen = 0;
			for (var i = 0; i < inputlen; i++)
			{
				var ch = input[i];
				var n = 1;
				while (i + 1 < inputlen && input[i + 1] == ch)
				{
					n++;
					i++;
				}
				output[outputlen++] = (byte)n;
				output[outputlen++] = ch;
			}
			return (output, outputlen);
		}

		// First attempt; slow string-operations
		//private static string LookAndSay(string s)
		//{
		//	// 1 becomes 11 (1 copy of digit 1).
		//	// 11 becomes 21 (2 copies of digit 1).
		//	// 21 becomes 1211 (one 2 followed by one 1).
		//	// 1211 becomes 111221 (one 1, one 2, and two 1s).
		//	// 111221 becomes 312211 (three 1s, two 2s, and one 1).
		//	var sb = new StringBuilder(s.Length * 2);
		//	for (var i = 0; i < s.Length; i++)
		//	{
		//		var ch = s[i];
		//		var n = 1;
		//		while (i + 1 < s.Length && s[i + 1] == ch)
		//		{
		//			n++;
		//			i++;
		//		}
		//		sb.Append(n);
		//		sb.Append(ch);
		//	}
		//	return sb.ToString();
		//}
	}
}
