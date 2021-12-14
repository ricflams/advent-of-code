using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2021.Day03.Raw
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "DAY3";
		public override int Year => 2021;
		public override int Day => 3;

		public void Run()
		{
			Run("test1").Part1(198).Part2(230);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(1131506).Part2(7863147);
		}

		protected override int Part1(string[] input)
		{
			var w = input.First().Length;

			var gamma = 0;
			var eps = 0;
			for (var i = 0; i < w; i++)
			{
				var on = input.Where(x => x[w - i - 1] == '1').Count();
				if (on > input.Length / 2)
				{
					gamma += 1 << i;
				}
				else
				{
					eps += 1 << i;
				}
			}

			var cons = gamma * eps;





			return cons;
		}

		protected override int Part2(string[] input)
		{
			var w = input.First().Length;

			var gamma = 0;
			var eps = 0;

			var input2 = input.Select(x => x).ToArray();

			for (var i = 0; i < w && input2.Length > 1; i++)
			{
				var on = input2.Where(x => x[i] == '1').Count();
				var off = input2.Length - on;
				if (on >= off)
				{
					input2 = input2.Where(x => x[i] == '1').ToArray();
				}
				else
				{
					input2 = input2.Where(x => x[i] == '0').ToArray();
				}
			}

			for (var i = 0; i < w && input.Length > 1; i++)
			{
				var on = input.Where(x => x[i] == '1').Count();
				var off = input.Length - on;
				if (on >= off)
				{
					input = input.Where(x => x[i] == '0').ToArray();
				}
				else
				{
					input = input.Where(x => x[i] == '1').ToArray();
				}
			}

			Console.WriteLine(input.First());
			Console.WriteLine(input2.First());

			var xx = Convert.ToInt32(input.First(), 2);
			var yy = Convert.ToInt32(input2.First(), 2);

			Console.WriteLine(xx);
			Console.WriteLine(yy);

			var xxx = xx * yy;



			return xxx;
		}
	}
}