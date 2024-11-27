using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.ComponentModel;

namespace AdventOfCode.Y2023.Day17.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 17;

		public override void Run()
		{
			//Run("test1").Part1(102).Part2(94);
			Run("test1").Part2(94);
		//	Run("test2").Part1(0).Part2(0);
			// Run("input").Part1(674).Part2(0);
			Run("input").Part2(773);
			// 775 too high
			// 766 too low
			// 757 too low
			// 770 not right
		//	Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var queue = Quack<(Point, Direction, int, int, List<Point>)>.Create(QuackType.PriorityQueue);
			var (start, dest) = map.MinMax();

			// int Loss(Point p) => map[p.X, p.Y] - '0';
			// int Dist(Point p) => p.ManhattanDistanceTo(dest);


			queue.Put((start, Direction.Right, 1, 0, new List<Point>(){start}), 0);
			var minlosses = new Dictionary<string, int>();

			var minLoss = int.MaxValue;

			while (queue.TryGet(out var item))
			{
				var (p, dir, blocks, loss, path) = item;
//				Console.WriteLine($"p={p} dir={dir} blocks={blocks} loss={loss}");
						// var map2 = map.Copy();
						// foreach (var pp in path)
						// 	map2[pp.X, pp.Y] = '#';
						//map2.ConsoleWrite();				

				if (loss >= minLoss)
					continue;

				var key = $"{p}-{dir}-{blocks}";
				if (minlosses.TryGetValue(key, out var x))
				{
					if (x <= loss)					
						continue;
				}
				minlosses[key] = loss;

				if (p == dest)
				{
					Console.WriteLine($"Found dest at loss={loss} q={queue.Count}");
					if (loss < minLoss)
					{
						// var map2 = map.Copy();
						// foreach (var pp in path)
						// {
						// 	Console.Write(map[pp.X, pp.Y]);
						// 	map2[pp.X, pp.Y] = '#';
						// }
						// Console.WriteLine();
						// map2.ConsoleWrite();
						minLoss = loss;
					}
					continue;
				}

				MaybeMove(p, dir.TurnLeft(), 1, loss, path);
				MaybeMove(p, dir.TurnRight(), 1, loss, path);
				if (blocks < 3)
				{
					MaybeMove(p, dir, blocks+1, loss, path);
				}
			}			

			void MaybeMove(Point p, Direction d, int blocks, int loss, List<Point> path)
			{
				var next = p.Move(d);
				if (map.InRange(next))
				{
					var newpath = path;//path.Append(next).ToList();
					queue.Put((next, d, blocks, loss + map[next.X, next.Y] - '0', newpath), next.ManhattanDistanceTo(dest));
				}
			}

			return minLoss;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);

			var queue = Quack<(Point, Direction, int, int, List<Direction>)>.Create(QuackType.PriorityQueue);
			var (start, dest) = map.MinMax();

			queue.Put((start, Direction.Right, 0, 0, new List<Direction>(){Direction.Right}), int.MaxValue);
			queue.Put((start, Direction.Down, 0, 0, new List<Direction>(){Direction.Down}), int.MaxValue);
			var minlosses = new Dictionary<string, int>();

			var minLoss = int.MaxValue;

			while (queue.TryGet(out var item))
			{
				var (p, dir, blocks, loss, path) = item;
//				Console.WriteLine($"p={p} dir={dir} blocks={blocks} loss={loss}");
						// var map2 = map.Copy();
						// foreach (var pp in path)
						// 	map2[pp.X, pp.Y] = '#';
						//map2.ConsoleWrite();				

				//if (loss + p.ManhattanDistanceTo(dest) + (map[dest.X, dest.Y] - '0' - 1) > minLoss)
				if (loss + p.ManhattanDistanceTo(dest) >= minLoss)
				{
					//Console.Write("B");
					continue;
				}

				// if (blocks + p.ManhattanDistanceTo(dest) < 4 && (p.X != dest.X || p.Y != dest.Y))
				// {
				// 	Console.Write("=");
				// 	continue;
				// }

				var key = $"{p}-{dir}-{blocks}";
				if (minlosses.TryGetValue(key, out var x))
				{
					if (x <= loss)					
						continue;
				}
				minlosses[key] = loss;

				if (p == dest)
				{
					if (blocks < 4)
					{
						Console.Write("X");
						continue;
					}
					Console.WriteLine($"Found dest at loss={loss} q={queue.Count}");
					if (loss < minLoss)
					{
						// var map2 = map.Copy();
						// foreach (var pp in path)
						// {
						// 	Console.Write(map[pp.X, pp.Y]);
						// 	map2[pp.X, pp.Y] = '#';
						// }
						// Console.WriteLine();
						// map2.ConsoleWrite();
						if (loss < 800)
							Console.WriteLine(string.Join("", path.Select(x=>x.AsChar())));
						minLoss = loss;
					}
					continue;
				}

				if (blocks >= 4)
				{
					MaybeMove(p, dir.TurnLeft(), 1, loss, path);
					MaybeMove(p, dir.TurnRight(), 1, loss, path);
				}
				if (blocks < 10)
				{
					MaybeMove(p, dir, blocks+1, loss, path);
				}
			}			

			void MaybeMove(Point p, Direction d, int blocks, int loss, List<Direction> path)
			{
				var next = p.Move(d);
				if (map.InRange(next))
				{
					var newpath = path;//.Append(d).ToList();
					queue.Put((next, d, blocks, loss + map[next.X, next.Y] - '0', newpath), next.ManhattanDistanceTo(dest));
				}
			}

			return minLoss;
		}
	}
}
