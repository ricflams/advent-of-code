using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day06.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(288).Part2(71503);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(4811940).Part2(30077773);
		}

		protected override long Part1(string[] input)
		{
			var time = input[0].Split(':')[1].ToIntArray();
			var dist = input[1].Split(':')[1].ToIntArray();

			var result = 1;
			for (var race = 0; race < time.Length; race++)
			{
				var t = time[race];
				var d = dist[race];
				var wins = 0;
				for (var wait = 0; wait < t; wait++)
				{
					var totdist = wait * (t - wait);
					if (totdist > d)
						wins++;
				}
				result *= wins;
			}

			return result;
		}

		protected override long Part2(string[] input)
		{
			var t = long.Parse(input[0].Split(':')[1].Replace(" ", ""));
			var d = long.Parse(input[1].Split(':')[1].Replace(" ", ""));

			var wins = 0;
			for (var wait = 0; wait < t; wait++)
			{
				var totdist = wait * (t - wait);
				if (totdist > d)
					wins++;
			}

			return wins;
		}
	}
}
