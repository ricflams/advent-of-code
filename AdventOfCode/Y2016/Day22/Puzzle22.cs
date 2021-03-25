using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2016.Day22
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Grid Computing";
		public override int Year => 2016;
		public override int Day => 22;

		public void Run()
		{
			Run("test1").Part2(7);
			Run("input").Part1(993).Part2(202);
		}

		protected override int Part1(string[] input)
		{
			var cluster = new Cluster(input);

			// Find all discs, ordered by space available
			var allDiscs = cluster.Disks
				.AllValues()
				.OrderByDescending(d => d.Avail)
				.ToArray();

			// For all discs, count how many other discs this content would
			// fit onto. Because the discs are sorted by available space we
			// can simply count how many we can Take which is ~6x faster than
			// no sorting and checking through all.
			var viablePairs = allDiscs
				.Where(v => v.Used > 0)
				.Sum(disk => allDiscs
					.Where(v => v != disk)
					.TakeWhile(v => v.Avail >= disk.Used)
					.Count()
				);

			return viablePairs;
		}

		protected override int Part2(string[] input)
		{
			var cluster = new Cluster(input);

			// For moving the first point (and only for that, it seems) we need to
			// know the preferred direction for moving data around. There is 1 empty
			// disc and it's the only one we can move anything onto, and there are
			// a number of big blocks that can't be moved at all. With that in mind
			// we create a distance-field with shortest distance from (== to) the
			// empty spot, working around the big blocks. When examining the routes
			// this will tell us which discs are closest to the empty disc.
			var distField = new SparseMap<int>();
			var distq = new Queue<Point>();
			var bigblocks = new HashSet<Point>(cluster.Disks.All(d => d.Used > 100).Select(x => x.Item1));
			var empty = cluster.Disks.All(d => d.Used == 0).Single().Item1;
			distq.Enqueue(empty);
			while (distq.Any())
			{
				var p = distq.Dequeue();
				var dist = distField[p];
				var neighbors = p.LookAround().Where(n => distField[n] == 0 && cluster.Disks[n] != null && !bigblocks.Contains(n));
				foreach (var n in neighbors)
				{
					distField[n] = dist + 1;
					distq.Enqueue(n);
				}
			}

			// The stragegy:
			// Move the Goal to the left, one spot at a time. (That seems to produce the
			// shortest path overall). Do so by examining which of the neighbors has room
			// to spare can can move their data away, either directly or by pushing data
			// data away from their neighbors, recursively.
			// We only need the distance-field for the first move. It's costly to calculate
			// for the remaining moves and doesn't seem to make a difference so just set
			// it to null after the first move.
			var moves = 0;
			while (cluster.Goal != Point.Origin)
			{
				cluster = MoveLeft(distField, cluster);
				moves += cluster.Moves;
				distField = null;
				//cluster.WriteToConsole();
			}
			return moves;

			static Cluster MoveLeft(SparseMap<int> distField, Cluster c)
			{
				// Keep track of the paths examined in a stack, starting with
				// the Goal-point. MoveTo the Left will find the next cluster,
				// ie the one that took the fewest moves.
				var path = new Stack<Point>();
				path.Push(c.Goal);
				Cluster nextCluster = null;
				MoveTo(path, c.Goal.Left);
				return nextCluster;

				void MoveTo(Stack<Point> path, Point moveto)
				{
					// The path is all the discs that will have to be moved
					var movefrom = path.Peek();
					var moves = path.Count();

					// If can't do any better than the moves found for the best next
					// cluster then don't examine this path any further
					if (moves + (distField?[movefrom] ?? 0) >= nextCluster?.Moves)
					{
						return;
					}

					// If we can move the content then we know it will be the sortest
					// path found so far (see above) so move the entire centipede of
					// disc contents all the way back to the original goal+destination.
					if (c.CanMoveContentTo(movefrom, moveto))
					{
						// yes we can
						nextCluster = c.Copy();
						foreach (var p in path)
						{
							nextCluster.MoveContent(p, moveto);
							moveto = p;
						}
						return;
					}

					// No room on this disc. Examine all neighbors that has a disc that
					// would fit the data if it was empty. If there's a distance-field
					// (only for the first move) then visit the closest ones first; if
					// this optimization isn't done then it'll take forever.
					var sizeNeeded = c.Disks[movefrom].Used;
					var neighborsWithEnoughSize = moveto
						.LookAround()
						.Where(p => !path.Contains(p) && c.Disks?[p]?.Size >= sizeNeeded)
						.ToArray();
					path.Push(moveto);
					foreach (var n in neighborsWithEnoughSize.OrderBy(n => distField?[n] ?? 0))
					{
						MoveTo(path, n);
					}
					path.Pop();
				}
			}
		}


		internal class Disk
		{
			public int Size { get; set; }
			public int Used { get; set;}
			public int Avail => Size - Used;
		}

		private class Cluster
		{
			public SparseMap<Disk> Disks { get; private set; }
		
			public Point Goal { get; set; }
			public int Moves { get; set; }

			public Cluster(string[] input)
			{
				Disks = new SparseMap<Disk>();
				foreach (var line in input.SkipWhile(x => !x.StartsWith("/dev")))
				{
					// root@ebhq-gridcenter# df -h
					// Filesystem              Size  Used  Avail  Use%				
					// /dev/grid/node-x0-y2     90T   70T    20T   77%
					var (x, y, size, used) = line
						.RxMatch("/dev/grid/node-x%d-y%d %DT %DT")
						.Get<int, int, int, int>();
					Disks[x][y] = new Disk
					{
						Size = size,
						Used = used
					};
				}
				Goal = Point.From(Disks.Area().Item2.X, 0);
			}

			private Cluster() {}

			public bool CanMoveContentTo(Point movefrom, Point moveto)
			{
				var needed = Disks[movefrom].Used;
				var available = Disks[moveto].Avail;
				return needed <= available;
			}

			public void MoveContent(Point movefrom, Point moveto)
			{
				Disks[moveto].Used += Disks[movefrom].Used;
				Disks[movefrom].Used = 0;
				if (movefrom == Goal)
				{
					Goal = moveto; // Update Data if it was moved
				}
				Moves++;
			}

			public Cluster Copy()
			{
				var copy = new Cluster();
				copy.Disks = new SparseMap<Disk>();
				foreach (var (p, disk) in Disks.All())
				{
					copy.Disks[p] = new Disk { Size = disk.Size, Used = disk.Used };
				}
				copy.Goal = Goal;
				return copy;
			}

			public void WriteToConsole()
			{
				Console.WriteLine();
				Disks.ConsoleWrite((p, disk) =>
				{
					if (p == Goal)
						return 'G';
					if (p == Point.Origin)
						return 'H';
					if (disk.Used == 0)
						return '_';
					if (disk.Used > 100)
						return '#';
					if (disk.Avail >= Disks[Goal].Used)
						return '+';
					return '.';
				});				
				// foreach (var (p,d) in Disks.AllValues())
				// {
				// 	Console.WriteLine($"At {p}: [{d.Used}/{d.Size}]");
				// }
			}
		}
	}
}
