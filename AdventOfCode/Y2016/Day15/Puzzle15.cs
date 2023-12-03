using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2016.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Timing is Everything";
		public override int Year => 2016;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(5);
			Run("input").Part1(400589).Part2(3045959);
		}

		protected override int Part1(string[] input)
		{
			return FindDropTime(input);
		}

		protected override int Part2(string[] input)
		{
			return FindDropTime(input, new Disc(7, 11, 0));
		}

		private class Disc
		{
			public Disc(int d, int n, int p) => (D, N, P) = (d, n, p);
			public int D { get; private set;} // Disc number
			public int N { get; private set;} // Number of positions (cycle)
			public int P { get; private set;} // Start position
			// What should the remainder be at t=0? It's a bit tricky:
			//   At t+0: At next step N shall be 0, meaning remainder A shall be N-1
			// This goes for all discs; at t=0 they should have N - discnumber as the
			// remainder.
			// Also the cycle starts with an offset P, so we should be looking for a
			// remainder A that is P lower, ie adding -P. To retain a positive modulus
			// we add N to that, ie -P+N.
			// The combined expression is therefore:
			//   A = N-D + -P+N = N-D + N-P
			public int A => (N-D + N-P) % N; // A is the remainder in CRT
		}

		private static int FindDropTime(string[] input, Disc extraDisc = null)
		{
			var factors = input
				.Select(line =>
				{
					// Disc #1 has 5 positions; at time=0, it is at position 4.
					// Disc #2 has 2 positions; at time=0, it is at position 1.
					var (d, n, p) = line
						.RxMatch("Disc #%d has %d positions; at time=0, it is at position %d.")
						.Get<int, int, int>();
					return new Disc(d, n, p);
				})
				.Concat(extraDisc != null ? new[] {extraDisc} : Enumerable.Empty<Disc>())
				.ToArray();

			var n = factors.Select(x => x.N).ToArray();
			var rem = factors.Select(x => x.A).ToArray();
			var result = MathHelper.SolveChineseRemainderTheorem(n, rem);

			// for (var t = Math.Max(0, result - 10); t < result + 10; t++)
			// {
			// 	if (t == result)
			// 	{
			// 		Console.WriteLine($"Release at t={t}   <---");
			// 	}
			// 	foreach (var f in factors)
			// 	{
			// 		var pos = (f.P + t) % f.N;
			// 		Console.WriteLine($"Disc {f.D} [{f.N,2}] at t={t} at pos={pos}{(t==result+f.D ? "   <--" : "")}");
			// 	}
			// 	Console.WriteLine();
			// }

			return result;
		}
	}
}
