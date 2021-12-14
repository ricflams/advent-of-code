using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day07
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Treachery of Whales";
		public override int Year => 2021;
		public override int Day => 7;

		public void Run()
		{
			Run("test1").Part1(37).Part2(168);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(344535).Part2(95581659);

			// todo: clean
		}

		protected override int Part1(string[] input)
		{
			var v = input.First().ToIntArray();

			var s = v.Sum();

			var min = v.Min();
			var max = v.Max();
			var minresult = int.MaxValue;
			for (var xx = min; xx <= max; xx++)
			{
				var dif = v.Select(x => Math.Abs(x - xx)).Sum();
				if (dif < minresult)
					minresult = dif;
			}


			return minresult;
		}

		protected override int Part2(string[] input)
		{
			var v = input.First().ToIntArray();

			var s = v.Sum();

			var min = v.Min();
			var max = v.Max();
			var minresult = int.MaxValue;
			for (var xx = min; xx <= max; xx++)
			{
				var dif = v.Select(x =>
				{
					var d = Math.Abs(x - xx);
					var fuel = d * (d + 1) / 2;
					return fuel;
				}).Sum();
				if (dif < minresult)
					minresult = dif;
			}


			return minresult;
		}
	}
}
