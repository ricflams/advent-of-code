using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2021.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Sonar Sweep";
		public override int Year => 2021;
		public override int Day => 1;

		public void Run()
		{
			Run("test1").Part1(7).Part2(5);
			Run("input").Part1(1482).Part2(1518);
		}

		protected override int Part1(string[] input)
		{
			var v = input.Select(int.Parse).ToArray();

			var n = 0;
			for (var i = 0; i < v.Length - 1; i++)
			{
				if (v[i] < v[i + 1])
					n++;
			}

			return n;
		}

		protected override int Part2(string[] input)
		{
			var v = input.Select(int.Parse).ToArray();

			var last = (int?)null;
			var n = 0;
			for (var i = 0; i < v.Length - 2; i++)
			{
				var sum = v[i] + v[i + 1] + v[i + 2];
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
