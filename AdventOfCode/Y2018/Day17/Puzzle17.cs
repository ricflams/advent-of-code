using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day17
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Reservoir Research";
		public override int Year => 2018;
		public override int Day => 17;

		public void Run()
		{
			Run("test1").Part1(57);
			//Run("input").Part1(607).Part2(577);
		}

		protected override int Part1(string[] input)
		{
			//var (effects, _) = ReadInput(input);

			//// Count the number of effects that satisfies 3 or more opcodes
			//var n = effects
			//	.Select(e => Computer.Opcodes.Count(opc => e.SatisfiesOpcode(opc)))
			//	.Count(x => x >= 3);

			//return n;
			return 0;
		}

		protected override int Part2(string[] input)
		{
			return 0;
			//var (effects, instructions) = ReadInput(input);

			//// For every opcode, find the effects that is satisified by that opcode and pick
			//// out their first instruction-value. Those are the possible numeric values that
			//// the opcode can be, so make a mapping from opcode to those candidates
			//var opcodeCandidates = Computer.Opcodes
			//	.ToDictionary(x => x, x => effects.Where(e => e.SatisfiesOpcode(x)).Select(e => e.Ins[0]).ToHashSet());

			//// Turns out that there is (by puzzle design) always an opcode that has
			//// just one single numeric value candidate; when adding that mapping and
			//// removing that option, there's another (or more) with just one candicate,
			//// all the way through the very last opcode. That's our mapping.
			//var intToOpcode = new Dictionary<int, Computer.Opcode>();
			//while (opcodeCandidates.Any())
			//{
			//	foreach (var next in opcodeCandidates.Where(x => x.Value.Count() == 1))
			//	{
			//		var opc = next.Key;
			//		var val = next.Value.Single();
			//		opcodeCandidates.Remove(opc);
			//		intToOpcode[val] = opc;
			//		foreach (var x in opcodeCandidates)
			//		{
			//			x.Value.Remove(val);
			//		}
			//	}
			//}

			//// Execute the instructions based on the int-to-opcode mapping
			//var regs = new int[4];
			//foreach (var ins in instructions)
			//{
			//	var v = ins.ToIntArray();
			//	var opc = intToOpcode[v[0]];
			//	Computer.Process(regs, opc, v[1], v[2], v[3]);
			//}

			//// Result is the value of register 0
			//var register0 = regs[0];
			//return register0;
		}

		private static (Effect[], string[]) ReadInput(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();
			var effects = parts
				.TakeWhile(p => p.Length == 3)
				.Select(p => new Effect(p))
				.ToArray();
			var instructions = parts.Last();
			return (effects, instructions);
		}

		internal class Computer
		{
			public enum Opcode
			{
				addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
			}

			public readonly static Computer.Opcode[] Opcodes = (Computer.Opcode[])Enum.GetValues(typeof(Computer.Opcode));

			public static void Process(int[] regs, Opcode opcode, int a, int b, int c)
			{
				switch (opcode)
				{
					case Opcode.addr: regs[c] = regs[a] + regs[b]; break;
					case Opcode.addi: regs[c] = regs[a] + b; break;
					case Opcode.mulr: regs[c] = regs[a] * regs[b]; break;
					case Opcode.muli: regs[c] = regs[a] * b; break;
					case Opcode.banr: regs[c] = regs[a] & regs[b]; break;
					case Opcode.bani: regs[c] = regs[a] & b; break;
					case Opcode.borr: regs[c] = regs[a] | regs[b]; break;
					case Opcode.bori: regs[c] = regs[a] | b; break;
					case Opcode.setr: regs[c] = regs[a]; break;
					case Opcode.seti: regs[c] = a; break;
					case Opcode.gtir: regs[c] = a > regs[b] ? 1 : 0; break;
					case Opcode.gtri: regs[c] = regs[a] > b ? 1 : 0; break;
					case Opcode.gtrr: regs[c] = regs[a] > regs[b] ? 1 : 0; break;
					case Opcode.eqir: regs[c] = a == regs[b] ? 1 : 0; break;
					case Opcode.eqri: regs[c] = regs[a] == b ? 1 : 0; break;
					case Opcode.eqrr: regs[c] = regs[a] == regs[b] ? 1 : 0; break;
					default:
						throw new Exception($"Unknown opcode {opcode}");
				}
			}
		}

		internal class Effect
		{
			public int[] Before { get; }
			public int[] After { get; }
			public int[] Ins { get; }
			public Effect(string[] input)
			{
				// Before: [1, 2, 0, 3]
				// 14 0 3 2
				// After:  [1, 2, 0, 3]
				var (b1, b2, b3, b4) = input[0].RxMatch("Before: [%d, %d, %d, %d]").Get<int, int, int, int>();
				var (i1, i2, i3, i4) = input[1].RxMatch("%d %d %d %d").Get<int, int, int, int>();
				var (a1, a2, a3, a4) = input[2].RxMatch("After:  [%d, %d, %d, %d]").Get<int, int, int, int>();
				Before = new []{ b1, b2, b3, b4 };
				After = new []{ a1, a2, a3, a4 };
				Ins = new []{ i1, i2, i3, i4 };
			}

			public bool SatisfiesOpcode(Computer.Opcode opc)
			{
				var regs = Before.ToArray(); // Don't mutate Before, even though it really doesn't matter
				Computer.Process(regs, opc, Ins[1], Ins[2], Ins[3]);
				return regs.SequenceEqual(After);
			}
		}
	}
}
