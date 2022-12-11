using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day11.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 11";
		public override int Year => 2022;
		public override int Day => 11;

		public void Run()
		{
			Run("test1").Part1(10605).Part2(2713310158);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(54036).Part2(13237873355);
		}

		internal class Monkey
		{
			//private readonly 
			public int Number;
			public Queue<long> Items;
			public char Operator;
			public string Factor;
			public int TestDivisor;
			public int DestIfTrue;
			public int DestIfFalse;
			public int WorryLevel;

			public Monkey(string[] lines)
			{
				Number = lines[0].RxMatch("Monkey %d").Get<int>();
				Items = new Queue<long>(lines[1].TrimStart().Split(':')[1].ToIntArray().Select(x => (long)x));
				
				(Operator, Factor) = lines[2].TrimStart().RxMatch("Operation: new = old %c %s").Get<char, string>();

				TestDivisor = lines[3].TrimStart().RxMatch("Test: divisible by %d").Get<int>();
				DestIfTrue = lines[4].TrimStart().RxMatch("If true: throw to monkey %d").Get<int>();
				DestIfFalse = lines[5].TrimStart().RxMatch("If false: throw to monkey %d").Get<int>();
			}
		}


		protected override long Part1(string[] input)
		{
			var monkeys = input
					.GroupByEmptyLine()
					.Select(lines => new Monkey(lines))
					.ToArray();

			var inspections = new int[monkeys.Length];

			for (var round = 0; round < 20; round++)
			{
				for (var i = 0; i < monkeys.Length; i++)
				{
					var m = monkeys[i];
					inspections[i] += m.Items.Count();
					while (m.Items.Any())
					{
						var worry = m.Items.Dequeue();
						var factor = m.Factor == "old" ? worry : int.Parse(m.Factor);
						var level = m.Operator switch
						{
							'+' => worry + factor,
							'*' => worry * factor,
							_ => throw new Exception()
						} / 3;
						var destmonkey = monkeys[level % m.TestDivisor == 0 ? m.DestIfTrue : m.DestIfFalse];
						destmonkey.Items.Enqueue(level);
					}
				}
			}

			var mostactive = inspections
				.OrderByDescending(x => x)
				.Take(2)
				.ToArray();
			var monkeybusiness = mostactive[0] * mostactive[1];


			return monkeybusiness;
		}

		protected override long Part2(string[] input)
		{
			var monkeys = input
					.GroupByEmptyLine()
					.Select(lines => new Monkey(lines))
					.ToArray();

			var inspections = new long[monkeys.Length];

			var gcd = MathHelper.LeastCommonMultiple(monkeys.Select(m => (long)m.TestDivisor).ToArray());
			Console.WriteLine(gcd);

			for (var round = 0; round < 10000; round++)
			{
				for (var i = 0; i < monkeys.Length; i++)
				{
					var m = monkeys[i];
					inspections[i] += m.Items.Count();
					while (m.Items.Any())
					{
						var worry = m.Items.Dequeue();
						var factor = m.Factor == "old" ? worry : long.Parse(m.Factor);

						if (m.Operator == '*')
						{
							if (worry * factor < 0)
								;
						}

						var level = m.Operator switch
						{
							'+' => worry + factor,
							'*' => worry * factor,
							_ => throw new Exception()
						};
						var destmonkey = monkeys[level % m.TestDivisor == 0 ? m.DestIfTrue : m.DestIfFalse];
						destmonkey.Items.Enqueue(level % gcd);
					}
				}
			}

			var mostactive = inspections
				.OrderByDescending(x => x)
				.Take(2)
				.ToArray();
			var monkeybusiness = mostactive[0] * mostactive[1];


			return monkeybusiness;
		}
	}
}
