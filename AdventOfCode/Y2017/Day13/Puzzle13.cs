using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2017.Day13
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Packet Scanners";
		public override int Year => 2017;
		public override int Day => 13;

		public void Run()
		{
			RunFor("test1", 24, 10);
			RunFor("input", 1476, 3937334);
		}

		protected override int Part1(string[] input)
		{
			var scanners = ReadScanners(input);

			return scanners
				.Where(s => s.Position % s.Cycle == 0)
				.Sum(s => s.Position * s.Range);
		}

		protected override int Part2(string[] input)
		{
			var scanners = ReadScanners(input);

			// Sorting by Cycle is 15% faster
			scanners = scanners.OrderBy(s => s.Cycle).ToArray();

			// Find first (lowest) delay that isn't caught
			for (var delay = 0;; delay++)
			{
				var caught = false;
				foreach (var s in scanners)
				{
					if ((delay + s.Position) % s.Cycle == 0)
					{
						caught = true;
						break;
					}
				}
				if (!caught)
				{
					return delay;
				}
			}
			throw new Exception("Delay not found");
		}

		internal class Scanner
		{
			public int Position { get; set;}
			public int Range { get; set;}
			public int Cycle { get; set;}
		}

		private static Scanner[] ReadScanners(string[] input)
		{
			return input
				.Select(line =>
				{
					var (pos, range) = line.RxMatch("%d: %d").Get<int,int>();
					return new Scanner
					{
						 Position = pos,
						 Range = range,
						 Cycle = 2 * (range - 1)
					};
				}).ToArray();
		}
	}
}
