using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Text.RegularExpressions;
using MathNet.Numerics;

namespace AdventOfCode.Y2024.Day03.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 3;

		public override void Run()
		{
			// not 30632041
			Run("test1").Part1(161);//.Part2(48);
			Run("test2").Part2(48);
			Run("input").Part1(187825547).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var sum = 0L;

			var line = string.Concat(input);

			//foreach (var line in input)
			{
				var matches = Regex.Matches(line, @"mul\((\d{1,3}),(\d{1,3})\)");

				foreach (Match m in matches)
				{
					//Console.WriteLine(m);
					var o1 = int.Parse(m.Groups[1].Value);
					var o2 = int.Parse(m.Groups[2].Value);
					sum += o1 * o2;
				}
			}


			return sum;
		}

		protected override long Part2(string[] input)
		{
			var sum = 0L;

			var line = string.Concat(input);

			//Console.WriteLine(line);
			var rx = new Regex(@"don't\(\).*?(do\(\)|$)");
			line = rx.Replace(line, "");
		//Console.WriteLine(line);

			//foreach (var line in input)
			{
				var matches = Regex.Matches(line, @"mul\((\d{1,3}),(\d{1,3})\)");

				foreach (Match m in matches)
				{
					//Console.WriteLine(m);
					var o1 = int.Parse(m.Groups[1].Value);
					var o2 = int.Parse(m.Groups[2].Value);
					sum += o1 * o2;
				}
			}


			return sum;
		}
	}
}
