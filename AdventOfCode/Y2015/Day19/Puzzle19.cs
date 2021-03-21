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

		public void Run()
		{
			//RunFor("test1", 4, 0);
			//RunPart2For("test1", 3);
			RunPart2For("test2", 6);
			RunFor("input", 535, 0);
		}



		protected override int Part1(string[] input)
		{
			// Ex: H => HCa
			var reductions = input
				.TakeWhile(x => x.Any())
				.Select(x =>
				{
					var (from, to) = x.RxMatch("%s => %s").Get<string, string>();
					return new 
					{
						From = from,
						To = to
					};
				})
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
					var (fromName, replacementNames) = line.RxMatch("%s => %s").Get<string, string>();
					var sequence = regex.Matches(replacementNames).Select(m => mols.IndexOf(m.Value)).ToArray();
					var from = mols.IndexOf(fromName);
					return (from, sequence);
				})
				.GroupBy(x => x.from, x => x.sequence)
				.ToDictionary(x => x.Key, x => x.ToArray());


			var allreductions = parts[0]
				.Select(line =>
				{
					var (fromName, replacementNames) = line.RxMatch("%s => %s").Get<string, string>();
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
			Console.WriteLine("Molecule name: " + moleculeName);
			Console.WriteLine("Molecule: " + string.Join(" ", molecule));
			Console.WriteLine("Molecule parts:");
			for (var i = 0; i < mols.Count; i++)			
			{
				Console.WriteLine($"{i,2} {mols[i],-5}:  {molecule.Count(x => x == i)}");
			}	

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

			Console.WriteLine("Can be reduced to: ");
			for (var i = 0; i < mols.Count(); i++)
			{
				var reducable = CanReduceTo(i, new List<int>());
				Console.WriteLine($"{i,2} {mols[i],-5}:  {string.Join(" ", reducable.OrderBy(x => x))}");
			}

			Console.WriteLine("Can end with: ");
			var canendwithall = new List<int>();
			for (var i = 0; i < mols.Count(); i++)
			{
				var endwith = CanEndWith(i, new List<int>());
				canendwithall = canendwithall.Concat(endwith).Distinct().ToList();
				Console.WriteLine($"{i,2} {mols[i],-5}:  {string.Join(" ", endwith.OrderBy(x => x))}");
			}
			Console.WriteLine($"Can end with in total: {string.Join(" ", canendwithall.OrderBy(x => x))}");

			Console.WriteLine("Can start with: ");
			var canStartWith = new List<int>[mols.Count];
			for (var i = 0; i < mols.Count(); i++)
			{
				var startwith = CanStartWith(i, new List<int>());
				canStartWith[i] = startwith;
				Console.WriteLine($"{i,2} {mols[i],-5}:  {string.Join(" ", startwith.OrderBy(x => x))}");
			}

			List<int> CanReduceTo(int from, List<int> seen)
			{
				if (!reductions.ContainsKey(from))
					return new List<int>();
				var reduceto = reductions[from]
					//.SelectMany(x => x.Select(y => y))
					.SelectMany(x => x)
					.Distinct()
					.Where(x => !seen.Contains(x) && x != from)
					.ToList();
				seen = seen.Concat(reduceto).ToList();
				foreach (var r in reduceto)
				{
					var reduce2 = CanReduceTo(r, seen);
					seen = seen.Concat(reduce2).Distinct().ToList();
				}
				return seen;
			}

			List<int> CanEndWith(int from, List<int> seen)
			{
				if (!reductions.ContainsKey(from))
					return new List<int>();
				var canendwith = reductions[from]
					//.SelectMany(x => x.Select(y => y))
					.Select(x => x.Last())
					.Distinct()
					.Where(x => !seen.Contains(x) && x != from)
					.ToList();
				seen = seen.Concat(canendwith).ToList();
				foreach (var r in canendwith)
				{
					var canendwith2 = CanEndWith(r, seen);
					seen = seen.Concat(canendwith2).Distinct().ToList();
				}
				return seen;
			}

			List<int> CanStartWith(int from, List<int> seen)
			{
				if (!reductions.ContainsKey(from))
					return new List<int>();
				var canstartwith = reductions[from]
					//.SelectMany(x => x.Select(y => y))
					.Select(x => x.First())
					.Distinct()
					.Where(x => !seen.Contains(x) && x != from)
					.ToList();
				seen = seen.Concat(canstartwith).ToList();
				foreach (var r in canstartwith)
				{
					var canstartwith2 = CanStartWith(r, seen);
					seen = seen.Concat(canstartwith2).Distinct().ToList();
				}
				return seen;
			}

return 0;


			foreach (var x in MathHelper.AllCombinations<int>(new [] { new int[]{1, 2, 3, 4}, new int[]{10, 11, 12}, new int[]{99}, new int[]{33,44} } ))
			{
				System.Console.WriteLine(string.Join(" ", x));
			}

			var molredux = new Dictionary<int[], int>();
			var molq = new Queue<(int[], int)>();
			molq.Enqueue((new int[] { mols.IndexOf("e") }, 0));
			while (molq.Any())
			{
				var (seq, redux) = molq.Dequeue();
				if (seq.Length == molecule.Length)
				{
					if (seq.SequenceEqual(molecule))
					{
						Console.WriteLine("Found it!!!!!!!!!!!! " + redux);
						break;
					}
					continue;
				}
				molredux[seq] = redux;

				var reduxtions = seq
					.Select(s =>
					{
						if (!reductions.ContainsKey(s) || !reductions[s].Any())
							return new int[][] { new [] {s} };
						return reductions[s];

					})
					.ToArray();
				
				foreach (var x in MathHelper.AllCombinations(reduxtions))
				{
					var mol2 = x.SelectMany(x => x).ToArray();
					if (molredux.ContainsKey(mol2))
						continue;
					var redux2 = mol2.Zip(seq, (x1, x2) => x1 == x2 ? 0 : 1).Count();
					molq.Enqueue((mol2, redux + redux2));
					System.Console.WriteLine(string.Join(" ", mol2));
				}
				//foreach (var reduc in mols[seq.Last()])
			};



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
