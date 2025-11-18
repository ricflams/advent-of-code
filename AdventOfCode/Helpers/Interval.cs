using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Helpers
{
	[DebuggerDisplay("{ToString()}")]
	public record Interval(int Start, int End)
	{
		public static readonly Interval Empty = new(0, 0);
		public bool Overlaps(Interval o) => Start <= o.End && o.Start <= End;
		public bool Contains(int v) => Start <= v && v < End;
		public int Length => End - Start;

		public Interval Combine(Interval o)
		{
			return new Interval(Math.Min(Start, o.Start), Math.Max(End, o.End));
		}

		public Interval Intersect(Interval o)
		{
			if (o.Start > End || Start > o.End)
				return Empty;
			return new Interval(Math.Max(Start, o.Start), Math.Min(End, o.End));
		}

		public override string ToString() => $"[{Start}-{End}[";
	}

	public static class IntervalExtensions
	{
		public static Interval[] Reduce(this IEnumerable<Interval> ranges)
		{
			var rs = ranges.OrderBy(r => r.Start).ToArray();

			var result = new List<Interval>();
			for (var i = 0; i < rs.Length; i++)
			{
				var merged = 0;
				while (i+merged+1 < rs.Length && rs[i].Overlaps(rs[i+merged+1]))
				{
					rs[i] = rs[i].Combine(rs[i+merged+1]);
					merged++;
				}
				result.Add(rs[i]);
				i += merged;
			}

			return result.ToArray();
		}

		public static int TotalLength(this IEnumerable<Interval> ranges)
		{
			return ranges.Sum(r => r.Length);
		}
	}
}