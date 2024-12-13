using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day13
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Claw Contraption";
		public override int Year => 2024;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(480);
			Run("input").Part1(28059).Part2(102255878088512);
			Run("extra").Part1(32067).Part2(92871736253789);
		}

		protected override long Part1(string[] input)
		{
			var machines = input.GroupByEmptyLine().Select(Machine.Parse).ToArray();
			var sum = machines.Sum(m => m.Tokens(0));
			return sum;
		}

		protected override long Part2(string[] input)
		{
			var machines = input.GroupByEmptyLine().Select(Machine.Parse).ToArray();
			var sum = machines.Sum(m => m.Tokens(10000000000000L));
			return sum;
		}

		internal class Machine
		{
			private BigPoint _a;
			private BigPoint _b;
			private BigPoint _prize;

			public static Machine Parse(string[] input) => new()
			{ 
				_a = new BigPoint(input[0].RxMatch("Button A: X+%d, Y+%d").Get<long, long>()),
				_b = new BigPoint(input[1].RxMatch("Button B: X+%d, Y+%d").Get<long, long>()),
				_prize = new BigPoint(input[2].RxMatch("Prize: X=%d, Y=%d").Get<long, long>())
			};

			private record BigPoint((long X, long Y) Val);			

			public long Tokens(long offset) =>
				CanSolve(offset, out var pushes) ? pushes.A * 3 + pushes.B : 0;

			public bool CanSolve(long offset, out (long A, long B) result)
			{
				var c = new BigPoint((_prize.Val.X + offset, _prize.Val.Y + offset));
				return MathHelper.SolveLinearEquation(_a.Val, _b.Val, c.Val, out result);
			}
		}
	}
}
