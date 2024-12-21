using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day19.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(6).Part2(16);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(365).Part2(730121486795169);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var patterns = input[0].SplitByAny(" ,").ToArray();
			var designs = input[2..];

			var possible = new Dictionary<string, bool>();

			bool IsPossible(string des)
			{
				if (des.Length == 0)
					return true;
				if (possible.TryGetValue(des, out var ok))
					return ok;
				foreach (var p in patterns)
				{
					if (des.StartsWith(p) && IsPossible(des[p.Length..]))
					{
						possible[des] = true;
						return true;
					}
				}
				possible[des] = false;
				return false;
			}

			var poss = designs.Count(IsPossible);

			return poss;
		}

		protected override long Part2(string[] input)
		{
			var patterns = input[0].SplitByAny(" ,").ToArray();
			var designs = input[2..];

			var possible = new Dictionary<string, bool>();
			var variants = new SafeDictionary<string, long>();

			long CountVariations(string des)
			{
				if (des.Length == 0)
					return 1;
				if (variants.TryGetValue(des, out var n))
					return n;
				foreach (var p in patterns)
				{
					if (des.StartsWith(p))
					{
						variants[des] += CountVariations(des[p.Length..]);
					}
				}
				//variants[des] = 0;
				return variants[des];
			}

			var varia = designs.Sum(CountVariations);

			return varia;
		}
	}
}
