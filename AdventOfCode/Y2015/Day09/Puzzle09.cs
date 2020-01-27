using AdventOfCode.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

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
				var match = Regex.Match(line, @"(\w+) to (\w+) = (\d+)");
				if (!match.Success)
					throw new Exception($"Unexpected format at {line}");
				var city1 = match.Groups[1].Value;
				var city2 = match.Groups[2].Value;
				var distance = int.Parse(match.Groups[3].Value);

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
