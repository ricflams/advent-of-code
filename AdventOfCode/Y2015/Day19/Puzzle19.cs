using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day19
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Medicine for Rudolph";
		public override int Year => 2015;
		public override int Day => 19;

		public void Run()
		{
			RunFor("test1", 4, 0);
			RunFor("input", 535, 0);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			// Ex: H => HCa
			var reductions = input
				.TakeWhile(x => x.Any())
				.Select(x => SimpleRegex.Match(x, "%s => %s"))
				.Select(x => new { From = x[0], To = x[1] });
			var molecule = input.Last();

			var molecules = new HashSet<string>();
			foreach (var r in reductions)
			{
				foreach (var m in molecule.Replacements(r.From, r.To))
				{
					molecules.Add(m);
				}
			}

			var n = molecules.Count();

			var froms = reductions.Select(x => x.From).Distinct().ToList();
			var loops = reductions.Where(r => froms.Any(from => r.To.Contains(from))).ToList();
			var unredux = reductions.Select(r =>
			new
			{
				Original = r.To,
				Reduced = froms.Aggregate(r.To, (to, from) => to.Replace(from, ""))
			}).ToList();
			var unreduxes = unredux.Where(u => u.Reduced != "").Distinct().OrderBy(x => x.Original).Select(x => $"{x.Original} => {x.Reduced}").ToList();
			////foreach (var unr in unreduxes)
			////{
			////	Console.WriteLine(unr);
			////}
			//var graph = new BaseUnitGraph<string>();
			//foreach (var r in reductions)
			//{
			//	graph.AddDirectedEdge(r.From, r.To);
			//}
			//graph.WriteAsGraphwiz();

			return (n, 1);


			molecules.Clear();
			var queue = new Queue<(string, int)>();
			var redux = reductions.GroupBy(x => x.From);
			var produced = new SimpleMemo<string>();



			queue.Enqueue(("e", 0));
			while (queue.Any())
			{
				var (mol, steps) = queue.Dequeue();
				foreach (var r in reductions)
				{
					foreach (var m in mol.Replacements(r.From, r.To))
					{
						if (produced.IsSeenBefore(m))
							continue;
						if (m == molecule)
						{
							Console.WriteLine("Found it: " + steps);
							////return;
						}
						queue.Enqueue((m, steps + 1));
					}
				}
			}
		}
	}
}
