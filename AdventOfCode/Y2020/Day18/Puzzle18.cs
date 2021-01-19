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

		public void Run()
		{
			RunFor("test1", 71, 231);
			RunFor("test2", 51, 51);
			RunFor("test3", 26, 46);
			RunFor("test4", 437, 1445);
			RunFor("test5", 12240, 669060);
			RunFor("test6", 13632, 23340);
			RunFor("input", 69490582260, 362464596624526);
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
