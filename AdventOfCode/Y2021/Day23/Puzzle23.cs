using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 23";
		public override int Year => 2021;
		public override int Day => 23;

		public void Run()
		{
			Run("test1").Part1(12521).Part2(0);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(0).Part2(0);
			// 14540 too low
		}

		private static int[] EnergyPerMove = new int[] { 1, 10, 100, 1000};
		class Amphipod
        {
			public int Type;
			public Point Pos;
        }
		class State
        {
			private static string[] EmptyMap =
			{
				"#############",
				"#...........#",
				"###.#.#.#.###",
				"  #.#.#.#.#  ",
				"  #########  ",
			};
			private static string[] FinalMap =
			{
				"#############",
				"#...........#",
				"###A#B#C#D###",
				"  #A#B#C#D#  ",
				"  #########  ",
			};
            private static readonly string FinalKey =
                new State
                {
                    Map = CharMap.FromArray(FinalMap)
                }.Key;

            //public Amphipod[] Pods = ;
            public CharMap Map;
			public Point[] Pods => Map.AllPoints(char.IsUpper).ToArray();
			public int Energy;

			public State Copy()
            {
				return new State
				{
					Map = Map.Copy(),
					Energy = Energy
				};
            }
			public string Key
            {
				get
                {
					var sb = new StringBuilder();
					foreach (var p in Pods.OrderBy(p => Map[p]).ThenBy(x => x.X).ThenBy(x => x.Y))
                    {
						sb.Append(p);
                    }
					return sb.ToString();
				}
			}
			public bool IsDone => Key == FinalKey;

			public static int HallwayY = 1;

			public IEnumerable<State> NextMoves()
            {
				foreach (var pod in Pods)
                {
					foreach (var move in CanMove(pod)) yield return move;
				}
            }

			private IEnumerable<State> CanMove(Point pod)
            {
				//0123456789012
				//#############
				//#...........#
				//###D#A#D#C###
				//  #C#A#B#B#
				//  #########

				State MoveTo(Point moveto)
                {
				//	Console.WriteLine($"Move {Map[pod]} from {pod} to {moveto}");
					var move = Copy();
					var type = move.Map[pod];
					move.Map[pod] = '.';
					move.Map[moveto] = type;
					move.Energy += EnergyPerMove[type - 'A'] * pod.ManhattanDistanceTo(moveto);
					return move;
				}

				// Can move out of room
				if (pod.Y == 2 && Map[pod.Up] == '.' ||
					pod.Y == 3 && Map[pod.Up] == '.' && Map[pod.Up] == '.')
				{
					for (var moveto = Point.From(pod.X - 1, HallwayY); Map[moveto] == '.'; moveto = moveto.Left)
                    {
						yield return MoveTo(moveto);
					}
					for (var moveto = Point.From(pod.X + 1, HallwayY); Map[moveto] == '.'; moveto = moveto.Right)
					{
						yield return MoveTo(moveto);
					}
				}

				// Can move into room
				if (pod.Y == HallwayY)
                {
					var destX = 3 + 2 * (Map[pod] - 'A');

					var freeHallway =
						pod.X < destX ? Enumerable.Range(pod.X + 1, destX - pod.X).All(x => Map[x][HallwayY] == '.') :
						destX < pod.X ? Enumerable.Range(destX, pod.X - 1 - destX).All(x => Map[x][HallwayY] == '.') :
						true;
					if (freeHallway)
                    {
						if (Map[destX][2] == '.')
                        {
							if (Map[destX][3] == '.')
								yield return MoveTo(Point.From(destX, 3));
							else if (Map[destX][3] == Map[pod])
								yield return MoveTo(Point.From(destX, 2));
						}
					}
				}
			}

		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);
			var state0 = new State
			{
				Map = map,
				Energy = 0
			};

			var seen = new HashSet<string>();
			var queue = new PriorityQueue<State,int>();
			queue.Enqueue(state0, state0.Energy);
			while (queue.TryDequeue(out var state, out var _))
			{
				if (seen.Contains(state.Key))
					continue;
				seen.Add(state.Key);

				if (seen.Count % 10000 == 0)
					Console.WriteLine($"{seen.Count}: {state.Energy}");

				//Console.WriteLine(state.Key);
				//state.Map.ConsoleWrite();
				//Console.WriteLine();

				if (state.IsDone)
				{
					return state.Energy;
				}
				foreach (var s in state.NextMoves().Where(x => !seen.Contains(x.Key)))
				{
					queue.Enqueue(s, s.Energy);
				}
			}

			return 0;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}


		/////////////////////////////////////////////////////////////////////////////////////////////////////

	//	internal class Thing
	//	{
	//		//private readonly 
	//		public Thing(string[] lines)
	//		{
	//		}
	//	}

	////	class SomeGraph : Graph<HashSet<uint>> { }

	//	internal void Sample(string[] input)
	//	{
	//		{
	//			var v = input.Select(int.Parse).ToArray();
	//		}
	//		{
	//			var v = input[0].ToIntArray();
	//		}
	//		{
	//			var things = input
	//				.Skip(1)
	//				.GroupByEmptyLine()
	//				.Select(lines => new Thing(lines))
	//				.ToMutableArray();
	//		}
	//		{
	//			var map = new SparseMap<int>();
	//			foreach (var s in input)
	//			{
	//				var (x1, y1, x2, y2) = s.RxMatch("%d,%d -> %d,%d").Get<int, int, int, int>();
	//			}
	//		}
	//		{
	//			var map = CharMap.FromArray(input);
	//			var maze = new Maze(map)
	//				.WithEntry(map.FirstOrDefault(c => c == '0')); // or Point.From(1, 1);
	//			var dest = Point.From(2, 3);
	//			var graph = Graph<char>.BuildUnitGraphFromMaze(maze);
	//			var steps = graph.ShortestPathDijkstra(maze.Entry, dest);
	//		}
	//		{
	//			var map = new CharMap('#');
	//			var maze = new Maze(map).WithEntry(Point.From(1, 1));
	//			var graph = SomeGraph.BuildUnitGraphFromMaze(maze);
	//			var queue = new Queue<(SomeGraph.Vertex, uint, int)>();
	//			queue.Enqueue((graph.Root, 0U, 0));
	//			while (queue.Any())
	//			{
	//				var (node, found, steps) = queue.Dequeue();
	//				if (node.Value.Contains(found))
	//					continue;
	//				node.Value.Add(found);
	//				var ch = map[node.Pos];
	//				if (char.IsDigit(ch))
	//				{

	//				}
	//				foreach (var n in node.Edges.Keys.Where(n => !n.Value.Contains(found)))
	//				{
	//					queue.Enqueue((n, found, steps + 1));
	//				}
	//			}
	//		}
	//		{
	//			var ship = new Pose(Point.Origin, Direction.Right);
	//			foreach (var line in input)
	//			{
	//				var n = int.Parse(line.Substring(1));
	//				switch (line[0])
	//				{
	//					case 'N': ship.MoveUp(n); break;
	//					case 'S': ship.MoveDown(n); break;
	//					case 'E': ship.MoveRight(n); break;
	//					case 'W': ship.MoveLeft(n); break;
	//					case 'L': ship.RotateLeft(n); break;
	//					case 'R': ship.RotateRight(n); break;
	//					case 'F': ship.Move(n); break;
	//					default:
	//						throw new Exception($"Unknown action in {line}");
	//				}
	//			}
	//			var dist = ship.Point.ManhattanDistanceTo(Point.Origin);
	//		}
	//		{
	//			var departure = int.Parse(input[0]);
	//			var id = input[1]
	//				.Replace(",x", "")
	//				.Split(",")
	//				.Select(int.Parse)
	//				.Select(id => new
	//				{
	//					Id = id,
	//					Time = id - departure % id
	//				})
	//				.OrderBy(x => x.Time)
	//				.First();
	//		}
	//		{
	//			var map = CharMatrix.FromArray(input);
	//			for (var i = 0; i < 100; i++)
	//			{
	//				map = map.Transform((ch, adjacents) =>
	//				{
	//					var n = 0;
	//					foreach (var c in adjacents)
	//					{
	//						if (c == '|' && ++n >= 3)
	//							return '|';
	//					}
	//					return ch;
	//				});
	//			}
	//		}
	//	}

	}
}
