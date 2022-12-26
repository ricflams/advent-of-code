using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Monkey Math";
		public override int Year => 2022;
		public override int Day => 21;

		public void Run()
		{
			Run("test1").Part1(152).Part2(301);
			Run("test9").Part1(194058098264286).Part2(3592056845086);
			Run("input").Part1(309248622142100).Part2(3757272361782);
		}

		protected override long Part1(string[] input)
		{
			var troop = new MonkeyTroop(input);
			var monkeys = troop.Monkeys;

			while (true)
			{
				var mon = monkeys.Values
					.Where(m => m is MonkeyOp)
					.Cast<MonkeyOp>()
					.FirstOrDefault(m => monkeys[m.Monkey1] is MonkeyVal && monkeys[m.Monkey2] is MonkeyVal);
				if (mon == null)
					break;
				troop.ReduceOperation(mon);
			}

			return (monkeys["root"] as MonkeyVal).Value;
		}

		protected override long Part2(string[] input)
		{
			var troop = new MonkeyTroop(input);
			var monkeys = troop.Monkeys;
			
			// Modify humn to be an operation with no dependencies
			monkeys["humn"] = new MonkeyOp("humn", null, (char)0, null);

			// Recursively/repeatedly reduce all monkeys that operate on 2 values
			Reduce("root");
			void Reduce(string name)
			{
				if (name == "humn")
					return;
				if (monkeys[name] is MonkeyOp m)
				{
					Reduce(m.Monkey1);
					Reduce(m.Monkey2);
					if (monkeys[m.Monkey1] is MonkeyVal mv1 && monkeys[m.Monkey2] is MonkeyVal mv2)
					{
						troop.ReduceOperation(m);
					}
				}
			}

			// Prepare to look for the sought-after value (seek) in the tree of
			// monkey-operations. The value/op can be on either side of the root.
			var root = monkeys["root"] as MonkeyOp;
			var leftIsVal = monkeys[root.Monkey1] is MonkeyVal;
			var (a, b) = leftIsVal ? (root.Monkey1, root.Monkey2) : (root.Monkey2, root.Monkey1);
			var seek = (monkeys[a] as MonkeyVal).Value;
			var mon = monkeys[b] as MonkeyOp;

			while (true)
			{
				// We're done when we eventually reach humn
				if (mon.Name == "humn")
					return seek; // We're done

				var m1 = monkeys[mon.Monkey1];
				var m2 = monkeys[mon.Monkey2];
				if (m1 is MonkeyVal)
				{
					var v = (m1 as MonkeyVal).Value;
					var m = m2 as MonkeyOp;
					// v op x == seek
					var seek2 = mon.Op switch
					{
						'+' => seek - v,  // v+x=seek  <=>  x=seek-v
						'-' => v - seek,  // v-x=seek  <=>  x=v-seek
						'*' => seek / v,  // v*x=seek  <=>  x=seek/v
						'/' => v / seek,  // v/x=seek  <=>  x=v/seek
						_ => throw new Exception()
					};
					seek = seek2;
					mon = m;
				}
				else if (m2 is MonkeyVal)
				{
					// x op v == seek
					var m = m1 as MonkeyOp;
					var v = (m2 as MonkeyVal).Value;
					var seek2 = mon.Op switch
					{
						'+' => seek - v,  // x+v=seek  <=>  x=seek-v
						'-' => seek + v,  // x-v=seek  <=>  x=seek+v
						'*' => seek / v,  // x*v=seek  <=>  x=seek/v
						'/' => seek * v,  // x/v=seek  <=>  x=seek*v
						_ => throw new Exception()
					};
					seek = seek2;
					mon = m;
				}
				else throw new Exception("Unexpected monkey-states");
			}
			throw new Exception("No value found");
		}

		private record Monkey(string Name);
		private record MonkeyVal(string Name, long Value) : Monkey(Name);
		private record MonkeyOp(string Name, string Monkey1, char Op, string Monkey2) : Monkey(Name);

		private class MonkeyTroop
		{
			public readonly IDictionary<string, Monkey> Monkeys;

			public MonkeyTroop(string[] input)
			{
				Monkeys = input
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
			}

			public void ReduceOperation(MonkeyOp m)
			{
				var val1 = (Monkeys[m.Monkey1] as MonkeyVal).Value;
				var val2 = (Monkeys[m.Monkey2] as MonkeyVal).Value;
				var val = m.Op switch
				{
					'+' => val1 + val2,
					'-' => val1 - val2,
					'*' => val1 * val2,
					'/' => val1 / val2,
					_ => throw new Exception()
				};
				Monkeys[m.Name] = new MonkeyVal(m.Name, val);
			}			
		}
	}
}
