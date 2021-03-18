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
			//RunPart2For("test1", 7);
			//RunFor("test2", 0, 0); 312, 305 too high
			RunFor("input", 993, 0);
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

			//var queue = new Queue<(Cluster,int)>();
			var queue = new SimplePriorityQueue<(Cluster,int)>();
			queue.Enqueue((cluster0, 0), 10000);

			var cseen = new HashSet<ulong>();
			var minx = 40;
			var loops = 0;
			var skipped = 0;

			while (queue.Any())
			{
				var (c, totalmoves) = queue.Dequeue();

				//c.WriteToConsole();

				if (loops++ % 1000 == 0)
				{
					Console.WriteLine($"At {loops} queue={queue.Count} skipped={skipped}");
				}

				if (c.Data.X < minx)
				{
					minx = c.Data.X;
//					Console.WriteLine($"At {minx} queue:{queue.Count} totalmoves={totalmoves}");
				}

				if (totalmoves >= minmovements)
				{
					//Console.WriteLine($"Skip");
					skipped++;
					continue;
				}
				
				if (c.Data == Point.Origin)
				{
					if (totalmoves < minmovements)
					{
						minmovements = totalmoves;
						Console.WriteLine($"Found minimum={minmovements}");
					}
					continue;
				}

				var seen = new HashSet<Point>();
				var dest = c.Data.Left;
				var nextClustersInfo = PushAway(c, c.Data, dest, seen, 0).ToArray();
				Console.WriteLine($"Found {nextClustersInfo.Count()} with steps {string.Join(" ", nextClustersInfo.Select(x=>x.Item2))}");
				// if (nextClustersInfo.Count() == 2)
				// 	nextClustersInfo = nextClustersInfo.OrderByDescending(x => x.Item2).Skip(1).ToArray();
				foreach (var xx in nextClustersInfo.OrderBy(x => x.Item2).Take(1))
				{
					xx.Item1.Data = dest;
					//if (!cseen.Contains(xx.Item1.Id))
					{
						var movs = totalmoves + xx.Item2;
						//Console.WriteLine($"  Enqueue for {movs} moves");
						queue.Enqueue((xx.Item1, movs), dest.X);
					//	cseen.Add(xx.Item1.Id);
					}
				}
			}

			return minmovements;


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

		private static IEnumerable<(Cluster,int)> PushAway(Cluster c, Point data, Point dest, HashSet<Point> seen, int moves)
		{
			var cx = c.MaybeMoveDataToDestination(data, dest);
			if (cx != null)
			{
				//Console.WriteLine($"  At step {moves+1}: could directly move {data} to {dest}");
				//cx.NumberOfMoves = c.NumberOfMoves;
				yield return (cx, moves+1);
				yield break;
			}

			seen.Add(data);
			var needed = c.Disks[data].Used;
			var neighborsWithEnoughSize = dest.LookAround().Where(p => !seen.Contains(p) && c.Disks[p] != null && c.Disks[p].Size >= needed).ToArray();
			// var vacancies = neighbors.Where(p => c.Disks[p].Avail >= used).ToArray();

	//		Console.WriteLine($"At step={c.NumberOfMoves} seen={seen.Count} examine {spot}: neighbors=[{string.Join<Point>(" ", neighbors)}] vacancies=[{string.Join<Point>(" ", vacancies)}]");

			// foreach (var v in vacancies)
			// {
			// 	var c2 = c.MaybeMoveFrom(dest, v);
			// 	if (c2 != null)
			// 	{
			// 		yield return c2;
			// 	}
			// }

			foreach (var n in neighborsWithEnoughSize)
			{
				var seen2 = new HashSet<Point>(seen);
				foreach (var (cc,mov) in PushAway(c, dest, n, seen2, moves).OrderBy(x => x.Item2).Take(1))
				{
					var c4 = cc.MaybeMoveDataToDestination(data, dest);
					if (c4 != null)
					{
						//Console.WriteLine($"  At step {c.NumberOfMoves}: could indirectly move {dest} to {n}");
						yield return (c4, mov+1);
					}
					else
						throw new Exception("Expected to be able to move it");
				}

			 	// var c2 = c.MaybeMoveDataToDestination(dest, n);
				// if (c2 != null)
				// {
				// 	Console.WriteLine($"  At step {c.NumberOfMoves}: could directly move {dest} to {n}");
				// 	yield return c2;
				// }
				// else
				// {
				// 	var seen2 = seen.ToHashSet();
				// 	foreach (var c3 in PushAway(c, dest, n, seen2))
				// 	{
				// 		var c4 = c3.MaybeMoveDataToDestination(dest, n);
				// 		if (c4 != null)
				// 		{
				// 			Console.WriteLine($"  At step {c.NumberOfMoves}: could indirectly move {dest} to {n}");
				// 			yield return c4;
				// 		}
				// 		else
				// 			throw new Exception("Expected to be able to move it");
				// 	}

				// }
			}
			// //var pick = choices.OrderBy(c => c.Moves).ThenBy(c => c.Goal.ManhattanDistanceTo(Point.Origin)).First();

			// var pick = choices.OrderBy(c => c.NumberOfMoves + c.Goal.ManhattanDistanceTo(Point.Origin)).First();
			// return pick;
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

			public Cluster MaybeMoveDataToDestination(Point data, Point dest)
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