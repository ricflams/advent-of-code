using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2018.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Chronal Calibration";
		public override int Year => 2018;
		public override int Day => 1;

		public void Run()
		{
			Run("test1").Part1(3).Part2(2);
			Run("input").Part1(538).Part2(77271);
		}

		protected override int Part1(string[] input)
		{
			var deltas = input.Select(int.Parse);
			var freq = deltas.Aggregate(0, (total, v) => total + v);
			return freq;
		}

		protected override int Part2(string[] input)
		{
			var deltas = input.Select(int.Parse).ToArray();

			// Loop deltas to build frequency, until one is reached we've
			// seen before. Using while(true) + for() is faster than %.
			var seen = new SimpleMemo<int>();
			var freq = 0;
			while (true)
			{
				foreach (var d in deltas)
				{
					freq += d;
					if (seen.IsSeenBefore(freq))
					{
						return freq;
					}
				}
			}
		}
	}
}
