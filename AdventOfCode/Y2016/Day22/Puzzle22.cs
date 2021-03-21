using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using Priority_Queue;

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
			RunPart2For("test1", 7);
			//RunFor("test2", 0, 0); 312, 305, 284 276 too high
			RunFor("input", 993, 202);
		}

		protected override int Part1(string[] input)
		{
			var cluster = new Cluster(input);

			var viables = cluster.Disks
				.AllValues()
				.Select(x => x.Item2)
				.OrderByDescending(d => d.Avail)
				.ToArray();

			var viablePairs = viables
				.Where(v => v.Used > 0)
				.Sum(disk => viables
					.Where(v => v != disk)
					.TakeWhile(v => v.Avail >= disk.Used)
					.Count()
				);

			return viablePairs;
		}

		protected override int Part2(string[] input)
		{
			var cluster0 = new Cluster(input);

			var minmovements = int.MaxValue;

			var disks = cluster0.Disks;

			var bigblocks = new HashSet<Point>(disks.AllValues(d => d.Used > 100).Select(x => x.Item1));
			var empty = disks.AllValues(d => d.Used == 0).Single().Item1;
			var distmap = new SparseMap<int>();
			var distq = new Queue<Point>();
			distq.Enqueue(empty);
			while (distq.Any())
			{
				var p = distq.Dequeue();
				var dist = distmap[p];
				var neighbors = p.LookAround().Where(n => distmap[n] == 0 && disks[n] != null && !bigblocks.Contains(n));
				foreach (var n in neighbors)
				{
					distmap[n] = dist + 1;
					distq.Enqueue(n);
				}
			}

			//distmap.ConsoleWrite((p, v) => (char)('0' + (v%10)));

				// .Where(p => p.LookAround().Any(n => disks[n] != null && disks[p].Avail >= disks[n].Used))
				// .ToArray();

			// how exactly to find the big blocks?

	//		cluster0.WriteToConsole();

			//var queue = new Queue<(Cluster,int)>();
			var queue = new SimplePriorityQueue<(Cluster,int)>();
			queue.Enqueue((cluster0, 0), 10000);

			var cseen = new HashSet<ulong>();


			while (queue.Any())
			{
				var (c, totalmoves) = queue.Dequeue();

				//c.WriteToConsole();

// 				if (loops++ % 1000 == 0)
// 				{
// 					Console.WriteLine($"At {loops} queue={queue.Count} skipped={skipped}");
// 				}

// 				if (c.Data.X < minx)
// 				{
// 					minx = c.Data.X;
// //					Console.WriteLine($"At {minx} queue:{queue.Count} totalmoves={totalmoves}");
// 				}

// 				if (totalmoves >= minmovements)
// 				{
// 					//Console.WriteLine($"Skip");
// 					skipped++;
// 					continue;
// 				}
				
				if (c.Data == Point.Origin)
				{
					if (totalmoves < minmovements)
					{
						minmovements = totalmoves;
//						Console.WriteLine($"Found minimum={minmovements}");
					}
					continue;
				}

				var seen = new HashSet<Point>();
				// seen.Add(c.Data.Right);
				// seen.Add(c.Data.Down);

				var nextClustersInfo = new List<(Cluster, int)>();
				var path = new Stack<Point>();
				var dest = c.Data.Left;
				path.Push(c.Data);
				Cluster nextcluster = null;
				var minmoves = int.MaxValue;
				Walk(distmap, c, path, dest, ref minmoves, ref nextcluster);
				distmap = null;
				nextcluster.Data = dest;
				var movs = totalmoves + minmoves;
				queue.Enqueue((nextcluster, movs), 0);

				// // var nextClustersInfo = PushAway(c, c.Data, dest, seen, 0).ToArray();
				// Console.WriteLine($"Found {nextClustersInfo.Count()} with steps {string.Join(" ", nextClustersInfo.Select(x=>x.Item2))}");
				// if (nextClustersInfo.Count() == 2)
				// 	nextClustersInfo = nextClustersInfo.OrderByDescending(x => x.Item2).Skip(1).ToArray();
				// foreach (var xx in nextClustersInfo.OrderBy(x => x.Item2).Take(1))
				// {
				// 	xx.Item1.Data = dest;
				// 	//if (!cseen.Contains(xx.Item1.Id))
				// 	{
				// 		var movs = totalmoves + xx.Item2;
				// 		//Console.WriteLine($"  Enqueue for {movs} moves");
				// 		queue.Enqueue((xx.Item1, movs), dest.X);
				// 	//	cseen.Add(xx.Item1.Id);
				// 	}
				// }
			}

			return minmovements;

			static void Walk(SparseMap<int> distmap, Cluster c, Stack<Point> path, Point dest, ref int minmoves, ref Cluster cluster)
			{
				var data = path.Peek();

				if (distmap != null && path.Count() + distmap[data] >= minmoves)
				{
					return;
				}
				if (path.Count() >= minmoves)
				{
					return;
				}

				if (c.CanMoveDataToDestination(data, dest))
				{
					// yes we can
					var c2 = c.Copy();
					foreach (var p in path)
					{
						c2.MoveDataToDestination(p, dest);
						dest = p;
					}
					var moves = path.Count();
					if (moves < minmoves)
					{
						cluster = c2;
						minmoves = moves;	
						var map = new CharMap();
						var i = 0;
						for (var x = 0; x < 34; x++)
						{
							for (var y = 0; y < 15; y++)
							{
								map[x][y] = '.';
							}
						}
						foreach (var p in path.Reverse())
						{
							map[p] = (char)('0' + (i++%10));
						}
						// Console.WriteLine($"Found new minimum at {moves} moves: {string.Join(" ", path.Reverse())}:");
						// map.ConsoleWrite();
						// Console.WriteLine();
					}
					return;
				}

				var needed = c.Disks[data].Used;
				var neighborsWithEnoughSize = dest.LookAround().Where(p => !path.Contains(p) && c.Disks[p] != null && c.Disks[p].Size >= needed).ToArray();


				path.Push(dest);
				if (distmap != null)
				{
					foreach (var n in neighborsWithEnoughSize.OrderBy(n => distmap[n]))
					{
						Walk(distmap, c, path, n, ref minmoves, ref cluster);
					}
				}
				else
				{
					foreach (var n in neighborsWithEnoughSize)
					{
						Walk(distmap, c, path, n, ref minmoves, ref cluster);
					}
				}
				path.Pop();
			}


			// var clustersSeen = new HashSet<ulong>();
			// clustersSeen.Add(cluster0.Id);
			
			// var queue = new SimplePriorityQueue<Cluster>();
			// queue.Enqueue(cluster0, cluster0.NumberOfMoves);
			// while (queue.Any())
			// {
			// 	var cluster = queue.Dequeue();
			// 	cluster.WriteToConsole();

			// 	if (cluster.Goal == Point.Origin)
			// 	{
			// 		Console.WriteLine($"BAM! Found");
			// 		return cluster.NumberOfMoves;
			// 	}

			// 	var seen = new HashSet<Point>();
			// 	//var nextClusters = PushAway(cluster, cluster.Goal, seen).Where(c => !clustersSeen.Contains(c.Id)).ToArray();
			// 	var nextClusters = PushAway(cluster, cluster.Goal, seen).ToArray();
			// 	Console.WriteLine($"Enqueue {nextClusters.Length} clusters");
			// 	foreach (var c in nextClusters)
			// 	{
			// 		//clustersSeen.Add(c.Id);
			// 		queue.Enqueue(c, c.NumberOfMoves);
			// 	}
			// }
			// throw new Exception("Not found");


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
	//		public HashSet<ulong> Movements { get; private set; }
			public int NumberOfMoves { get; set; }
			
	//		public ulong Id { get; private set; }
			public Point Data { get; set; }

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
				Data = Point.From(Disks.Area().Item2.X, 0);

	//			Id = 0;
	//			Movements = new HashSet<ulong>();
			}

			// public void MoveFrom(Cluster c0)
			// {
			// 	var ids = Disks.AllValues().Select(x => x.Item2.Used).ToArray();
			// 	Id = Hashing.KnuthHash(ids);

			// 	Moves = 
			// 	 = new HashSet<ulong>()

			// }

			private Cluster() {}

			public bool CanMoveDataToDestination(Point data, Point dest)
			{
				var needed = Disks[data].Used;
				var available = Disks[dest].Avail;
				return needed <= available;
			}

			public void MoveDataToDestination(Point data, Point dest)
			{
				if (!CanMoveDataToDestination(data, dest))
					throw new Exception();
				Disks[dest].Used += Disks[data].Used;
				Disks[data].Used = 0;
			}

			public Cluster Copy()
			{
				var copy = new Cluster();
				copy.Disks = new SparseMap<Disk>();
				foreach (var (p, disk) in Disks.AllValues())
				{
					copy.Disks[p] = new Disk { Size = disk.Size, Used = disk.Used };
				}
				return copy;
			}

			public Cluster OldMaybeMoveDataToDestination(Point data, Point dest)
			{
				var needed = Disks[data].Used;
				var available = Disks[dest].Avail;
				if (available < needed)
				{
					return null;
				}

				var copy = new Cluster();

				copy.Disks = new SparseMap<Disk>();
				foreach (var (p, disk) in Disks.AllValues())
				{
					copy.Disks[p] = new Disk { Size = disk.Size, Used = disk.Used };
				}
				copy.Disks[dest].Used += copy.Disks[data].Used;
				copy.Disks[data].Used = 0;
				// copy.Data = data == Data ? dest : Data;

	//			var ids = copy.Disks.AllValues().Select(x => x.Item2.Used).ToArray();
				// copy.Id = Hashing.KnuthHash(ids);

				// if (Movements.Contains(copy.Id))
				// {
				// 	return null;
				// }

	//			copy.Movements = Movements.ToHashSet();
//				copy.Movements.Add(Id);
				// copy.NumberOfMoves = NumberOfMoves + 1;
				return copy;
			}
			public void WriteToConsole()
			{
				Console.WriteLine();
				Disks.ConsoleWrite((p, disk) =>
				{
					if (p == Data)
						return 'G';
					if (p == Point.Origin)
						return 'H';
					if (disk.Used == 0)
						return '_';
					if (disk.Used > 100)
						return '#';
					if (disk.Avail >= Disks[Data].Used)
						return '+';
					return '.';
				});				
				foreach (var (p,d) in Disks.AllValues())
				{
					Console.WriteLine($"At {p}: [{d.Used}/{d.Size}]");
				}
			}
		}
	}

}



			// // var goal = Point.From(cluster.Disks.Area().Item2.X, 0);
			// // var home = disks[0][0];
			
			// // var (min, max) = cluster0.Disks.Area();
			// // return Enumerable.Range(min.Y, max.Y- min.Y + 1)
			// // 	.Select(y => Enumerable.Range(min.X, max.X - min.X + 1)
			// // 		.Select(x => rendering(Point.From(x, y), this[x][y]))
			// // 		.ToArray()
			// // 	)
			// // 	.Select(ch => new string(ch))
			// // 	.ToArray();





			// var c = cluster0;
			// while (c.Goal != Point.Origin)
			// {

			// 	c = MakeWay(c, c.Goal, null);
			// }


			// // var goaldisk = disks[goal];

			// var seen = new HashSet<ulong>();
			// var queue = new Queue<(Cluster, int)>();
			// queue.Enqueue((cluster0, 0));
			// var round = 0;
			// while (true)
			// {
			// 	// if what, stop
			// 	var (cluster, steps) = queue.Dequeue();

			// 	if (seen.Contains(cluster.Id))
			// 		continue;
			// 	seen.Add(cluster.Id);

			// 	//cluster.Disks.ConsoleWrite((p, disk) =>
			// 	//{
			// 	//	if (p == cluster.Goal)
			// 	//		return 'G';
			// 	//	if (p == Point.Origin)
			// 	//		return 'H';
			// 	//	if (disk.Used == 0)
			// 	//		return '_';
			// 	//	//if (disk.Avail >= cluster.Disks[cluster.Goal].Used)
			// 	//	//	return '+';
			// 	//	return '.';
			// 	//});

			// 	if (cluster.Goal == Point.Origin)
			// 	{
			// 		Console.WriteLine($"#### BAM2! Found at steps={steps}");
			// 		return steps;
			// 	}

			// 	if (round++ % 10000 == 0)
			// 	{
			// 		Console.Write($"[{steps}]");
			// 		//Console.Clear();
			// 		//Console.WriteLine();
			// 		//Console.WriteLine($"Steps: {steps}");
			// 		//cluster.Disks.ConsoleWrite((p, disk) =>
			// 		//{
			// 		//	var moves = p.LookAround().Count(x => disk.Used > 0 && cluster.Disks[x] != null && cluster.Disks[x].Avail >= disk.Used);
			// 		//	return moves == 0 ? '.' : moves.ToString().First();
			// 		//});
			// 		//Console.Out.Flush();
			// 		//System.Threading.Thread.Sleep(300);
			// 	}

			// 	//var nextsteps = new List<Cluster>();
			// 	foreach (var (p, disk) in cluster.Disks.AllValues())
			// 	{
			// 		var used = disk.Used;
			// 		if (used == 0)
			// 			continue;
			// 		foreach (var dest in p.LookAround().Where(m => cluster.Disks[m] != null && cluster.Disks[m].Avail >= used))
			// 		{
			// 			if (p == cluster.Goal)
			// 			{
			// 				Console.WriteLine($"Found goal at steps={steps}");
			// 			}

			// 			var cluster2 = cluster.MoveFrom(p, dest);
			// 			if (!seen.Contains(cluster2.Id))
			// 			{
			// 				//Console.WriteLine($"Step {steps} can copy size={used} from {p} to {dest}");
			// 				queue.Enqueue((cluster2, steps + 1));
			// 			}
			// 		}
			// 	}
			// }

			//return 0;