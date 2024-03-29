using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day11
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Monkey in the Middle";
		public override int Year => 2022;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").Part1(10605).Part2(2713310158);
			Run("input").Part1(54036).Part2(13237873355);
			Run("extra").Part1(90294).Part2(18170818354);
		}

		protected override long Part1(string[] input)
		{
			return PlayMonkeyGame(input, 20, 3);
		}

		protected override long Part2(string[] input)
		{
			return PlayMonkeyGame(input, 10000, 1);

		}		

		private class Monkey
		{
			public Queue<int> Items;
			public char Operator;
			public int? Factor;
			public int TestDivisor;
			public int DestIfTrue;
			public int DestIfFalse;

			public Monkey(string[] lines)
			{
				Items = new Queue<int>(lines[1].TrimStart().RxMatch("Starting items: %*").Get<string>().ToIntArray());
				(Operator, var f) = lines[2].TrimStart().RxMatch("Operation: new = old %c %s").Get<char, string>();
				Factor = f == "old" ? null : int.Parse(f);
				TestDivisor = lines[3].TrimStart().RxMatch("Test: divisible by %d").Get<int>();
				DestIfTrue = lines[4].TrimStart().RxMatch("If true: throw to monkey %d").Get<int>();
				DestIfFalse = lines[5].TrimStart().RxMatch("If false: throw to monkey %d").Get<int>();
			}
		}

		private long PlayMonkeyGame(string[] input, int rounds, int relief)
		{
			var monkeys = input
				.GroupByEmptyLine()
				.Select(lines => new Monkey(lines))
				.ToArray();

			var inspections = new long[monkeys.Length];

			// Really only needed for part 2, but harmless to always do
			var lcd = (int)MathHelper.LeastCommonMultiple(monkeys.Select(m => (long)m.TestDivisor).ToArray());

			for (var round = 0; round < rounds; round++)
			{
				for (var i = 0; i < monkeys.Length; i++)
				{
					var m = monkeys[i];
					while (m.Items.Any())
					{
						inspections[i]++;
						var worry = (long)m.Items.Dequeue();
						var level = m.Operator switch
						{
							'*' => worry * (m.Factor ?? worry),
							'+' => worry + (m.Factor ?? worry),
							_ => throw new Exception()
						} / relief;
						var dest = level % m.TestDivisor == 0 ? m.DestIfTrue : m.DestIfFalse;
						monkeys[dest].Items.Enqueue((int)(level % lcd));
					}
				}
			}

			var monkeybusiness = inspections
				.OrderByDescending(x => x)
				.Take(2)
				.ToArray()
				.Prod();

			return monkeybusiness;
		}
	}
}
