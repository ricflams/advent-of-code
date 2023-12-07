using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2015.Day19
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Medicine for Rudolph";
		public override int Year => 2015;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(4);
			Run("test2").Part2(3);
			Run("test3").Part1(7);
			Run("test4").Part2(6);
			Run("input").Part1(535).Part2(212);
			Run("extra").Part1(518).Part2(200);
		}

		protected override int Part1(string[] input)
		{
			// Ex: H => HCa
			var reductions = input
				.TakeWhile(x => x.Any())
				.Select(x =>
				{
					var (from, to) = x.RxMatch("%s => %s").Get<string, string>();
					return (From: from, To: to);
				})
				.ToArray();
			var molecule = input.Last();

			var molecules = new HashSet<string>();
			foreach (var (from, to) in reductions)
			{
				foreach (var m in molecule.AllReplacements(from, to))
				{
					molecules.Add(m);
				}
			}

			return molecules.Count;
		}

		protected override int Part2(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();
			var (reductionsByNames, moleculeByName) = (parts[0], parts[1][0]);

			// Extract all the molecule names and find the e-molecule's index
			// Ex: H => HCa
			var regex = new Regex("([A-Z][a-z]?)|e");
			var moleculeNames = regex
				.Matches(string.Join(" ", reductionsByNames))
				.Select(m => m.Value)
				.Distinct()
				.OrderByDescending(x => x.Length)
				.ToList();
			var electron = moleculeNames.IndexOf(m => m == "e");

			// Turn all reductions into numbers (indexes into moleculeNames) because it's
			// much easier to work on than parsing the names all the time. Also turn the
			// molecule-name itself into an int[] of its constituent molecules.
			var reductions = reductionsByNames
				.Select(line =>
				{
					var (name, rep) = line.RxMatch("%s => %s").Get<string, string>();
					var replacements = regex.Matches(rep).Select(m => moleculeNames.IndexOf(m.Value)).ToArray();
					var from = moleculeNames.IndexOf(name);
					return (from, replacements);
				})
				.GroupBy(x => x.from, x => x.replacements)
				.ToDictionary(x => x.Key, x => x);
			var tempname = moleculeByName;
			for (var i = 0; i < moleculeNames.Count; i++)
			{
				tempname = tempname.Replace(moleculeNames[i], $"{i} ");
			}
			var molecule = tempname.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

			// For easier reduction, create an "inverse reductions" lookup
			var inverseReductions = new List<(int[], int)>();
			foreach (var (from, toall) in reductions)
			{
				foreach (var to in toall)
				{
					inverseReductions.Add((to, from));
				}
			}

			// Solve the reductions
			molecule = molecule.Append(-1).ToArray(); // add sentinel
			var minSteps = 0;
			var seen = new HashSet<uint>();
			var queue = Quack<(int[], int)>.Create(QuackType.PriorityQueue);
			queue.Put((molecule, 0));
			while (queue.TryGet(out var item))
			{
				var (mol, steps) = item;

				var key = Hashing.Hash(mol);
				if (seen.Contains(key))
					continue;
				seen.Add(key);

				if (mol.Length == 2 && mol[0] == electron)
				{
					minSteps = steps;
					break;
				}

				for (var i = 0; i < mol.Length; i++)
				{
					foreach (var (to,from) in inverseReductions)
					{
						if (LookingAt(mol, i, to))
						{
							var mol2 = mol[..i].Append(from).Concat(mol[(i + to.Length)..]).ToArray();
							queue.Put((mol2, steps+1), mol2.Length);
						}
					}
				}
			}
			
			static bool LookingAt(int[] a, int offset, int[] b)
			{
				for (var i = 0; i < b.Length; i++)
					if (a[i + offset] != b[i])
						return false;
				return true;
			}

			return minSteps;
		}
	}
}
