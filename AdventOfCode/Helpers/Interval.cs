using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Helpers
{
	[DebuggerDisplay("{ToString()}")]
	public record Interval<T>(T Start, T End)
        where T : INumber<T>
	{
		public static readonly Interval<T> Empty = new(default, default);
		public bool Overlaps(Interval<T> o) => Start <= o.End && o.Start <= End;
		public bool Contains(T v) => Start <= v && v < End;
		public T Length => End - Start;

		public Interval<T> Combine(Interval<T> o)
		{
			return new Interval<T>(T.Min(Start, o.Start), T.Max(End, o.End));
		}

		public Interval<T> Intersect(Interval<T> o)
		{
			if (o.Start > End || Start > o.End)
				return Empty;
			return new Interval<T>(T.Max(Start, o.Start), T.Min(End, o.End));
		}

		public override string ToString() => $"[{Start}-{End}[";
	}

	public static class IntervalExtensions
	{
		public static Interval<T>[] Reduce<T>(this IEnumerable<Interval<T>> ranges) where T : INumber<T>
        {
			var rs = ranges.OrderBy(r => r.Start).ToArray();

			var result = new List<Interval<T>>();
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

		public static T TotalLength<T>(this IEnumerable<Interval<T>> ranges) where T : INumber<T>
        {
            var length = default(T);
            foreach (var range in ranges)
            {
                length += range.Length;
            }
			return length;
		}
	}
}