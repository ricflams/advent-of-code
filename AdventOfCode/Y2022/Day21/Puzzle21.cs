using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 21";
		public override int Year => 2022;
		public override int Day => 21;

		public void Run()
		{
			Run("test1").Part1(152).Part2(301);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(309248622142100).Part2(0);
		}

		private record Monkey(string Name);
		private record MonkeyVal(string Name, long Value) : Monkey(Name);
		private record MonkeyOp(string Name, string Monkey1, char Op, string Monkey2) : Monkey(Name);


		protected override long Part1(string[] input)
		{
			var monkeys = input
				.Select<string, Monkey>(s =>
				{
					if (s.IsRxMatch("%s: %d", out var cap1))
					{
						var (name, val) = cap1.Get<string, int>();
						return new MonkeyVal(name, val);
					}
					if (s.IsRxMatch("%s: %s %c %s", out var cap2))
					{
						var (name, m1, op, m2) = cap2.Get<string, string, char, string>();
						return new MonkeyOp(name, m1, op, m2);
					}
					throw new Exception();
				})
				.ToDictionary(x => x.Name, x => x);
			while (monkeys.Any(m => m.Value is MonkeyOp))
			{
				var mon = monkeys.Values
					.Where(m => m is MonkeyOp)
					.Cast<MonkeyOp>()
					.First(m => monkeys[m.Monkey1] is MonkeyVal && monkeys[m.Monkey2] is MonkeyVal);
				var val1 = (monkeys[mon.Monkey1] as MonkeyVal).Value;
				var val2 = (monkeys[mon.Monkey2] as MonkeyVal).Value;
				var val = mon.Op switch
				{
					'+' => val1 + val2,
					'-' => val1 - val2,
					'*' => val1 * val2,
					'/' => val1 / val2,
					_ => throw new Exception()
				};
				monkeys[mon.Name] = new MonkeyVal(mon.Name, val);
			}

			return (monkeys["root"] as MonkeyVal).Value;
		}

		private bool Shout(string[] input, int humanvalue)
		{
			var monkeys = input
				.Select<string, Monkey>(s =>
				{
					if (s.IsRxMatch("%s: %d", out var cap1))
					{
						var (name, val) = cap1.Get<string, int>();
						return new MonkeyVal(name, val);
					}
					if (s.IsRxMatch("%s: %s %c %s", out var cap2))
					{
						var (name, m1, op, m2) = cap2.Get<string, string, char, string>();
						return new MonkeyOp(name, m1, op, m2);
					}
					throw new Exception();
				})
				.ToDictionary(x => x.Name, x => x);

			monkeys["humn"] = new MonkeyVal("humn", humanvalue);

			while (monkeys.Any(m => m.Value is MonkeyOp))
			{
				var mon = monkeys.Values
					.Where(m => m is MonkeyOp)
					.Cast<MonkeyOp>()
					.First(m => monkeys[m.Monkey1] is MonkeyVal && monkeys[m.Monkey2] is MonkeyVal);
				var val1 = (monkeys[mon.Monkey1] as MonkeyVal).Value;
				var val2 = (monkeys[mon.Monkey2] as MonkeyVal).Value;
				var val = mon.Op switch
				{
					'+' => val1 + val2,
					'-' => val1 - val2,
					'*' => val1 * val2,
					'/' => val1 / val2,
					_ => throw new Exception()
				};
				monkeys[mon.Name] = new MonkeyVal(mon.Name, val);
				if (mon.Name == "root")
					return val1 == val2;
			}
			throw new Exception();
			//return false;
		}

		protected override long Part2(string[] input)
		{
			for (var v = 0; v < 10000000; v++)
			{
				if (Shout(input, v))
					return v;
			}
			return 0;
		}


		/////////////////////////////////////////////////////////////////////////////////////////////////////

		internal class Thing
		{
			//private readonly 
			public Thing(string[] lines)
			{
			}
		}

		class SomeGraph : Graph<HashSet<uint>> { }

		internal void Sample(string[] input)
		{
			{
				var v = input.Select(int.Parse).ToArray();
			}
			{
				var v = input[0].ToIntArray();
			}
			{
				var things = input
					.Skip(1)
					.GroupByEmptyLine()
					.Select(lines => new Thing(lines))
					.ToMutableArray();
			}
			{
				var map = new SparseMap<int>();
				foreach (var s in input)
				{
					var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();
				}
			}
			{
				var map = CharMap.FromArray(input);
				var maze = new Maze(map)
					.WithEntry(map.FirstOrDefault(c => c == '0')); // or Point.From(1, 1);
				var dest = Point.From(2, 3);
				var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
				var steps = graph.ShortestPathDijkstra(maze.Entry, dest);
			}
			{
				var map = new CharMap('#');
				var maze = new Maze(map).WithEntry(Point.From(1, 1));
				var graph = SomeGraph.BuildUnitGraphFromMaze(maze);
				var queue = new Queue<(SomeGraph.Vertex, uint, int)>();
				queue.Enqueue((graph.Root, 0U, 0));
				while (queue.Any())
				{
					var (node, found, steps) = queue.Dequeue();
					if (node.Value.Contains(found))
						continue;
					node.Value.Add(found);
					var ch = map[node.Pos];
					if (char.IsDigit(ch))
					{

					}
					foreach (var n in node.Edges.Keys.Where(n => !n.Value.Contains(found)))
					{
						queue.Enqueue((n, found, steps + 1));
					}
				}
			}
			{
				var ship = new Pose(Point.Origin, Direction.Right);
				foreach (var line in input)
				{
					var n = int.Parse(line.Substring(1));
					switch (line[0])
					{
						case 'N': ship.MoveUp(n); break;
						case 'S': ship.MoveDown(n); break;
						case 'E': ship.MoveRight(n); break;
						case 'W': ship.MoveLeft(n); break;
						case 'L': ship.RotateLeft(n); break;
						case 'R': ship.RotateRight(n); break;
						case 'F': ship.Move(n); break;
						default:
							throw new Exception($"Unknown action in {line}");
					}
				}
				var dist = ship.Point.ManhattanDistanceTo(Point.Origin);
			}
			{
				var departure = int.Parse(input[0]);
				var id = input[1]
					.Replace(",x", "")
					.Split(",")
					.Select(int.Parse)
					.Select(id => new
					{
						Id = id,
						Time = id - departure % id
					})
					.OrderBy(x => x.Time)
					.First();
			}
			{
				var map = CharMatrix.FromArray(input);
				for (var i = 0; i < 100; i++)
				{
					map = map.Transform((ch, adjacents) =>
					{
						var n = 0;
						foreach (var c in adjacents)
						{
							if (c == '|' && ++n >= 3)
								return '|';
						}
						return ch;
					});
				}
			}
		}

	}
}
