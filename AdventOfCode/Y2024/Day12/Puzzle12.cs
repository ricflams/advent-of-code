using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day12
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Garden Groups";
		public override int Year => 2024;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(140).Part2(80);
			Run("test2").Part1(772).Part2(436);
			Run("test3").Part1(1930).Part2(1206);
			Run("test4").Part2(236);
			Run("input").Part1(1452678).Part2(873584);
			Run("extra").Part1(1461806).Part2(887932);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var regions = FindRegions(map);
			var cost = regions.Sum(Price);
			return cost;

			int Price(Region region)
			{
				var perimeter = region.Sum(p0 => p0.LookAround().Count(p => !region.Contains(p)));
				return perimeter * region.Count;
			}
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var regions = FindRegions(map);
			var cost = regions.Sum(Price);
			return cost;

			static int Price(Region region)
			{
				var top = new SafeDictionary<int, List<Interval<int>>>(() => []);
				var bot = new SafeDictionary<int, List<Interval<int>>>(() => []);
				var lef = new SafeDictionary<int, List<Interval<int>>>(() => []);
				var rig = new SafeDictionary<int, List<Interval<int>>>(() => []);
				foreach (var p in region)
				{
					if (!region.Contains(p.N)) top[p.Y].Add(new Interval<int>(p.X, p.X + 1));
					if (!region.Contains(p.S)) bot[p.Y].Add(new Interval<int>(p.X, p.X + 1));
					if (!region.Contains(p.W)) lef[p.X].Add(new Interval<int>(p.Y, p.Y + 1));
					if (!region.Contains(p.E)) rig[p.X].Add(new Interval<int>(p.Y, p.Y + 1));
				}

				var perimeter =
					top.Values.Sum(x => x.Reduce().Length) +
					bot.Values.Sum(x => x.Reduce().Length) +
					lef.Values.Sum(x => x.Reduce().Length) +
					rig.Values.Sum(x => x.Reduce().Length);

				return perimeter * region.Count;
			}
		}

		private class Region : HashSet<Point>
		{
			public Region(IEnumerable<Point> points) : base(points) { }
			public Region() { }
		}

		private static IEnumerable<Region> FindRegions(CharMap map)
		{
			// Pick out a plot and fill a region with all nearby plots of
			// the same type until there are no more plots left
			var plots = new Region(map.AllPoints());
			while (plots.Count > 0)
			{
				var region = new Region();
				FillRegion(region, plots.First());
				yield return region;
			}

			void FillRegion(Region region, Point p0)
			{
				var type = map[p0];
				region.Add(p0);
				plots.Remove(p0);
				foreach (var p in p0.LookAround().Where(x => plots.Contains(x) && map[x] == type))
				{
					FillRegion(region, p);
				}
			}
		}
	}
}
