using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2015.Day09
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "All in a Single Night";
		public override int Year => 2015;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(605).Part2(982);
			Run("input").Part1(117).Part2(909);
			Run("extra").Part1(117).Part2(909);
		}

		protected override int Part1(string[] input)
		{
			var map = GetMap(input);
			var shortestDistance = map.TspShortestDistanceBruteForce();
			return shortestDistance;
		}

		protected override int Part2(string[] input)
		{
			var map = GetMap(input);
			var longestDistance = map.TspLongestDistanceBruteForce();
			return longestDistance;
		}

		private static Graph<string> GetMap(string[] input)
		{
			var map = new Graph<string>();
			foreach (var line in input)
			{
				// Example: Faerun to Tambi = 129
				var (city1, city2, distance) = line.RxMatch("%s to %s = %d").Get<string, string, int>();
				map.AddNodes(city1, city2, distance);
			}
			// map.WriteAsGraphwiz();
			return map;
		}

	}
}
