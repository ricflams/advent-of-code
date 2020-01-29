using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode.Y2015.Day09
{
	internal class Puzzle09
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2015/Day09/input.txt");

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
			Console.WriteLine($"Day  9 Puzzle 1: {shortestDistance}");
			Debug.Assert(shortestDistance == 117);

			var longestDistance = graph.TspLongestDistanceBruteForce();
			Console.WriteLine($"Day  9 Puzzle 2: {longestDistance}");
			Debug.Assert(longestDistance == 909);
		}
	}
}
