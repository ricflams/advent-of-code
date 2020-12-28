using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day06
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Universal Orbit Map";
		protected override int Year => 2019;
		protected override int Day => 6;

		public void Run()
		{
			RunFor("input", 387356, 532);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var orbitdefs = input
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Split(')'))
				.ToList();
			var nodes = new Dictionary<string, List<string>>();
			foreach (var o in orbitdefs)
			{
				if (!nodes.ContainsKey(o[0]))
				{
					nodes[o[0]] = new List<string> { o[1] };
				}
				else
				{
					nodes[o[0]].Add(o[1]);
				}
			}
			var root = nodes.Keys.Except(nodes.SelectMany(x => x.Value)).First();
			var orbitCount = CountOrbits(0, root);

			// Part 2
			int CountOrbits(int orbitlevel, string name) =>
				nodes.TryGetValue(name, out var o)
					? orbitlevel + o.Select(x => CountOrbits(orbitlevel + 1, x)).Sum()
					: orbitlevel;

			var you = FindPathTo("YOU").ToList();
			var san = FindPathTo("SAN").ToList();
			var dist = you.Count + san.Count - 2;
			for (var i = 0; you[i] == san[i]; i++)
			{
				dist -= 2;
			}

			return (orbitCount, dist);


			IEnumerable<string> FindPathTo(string name)
			{
				if (name == root)
				{
					yield return name;
				}
				else
				{
					var obj = orbitdefs.First(o => o[1] == name);
					foreach (var o in FindPathTo(obj[0]))
					{
						yield return o;
					}
					yield return obj[1];
				}
			}
		}
	}
}