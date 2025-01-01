using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day23
{
	internal class Puzzle : Puzzle<long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "LAN Party";
		public override int Year => 2024;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(7).Part2("co,de,ka,ta");
			Run("input").Part1(1046).Part2("de,id,ke,ls,po,sn,tf,tl,tm,uj,un,xw,yz");
			Run("extra").Part1(1323).Part2("er,fh,fi,ir,kk,lo,lp,qi,ti,vb,xf,ys,yu");
		}

		protected override long Part1(string[] input)
		{
			var conns = input.Select(s => s.Split('-')).Select(x => (a: x[0], b: x[1])).ToArray();

			var sets = new SafeDictionary<string, HashSet<string>>(() => []);
			foreach (var (a, b) in conns)
			{
				sets[a].Add(b);
				sets[b].Add(a);
			}

			var n = 0;
			foreach (var node in sets)
			{
				var name1 = node.Key;
				foreach (var name2 in node.Value)
				{
					foreach (var name3 in sets[name2])
					{
						if (sets[name3].Contains(name1))
						{
							// found set of three
							if (name1[0] == 't' || name2[0] == 't' || name3[0] == 't')
								n++;
						}
					}
				}
			}

			// We counted all combinations of triplets, ie 3*2*1 = 6 combinations
			// so divide by 6 to get the real number of triplets
			n /= 6;

			return n;
		}

		protected override string Part2(string[] input)
		{
			var conns = input.Select(s => s.Split('-')).Select(x => (a: x[0], b: x[1])).ToArray();

			var graph = new Graph<string>();
			foreach (var (a, b) in conns)
			{
				graph.AddNodes(a, b, 1);
			}

			var cliques = graph.MaximumClique();
			var clique = cliques.OrderByDescending(x => x.Count).First();
			var code = string.Join(',', clique.Select(x => x.Id).OrderBy(x => x));

			return code;
		}
	}
}
