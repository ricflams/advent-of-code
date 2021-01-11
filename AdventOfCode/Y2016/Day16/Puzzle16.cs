using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2016.Day16
{
	internal class Puzzle : SoloParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Dragon Checksum";
		public override int Year => 2016;
		public override int Day => 16;

		public void Run()
		{
			RunPart1For("test1", "01100");
			RunFor("input", "00100111000101111", "11101110011100110");
		}

		protected override string Part1(string[] input)
		{
			int length = int.Parse(input[0]);
			var state = input[1];
			return Checksum(state, length);
		}

		protected override string Part2(string[] input)
		{
			int length = 35651584;
			var state = input[1];
			return Checksum(state, length);
		}

		private static string Checksum(string s, int length)
		{
			while (s.Length < length)
			{
				var b = new string(s.Reverse().ToArray()).Replace("0", "X").Replace("1", "0").Replace("X", "1");
				s = s + "0" + b;
			}

			s = s.Substring(0, length);
			while (s.Length % 2 == 0)
			{
				var half = new char[s.Length / 2];
				for (var i = 0; i < half.Length; i++)
				{
					half[i] = s[i*2] == s[i*2+1] ? '1' : '0';
				}
				s = new string(half);
			}

			return s;			
		}
	}
}
