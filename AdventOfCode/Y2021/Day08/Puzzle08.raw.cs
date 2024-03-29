using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day08.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Seven Segment Search";
		public override int Year => 2021;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(0).Part2(5353);

			Run("test2").Part1(26).Part2(61229);

			Run("input").Part1(548).Part2(1074888);
		}

		private static string[] SanitizeInput(string[] input)
		{
			var i2 = new List<string>();
			for (var i = 0; i < input.Length; i++)
			{
				var s = input[i];
				var line = s.EndsWith('|')
					? s + input[++i]
					: s;
				i2.Add(line);
			}
			return i2.ToArray();
		}

		protected override long Part1(string[] input)
		{
			input = SanitizeInput(input);
			// acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab |cdfeb fcadb cdfeb cdbaf

			var digits = input.Select(s => s.Split('|')[1]).ToArray();
			var segments = new char[][]
			{
				new[] { 'a', 'b', 'c', 'e', 'f', 'g' }, // 0
				new[] { 'c', 'f' }, // 1
				new[] { 'a', 'c', 'd', 'e', 'g' }, // 2
				new[] { 'a', 'c', 'd', 'f', 'g' }, // 3
				new[] { 'b', 'c', 'd', 'f' }, // 4
				new[] { 'a', 'b', 'd', 'f', 'g' }, // 5
				new[] { 'a', 'b', 'd', 'e', 'f', 'g' }, // 6
				new[] { 'a', 'c', 'f' }, // 7
				new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' }, // 8
				new[] { 'a', 'b', 'c', 'd', 'f', 'g' }, // 9
			};
			var numberOfDigits = segments.Select(x => x.Length).ToArray();

			var n = 0;
			foreach (var vv in digits)
			{
				foreach (var v in vv.Split(' '))
				{
					var len = v.Length;
					if (len == 2 || len == 3 || len == 4 || len == 7)
						n++;
				}
			}


			return n;
		}

		internal class Input
		{
			public string[] Signal0;
			public string[] Out0;
			public string[] Signal;
			public string[] Out;
		}

		protected override long Part2(string[] input)
		{
			input = SanitizeInput(input);

			var inp2 = input
				.Select(s => s.Split('|'))
				.Select(x => new Input
				{
					Signal0 = x[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray(),
					Out0 = x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray(),
					Signal = x[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => new string(x.OrderBy(z => z).ToArray())).ToArray(),
					Out = x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => new string(x.OrderBy(z => z).ToArray())).ToArray()
				})
				.ToArray();


			var n = 0;
			foreach (var ii in inp2)
			{
				n += CalcInput(ii);
			}
			return n;

		}

		private int CalcInput(Input input)
		{

			var segments = new string[]
			{
				"abcefg", // 0
				"cf", // 1
				"acdeg", // 2
				"acdfg", // 3
				"bcdf", // 4
				"abdfg", // 5
				"abdefg", // 6
				"acf", // 7
				"abcdefg", // 8
				"abcdfg" // 9
			};
			var numberOfDigits = segments.Select(x => x.Length).ToArray();

			var map = new string[10];

			map[1] = input.Signal.First(x => x.Length == 2);
			map[4] = input.Signal.First(x => x.Length == 4);
			map[7] = input.Signal.First(x => x.Length == 3);
			map[8] = input.Signal.First(x => x.Length == 7);

			var digits = new int[10];

			// aaaa is top of 7 minus 1
			var aaaa = map[7].Where(x => !map[1].Contains(x)).First();

			// 9 has 4 in it
			map[9] = input.Signal.Where(x => x.Length == 6).Where(x => map[4].All(c => x.Contains(c))).Single();
			var eeee = map[8].Where(x => !map[9].Contains(x)).First();

			// 0 has 1 in it
			map[0] = input.Signal.Where(x => x.Length == 6 && x != map[9]).Where(x => map[1].All(c => x.Contains(c))).Single();

			// 6 is remaining 6-length
			map[6] = input.Signal.Where(x => x.Length == 6 && x != map[9] && x != map[0]).Single();

			// 3 has 1 in it
			map[3] = input.Signal.Where(x => x.Length == 5).Where(x => map[1].All(c => x.Contains(c))).Single();

			// 6 has 5 in it
			map[5] = input.Signal.Where(x => x.Length == 5 && x != map[3]).Where(x => x.All(c => map[6].Contains(c))).Single();

			// 2 is the last 5'er
			map[2] = input.Signal.Where(x => x.Length == 5 && x != map[3] && x != map[5]).Single();

			//Console.WriteLine(zz);

			//var dddd = map[8].Where(x => !)

			var val = 0;
			foreach (var o in input.Out)
			{
				for (var i = 0; i < 10; i++)
				{
					if (o == map[i])
					{
						val = val * 10;
						val += i;
						break;
					}
				}
			}

			return val;

			//  0:      1:      2:      3:      4:
			// aaaa    ....    aaaa    aaaa    ....
			//b    c  .    c  .    c  .    c  b    c
			//b    c  .    c  .    c  .    c  b    c
			// ....    ....    dddd    dddd    dddd
			//e    f  .    f  e    .  .    f  .    f
			//e    f  .    f  e    .  .    f  .    f
			// gggg    ....    gggg    gggg    ....

			//  5:      6:      7:      8:      9:
			// aaaa    aaaa    aaaa    aaaa    aaaa
			//b    .  b    .  .    c  b    c  b    c
			//b    .  b    .  .    c  b    c  b    c
			// dddd    dddd    ....    dddd    dddd
			//.    f  e    f  .    f  e    f  .    f
			//.    f  e    f  .    f  e    f  .    f
			// gggg    gggg    ....    gggg    gggg


		}
	}
}