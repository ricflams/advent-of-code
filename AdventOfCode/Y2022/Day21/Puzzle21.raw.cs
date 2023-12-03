using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day21.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 21";
		public override int Year => 2022;
		public override int Day => 21;

		public override void Run()
		{
			Run("test1").Part1(152).Part2(301);
			Run("input").Part1(309248622142100).Part2(3757272361782);

			// 5387037868883 too high
		}

		private record Monkey(string Name);
		private record MonkeyVal(string Name, long Value) : Monkey(Name);
		private record MonkeyOp(string Name, string Monkey1, char Op, string Monkey2) : Monkey(Name);


		protected override long Part1(string[] input)
		{
			var monkeys = input
				.Select<string, Monkey>(s =>
				{
					if (s.IsRxMatch("%s: %d", out var cap1))
					{
						var (name, val) = cap1.Get<string, int>();
						return new MonkeyVal(name, val);
					}
					if (s.IsRxMatch("%s: %s %c %s", out var cap2))
					{
						var (name, m1, op, m2) = cap2.Get<string, string, char, string>();
						return new MonkeyOp(name, m1, op, m2);
					}
					throw new Exception();
				})
				.ToDictionary(x => x.Name, x => x);
			while (monkeys.Any(m => m.Value is MonkeyOp))
			{
				var mon = monkeys.Values
					.Where(m => m is MonkeyOp)
					.Cast<MonkeyOp>()
					.First(m => monkeys[m.Monkey1] is MonkeyVal && monkeys[m.Monkey2] is MonkeyVal);
				var val1 = (monkeys[mon.Monkey1] as MonkeyVal).Value;
				var val2 = (monkeys[mon.Monkey2] as MonkeyVal).Value;
				var val = mon.Op switch
				{
					'+' => val1 + val2,
					'-' => val1 - val2,
					'*' => val1 * val2,
					'/' => val1 / val2,
					_ => throw new Exception()
				};
				monkeys[mon.Name] = new MonkeyVal(mon.Name, val);
			}

			return (monkeys["root"] as MonkeyVal).Value;
		}

		private long FindShout(string[] input)
		{
			var monkeys = input
				.Select<string, Monkey>(s =>
				{
					if (s.IsRxMatch("%s: %d", out var cap1))
					{
						var (name, val) = cap1.Get<string, int>();
						return new MonkeyVal(name, val);
					}
					if (s.IsRxMatch("%s: %s %c %s", out var cap2))
					{
						var (name, m1, op, m2) = cap2.Get<string, string, char, string>();
						return new MonkeyOp(name, m1, op, m2);
					}
					throw new Exception();
				})
				.ToDictionary(x => x.Name, x => x);

			//monkeys["humn"] = new MonkeyVal("humn", humanvalue);

			// Console.WriteLine("digraph {");
			// foreach (var m in monkeys.Values)
			// {
			// 	// if (m is MonkeyVal mv)
			// 	// {
			// 	// 	Console.WriteLine($"  \"{mv.Name}\"");
			// 	// }
			// 	if (m is MonkeyOp mo)
			// 	{
			// 		Console.WriteLine($"  \"{mo.Name}\" -> \"{mo.Monkey1}\"");
			// 		Console.WriteLine($"  \"{mo.Name}\" -> \"{mo.Monkey2}\"");
			// 	}
			// }
			// Console.WriteLine("}");

			monkeys["humn"] = new MonkeyOp("humn", null, '\0', null);

			Reduce("root");

			Console.WriteLine("digraph {");
			foreach (var m in monkeys.Values)
			{
				// if (m is MonkeyVal mv)
				// {
				// 	Console.WriteLine($"  \"{mv.Name}\"");
				// }
				if (m is MonkeyOp mo)
				{
					Console.WriteLine($"  \"{mo.Name}\" -> \"{mo.Monkey1}\"");
					Console.WriteLine($"  \"{mo.Name}\" -> \"{mo.Monkey2}\"");
				}
			}
			Console.WriteLine("}");


			void Reduce(string name)
			{
				if (name == "humn")
					return;
				if (monkeys[name] is MonkeyOp mon)
				{
					Reduce(mon.Monkey1);
					Reduce(mon.Monkey2);
					if (monkeys[mon.Monkey1] is MonkeyVal mv1 && monkeys[mon.Monkey2] is MonkeyVal mv2)
					{
						var val1 = (monkeys[mon.Monkey1] as MonkeyVal).Value;
						var val2 = (monkeys[mon.Monkey2] as MonkeyVal).Value;
						var val = mon.Op switch
						{
							'+' => val1 + val2,
							'-' => val1 - val2,
							'*' => val1 * val2,
							'/' => val1 / val2,
							_ => throw new Exception()
						};
						monkeys[mon.Name] = new MonkeyVal(mon.Name, val);
					}
				}
			}

			var mon = monkeys["root"] as MonkeyOp;
			var seek = monkeys[mon.Monkey1] is MonkeyVal ? (monkeys[mon.Monkey1] as MonkeyVal).Value : (monkeys[mon.Monkey2] as MonkeyVal).Value;
			mon = monkeys[mon.Monkey1] is MonkeyOp ? monkeys[mon.Monkey1] as MonkeyOp : monkeys[mon.Monkey2] as MonkeyOp;

			while (true)
			{
				if (mon.Name == "humn")
				{
					// done
					return seek;
				}

				var mon1 = monkeys[mon.Monkey1];
				var mon2 = monkeys[mon.Monkey2];
				if (mon1 is MonkeyVal)
				{
					var v = (mon1 as MonkeyVal).Value;
					var m = mon2 as MonkeyOp;
					// v op x == seek
					var seek2 = mon.Op switch
					{
						'+' => seek - v,  // v+x=seek  <=>  x=seek-v
						'-' => v - seek,  // v-x=seek  <=>  x=v-seek
						'*' => seek / v,  // v*x=seek  <=>  x=seek/v
						'/' => v / seek,  // v/x=seek  <=>  x=v/seek
						_ => throw new Exception()
					};
					Console.WriteLine($"{v} {mon.Op} {seek2} = {seek}");
					seek = seek2;
					mon = m;
				}
				else if (mon2 is MonkeyVal)
				{
					// x op v == seek
					var m = mon1 as MonkeyOp;
					var v = (mon2 as MonkeyVal).Value;
					var seek2 = mon.Op switch
					{
						'+' => seek - v,  // x+v=seek  <=>  x=seek-v
						'-' => seek + v,  // x-v=seek  <=>  x=seek+v
						'*' => seek / v,  // x*v=seek  <=>  x=seek/v
						'/' => seek * v,  // x/v=seek  <=>  x=seek*v
						_ => throw new Exception()
					};
					Console.WriteLine($"{seek2} {mon.Op} {v} = {seek}");
					seek = seek2;
					mon = m;
				}
				else throw new Exception();
			}

			throw new Exception();
			//return false;
		}

		protected override long Part2(string[] input)
		{
			return FindShout(input);
		}

	}
}
