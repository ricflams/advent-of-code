using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2016.Day20
{
	internal class Puzzle : Puzzle<uint, uint>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Firewall Rules";
		public override int Year => 2016;
		public override int Day => 20;

		public override void Run()
		{
			Run("input").Part1(32259706).Part2(113);
			Run("extra").Part1(31053880).Part2(117);
		}

		protected override uint Part1(string[] input)
		{
			var ranges = input.Select(Exclusions.Parse).ToArray();

			// There are ranges starting at 0, otherwise the answer would just be 0.
			// Pick the value just ABOVE the lowest range and then check if it appears
			// in another exclude-range; if it does then choose the value just above
			// that range's end. Repeat so until it's not part of any excludes.
			var lowest = ranges.Where(x => x.Start == 0).Min(x => x.End) + 1;
			while (true)
			{
				var overlaps = ranges.Where(r => r.Contains(lowest)).ToArray();
				if (!overlaps.Any())
					break;
				lowest = overlaps.Max(x => x.End) + 1;
			}

			return lowest;
		}

		protected override uint Part2(string[] input)
		{
			var ranges = input.Select(Exclusions.Parse).ToArray();

			// Build up the union of all the exclude-ranges. The number of allowed IPs
			// are then the NUMBER of uint32-values minus the sum of those ranges, which
			// is the same as uint.MaxValue PLUS ONE minus that sum. Do the +1 at the
			// end to avoid overflow (unlike C, C# guarantees operands are not reordered).
			var union = new List<Exclusions>();
			foreach (var r in ranges)
			{
				Exclusions.MergeInto(union, r);
			}
			var allowed = uint.MaxValue - (uint)union.Sum(r => r.Length) + 1;
 
			return allowed;
		}

		private class Exclusions
		{
			public static Exclusions Parse(string line)
			{
				var p = line.Split('-');
				return new Exclusions
				{
					Start = uint.Parse(p[0]),
					End = uint.Parse(p[1])
				};
			}
			public uint Start { get; private set; }
			public uint End { get; private set; } // End is INCLUSIVE!
			public uint Length => End - Start + 1;
			public bool Contains(uint v) => Start <= v && v <= End;
			public bool CanMerge(Exclusions r) => r.Start <= End && Start <= r.End;
			public void Absorbe(Exclusions r)
			{
				Start = Math.Min(Start, r.Start);
				End = Math.Max(End, r.End);
			}
			public static void MergeInto(List<Exclusions> set, Exclusions range)
			{
				while (true)
				{
					var overlap = set.FirstOrDefault(r => range.CanMerge(r));
					if (overlap == null)
						break;
					range.Absorbe(overlap);
					set.Remove(overlap);
				}
				set.Add(range);
			}
		}
	}
}
