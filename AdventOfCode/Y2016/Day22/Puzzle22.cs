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
			//RunFor("test2", 0, 0);
			//RunFor("input", 993, 0);
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

			var clustersSeen = new HashSet<ulong>();
			clustersSeen.Add(cluster0.Id);
			
			var queue = new SimplePriorityQueue<Cluster>();
			queue.Enqueue(cluster0, cluster0.NumberOfMoves);
			while (queue.Any())
			{
				var cluster = queue.Dequeue();
				cluster.WriteToConsole();

				if (cluster.Goal == Point.Origin)
				{
					Console.WriteLine($"BAM! Found");
					return cluster.NumberOfMoves;
				}

				var seen = new HashSet<Point>();
				//var nextClusters = PushAway(cluster, cluster.Goal, seen).Where(c => !clustersSeen.Contains(c.Id)).ToArray();
				var nextClusters = PushAway(cluster, cluster.Goal, seen).ToArray();
				Console.WriteLine($"Enqueue {nextClusters.Length} clusters");
				foreach (var c in nextClusters)
				{
					//clustersSeen.Add(c.Id);
					queue.Enqueue(c, c.NumberOfMoves);
				}
			}
			throw new Exception("Not found");


		}

		private static IEnumerable<Cluster> PushAway(Cluster c, Point spot, HashSet<Point> seen)
		{
			var used = c.Disks[spot].Used;
			var neighbors = spot.LookAround().Where(p => !seen.Contains(p) && c.Disks[p] != null && c.Disks[p].Size >= used).ToArray();
			var vacancies = neighbors.Where(p => c.Disks[p].Avail >= used).ToArray();

	//		Console.WriteLine($"At step={c.NumberOfMoves} seen={seen.Count} examine {spot}: neighbors=[{string.Join<Point>(" ", neighbors)}] vacancies=[{string.Join<Point>(" ", vacancies)}]");

			// foreach (var v in vacancies)
			// {
			// 	var c2 = c.MoveFrom(spot, v);
			// 	if (c2 != null)
			// 	{
			// 		yield return c2;
			// 	}
			// }
			seen.Add(spot);

			foreach (var n in neighbors)
			{
			 	var c2 = c.MaybeMoveFrom(spot, n);
				if (c2 != null)
				{
					Console.WriteLine($"  Could move {spot} to {n}");
					yield return c2;
				}
				else
				{
					var seen2 = seen.ToHashSet();
					foreach (var c3 in PushAway(c, n, seen2))
					{
						var c4 = c3.MaybeMoveFrom(spot, n);
						if (c4 != null)
						{
							Console.WriteLine($"  And could move {spot} to {n}");
							yield return c4;
						}
						else
							throw new Exception("Expected to be able to move it");
					}

				}
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
			public HashSet<ulong> Movements { get; private set; }
			public int NumberOfMoves => Movements.Count();
			
			public ulong Id { get; private set; }
			public Point Goal { get; private set; }

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

				Id = 0;
				Movements = new HashSet<ulong>();
			}

			// public void MoveFrom(Cluster c0)
			// {
			// 	var ids = Disks.AllValues().Select(x => x.Item2.Used).ToArray();
			// 	Id = Hashing.KnuthHash(ids);

			// 	Moves = 
			// 	 = new HashSet<ulong>()

			// }

			private Cluster() {}

			public Cluster MaybeMoveFrom(Point pos, Point dest)
			{
				if (Disks[pos].Used > Disks[dest].Avail)
				{
					return null;
				}

				var copy = new Cluster();

				copy.Disks = new SparseMap<Disk>();
				foreach (var (p, disk) in Disks.AllValues())
				{
					copy.Disks[p] = new Disk { Size = disk.Size, Used = disk.Used };
				}
				copy.Disks[dest].Used += copy.Disks[pos].Used;
				copy.Disks[pos].Used = 0;
				copy.Goal = pos == Goal ? dest : Goal;

				var ids = copy.Disks.AllValues().Select(x => x.Item2.Used).ToArray();
				copy.Id = Hashing.KnuthHash(ids);

				if (Movements.Contains(copy.Id))
				{
					return null;
				}

				copy.Movements = Movements.ToHashSet();
				copy.Movements.Add(Id);
				return copy;
			}

			public void WriteToConsole()
			{
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