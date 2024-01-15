using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day03
{
	internal class Puzzle : Puzzle<int, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Toboggan Trajectory";
		public override int Year => 2020;
		public override int Day => 3;

		public override void Run()
		{
			Run("test1").Part1(7).Part2(336);
			Run("input").Part1(237).Part2(2106818610);
			Run("extra").Part1(268).Part2(3093068400);
		}

		protected override int Part1(string[] input)
		{
			var result = input
				.Select((line, i) => line[i * 3 % line.Length] == '#' ? 1 : 0)
				.Sum();

			return result;
		}

		protected override long Part2(string[] input)
		{
			var result =
				CountTrees(input, 1, 1) *
				CountTrees(input, 3, 1) *
				CountTrees(input, 5, 1) *
				CountTrees(input, 7, 1) *
				CountTrees(input, 1, 2);

			return result;
		}

		private static long CountTrees(string[] input, int right, int down)
		{
			var trees = 0;
			for (var (xpos, ypos) = (0, 0); ypos < input.Length; ypos += down, xpos += right)
			{
				var line = input[ypos];
				if (line[xpos % line.Length] == '#')
				{
					trees++;
				}
			}
			return trees;
		}
	}
}
