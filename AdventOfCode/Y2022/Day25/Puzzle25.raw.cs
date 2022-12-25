using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day25.Raw
{
	internal class Puzzle : Puzzle<string, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 25";
		public override int Year => 2022;
		public override int Day => 25;

		public void Run()
		{
			Run("test1").Part1("2=-1=0").Part2(0);
			//Run("test2").Part1(0).Part2(0);
			
			Run("input").Part1("122-2=200-0111--=200").Part2(0);
		}

		protected override string Part1(string[] input)
		{
			// =-012

			// 1=-0-2
			// 12111
			// 2=0=
			// 21
			// 2=01
			// 111
			// 20012
			// 112
			// 1=-1=
			// 1-12
			// 12
			// 1=
			// 122	


			var digits = "=-012";
			var sum = 0L;
			foreach (var s in input)		
			{
				var v = 0L;
				foreach (var ch in s)
				{
					var x = digits.IndexOf(ch)-2;
					v = v*5 + x;
				}
				//Console.WriteLine(v);
				sum += v;
			}

			var result = "";
			var exp = 1;
			while (sum > 0)
			{
				var dig = (int)( ((sum+2) % 5) - 2 );
				result = digits[dig+2] + result;
				sum = (sum -dig) / 5;// + dig*exp;
				//exp*=5;
			}
			return result;
		}



		protected override long Part2(string[] _) => 0;

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
