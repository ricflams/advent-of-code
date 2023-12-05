using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day05.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(35).Part2(0);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(313045984).Part2(0);
		}

		internal record Map
		{
			public Map(string s) {
				var x = s.SplitSpace().Select(long.Parse).ToArray();
				(DestStart, SourceStart, Length) = (x[0], x[1], x[2]);
				SourceEnd = SourceStart + Length;
			}
			public long DestStart;
			public long SourceStart;
			public long SourceEnd;
			public long Length;
		}

		protected override long Part1(string[] input)
		{
			var seeds = input[0].Split(':')[1].SplitSpace().Select(long.Parse).ToArray();
			var maps = input
				.Skip(2)
				.GroupByEmptyLine()
				.Select(lines => lines.Skip(1).Select(x => new Map(x)).ToArray())
				.ToArray();

			var minpos = long.MaxValue;
			foreach (var seed in seeds)
			{
				var v = seed;
				foreach (var map in maps)
				{
					var range = map.FirstOrDefault(m => m.SourceStart <= v && v < m.SourceStart+m.Length);
					if (range != null)
						v += range.DestStart - range.SourceStart;					
				}
				if (v < minpos)
					minpos = v;
			}

			return minpos;
		}

		protected override long Part2(string[] input)
		{
			var seedvals = input[0].Split(':')[1].SplitSpace().Select(long.Parse).ToArray();

			var seedRanges = new List<(long, long)>();
			for (var i = 0; i < seedvals.Length/2; i++)
			{
				seedRanges.Add((seedvals[i*2], seedvals[i*2+1]));
			}

			var maps = input
				.Skip(2)
				.GroupByEmptyLine()
				.Select(lines => lines.Skip(1).Select(x => new Map(x)).ToArray())
				.ToArray();

			var minpos = long.MaxValue;
			foreach (var seedRange in seedRanges)
			{
				for (var seed = seedRange.Item1; seed < seedRange.Item1+seedRange.Item2; seed++)
				{
					var v = seed;
					foreach (var map in maps)
					{
						foreach (var r in map)
						{
							if (r.SourceStart <= v && v < r.SourceEnd)
							{
								v += r.DestStart - r.SourceStart;
								break;
							}
						}
						// var range = map.FirstOrDefault(m => m.SourceStart <= v && v < m.SourceStart+m.Length);
						// if (range != null)
						// 	v += range.DestStart - range.SourceStart;
					}
					if (v < minpos)
						minpos = v;

				}
			}

			return minpos;
		}
	}
}
