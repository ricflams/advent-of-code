using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2017.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2017;
		public override int Day => 15;

		public void Run()
		{
			RunFor("test1", 588, 0);
			//RunFor("test2", 0, 0);
			RunFor("input", 0, 0);
		}

		protected override int Part1(string[] input)
		{
			var a = input[0].RxMatch("%d").Get<ulong>();
			var b = input[1].RxMatch("%d").Get<ulong>();
			const ulong fa = 16807;
			const ulong fb = 48271;

			var n = 0;
			for (var i = 0; i < 40_000_000; i++)
			{
				a = (a * fa) % 0x7fffffffu;
				b = (b * fb) % 0x7fffffffu;
				//Console.WriteLine($"{a} {b}");
				if (((a ^ b) & 0xffffu) == 0)
				{
					n++;
				}
			}

			return n;
		}

		protected override int Part2(string[] input)
		{





			return 0;
		}
	}
}
