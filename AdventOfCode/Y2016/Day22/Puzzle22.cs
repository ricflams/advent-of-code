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
			//RunPart2For("test1", 0);
			//RunFor("test2", 0, 0);
			RunFor("input", 993, 0);
		}

		protected override int Part1(string[] input)
		{
			var disks = GetDisks(input);

			var viables = disks
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
			var disks = GetDisks(input);

			var goal = Point.From(disks.Area().Item2.X, 0);
			var home = disks[0][0];
			disks.ConsoleWrite((p, disk) =>
			{
				if (p == goal)
					return 'G';
				if (p == Point.Origin)
					return 'H';
				if (disk.Avail >= disks[goal].Used)
					return '+';
				if (disk.Used == 0)
					return '0';
				return '.';
			});

			var goaldisk = disks[goal];
			var fitdisks = disks.AllPoints(d => d.Avail >= goaldisk.Used).ToArray();
			var fitdisk = fitdisks.First();
			var disk1 = disks[fitdisk];
			var remains = disk1.Avail - disks[goal].Used;

			Console.WriteLine();
			disks.ConsoleWrite((p, disk) =>
			{
				var moves = p.LookAround().Count(x => disks[x] != null && disks[x].Avail >= disk.Used);
				return moves switch {
					0 => '_',
					1 => '.',
					2 => 'x',
					3 => 'X',
					4 => '#'
				};
			});


			return 0;
		}

		internal class Disk
		{
			public int Used { get; set;}
			public int Avail { get; set;}
		}

		private static SparseMap<Disk> GetDisks(string[] input)
		{
			var disks = new SparseMap<Disk>();
			foreach (var line in input.SkipWhile(x => !x.StartsWith("/dev")))
			{
				// root@ebhq-gridcenter# df -h
				// Filesystem              Size  Used  Avail  Use%				
				// /dev/grid/node-x0-y2     90T   70T    20T   77%
				var (x, y, size, used) = line
					.RxMatch("/dev/grid/node-x%d-y%d %DT %DT")
					.Get<int, int, int, int>();
				disks[x][y] = new Disk
				{
					Used = used,
					Avail = size - used
				};
			}
			return disks;
		}
	}

}
