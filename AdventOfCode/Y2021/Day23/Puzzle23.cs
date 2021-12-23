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

			//Run("input").Part1(0).Part2(0);
		}

		private static int[] EnergyPerMove = new int[] { 1, 10, 100, 1000};
		class Amphipod
        {
			public int Type;
			public Point Pos;
        }
		class State
        {
			private static readonly string Dest =
				new State
				{
					Pods = new[]
					{
						new Amphipod { Type = 0, Pos = Point.From(3, 3) },
						new Amphipod { Type = 0, Pos = Point.From(3, 4) },
						new Amphipod { Type = 1, Pos = Point.From(5, 3) },
						new Amphipod { Type = 1, Pos = Point.From(5, 4) },
						new Amphipod { Type = 2, Pos = Point.From(7, 3) },
						new Amphipod { Type = 2, Pos = Point.From(7, 4) },
						new Amphipod { Type = 3, Pos = Point.From(9, 3) },
						new Amphipod { Type = 3, Pos = Point.From(9, 4) },
					}
				}.Key;

			public Amphipod[] Pods;
			public int Energy;

			public State Copy()
            {
				return new State
				{
					Pods = Pods,
					Energy = Energy
				};
            }
			public string Key
            {
				get
                {
					var sb = new StringBuilder();
					foreach (var p in Pods.OrderBy(x => x.Type).OrderBy(x => x.Pos.X).ThenBy(x => x.Pos.Y))
                    {
						sb.Append(p);
                    }
					return sb.ToString();
				}
			}
			public bool IsDone => Key == Dest;

			public static int HallwayY = 1;
			public static int[] RoomX = new int[] { 3, 5, 7, 9 };

			public IEnumerable<State> NextMoves()
            {
				foreach (var x in RoomX)
                {
					foreach (var move in CanMove(x)) yield return move;
				}
				yield break;
            }

			private IEnumerable<State> CanMove(int roomX)
            {
				// Can move out of room
				foreach (var pod in Pods.Where(p => p.Pos.X == roomX && p.Pos.Y > HallwayY))
                {
					var ydist = HallwayY - pod.Pos.Y;
					var occupadoLeft = Pods.Any(p => p.Pos.Y == HallwayY && (p.Pos.X == pod.Pos.X || p.Pos.X == pod.Pos.X - 1));
					if (!occupadoLeft)
					{
						var moveleft = Copy();
						moveleft.Energy += (1 + ydist) * EnergyPerMove[pod.Type];
						var mover = moveleft.Pods.First(x => x.Pos == pod.Pos);
						mover.Pos = Point.From(mover.Pos.X - 1, HallwayY);
						yield return moveleft;
					}
					var occupadoRight = Pods.Any(p => p.Pos.Y == HallwayY && (p.Pos.X == pod.Pos.X || p.Pos.X == pod.Pos.X + 1));
					if (!occupadoRight)
					{
						var moveleft = Copy();
						moveleft.Energy += 2 * EnergyPerMove[pod.Type];
						var mover = moveleft.Pods.First(x => x.Pos == pod.Pos);
						mover.Pos = Point.From(mover.Pos.X + 1, HallwayY);
						yield return moveleft;
					}
				}

				// Can move into a room
				foreach (var pod in Pods.Where(p => p.Pos.X == roomX && p.Pos.Y == HallwayY))
				{
					// Is sitting outside room
					if (RoomX.Any(x => x == pod.Pos.X))
                    {
						var occ1 = Pods.FirstOrDefault(p => p.Pos.X == pod.Pos.X && p.Pos.Y == HallwayY + 1);
						var occ2 = Pods.FirstOrDefault(p => p.Pos.X == pod.Pos.X && p.Pos.Y == HallwayY + 2);
						if (occ1 == null && occ2 == null)
                        {
							var movedown = Copy();
							movedown.Energy += EnergyPerMove[pod.Type];
							var mover1 = movedown.Pods.First(x => x.Pos == pod.Pos);
							mover1.Pos = Point.From(mover1.Pos.X, HallwayY + 1);
							yield return movedown;

							var movefullydown = Copy();
							movefullydown.Energy += 2 * EnergyPerMove[pod.Type];
							var mover2 = movefullydown.Pods.First(x => x.Pos == pod.Pos);
							mover2.Pos = Point.From(mover2.Pos.X, HallwayY + 2);
							yield return movefullydown;
						}
					}
				}
			}

		}


		class SomeGraph : Graph<State> { }


		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			//var maze = new Maze(map);

			var A = map.AllPoints(ch => ch == 'A').ToArray();
			var B = map.AllPoints(ch => ch == 'B').ToArray();
			var C = map.AllPoints(ch => ch == 'C').ToArray();
			var D = map.AllPoints(ch => ch == 'D').ToArray();

			var state0 = new State
			{
				Pods = new[]
					{
						new Amphipod { Type = 0, Pos = A[0] },
						new Amphipod { Type = 0, Pos = A[1] },
						new Amphipod { Type = 1, Pos = B[0] },
						new Amphipod { Type = 1, Pos = B[1] },
						new Amphipod { Type = 2, Pos = C[0] },
						new Amphipod { Type = 2, Pos = C[1] },
						new Amphipod { Type = 3, Pos = D[0] },
						new Amphipod { Type = 3, Pos = D[1] },
					},
				Energy = 0
			};
//
		//	var graph = SomeGraph.BuildUnitGraphFromMaze(maze);

			var seen = new HashSet<string>();
			var queue = new Queue<State>();
			queue.Enqueue(state0);
			while (queue.Any())
			{
				var state = queue.Dequeue();
				if (seen.Contains(state.Key))
					continue;
				seen.Add(state.Key);

				if (state.IsDone)
				{
					return state.Energy;
				}
				foreach (var f in state.NextMoves().Where(x => !seen.Contains(x.Key)))
				{
					queue.Enqueue(f);
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
