using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day05
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "If You Give A Seed A Fertilizer";
		public override int Year => 2023;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1(35).Part2(46);
			Run("input").Part1(313045984).Part2(20283860);
			Run("test9").Part1(227653707).Part2(78775051);
		}

		protected override long Part1(string[] input)
		{
			var seeds = input[0]
				.Split(':')[1]
				.SplitSpace()
				.Select(long.Parse);

			var layers = input
				.Skip(2)
				.GroupByEmptyLine()
				.Select(lines => new Layer(lines.Skip(1)))
				.ToArray();

			// Map all seeds through all layers to find the lowest position
			var lowest = seeds.Min(v => layers.Aggregate(v, (v, layer) => layer.Map(v)));

			return lowest;
		}

		protected override long Part2(string[] input)
		{
			var seed = input[0]
				.Split(':')[1]
				.SplitSpace()
				.Select(long.Parse)
				.Chunk(2)
				.Select(x => (Start: x[0], Length: x[1]))
				.ToArray();

			var layers = input
				.Skip(2)
				.GroupByEmptyLine()
				.Select(lines => new Layer(lines.Skip(1)))
				.ToArray();

			// Look through all the layers. Fetch the lowest destinations of any mappings.
			// For those that could come from a valid seed, calculate their location and
			// pick the lowest of those locations.
			// This should work unless the lowest location doesn't go through any mappings
			// at all and that's pretty much guaranteed not to be the case for this puzzle.
			var lowest = long.MaxValue;
			for (var i = layers.Length; i-- > 0; )
			{
				var minv = layers[i].Maps
					.Select(m => m.Dest)
					.Where(v => IsValidSeed(FindSeedForValue(i, v)));
				foreach (var v in minv)
				{
					var location = FindLocationForValue(i+1, v);
					if (location < lowest)
						lowest = location;
				}
			}

			return lowest;

			bool IsValidSeed(long v) => seed.Any(x => x.Start <= v && v < x.Start+x.Length);

			long FindSeedForValue(int from, long v)
			{
				for (var i = from; i >= 0; i--)
					v = layers[i].ReverseMap(v);
				return v;
			}

			long FindLocationForValue(int from, long v)
			{
				for (var i = from; i < layers.Length; i++)
					v = layers[i].Map(v);
				return v;
			}
		}

		internal class Layer(IEnumerable<string> lines)
		{
			public Mapping[] Maps = lines.Select(x => new Mapping(x)).ToArray();

			public long Map(long v)
			{
				foreach (var r in Maps)
					if (r.Source <= v && v < r.Source + r.Length)
						return v - r.Source + r.Dest;
				return v;
			}

			public long ReverseMap(long v)
			{
				foreach (var r in Maps)
					if (r.Dest <= v && v < r.Dest + r.Length)
						return v - r.Dest + r.Source;
				return v;
			}

			public class Mapping
			{
				public Mapping(string s)
				{
					var x = s.SplitSpace().Select(long.Parse).ToArray();
					(Dest, Source, Length) = (x[0], x[1], x[2]);
				}
				public long Dest;
				public long Source;
				public long Length;
			}
		}
	}
}
