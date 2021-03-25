using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

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
			Run("test1").Part1(588).Part2(309);
			Run("input").Part1(619).Part2(290);
		}

		private const ulong N = 0x7fffffff;
		
		protected override int Part1(string[] input)
		{
			var a = input[0].RxMatch("%d").Get<ulong>();
			var b = input[1].RxMatch("%d").Get<ulong>();
			const uint fa = 16807;
			const uint fb = 48271;

			// Sequence for a is actually the Lehmer random number generator
			// The natural way of calculating a would be:
			//   a = (a*fa) % 0x7fffffff;
			// However, there's a shortcut because 2^31-1 is a Mersenne-prime; see
			// https://stackoverflow.com/questions/65909389/speeding-up-modulo-operations-in-cpython
			// https://en.wikipedia.org/wiki/Lehmer_random_number_generator
			// The correspondinc bitwise expression is about twice as fast:
			//   a *= fa
			//   a = (a & 0x7fffffff) + (a >> 31)
			//   a = (a & 0x7fffffff) + (a >> 31)

			var n = 0;
			for (var pair = 0; pair < 40_000_000; pair++)
			{
    			a *= fa;
    			a = ((a & N) + (a >> 31));
    			a = ((a & N) + (a >> 31));

    			b *= fb;
    			b = ((b & N) + (b >> 31));
    			b = ((b & N) + (b >> 31));

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
			var a = input[0].RxMatch("%d").Get<ulong>();
			var b = input[1].RxMatch("%d").Get<ulong>();
			const ulong fa = 16807;
			const ulong fb = 48271;

			// Loop until we've seen 5M pairs. First find the next a, then the
			// next b. We don't need to keep track of the number of iterations
			// and are really only concerned with "the next a/b".
			var n = 0;
			for (var i = 0; i < 5_000_000; i++)
			{
				do
				{
					a *= fa;
					a = ((a & N) + (a >> 31));
					a = ((a & N) + (a >> 31));
				} while ((a & 3) != 0);
				do
				{
					b *= fb;
					b = ((b & N) + (b >> 31));
					b = ((b & N) + (b >> 31));
				} while ((b & 7) != 0);

				if (((a ^ b) & 0xffffu) == 0)
				{
					n++;
				}
			}

			return n;
		}
	}
}
