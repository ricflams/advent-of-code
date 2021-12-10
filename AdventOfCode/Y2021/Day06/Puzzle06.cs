using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day06
{
	internal class Puzzle : Puzzle<int, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Lanternfish";
		public override int Year => 2021;
		public override int Day => 6;

		public void Run()
		{
			Run("test1").Part1(5934).Part2(26984457539);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(360268).Part2(1632146183902);
		}

		class Fish
		{
			public int Timer { get; set; }
		}

		protected override int Part1(string[] input)
		{
			var v = input.First().ToIntArray();

			var fishes = v.Select(x => new Fish
				{
					Timer = x,
				})
				.ToArray();

			for (var day = 0; day < 80; day++)
			{
				var newfish = 0;
				for (var i = 0; i < v.Length; i++)
				{
					if (v[i]-- == 0)
					{
						v[i] = 6;
						newfish++;
					}
				}
				if (newfish > 0)
				{
					v = v.Concat(Enumerable.Repeat(8, newfish)).ToArray();
				}
			}

			return v.Length;
		}

		protected override long Part2(string[] input)
		{
			var v = input.First().ToIntArray();

			var result = v.Sum(x => FishesAfter(x, 256));

			return result;

		}

		private long FishesAfter(int timer, int loops)
		{
			var v = new long[9];
			v[timer] = 1;
			for (var day = 0; day < loops; day++)
			{
				//				Console.WriteLine(v.Sum());

				var v0 = v[0];
				//v[8] += v[0];
				//v[6] += v[0];
				v[0] = v[1];
				v[1] = v[2];
				v[2] = v[3];
				v[3] = v[4];
				v[4] = v[5];
				v[5] = v[6];
				v[6] = v[7] + v0;
				v[7] = v[8];
				v[8] = v0;

			}
			return v.Sum();
		}
	}
}
