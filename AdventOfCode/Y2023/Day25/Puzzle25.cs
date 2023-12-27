using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day25
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

			var comps = conns.Keys.ToArray();

			var stat = conns
				.Select(x => (x.Key, x.Value.Count()))
				.OrderByDescending(x => x.Item2)
				.ToArray();


			for (var i1 = 0; i1 < comps.Length; i1++)
			{
				for (var i2 = i1 + 1; i2 < comps.Length; i2++)
				{
					for (var i3 = 0; i3 < comps.Length; i3++)
					{
						for (var i4 = i3+1; i4 < comps.Length; i4++)
						{
							
						}
					}
				}
			}


			return 0;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
