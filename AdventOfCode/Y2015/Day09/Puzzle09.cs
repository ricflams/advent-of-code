using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2015.Day09
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "All in a Single Night";
		public override int Year => 2015;
		public override int Day => 9;

		public void Run()
		{
			RunFor("test1", 605, 982);
			RunFor("input", 117, 909);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var graph = new WeightedGraph<string>();
			foreach (var line in input)
			{
				// Example: Faerun to Tambi = 129
				var val = SimpleRegex.Match(line, "%s to %s = %d");
				var city1 = val[0];
				var city2 = val[1];
				var distance = int.Parse(val[2]);

				graph.AddVertices(city1, city2, distance);
			}
			// graph.WriteAsGraphwiz();

			var shortestDistance = graph.TspShortestDistanceBruteForce();
			var longestDistance = graph.TspLongestDistanceBruteForce();

			return (shortestDistance, longestDistance);
		}
	}
}
