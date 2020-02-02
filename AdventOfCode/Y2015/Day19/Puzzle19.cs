using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day19
{
	internal class Puzzle19
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllLines("Y2015/Day19/input.txt");

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
			Console.WriteLine($"Day 19 Puzzle 1: {n}");
			Debug.Assert(n == 535);
		}

		private static void Puzzle2()
		{

			//Console.WriteLine($"Day 19 Puzzle 2: {result}");
			//Debug.Assert(result == );
		}
	}
}
