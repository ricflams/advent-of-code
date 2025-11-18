using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day12
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(140).Part2(80);
			Run("test2").Part1(772).Part2(436);
			Run("test3").Part1(1930).Part2(1206);
			Run("test4").Part2(236);
			Run("input").Part1(1452678).Part2(873584);
			// 867484 too low

			Run("extra").Part1(1461806).Part2(887932);
			// 880828 too low
			// 880850 too low
			// 881000 not right
			// 900000 too high
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var plots = new HashSet<Point>(map.AllPoints());
			var regions = new List<HashSet<Point>>();
			while (plots.Count > 0)
			{
				var region = new HashSet<Point>();
				var p0 = plots.First();
				FindRegion(region, p0, map[p0]);
				regions.Add(region);
			}

			var cost = regions.Sum(r => Price(r));

			void FindRegion(HashSet<Point> region, Point p0, char type)
			{
				region.Add(p0);
				plots.Remove(p0);
				foreach (var p in p0.LookAround().Where(x => plots.Contains(x) && map[x] == type))
				{
					FindRegion(region, p, type);
				}
			}

			int Price(HashSet<Point> region)
			{
				var perimeter = region.Sum(p0 => p0.LookAround().Count(p => !region.Contains(p)));
				return perimeter * region.Count;
			}

			return cost;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			var plots = new HashSet<Point>(map.AllPoints());
			var regions = new List<HashSet<Point>>();
			while (plots.Count > 0)
			{
				var region = new HashSet<Point>();
				var p0 = plots.First();
				FindRegion(region, p0, map[p0]);
				regions.Add(region);
			}

			var cost = regions.Sum(r => Price(r));

			void FindRegion(HashSet<Point> region, Point p0, char type)
			{
				region.Add(p0);
				plots.Remove(p0);
				foreach (var p in p0.LookAround().Where(x => plots.Contains(x) && map[x] == type))
				{
					FindRegion(region, p, type);
				}
			}

			int Price(HashSet<Point> region)
			{
				var htop = new SafeDictionary<int, List<Interval>>(() => []);
				var hbot = new SafeDictionary<int, List<Interval>>(() => []);
				var vlef = new SafeDictionary<int, List<Interval>>(() => []);
				var vrig = new SafeDictionary<int, List<Interval>>(() => []);
				foreach (var p0 in region)
				{
					if (!region.Contains(p0.N)) htop[p0.Y].Add(new Interval(p0.X, p0.X + 1));
					if (!region.Contains(p0.S)) hbot[p0.Y].Add(new Interval(p0.X, p0.X + 1));
					if (!region.Contains(p0.W)) vlef[p0.X].Add(new Interval(p0.Y, p0.Y + 1));
					if (!region.Contains(p0.E)) vrig[p0.X].Add(new Interval(p0.Y, p0.Y + 1));
				}

				//var perim = 0;
				var hsides = 0;
				var vsides = 0;

				var perim =
					htop.Values.Sum(x => x.Reduce().Length) +
					hbot.Values.Sum(x => x.Reduce().Length) +
					vlef.Values.Sum(x => x.Reduce().Length) +
					vrig.Values.Sum(x => x.Reduce().Length);

				// foreach (var h in hor.OrderBy(x => x.Key))
				// {
				// 	var sides = h.Value.Reduce();
				// 	Console.WriteLine($"Y={h.Key} {string.Join(" ", h.Value.OrderBy(x => x.Start).Select(x => x.ToString()))} => ${string.Join(" ", sides.OrderBy(x => x.Start).Select(x => x.ToString()))}");

				// 	Debug.Assert(sides.TotalLength() == h.Value.TotalLength());
				// 	hsides += sides.Length; 
				// 	perim += sides.Length;
				// }
				// foreach (var v in ver.OrderBy(x => x.Key))
				// {
				// 	var sides = v.Value.Reduce();

				// 	Console.WriteLine($"X={v.Key} {string.Join(" ", v.Value.OrderBy(x => x.Start).Select(x => x.ToString()))} => ${string.Join(" ", sides.OrderBy(x => x.Start).Select(x => x.ToString()))}");
				// 	Debug.Assert(sides.TotalLength() == v.Value.TotalLength());
				// 	vsides += sides.Length; 
				// 	perim += sides.Length;
				// }

				// var map2 = new CharMap(' ');
				// foreach (var p in region)
				// 	map2[p] = map[p];
				// Console.WriteLine($"{hsides}--   {vsides}|");
				// map2.ConsoleWrite();
				// Console.WriteLine();


				return perim * region.Count;

				// var hsum = hor.Sum(x => x.Value.Reduce().Length);
				// var vsum = ver.Sum(x => x.Value.Reduce().Length);
				// var perimeter = hsum + vsum;

				// return perimeter * region.Count;
			}

			return cost;
		}
	}
}
