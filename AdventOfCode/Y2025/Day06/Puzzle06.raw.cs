using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day06.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2025;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(4277556).Part2(3263827);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(5171061464548).Part2(10189959087258);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var numbers = input
				.Take(input.Length - 1)
				.Select(x => x.SplitSpace().Select(long.Parse).ToArray())
				.ToArray();
			var ops = input.Last().SplitSpace().ToArray();
			var len = ops.Length;
			var opcount = numbers.Length;

			var sum = 0L;
			for (var i = 0; i < ops.Length; i++)
			{
				if (ops[i] == "+")
				{
					sum += Enumerable.Range(0, numbers.Length).Select(index => numbers[index][i]).Sum();
				}
				else
				{
					sum += MathHelper.Prod(Enumerable.Range(0, numbers.Length).Select(index => numbers[index][i]).ToArray());
				}
			}

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var probs = CharMatrix.FromArray(input[..^1]).RotateClockwise(270);
			var ops = input.Last().SplitSpace().ToArray();
			var lines = probs.ToStringArray();
			var operands = lines
				.GroupByEmptyLine()
				.Reverse()
				.Select(x => x.Select(long.Parse).ToArray())
				.ToArray();

			var sum = 0L;
			for (var i = 0; i < ops.Length; i++)
			{
				var opoffset = i * input.Length;
				if (ops[i] == "+")
				{
					sum += operands[i].Sum();
				}
				else
				{
					sum += MathHelper.Prod(operands[i]);
				}
			}


			//probs.ConsoleWrite();

			return sum;
		}
	}
}
