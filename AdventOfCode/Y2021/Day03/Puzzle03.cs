using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2021.Day03
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Binary Diagnostic";
		public override int Year => 2021;
		public override int Day => 3;

		public void Run()
		{
			Run("test1").Part1(198).Part2(230);
			Run("input").Part1(1131506).Part2(7863147);
		}

		protected override int Part1(string[] input)
		{
			var width = input.First().Length;

			var gamma = 0;
			var epsilon = 0;

			var bit = 1 << (width-1);
			for (var i = 0; i < width; i++, bit >>= 1)
			{
				var on = input.Count(x => x[i] == '1');
				if (on > input.Length / 2)
				{
					gamma += bit;
				}
				else
				{
					epsilon += bit;
				}
			}

			var consumption = gamma * epsilon;
			return consumption;
		}

		protected override int Part2(string[] input)
		{
			var oxygen = CalcRating(input, true);
			var co2 = CalcRating(input, false);
			var lifeSupportRating = oxygen * co2;
			return lifeSupportRating;

			static int CalcRating(string[] input, bool keepMostCommonValue)
			{
				var bitmasks = input.ToArray();
				var width = bitmasks.First().Length;

				for (var i = 0; i < width && bitmasks.Length > 1; i++)
				{
					var bit0 = bitmasks.Count(x => x[i] == '0');
					var bit1 = bitmasks.Length - bit0;
					var keep = keepMostCommonValue
						? (bit1 >= bit0 ? '1' : '0')
						: (bit1 >= bit0 ? '0' : '1');
					bitmasks = bitmasks.Where(x => x[i] == keep).ToArray();
				}
				return Convert.ToInt32(bitmasks.Single(), 2);
			}
		}
	}
}
