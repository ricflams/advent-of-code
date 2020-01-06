using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2019.Day06
{
	internal class Puzzle06
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var orbitdefs = File.ReadLines("Day06/input.txt")
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
			Console.WriteLine($"Day  6 Puzzle 1: {orbitCount}");
			Debug.Assert(orbitCount == 387356);

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
			Console.WriteLine($"Day  6 Puzzle 2: {dist}");
			Debug.Assert(dist == 532);

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