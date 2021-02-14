using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

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
			//RunFor("test2", 0, 0);
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

			// var goal = Point.From(cluster.Disks.Area().Item2.X, 0);
			// var home = disks[0][0];
			
			cluster0.Disks.ConsoleWrite((p, disk) =>
			{
				if (p == cluster0.Goal)
					return 'G';
				if (p == Point.Origin)
					return 'H';
				if (disk.Avail >= cluster0.Disks[cluster0.Goal].Used)
					return '+';
				if (disk.Used == 0)
					return '0';
				if (disk.Used > 100)
					return '#';
				return '.';
			});

			// var goaldisk = disks[goal];

			var seen = new HashSet<string>();
			var queue = new Queue<(Cluster, int)>();
			queue.Enqueue((cluster0, 0));
			var round = 0;
			while (true)
			{
				// if what, stop
				var (cluster, steps) = queue.Dequeue();

				if (seen.Contains(cluster.Id))
					continue;
				seen.Add(cluster.Id);

				//cluster.Disks.ConsoleWrite((p, disk) =>
				//{
				//	if (p == cluster.Goal)
				//		return 'G';
				//	if (p == Point.Origin)
				//		return 'H';
				//	if (disk.Used == 0)
				//		return '_';
				//	//if (disk.Avail >= cluster.Disks[cluster.Goal].Used)
				//	//	return '+';
				//	return '.';
				//});

				if (cluster.Goal == Point.Origin)
				{
					Console.WriteLine($"#### BAM2! Found at steps={steps}");
					return steps;
				}

				if (round++ % 10000 == 0)
				{
					Console.Write($"[{steps}]");
					//Console.Clear();
					//Console.WriteLine();
					//Console.WriteLine($"Steps: {steps}");
					//cluster.Disks.ConsoleWrite((p, disk) =>
					//{
					//	var moves = p.LookAround().Count(x => disk.Used > 0 && cluster.Disks[x] != null && cluster.Disks[x].Avail >= disk.Used);
					//	return moves == 0 ? '.' : moves.ToString().First();
					//});
					//Console.Out.Flush();
					//System.Threading.Thread.Sleep(300);
				}

				//var nextsteps = new List<Cluster>();
				foreach (var (p, disk) in cluster.Disks.AllValues())
				{
					var used = disk.Used;
					if (used == 0)
						continue;
					foreach (var dest in p.LookAround().Where(m => cluster.Disks[m] != null && cluster.Disks[m].Avail >= used))
					{
						if (p == cluster.Goal)
						{
							Console.WriteLine($"Found goal at steps={steps}");
						}

						var cluster2 = cluster.Copy();
						cluster2.Disks[dest].Used += used;
						cluster2.Disks[p].Used -= used;
						if (p == cluster.Goal)
						{
							cluster2.Goal = dest;
						}
						cluster2.UpdateId(cluster0);
						if (!seen.Contains(cluster2.Id))
						{
							//Console.WriteLine($"Step {steps} can copy size={used} from {p} to {dest}");
							queue.Enqueue((cluster2, steps + 1));
						}
					}
				}
			}

			return 0;
		}

		internal class Disk
		{
			public int Size { get; set; }
			public int Used { get; set;}
			public int Avail => Size - Used;
		}

		private class Cluster
		{
			public SparseMap<Disk> Disks { get; set; }

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

				UpdateId(this);
			}

			public string Id { get; private set; }

			public void UpdateId(Cluster c0)
			{
				var sb = new StringBuilder();
				foreach (var (p, disk) in Disks.AllValues())
				{
					var diff = c0.Disks[p].Used - disk.Used;
					if (diff != 0)
					{
						sb.Append($"{p.X}-{p.Y}-{diff} ");
					}
				}
				Id = sb.ToString();
			}

			private Cluster() {}

			public Cluster Copy()
			{
				var copy = new Cluster();
				copy.Disks = new SparseMap<Disk>();
				foreach (var (p, disk) in Disks.AllValues())
				{
					copy.Disks[p] = new Disk { Size = disk.Size, Used = disk.Used };
				}
				copy.Goal = Goal;
				return copy;
			}

			public Point Goal { get; set; }
		}
	}

}
