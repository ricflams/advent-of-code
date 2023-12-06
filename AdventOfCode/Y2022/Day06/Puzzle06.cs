using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day06
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Tuning Trouble";
		public override int Year => 2022;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(7).Part2(19);
			Run("test2").Part1(5).Part2(23);
			Run("test3").Part1(6).Part2(23);
			Run("test4").Part1(10).Part2(29);
			Run("test5").Part1(11).Part2(26);
			Run("input").Part1(1109).Part2(3965);
			Run("extra").Part1(1080).Part2(3645);
		}

		protected override long Part1(string[] input)
		{
			return FindResultFor(input, 4);
		}

		protected override long Part2(string[] input)
		{
			return FindResultFor(input, 14);
		}

		private int FindResultFor(string[] input, int length)
		{
			var n = length;
			foreach (var span in input[0].Windowed(length))
			{
				if (span.GroupBy(x => x).All(x => x.Count() == 1))
					return n;
				n++;
			}
			throw new Exception("No span found");
		}
	}
}
