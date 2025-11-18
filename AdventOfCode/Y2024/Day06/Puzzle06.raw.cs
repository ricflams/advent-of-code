using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day06.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(41).Part2(6);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(4890).Part2(1995);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (min, max) = map.MinMax();

			var guard = Pose.From(map.AllPointsWhere(x => x == '^').Single(), Direction.Up);
			var seen = new HashSet<Point>();

			while (true)
			{
				seen.Add(guard.Point);
				while (map[guard.PeekAhead] == '#')
					guard.TurnRight();
				guard.Move();
				if (guard.Point.X < 0 || guard.Point.X > max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
					break;
				// Console.WriteLine();
				// foreach (var s in map.Render((p,x) => seen.Contains(p) ? 'x' : x))
				// 	Console.WriteLine(s);
			}

			var n = seen.Count();

			return n;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (min, max) = map.MinMax();

			var guard0 = Pose.From(map.AllPointsWhere(x => x == '^').Single(), Direction.Up);

			var n = map.AllPointsWhere(x => x == '.').Count(WouldLoop);

			bool WouldLoop(Point obstacle)
			{
				try
				{
					map[obstacle] = '#';
					var seen = new HashSet<Pose>();
					var guard = guard0.Copy();

					while (true)
					{
						if (seen.Contains(guard))
							return true;
						seen.Add(guard);
						while (map[guard.PeekAhead] == '#')
							guard.TurnRight();
						guard.Move();
						if (guard.Point.X < 0 || guard.Point.X >= max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
							return false;
						// Console.WriteLine();
						// foreach (var s in map.Render((p,x) => seen.Contains(p) ? 'x' : x))
						// 	Console.WriteLine(s);
					}
				}
				finally
				{
					map[obstacle] = '.';
				}
			}

			return n;
		}
	}
}
