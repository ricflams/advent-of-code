using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2015.Day19
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Medicine for Rudolph";
		public override int Year => 2015;
		public override int Day => 19;

		public void Run()
		{
			//RunFor("test1", 4, 0);
			RunFor("input", 535, 0);
		}



		protected override int Part1(string[] input)
		{
			// Ex: H => HCa
			var reductions = input
				.TakeWhile(x => x.Any())
				.Select(x => SimpleRegex.Match(x, "%s => %s"))
				.Select(x => new { From = x[0], To = x[1] })
				.ToArray();
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
			return n;
		}


		internal class Reduction
		{
			//public string From { get; set; }
			public string[] Sequence { get; set; }
		}

		protected override int Part2(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();

			// Ex: H => HCa
			var regex = new Regex("([A-Z][a-z]?)|e");

			var mols = regex
				.Matches(string.Join(" ", parts[0]))
				.Select(m => m.Value)
				.Distinct()
				.OrderByDescending(x => x.Length)
				.ThenBy(x => x)
				.ToList();

			var reductions = parts[0]
				.Select(line =>
				{
					line.RegexCapture("%s => %s").Get(out string fromName).Get(out string replacementNames);
					var sequence = regex.Matches(replacementNames).Select(m => mols.IndexOf(m.Value)).ToArray();
					var from = mols.IndexOf(fromName);
					return (from, sequence);
				})
				.GroupBy(x => x.from, x => x.sequence)
				.ToDictionary(x => x.Key, x => x);


			var allreductions = parts[0]
				.Select(line =>
				{
					line.RegexCapture("%s => %s").Get(out string fromName).Get(out string replacementNames);
					var sequence = regex.Matches(replacementNames).Select(m => mols.IndexOf(m.Value)).ToArray();
					var from = mols.IndexOf(fromName);
					return (from, sequence);
				})
				.ToArray();
			var moleculeName = parts[1][0];



			var molname = moleculeName;
			for (var i = 0; i < mols.Count; i++)
			{
				molname = molname.Replace(mols[i], $"{i} ");
			}
			var molecule = molname.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

			for (var i = 0; i < mols.Count(); i++)
			{
				if (!reductions.ContainsKey(i))
				{
					Console.WriteLine($"{mols[i],-5} {i,2} --");
				}
				else
				{
					var r = reductions[i];
					foreach (var red in r)
					{
						Console.WriteLine($"{mols[i],-5} {i,2} => {string.Join(" ", red.Select(rr => rr.ToString()))}");
					}
				}
			}
			Console.WriteLine("Molecule: " + string.Join(" ", molecule));

			Console.WriteLine("Reductions to:");
			for (var i = 0; i < mols.Count(); i++)
			{
				var reducedbys = allreductions.Where(r => r.sequence.Any(x => x == i)).ToArray();
				if (reducedbys.Any())
				{
					foreach (var reducedby in reducedbys)
					{
						Console.WriteLine($"{i,2} {mols[i],-5}:  {mols[reducedby.from],-5} {reducedby.from,2} => {string.Join(" ", reducedby.sequence.Select(rr => rr.ToString()))}");
					}
				}
				else
				{
					Console.WriteLine($"{i,2} {mols[i],-5}:  none");
				}
			}


			var allredux = reductions
				.SelectMany(x => x.Value.SelectMany(y => y))
				.Distinct()
				.ToArray();

			var terminals = allredux.Where(x => !reductions.ContainsKey(x)).ToList();
			Console.WriteLine("Terminals: " + string.Join(" ", terminals));



			//var finals = allredux.Select(x => x.ToList .Where(x => x.Any(z => terminals.Contains(z)));


			return 0;
		}

		//var froms = reductions.Select(x => x.From).Distinct().ToList();
		//var loops = reductions.Where(r => froms.Any(from => r.To.Contains(from))).ToList();
		//var unredux = reductions.Select(r =>
		//new
		//{
		//	Original = r.To,
		//	Reduced = froms.Aggregate(r.To, (to, from) => to.Replace(from, ""))
		//}).ToList();
		//var unreduxes = unredux.Where(u => u.Reduced != "").Distinct().OrderBy(x => x.Original).Select(x => $"{x.Original} => {x.Reduced}").ToList();
		//////foreach (var unr in unreduxes)
		//////{
		//////	Console.WriteLine(unr);
		//////}
		////var graph = new BaseUnitGraph<string>();
		////foreach (var r in reductions)
		////{
		////	graph.AddDirectedEdge(r.From, r.To);
		////}
		////graph.WriteAsGraphwiz();

		//return (n, 1);


		//molecules.Clear();
		//var queue = new Queue<(string, int)>();
		//var redux = reductions.GroupBy(x => x.From);
		//var produced = new SimpleMemo<string>();



		//queue.Enqueue(("e", 0));
		//while (queue.Any())
		//{
		//	var (mol, steps) = queue.Dequeue();
		//	foreach (var r in reductions)
		//	{
		//		foreach (var m in mol.Replacements(r.From, r.To))
		//		{
		//			if (produced.IsSeenBefore(m))
		//				continue;
		//			if (m == molecule)
		//			{
		//				Console.WriteLine("Found it: " + steps);
		//				////return;
		//			}
		//			queue.Enqueue((m, steps + 1));
		//		}
		//	}
		//}
	}
}
