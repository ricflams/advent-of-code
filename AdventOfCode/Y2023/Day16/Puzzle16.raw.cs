using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace AdventOfCode.Y2023.Day16.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 16;

		public override void Run()
		{
			Run("test1").Part1(46);//.Part2(51);
		//	Run("test2").Part1(0).Part2(0);
			Run("input").Part1(7562);//.Part2(7793);
			Run("extra").Part1(6514).Part2(8089);
		}

		
		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			var p0 = Pose.From(0, 0, Direction.Right);
			return Energize(map, p0, []).Energy;
		}

		private static (int Energy, HashSet<Pose> Seen) Energize(CharMap map, Pose p0, Dictionary<Pose, int> totals)
		{
			var seen = new HashSet<Pose>();

			var queue = new Queue<Pose>();
			queue.Enqueue(p0.Copy());
			var (w, h) = map.Size();
			while (queue.TryDequeue(out var p))
			{
				// if (p.Point.X < 0)
				// 	p.MoveRight(w);
				// else if (p.Point.X == w)
				// 	p.MoveLeft(w);
				// else if (p.Point.Y < 0)
				// 	p.MoveDown(h);
				// else if (p.Point.Y == h)
				// 	p.MoveUp(h);

				if (p.Point.X < 0 || p.Point.X == w || p.Point.Y < 0 || p.Point.Y == h)
					continue;

				// if (map[p.Point] == '.' && totals.TryGetValue(p, out var size))
				// 	return (size, null);

				// var ch = map[p.Point];
				// if (ch == '.'
				// 	 || (ch=='-' && (p.Direction is Direction.Left or Direction.Right))
				// 	 || (ch=='|' && (p.Direction is Direction.Up or Direction.Down)))
				// 	 {
				// 		var back = p.Copy(); back.TurnAround();
				// 		if (seen.Contains(back))
				// 		{
				// 			continue;
				// 		}
				// 	 }

				if (seen.Contains(p))
					continue;
				seen.Add(p.Copy());

				if (map[p.Point] == '-')
				{
					if (p.Direction is Direction.Up or Direction.Down)
					{
						// split
//						var pRev = p.Copy(); pRev.TurnAround(); seen.Add(pRev);
						var p1 = p.Copy(); p1.TurnLeft(); p1.Move();
						var p2 = p.Copy(); p2.TurnRight(); p2.Move();
						//Console.Write('-');
						queue.Enqueue(p1);
						queue.Enqueue(p2);
					}
					else
					{
						p.Move();
						queue.Enqueue(p.Copy());						
					}
				}
				else if (map[p.Point] == '|')
				{
					if (p.Direction is Direction.Right or Direction.Left)
					{
						// split
//						var pRev = p.Copy(); pRev.TurnAround(); seen.Add(pRev);
						var p1 = p.Copy(); p1.TurnLeft(); p1.Move();
						var p2 = p.Copy(); p2.TurnRight(); p2.Move();
						//Console.Write('|');
						queue.Enqueue(p1);
						queue.Enqueue(p2);
					}
					else
					{
						p.Move();
						queue.Enqueue(p.Copy());						
					}
				}
				else if (map[p.Point] == '/')
				{
					if (p.Direction is Direction.Up or Direction.Down)
						p.TurnRight();
					else
						p.TurnLeft();
					p.Move();
					queue.Enqueue(p.Copy());
				}
				else if (map[p.Point] == '\\')
				{
					if (p.Direction is Direction.Up or Direction.Down)
						p.TurnLeft();
					else
						p.TurnRight();
					p.Move();
					queue.Enqueue(p.Copy());
				}
				else
				{
					p.Move();
					queue.Enqueue(p.Copy());
				}
			}

			var e = seen.Select(x => x.Point).Distinct().Count();
			//Console.WriteLine();

			return (e, seen);			
		}


		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);
			var (w, h) = map.Size();

			var starts = new HashSet<Pose>(Enumerable.Empty<Pose>()
				.Concat(Enumerable.Range(0, w).Select(x => Pose.From(x, 0, Direction.Down)))
				.Concat(Enumerable.Range(0, w).Select(x => Pose.From(x, h-1, Direction.Up)))
				.Concat(Enumerable.Range(0, h).Select(y => Pose.From(0, y, Direction.Right)))
				.Concat(Enumerable.Range(0, h).Select(y => Pose.From(w-1, y, Direction.Left)))
				.ToArray());

			var seen = new HashSet<Pose>();
			var totals = new Dictionary<Pose, int>();

			var maxE = 0;
			while (starts.Any())
			{
				var p = starts.First();
				starts.Remove(p);

				// if (seen.Contains(p))
				// {
				// 	Console.Write('.');
				// 	continue;
				// }
				var (e, pseen) = Energize(map, p, totals);
			//	Console.WriteLine($"{p}  {e} {(pseen==null?"seen":"")}");
				if (e > maxE)
					maxE = e;
				if (pseen != null)
				{
					foreach (var p2 in pseen)
					{
						if (!totals.TryGetValue(p2, out var t) || t < e)
							totals[p2] = e;
					}
				}
				// foreach (var pp in starts.Intersect(pseen))
				// {
				// 	Console.Write('+');
				// 	seen.Add(pp.Copy());
				// }
			}

//			var maxE = starts.Select(p => Energize(map, p).Energy).Max();
			// for (var x = 0; x < w; x++)
			// {
			// 	var e1 = Energize(map, Pose.From(x, 0, Direction.Down));
			// 	var e2 = Energize(map, Pose.From(x, h-1, Direction.Up));
			// 	if (e1 > maxE)
			// 		maxE = e1;
			// 	if (e2 > maxE)
			// 		maxE = e2;
			// }
			// for (var y = 0; y < h; y++)
			// {
			// 	var e1 = Energize(map, Pose.From(0, y, Direction.Right));
			// 	var e2 = Energize(map, Pose.From(w-1, y, Direction.Left));
			// 	if (e1 > maxE)
			// 		maxE = e1;
			// 	if (e2 > maxE)
			// 		maxE = e2;
			// }
			return maxE;
		}
	}
}
