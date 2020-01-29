using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day13
{
	internal class Puzzle13
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2015/Day13/input.txt");

			var relations = input.Select(Relation.ParseFrom).ToList();
			int maxHappiness = CalculateMaxHappiness(relations);

			Console.WriteLine($"Day 13 Puzzle 1: {maxHappiness}");
			Debug.Assert(maxHappiness == 618);

			var myRelations = relations
				.Select(r => r.Name)
				.Distinct()
				.SelectMany(name => new Relation[]
				{
					new Relation
					{
						Name = "Me",
						Happiness = 0,
						Neighbor = name
					},
					new Relation
					{
						Name = name,
						Happiness = 0,
						Neighbor = "Me"
					}
				})
				.ToList();
			relations.AddRange(myRelations);
			int maxHappinessWithMe = CalculateMaxHappiness(relations);

			Console.WriteLine($"Day 13 Puzzle 2: {maxHappinessWithMe}");
			Debug.Assert(maxHappinessWithMe == 601);
		}

		private static int CalculateMaxHappiness(List<Relation> relations)
		{
			var personNames = relations.Select(r => r.Name).Distinct().ToList();

			var persons = personNames.Select(name =>
				{
					var personRelations = relations.Where(r => r.Name == name);
					var happiness = personRelations.ToDictionary(x => personNames.IndexOf(x.Neighbor), x => x.Happiness);
					return happiness;
				})
				.ToList();
			var N = persons.Count();

			var maxHappiness = 0;
			foreach (var perm in MathHelper.AllPermutations(N))
			{
				var happiness = 0;
				var order = perm.ToArray();
				for (var i = 0; i < N; i++)
				{
					var i1 = order[i];
					var i2 = order[(i + 1 + N) % N];
					var p1 = persons[i1];
					var p2 = persons[i2];
					happiness += p1[i2] + p2[i1];
				}
				if (happiness > maxHappiness)
				{
					maxHappiness = happiness;
				}
			}

			return maxHappiness;
		}

		private class Relation
		{
			public string Name { get; set; }
			public int Happiness { get; set; }
			public string Neighbor { get; set; }

			public static Relation ParseFrom(string line)
			{
				// Examples:
				// David would gain 91 happiness units by sitting next to Eric.
				// David would lose 51 happiness units by sitting next to Alice.
				var val = SimpleRegex.Match(line, "%s would %s %d happiness units by sitting next to %s");
				return new Relation
				{
					Name = val[0],
					Happiness = (val[1] == "gain" ? 1 : -1) * int.Parse(val[2]),
					Neighbor = val[3]
				};
			}
		}
	}
}
