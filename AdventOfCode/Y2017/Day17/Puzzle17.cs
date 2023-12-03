using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2017.Day17
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Spinlock";
		public override int Year => 2017;
		public override int Day => 17;

		public override void Run()
		{
			Run("input").Part1(926).Part2(10150888);
		}

		protected override int Part1(string[] input)
		{
			var stepsize = int.Parse(input[0]);
			var N = 2017;

			// Simply build up the circular buffer, one number at a time
			var buffer = new List<int>();
			buffer.Add(0);
			var pos = 0;
			for (var i = 1; i <= N; i++)
			{
				pos = (pos + stepsize) % buffer.Count() + 1;
				buffer.Insert(pos, i);
			}

			// Fetch the value right after the last one inserted
			var val = buffer[(pos + 1) % buffer.Count];

			return val;
		}

		protected override int Part2(string[] input)
		{
			var stepsize = int.Parse(input[0]);
			var N = 50_000_000;

			// The value 0 will never move from position 0, which is the trick:
			// therefore we don't need to keep track of all the numbers at all but
			// just two things: the size of the buffer and the specific number that's
			// last placed after 0, ie at position 1.
			var pos = 0;
			var valueAtPosition1 = 0;
			for (var i = 1; i <= N; i++)
			{
				// The ordinary way to update pos would be using modulus:
				//    pos = (pos + stepsize) % i + 1;
				// However, doing modulus at every step is pretty expensive so
				// for efficiency (2.5x speedup) only do it when needed.
				pos += stepsize;
				if (pos >= i)
				{
					pos = pos % i;
				}
				pos++;

				if (pos == 1)
				{
					valueAtPosition1 = i;
				}
			}

			return valueAtPosition1;
		}
	}
}
