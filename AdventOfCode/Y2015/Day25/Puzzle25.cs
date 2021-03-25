using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Numerics;

namespace AdventOfCode.Y2015.Day25
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Let It Snow";
		public override int Year => 2015;
		public override int Day => 25;

		public void Run()
		{
			Run("input").Part1(9132360);
		}

		protected override int Part1(string[] input)
		{
			var (row, col) = input[0]
				.RxMatch("To continue, please consult the code grid in the manual.  Enter the code at row %d, column %d.")
				.Get<int, int>();

			var a0 = 20151125;
			var a = 252533;
			var n = 33554393;

			//    | 1   2   3   4   5   6  
			// ---+---+---+---+---+---+---+
			//  1 |  1   3   6  10  15  21
			//  2 |  2   5   9  14  20
			//  3 |  4   8  13  19
			//  4 |  7  12  18
			//  5 | 11  17
			//  6 | 16
			var pos = col*(col+1)/2 + row*(row-1)/2 + (row-1)*(col-1);
			var pow = (int)((a0 * BigInteger.ModPow(a, pos - 1, n)) % n);
			return pow;
		}

		protected override int Part2(string[] _) => 0;
	}
}
