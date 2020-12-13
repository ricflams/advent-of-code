using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day13
{
	internal class Puzzle : PuzzleRunner<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 13;

		public void Run()
		{
			RunPuzzles("test1.txt", 295, 1068781);
			RunPuzzles("test2.txt", null, 3417);
			RunPuzzles("test3.txt", null, 754018);
			RunPuzzles("test4.txt", null, 779210);
			RunPuzzles("test5.txt", null, 1261476);
			RunPuzzles("test6.txt", null, 1202161486);
			RunPuzzles("input.txt", 4135, 640856202464541);
		}

		protected override long Puzzle1(string[] input)
		{
			var t0 = int.Parse(input[0]);
			var ids = input[1].Split(",").Select(x => x == "x" ? (int?)null : int.Parse(x));

			var ids2 = ids
				.Where(x => x.HasValue)
				.Select(x => x.Value)
				.Select(id => (id, id - t0 % id))
				.OrderBy(x => x.Item2)
				.ToArray();
			var id1 = ids2
				.First();
			var result1 = id1.id * id1.Item2;

			return result1;
		}

		protected override long Puzzle2(string[] input)
		{
			var xxx = input[1].Split(",").ToArray();
			var cyclus =
				xxx.Select((x, i) =>
					{
						if (x == "x")
							return null;
						return new
						{
							Id = int.Parse(x),
							Diff = i
						};
					})
				.Where(x => x != null)
				//.OrderBy(x => x.Diff)
				.ToArray();

			//var ids = xxx.Skip(1).Select((x, i) => new
			//	{
			//		Id1 = xxx[i-1],
			//		Id2 = x,

			//	})
			//	.ToArray();

			//.Select(x => x == "x" ? (int?)null : int.Parse(x))


			Console.WriteLine();
			//Console.WriteLine(input[1]);
			//for (var i = 1; i < cyclus.Length; i++)
			//{
			//	var t = FindX(cyclus[0].Id, cyclus[i].Id, cyclus[i].Diff);
			//	var t2 = string.Join(" ", FindManyX(cyclus[0].Id, cyclus[i].Id, cyclus[i].Diff).Take(18));
			//	Console.WriteLine($"For {cyclus[i].Id} with diff={cyclus[i].Diff}: {cyclus[0].Id} at {t}: diff={t % cyclus[i].Id} +lcm={MathHelper.LeastCommonMultiple(cyclus[0].Id, cyclus[i].Id)}  t1..4={t2}");
			//}
			//Console.WriteLine(MathHelper.LeastCommonMultiple(cyclus.Select(x => (long)x.Id).ToArray()));
			//var z = FindX(221, 323, 187 - 102);

			long offset = 0;
			long cycle = cyclus[0].Id;
			for (var i = 1; i < cyclus.Length; i++)
			{
				//Console.WriteLine($"Checking {i} of {cyclus.Length - 1}: offset={offset} cycle={cycle}");
				var t0 = FindX(cycle, offset, cyclus[i].Id, cyclus[i].Diff);
				var lcm = MathHelper.LeastCommonMultiple(cycle, cyclus[i].Id);
				// now looking for to + n*lcm
				offset = t0;
				cycle = lcm;

				//var t2 = string.Join(" ", FindManyX(cyclus[0].Id, cyclus[i].Id, cyclus[i].Diff).Take(18));
				//Console.WriteLine($"For {cyclus[i].Id} with diff={cyclus[i].Diff}: {cyclus[0].Id} at {t}: diff={t % cyclus[i].Id} +lcm={MathHelper.LeastCommonMultiple(cyclus[0].Id, cyclus[i].Id)}  t1..4={t2}");
			}
			//Console.WriteLine($"offset={offset} cycle={cycle}");
			//var z = FindX(221, 323, 187 - 102);




			return offset;
		}

		private long FindX(long a, long offset, long b, int diff)
		{
			var target = b - diff;
			while (target < 0)
				target += b;
			Console.WriteLine($"   FindX: a={a} offset={offset} b={b} diff={diff} target={target}");
			var loops = 0;
			for (var x = offset; ; x += a)
			{
				loops++;
				if (x % b == target)
				{
					Console.WriteLine($"      loops={loops}");
					return x;
				}
			}
		}

		private IEnumerable<int> FindManyX(int a, int b, int diff)
		{
			for (var x = 1; ; x++)
			{
				if (b - x * a % b == diff)
					yield return x * a;
			}
		}
	}




	internal class Puzzle2 : PuzzleRunner<int,int>
	{
		public static Puzzle2 Instance = new Puzzle2();
		protected override int Year => 2020;
		protected override int Day => 13;

		public void Run()
		{
			//RunPuzzles("test1.txt", );
			//RunPuzzles("test2.txt", );
			//RunPuzzles("test3.txt", );
			RunPuzzles("input.txt", 0, 0);
		}

		protected override (int, int) Puzzle1And2(string[] input)
		{


			return (0, 0);
		}
	}
}
