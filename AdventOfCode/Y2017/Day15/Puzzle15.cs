using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;

namespace AdventOfCode.Y2017.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Dueling Generators";
		public override int Year => 2017;
		public override int Day => 15;

		public void Run()
		{
			RunFor("test1", 588, 309);
			RunFor("input", 619, 290);
		}

		protected override int Part1(string[] input)
		{
			var a = input[0].RxMatch("%d").Get<uint>();
			var b = input[1].RxMatch("%d").Get<uint>();
			const ulong fa = 16807;
			const ulong fb = 48271;

			var n = 0;
			for (var pair = 0; pair < 40_000_000; pair++)
			{
				// Slightly faster to keep all vars+operations in uint except this
				a = (uint)((a * fa) % 0x7fffffffu);
				b = (uint)((b * fb) % 0x7fffffffu);

				// Check that lower 16 bits are similar
				if (((a ^ b) & 0xffffu) == 0)
				{
					n++;
				}
			}

			return n;
		}

		protected override int Part2(string[] input)
		{
			var a = input[0].RxMatch("%d").Get<uint>();
			var b = input[1].RxMatch("%d").Get<uint>();
			const ulong fa = 16807;
			const ulong fb = 48271;

			// Loop until we've seen 5M pairs. For every a-candidate we check if there's
			// a b-value ready; if so then it was a pair and we do the check; else if was
			// still a pair, but no match. The same is done for b-candidates.
			// For best performance all operations are done straight in the loop without
			// trying to avoid duplicated code and without any method-calls or enumerations
			// via linq-expressions etc.
			var n = 0;
			var aq = new Queue<uint>();
			var bq = new Queue<uint>();
			for (var pair = 0; pair < 5_000_000; )
			{
				a = (uint)((a * fa) % 0x7fffffffu);
				b = (uint)((b * fb) % 0x7fffffffu);
				if ((a & 3) == 0)
				{
					if (bq.TryDequeue(out var bval))
					{
						pair++;
						if (((a ^ bval) & 0xffffu) == 0)
						{
							n++;
						}
					}
					else
					{
						aq.Enqueue(a);
					}
				}
				if ((b & 7) == 0)
				{
					if (aq.TryDequeue(out var aval))
					{
						pair++;
						if (((aval ^ b) & 0xffffu) == 0)
						{
							n++;
						}
					}
					else
					{
						bq.Enqueue(b);
					}
				}
			}

			return n;
		}
	}
}
