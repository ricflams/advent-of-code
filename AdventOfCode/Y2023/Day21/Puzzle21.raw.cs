using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day21.Raw
{
	internal class Puzzle : PuzzleWithParameter<(int, int), long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 21;

		public override void Run()
		{
	//		Run("test1").WithParameter((6, 5000)).Part1(16).Part2(16733044);
	//		Run("test2").Part1(0).Part2(0);
//			Run("input").WithParameter((64, 26501365)).Part1(3814).Part2(0);

			// 702322399865078 not right 3
			// 702322452867855 not right
			// 702322505870546 not right

			// 470149643712804 not right - too low
			// 8427833834362745577 not right
			// 8427931027024412911

						Run("extra").WithParameter((64, 26501365)).Part1(3764).Part2(0);
			// 470149643712804 too low
			// 13376107091282620562 too high
			// 622908990684824 not right
			// 622909049148936 not right
			// 632257949158206
		}

		protected override long Part1(string[] input)
		{
			var (steps, _) = PuzzleParameter;
			var map = CharMap.FromArray(input);

			var p0 = map.AllPoints(c => c =='S').Single();

			var gp = new HashSet<Point>
			{
				p0
			};
			for (var i = 0; i < steps; i++)
			{
				var newtiles = new HashSet<Point>();
				foreach (var p in gp.ToArray())
				{
					foreach (var dir in p.LookAround().Where(x => map[x] != '#'))
					{
						newtiles.Add(dir);
					}
				}
				gp = newtiles;
			}

			return gp.Count;
		}

		protected override long Part2(string[] input)
		{
			var (_, steps) = PuzzleParameter;




			var map = CharMap.FromArray(input);

			var p0 = map.AllPoints(c => c == 'S').Single();

			var gp = new HashSet<Point>
			{
				p0
			};
			var lastdiff = 0;
			var (w, h) = map.Size();


			var sum = 0UL;
			var stepcycle = steps+10;
			var diff = 0L;
			var diffs = new long[] { -133, 275, -138, -463, 139, 202, 136, -137, 278, -145, 2 };
			var diffdiffs = new int[] { -2, 4, -2, -7, 2, 3, 2, -2, 4, -2, 0 };

			var diff2seen = new List<long>();

			for (var step = 1; step <= steps && step < stepcycle+10; step++)
			{
				var newtiles = new HashSet<Point>();
				foreach (var p in gp.ToArray())
				{
					foreach (var dir in p
						.LookAround()
						.Select(p => (Preal: p, PMap: Point.From((p.X % w + w) % w, (p.Y % h + h) % h)))
						.Where(x => map[x.PMap] != '#')
						.Select(x => x.Preal)
						)
					{
						newtiles.Add(dir);
					}
				}
				var diff1 = newtiles.Count() - gp.Count();
				var diffdiff = diff1 - lastdiff;
				lastdiff = diff1;
				gp = newtiles;

				//if (i >= steps - 10)
				Console.WriteLine($"{step}:{gp.Count}:{diff1} {diffdiff} ");

				diff2seen.Add(diffdiff);
				for (var j = 2; j < 300; j++)
				{
					var ii = step-1 - j;
					if (ii < 0)
						break;
					if (diff2seen[ii] == diffdiff)
					{
						//Console.WriteLine($"{step}: #### seen {diff2} at delta {j}: {ii} {step} ... {step + j} ?");
						var iii = step-1 - 2 * j;
						if (iii < 0 || diff2seen[iii] != diffdiff)
							continue;
						Console.WriteLine($"{step}: ######## seen {diffdiff} at delta {j}: {iii} {ii} {step} ... {step + j} ?");
						var iiii = step - 1 - 3 * j;
						if (iiii < 0 || diff2seen[iiii] != diffdiff)
							continue;
						Console.WriteLine($"{step}: ++++++++++++++this is it seen {diffdiff} at delta {j}: {iii} {ii} {step} ... {step + j} ?");
						diffs = diff2seen[^j..].ToArray();
						diffdiffs = Enumerable.Range(0, j)
							.Select(jj => (int)(diff2seen[step-1 - jj] - diff2seen[step-1 - jj - j]))
							.Reverse()
							.ToArray();
						sum = (ulong)gp.Count;
						diff = lastdiff;
						stepcycle = step;
					}
				}


				//if (diff2seen.Contains(diff2))
				//	Console.WriteLine($"{i + 1}: ######## seen {diff2} ");

			}

			//return gp.Count;

			Console.WriteLine($"diffs={string.Join(' ', diffs)}");
			Console.WriteLine($"diffdiffs={string.Join(' ', diffdiffs)}");

			Console.WriteLine();


			for (var step = stepcycle+1; step <= steps; )
			{
				for (var j = 0; j < diffs.Length; j++)
				{
					diffs[j] += diffdiffs[j];
					diff += diffs[j];
					sum += (ulong)diff;
					if (step < stepcycle + 10 || step > steps-10)
						Console.WriteLine($"{step}:{sum}:{diff} {diffdiffs[j]}");
					if (++step > steps)
						break;
				}


			}
			Console.WriteLine(sum);
			return (long)sum;
		}
	}
}
