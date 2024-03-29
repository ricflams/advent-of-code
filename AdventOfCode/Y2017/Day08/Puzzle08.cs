using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2017.Day08
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "I Heard You Like Registers";
		public override int Year => 2017;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(1);
			Run("input").Part1(5966).Part2(6347);
			Run("extra").Part1(4567).Part2(5636);
		}

		protected override int Part1(string[] input)
		{
			var (maxFinal, _) = FindMaxValues(input);
			return maxFinal;
		}

		protected override int Part2(string[] input)
		{
			var (_, maxEver) = FindMaxValues(input);
			return maxEver;
		}

		private static (int, int) FindMaxValues(string[] input)
		{
			var regs = new SafeDictionary<string, int>();
			var maxEver = int.MinValue;

			foreach (var line in input)
			{
				var (dest, op, delta, test, cond, val) = line
					.RxMatch("%s %s %d if %s %* %d")
					.Get<string, string, int, string, string, int>();
				var condition = cond switch
				{
					"<" => regs[test] < val,
					">" => regs[test] > val,
					"<=" => regs[test] <= val,
					">=" => regs[test] >= val,
					"==" => regs[test] == val,
					"!=" => regs[test] != val,
					_ => throw new Exception($"Unknown condition operand {cond}")
				};
				if (condition)
				{
					var v = regs[dest] += op switch
					{
						"inc" => delta,
						"dec" => -delta,
						_ => throw new Exception($"Unknown operation operand {op}")
					};
					if (v > maxEver)
					{
						maxEver = v;
					}
				}
			}

			var maxFinal = regs.Values.Max();
			return (maxFinal, maxEver);
		}
	}
}
