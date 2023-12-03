using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2020.Day18
{
	internal class Puzzle : Puzzle<ulong, ulong>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Operation Order";
		public override int Year => 2020;
		public override int Day => 18;

		public override void Run()
		{
			Run("test1").Part1(71).Part2(231);
			Run("test2").Part1(51).Part2(51);
			Run("test3").Part1(26).Part2(46);
			Run("test4").Part1(437).Part2(1445);
			Run("test5").Part1(12240).Part2(669060);
			Run("test6").Part1(13632).Part2(23340);
			Run("input").Part1(69490582260).Part2(362464596624526);
		}

		protected override ulong Part1(string[] input)
		{
			return input.Select(line =>
			{
				var pos = 0;
				return EvalWithoutPrecedence(line.TrimAll(), ref pos);
			}).Sum();
		}

		protected override ulong Part2(string[] input)
		{
			return input.Select(line =>
			{
				var pos = 0;
				return EvalWithPrecedence(line.TrimAll(), ref pos);
			}).Sum();
		}

		private static ulong EvalWithoutPrecedence(string expr, ref int pos)
		{
			// Examples:
			// 1 + (2 * 3) + (4 * (5 + 6))
			// ((9 * 2 + 4) * (6 * 2 * 2 * 2 + 2) + 5 * 3 + 2) + 7 * 7 + 9
			// 6 + 5 * 7 * 9
			// (9 * 7 * (2 * 4 + 7 * 7 + 6) * 5) + 7
			ulong sum = 0;
			char op = default;
			while (pos < expr.Length)
			{
				var ch = expr[pos++];
				if (ch == ')')
				{
					return sum;
				}
				if (ch == '+' || ch == '*')
				{
					op = ch;
					continue;
				}
				var v = ch == '('
					? EvalWithoutPrecedence(expr, ref pos)
					: (ulong)(ch - '0');
				sum = sum == 0
					? v
					: op == '+' ? v + sum : v * sum;
			}
			return sum;
		}

		private static ulong EvalWithPrecedence(string expr, ref int pos)
		{
			ulong product = 1;
			char op = default;
			ulong? pending = null;
			while (pos < expr.Length)
			{
				var ch = expr[pos++];
				if (ch == ')')
				{
					break;
				}
				if (ch == '+' || ch == '*')
				{
					op = ch;
					continue;
				}
				var v = ch == '('
					? EvalWithPrecedence(expr, ref pos)
					: (ulong)(ch - '0');
				if (pending.HasValue)
				{
					if (op == '+')
					{
						pending += v;
					}
					else
					{
						product *= pending.Value;
						pending = v;
					}
				}
				else
				{
					pending = v;
				}
			}
			if (pending.HasValue)
			{
				product *= pending.Value;
			}
			return product;
		}
	}
}
