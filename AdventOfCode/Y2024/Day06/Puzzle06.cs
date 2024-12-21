using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Runtime.Serialization;

namespace AdventOfCode.Y2024.Day06
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Guard Gallivant";
		public override int Year => 2024;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(41).Part2(6);
			Run("input").Part1(4890).Part2(1995);
			Run("extra").Part1(5067).Part2(1793);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var guard = Pose.From(map.AllPointsWhere(x => x == '^').Single(), Direction.Up);
			map[guard.Point] = '.';
			var n = MapRoute(map, guard).Count();

			return n;
		}

		private static HashSet<Point> MapRoute(CharMap map, Pose guard0)
		{
			var (_, max) = map.MinMax();
			var seen = new HashSet<Point>();

			var guard = guard0.Copy();
			while (true)
			{
				seen.Add(guard.Point);
				while (map[guard.PeekAhead] == '#')
					guard.TurnRight();
				guard.Move();
				if (guard.Point.X < 0 || guard.Point.X > max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
					break;
			}

			return seen;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (min, max) = map.MinMax();

			var guard = Pose.From(map.AllPointsWhere(x => x == '^').Single(), Direction.Up);
			map[guard.Point] = '.';
			var guardorig = guard.Copy();



			// //var loop1995 = map.AllPoints(x => x == '.').Where(WouldLoop1).ToArray();

			// bool WouldLoop1(Point obstacle)
			// {
			// 	map[obstacle] = '#';
			// 	var seen = new HashSet<Pose>();
			// 	try
			// 	{
			// 		var g = guard.Copy();

			// 		while (true)
			// 		{
			// 			if (seen.Contains(g))
			// 			{
			// 				return true;
			// 			}
			// 			seen.Add(g.Copy());
			// 			while (map[g.PeekAhead] == '#')
			// 				g.TurnRight();
			// 			g.Move();
			// 			if (g.Point.X < 0 || g.Point.X >= max.X || g.Point.Y < 0 || g.Point.Y > max.Y)
			// 				return false;
			// 			// Console.WriteLine();
			// 			// foreach (var s in map.Render((p,x) => seen.Contains(p) ? 'x' : x))
			// 			// 	Console.WriteLine(s);
			// 		}
			// 	}
			// 	finally
			// 	{
			// 		map[obstacle] = '.';
			// 		Console.WriteLine($"WouldLoop 1, made {seen.Count} steps");
			// 		foreach (var s in map.Render((p,ch) => seen.Where(s => s.Point == p).Select(x => x.Direction.AsChar()).FirstOrDefault(ch)))
			// 			Console.WriteLine(s);					
			// 	}
			// }
			// //Console.WriteLine("loops: " + loop1995.Count());



			// var suspose = Pose.From(Point.From(42,108), Direction.Up);
			// var sus = Point.From(42, 107);
			// var wouldloopnot = WouldLoop1(Point.Origin);
			// var wouldloop1 = WouldLoop1(sus);
			// var wouldloop = WouldLoop(suspose, sus);

			// Console.WriteLine($"Original positions");
			// map[suspose.Point] = '^';
			// map[sus] = 'O';
			// map[guardorig.Point] = '&';
			// map.ConsoleWrite();


			var loops = 0;
			var blocks = new HashSet<Point>();
			while (true)
			{
				while (map[guard.PeekAhead] == '#')
					guard.TurnRight();
				if (!blocks.Contains(guard.PeekAhead) && WouldLoop(guard, guard.PeekAhead))
				{
					// if (!loop1995.Contains(guard.PeekAhead))
					// 	;
					loops++;
				}
				blocks.Add(guard.Point);
				guard.Move();
				if (guard.Point.X < 0 || guard.Point.X > max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
					break;
			}


			var n = loops;

			bool WouldLoop(Pose guard0, Point obstacle)
			{
				if (obstacle.X < 0 || obstacle.X > max.X || obstacle.Y < 0 || obstacle.Y > max.Y)
					return false;
				Debug.Assert(map[obstacle] == '.');
				try
				{
					map[obstacle] = '#';
					var seen = new HashSet<Pose>();
					var guard = guard0.Copy();

					while (true)
					{
						if (seen.Contains(guard))
						{
							// Console.WriteLine($"WouldLoop, made {seen.Count} steps");
							// foreach (var s in map.Render((p,ch) => seen.Where(s => s.Point == p).Select(x => x.Direction.AsChar()).FirstOrDefault(ch)))
							// 	Console.WriteLine(s);							
							return true;
						}
						seen.Add(guard.Copy());
						while (map[guard.PeekAhead] == '#')
						{
							guard.TurnRight();
							seen.Add(guard.Copy());
						}
						guard.Move();
						if (guard.Point.X < 0 || guard.Point.X > max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
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

			// var map = CharMap.FromArray(input);
			// var (min, max) = map.MinMax();

			// var guard0 = Pose.From(map.AllPoints(x => x == '^').Single(), Direction.Up);

			// var (seen, route) = MapRoute(map);

			// map[guard0.Point] = '.';

			// var loops = new List<List<Pose>>();
			// var obstacleHits = 0;
			// var allLoopPoses = new HashSet<Pose>();

			// var obstacles = seen.Where(WouldLoop).ToArray();

			// var outsiders = seen.Where(o => !seen.Contains(o)).ToArray();

			// var loopgroups = loops.GroupBy(x => x.Count());
			// var nloopgroups = loopgroups.Count();

			// var n = obstacles.Count();

			// // foreach (var s in map.Render((p,x) => seen.Contains(p) ? '*' : obstacles.Contains(p) ? 'O' : x))
			// // 	Console.WriteLine(s);



			// bool WouldLoop(Point obstacle)
			// {
			// 	var obstaclehit = 0;
			// 	var route = new List<Pose>();
			// 	try
			// 	{
			// 		map[obstacle] = '#';
			// 		var seen = new HashSet<Pose>();
			// 		var guard = guard0.Copy();

			// 		while (true)
			// 		{
			// 			if (obstaclehit == 1 && allLoopPoses.Contains(guard))
			// 				return true;
			// 			if (seen.Contains(guard))
			// 			{
			// 				while (route.First() != guard)
			// 					route.RemoveAt(0);
			// 				loops.Add(route);
			// 				return true;
			// 			}
			// 			seen.Add(guard.Copy());
			// 			route.Add(guard.Copy());
			// 			while (map[guard.PeekAhead] == '#')
			// 			{
			// 				if (guard.PeekAhead == obstacle)
			// 					obstaclehit++;
			// 				guard.TurnRight();
			// 			}
			// 			guard.Move();
			// 			if (guard.Point.X < 0 || guard.Point.X > max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
			// 				return false;
			// 			// Console.WriteLine();
			// 			// foreach (var s in map.Render((p,x) => seen.Contains(p) ? 'x' : x))
			// 			// 	Console.WriteLine(s);
			// 		}
			// 	}
			// 	finally
			// 	{
			// 		map[obstacle] = '.';
			// 		if (obstaclehit > 1)
			// 		{
			// 			foreach (var x in route)
			// 				allLoopPoses.Add(x);
			// 			obstacleHits++;
			// 		}
			// 	}
			// }

			// return n;



			// var map = CharMap.FromArray(input);
			// var (min, max) = map.MinMax();

			// var guard0 = Pose.From(map.AllPoints(x => x == '^').Single(), Direction.Up);
			// map[guard0.Point] = '.';

			// var loops = 0;
			// var seen0 = new HashSet<Pose>();
			// while (true)
			// {
			// 	seen0.Add(guard0);
			// 	while (map[guard0.PeekAhead] == '#')
			// 		guard0.TurnRight();

			// 	if (WouldLoop(guard0.PeekAhead))
			// 		loops++;

			// 	guard0.Move();
			// 	if (guard0.Point.X < 0 || guard0.Point.X > max.X || guard0.Point.Y < 0 || guard0.Point.Y > max.Y)
			// 		break;

			// }			


			// // Console.WriteLine(map.AllPoints(x => x == '.').Count());
			// // var seenp = MapRoute(map);
			// // var blockers = seenp.Where(p => p != guard0.Point).Where(WouldLoop).ToArray();
			// // var n = blockers.Count();


			// // Console.WriteLine();
			// // foreach (var s in map.Render((p,x) => blockers.Contains(p) ? 'O' : seenp.Contains(p) ? 'x' : x))
			// // 	Console.WriteLine(s);


			// bool WouldLoop(Point obstacle)
			// {
			// 	if (map[obstacle] == '#')
			// 		return false;
			// 	try
			// 	{
			// 		map[obstacle] = '#';
			// 		var seen = new HashSet<Pose>();
			// 		var guard = guard0.Copy();

			// 		while (true)
			// 		{
			// 			if (seen.Contains(guard))
			// 				return true;
			// 			seen.Add(guard);
			// 			while (map[guard.PeekAhead] == '#')
			// 				guard.TurnRight();
			// 			guard.Move();
			// 			if (guard.Point.X < 0 || guard.Point.X >= max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
			// 				return false;
			// 			// Console.WriteLine();
			// 			// foreach (var s in map.Render((p,x) => seen.Contains(p) ? 'x' : x))
			// 			// 	Console.WriteLine(s);
			// 		}
			// 	}
			// 	finally
			// 	{
			// 		map[obstacle] = '.';
			// 	}
			// }

			// bool WouldLoop2()
			// {
			// 	var obshit = 0;
			// 	var obstacle = guard0.PeekAhead;

			// 	if (map[obstacle] == '#')
			// 		return false;

			// 	try
			// 	{
			// 		map[obstacle] = '#';
			// 		var seen = new HashSet<Pose>();
			// 		var guard = guard0.Copy();

			// 		while (true)
			// 		{
			// 			while (map[guard.PeekAhead] == '#')
			// 			{
			// 				if (guard.PeekAhead == obstacle)
			// 					obshit++;
			// 				guard.TurnRight();
			// 			}

			// 			if (seen.Contains(guard))// || seen0.Contains(guard))
			// 			{
			// 				//Console.Write(obshit == 1 ? '.' : 'O');
			// 				return true;
			// 			}
			// 			seen.Add(guard);

			// 			guard.Move();

			// 			if (guard.Point.X < 0 || guard.Point.X > max.X || guard.Point.Y < 0 || guard.Point.Y > max.Y)
			// 				return false;
			// 			// Console.WriteLine();
			// 			// foreach (var s in map.Render((p,x) => seen.Contains(p) ? 'x' : x))
			// 			// 	Console.WriteLine(s);
			// 		}
			// 	}
			// 	finally
			// 	{
			// 		map[obstacle] = '.';
			// 	}
			// }

			// return loops;
		}
	}
}
