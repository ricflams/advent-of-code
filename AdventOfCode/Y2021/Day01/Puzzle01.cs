using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2021.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Sonar Sweep";
		public override int Year => 2021;
		public override int Day => 1;

		public void Run()
		{
			Run("test1").Part1(7).Part2(5);
			Run("test9").Part1(7).Part2(5);
			Run("input").Part1(1482).Part2(1518);
		}

		protected override int Part1(string[] input)
		{
			var v = input.Select(int.Parse);
			var n = v.Windowed(2).Count(x => x[0] < x[1]);
			return n;
		}

		protected override int Part2(string[] input)
		{
			var v = input.Select(int.Parse);

			var n = 0;
			var last = int.MaxValue;
			foreach (var sum in v.Windowed(3).Select(x => x.Sum()))
			{
				if (sum > last)
				{
					n++;
				}
				last = sum;
			}

			return n;
		}
	}
}
