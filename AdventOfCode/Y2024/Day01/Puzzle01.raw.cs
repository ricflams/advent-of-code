using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day01.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(11).Part2(31);
			Run("input").Part1(1341714).Part2(27384707);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var pairs = input.Select(x => x.SplitSpace()).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToArray();
			var p1 = pairs.Select(x => x.Item1).OrderBy(x => x).ToArray();
			var p2 = pairs.Select(x => x.Item2).OrderBy(x => x).ToArray();
			var sum = 0;
			for (var i = 0; i < p1.Length; i++)
			{
				sum += Math.Abs(p1[i] - p2[i]);
			}

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var pairs = input.Select(x => x.SplitSpace()).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToArray();
			var p1 = pairs.Select(x => x.Item1).OrderBy(x => x).ToArray();
			var p2 = pairs.Select(x => x.Item2).OrderBy(x => x).ToArray();
			var sum = 0;

			foreach (var p in p1)	
			{
				foreach (var pp in p2)
				{
					if (p == pp)
						sum += p;
				}
			}

			return sum;
		}
	}
}
