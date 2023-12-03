using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day06
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Chronal Coordinates";
		public override int Year => 2018;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(17);
			Run("input").Part1(3871).Part2(44667);
		}

		protected override int Part1(string[] input)
		{
			var places = input.Select(Point.Parse).ToArray();
			var (xmin, xmax, ymin, ymax) = SpannedArea(places);

			// Determine the nearest place for every point in the spanned
			// area, if one exists. No need to store them in a 2D-map since we're
			// only interested in their total sum. Every place that is nearest
			// some point along the edge will by definition be infinite (no other
			// point can ever be closer, Manhattan-distance-wise) so record those
			// to be skipped in the next step.
			var infinites = new HashSet<int>();
			var area = new int[places.Length];
			for (var x = xmin; x <= xmax; x++)
			{
				for (var y = ymin; y <= ymax; y++)
				{
					if (HasSingleNearestPlace(places, x, y, out var id))
					{
						area[id]++;
						if (x == xmin || x == xmax || y == ymin || y == ymax)
						{
							infinites.Add(id);
						}
					}
				}
			}

			// The largest area that is not infinite
			var largestArea = Enumerable.Range(0, input.Length)
				.Where(i => !infinites.Contains(i))
				.Select(i => area[i])
				.Max();

			return largestArea;

		}

		protected override int Part2(string[] input)
		{
			var places = input.Select(Point.Parse).ToArray();
			var (xmin, xmax, ymin, ymax) = SpannedArea(places);

			// Simply sum up all the "locations which have a total distance
			// to all given coordinates of less than 10000" :-)
			var regionSize = 0;
			var totalDistance = 10000;
			for (var x = xmin; x <= xmax; x++)
			{
				for (var y = ymin; y <= ymax; y++)
				{
					var here = Point.From(x, y);
					var totaldist = places.Sum(pt => pt.ManhattanDistanceTo(here));
					if (totaldist < totalDistance)
					{
						regionSize++;
					}
				}
			}

			return regionSize;
		}


		private static (int, int, int, int) SpannedArea(Point[] points) =>
			(
				points.Min(p => p.X),
				points.Max(p => p.X),
				points.Min(p => p.Y),
				points.Max(p => p.Y)
			);

		private static bool HasSingleNearestPlace(Point[] places, int x, int y, out int id)
		{
			// Doing this the "raw" way is quite much faster than eg using
			// linq to calculate the distances, then order/group by distance,
			// and then check if more than one place share that minimum distance.
			// Instead, simply loop all places and record the minimum distance
			// and nullify the find if that another place also has that minimum
			// distance.
			id = -1;
			var here = Point.From(x, y);
			var min = int.MaxValue;
			for (var i = 0; i < places.Length; i++)
			{
				var dist = places[i].ManhattanDistanceTo(here);
				if (dist < min)
				{
					min = dist;
					id = i;
				}
				else if (dist == min)
				{
					id = -1;
				}
			}
			return id >= 0;
		}
	}
}
