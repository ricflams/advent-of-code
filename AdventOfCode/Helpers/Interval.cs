using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Helpers
{
	public record Interval(int Start, int End)
	{
		public bool Overlaps(Interval o) => Start < o.End && o.Start < End;
		public bool Contains(int v) => Start <= v && v < End;
		public int Length => End - Start;

		public Interval Combine(Interval o)
		{
			return new Interval(Math.Min(Start, o.Start), Math.Max(End, o.End));
		}
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
	}
}