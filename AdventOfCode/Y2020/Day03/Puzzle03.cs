using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day03
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Toboggan Trajectory";
		public override int Year => 2020;
		public override int Day => 3;

		public void Run()
		{
			RunFor("test1", 7, 336);
			RunFor("input", 237, 2106818610);
		}

		protected override int Part1(string[] input)
		{
			var result = input
				.Select((line, i) => line[i * 3 % line.Length] == '#' ? 1 : 0)
				.Sum();

			return result;
		}

		protected override int Part2(string[] input)
		{
			var result =
				CountTrees(input, 1, 1) *
				CountTrees(input, 3, 1) *
				CountTrees(input, 5, 1) *
				CountTrees(input, 7, 1) *
				CountTrees(input, 1, 2);

			return result;
		}

		private static int CountTrees(string[] input, int right, int down)
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
