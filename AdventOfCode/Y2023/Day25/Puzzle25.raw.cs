using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day25.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1(54).Part2(0);
	//		Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
	//		Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var connpairs = input
				.SelectMany(s =>
				{
					var xx = s.Split(':').ToArray();
					var src = xx[0];
					return xx[1].SplitSpace().Select(x => (A: src, B: x));
				});
			var conns = new Dictionary<string, List<string>>();
			foreach (var c in connpairs)
			{
				Install(c.A, c.B);
				Install(c.B, c.A);
			}

			void Install(string a, string b)
			{
				if (!conns.TryGetValue(a, out var list))
					list = conns[a] = new();
				list.Add(b);
			}

			var nodes = conns.Keys.ToArray();

			var visitcount = new SafeDictionary<(string,string), int>();
			foreach (var n in nodes)
			{
				VisitAll(n);
			}

			var n1 = NetworkSize(nodes[0]);

			var result = visitcount
				.Select(x => (x.Key, x.Value))
				.OrderByDescending(x => x.Value)
				.Take(3)
				.ToArray();
			foreach (var n in result)
			{
				Cut(n.Key.Item1, n.Key.Item2);
				Cut(n.Key.Item2, n.Key.Item1);
			}
			void Cut(string n1, string n2)
			{
				conns[n1].Remove(n2);
			}

			var n2 = NetworkSize(nodes[0]);

			return (n1 - n2)*n2;

			void VisitAll(string n)
			{
				var seen = new HashSet<string>();
				var queue = new Queue<string>();
				queue.Enqueue(n);
				while (queue.TryDequeue(out var node))
				{
					var nexts = conns[node].Where(nn => !seen.Contains(nn)).ToList(); 
					foreach (var nn in nexts)
					{
						var (n1, n2) = node.CompareTo(nn) < 0 ? (node, nn) : (nn, node);
						visitcount[(n1, n2)]++;
						seen.Add(nn);
						queue.Enqueue(nn);
					}
				} 
			}

			int NetworkSize(string n)
			{
				var seen = new HashSet<string>();
				var queue = new Queue<string>();
				queue.Enqueue(n);
				while (queue.TryDequeue(out var node))
				{
					var nexts = conns[node].Where(nn => !seen.Contains(nn)).ToList(); 
					foreach (var nn in nexts)
					{
						seen.Add(nn);
						queue.Enqueue(nn);
					}
				}
				return seen.Count();
			}

			//return 0;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
