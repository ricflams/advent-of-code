using System;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day06
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Trash Compactor";
		public override int Year => 2025;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(4277556).Part2(3263827);
			Run("input").Part1(5171061464548).Part2(10189959087258);
			Run("extra").Part1(5227286044585).Part2(10227753257799);
		}

		protected override long Part1(string[] input)
		{
			var allOperands = input[..^1]
				.Select(x => x.SplitSpace().Select(long.Parse).ToArray())
				.ToArray();
			var operators = input.Last().SplitSpace().ToArray();

			var sum = 0L;
			for (var i = 0; i < operators.Length; i++)
			{
				var operands = Enumerable.Range(0, allOperands.Length).Select(index => allOperands[index][i]);
				sum += operators[i] == "+"
					? operands.Sum()
					: operands.Prod();
			}

			return sum;
		}

		protected override long Part2(string[] input)
		{
			// Read into CharMatrix:
			//   123 328  51 64 
			//    45 64  387 23 
			//     6 98  215 314
			// then rotate once counter-clockwise, turn into lines, and reverse them
			//   1
			//   24
			//   356
			     
			//   369
			//   248
			//   8
			     
			//    32
			//   581
			//   175
			     
			//   623
			//   431
			//     4
			// and then parse each group into an array of numbers
			//   [1 24 356]
			//   [369 248 8]
			//   [32 581 175]
			//   [623 431 4]
			var operands = CharMatrix.FromArray(input[..^1])
				.RotateClockwise(270)
				.ToStringArray()
				.Reverse()
				.GroupByEmptyLine()
				.Select(x => x.Select(long.Parse).ToArray())
				.ToArray();

			// Operators are still just the last line split by space
			var operators = input.Last().SplitSpace().ToArray();

			var sum = 0L;
			for (var i = 0; i < operators.Length; i++)
			{
				sum += operators[i] == "+"
					? operands[i].Sum()
					: operands[i].Prod();
			}

			return sum;
		}
	}
}
