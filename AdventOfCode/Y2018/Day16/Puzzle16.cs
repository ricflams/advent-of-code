using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day16
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Chronal Classification";
		public override int Year => 2018;
		public override int Day => 16;

		public void Run()
		{
			//Run("test1").Part1(0).Part2(0);

			Run("input").Part1(0).Part2(0);
		}

		internal class Computer
		{
			// addr (add register) stores into register C the result of adding register A and register B.
			// addi (add immediate) stores into register C the result of adding register A and value B.
			// Multiplication:

			// mulr (multiply register) stores into register C the result of multiplying register A and register B.
			// muli (multiply immediate) stores into register C the result of multiplying register A and value B.
			// Bitwise AND:

			// banr (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
			// bani (bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
			// Bitwise OR:

			// borr (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
			// bori (bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
			// Assignment:

			// setr (set register) copies the contents of register A into register C. (Input B is ignored.)
			// seti (set immediate) stores value A into register C. (Input B is ignored.)
			// Greater-than testing:

			// gtir (greater-than immediate/register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
			// gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
			// gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
			// Equality testing:

			// eqir (equal immediate/register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
			// eqri (equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
			// eqrr (equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.

			public enum Opcode
			{
				addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
			}
			public static int[] Process(int[] regs, Opcode opcode, int a, int b, int c)
			{
				regs = regs.ToArray();
				switch (opcode)
				{
					case Opcode.addr:
						regs[c] = regs[a] + regs[b];
						break;
					case Opcode.addi:
						regs[c] = regs[a] + b;
						break;
					case Opcode.mulr:
						regs[c] = regs[a] * regs[b];
						break;
					case Opcode.muli:
						regs[c] = regs[a] * b;
						break;
					case Opcode.banr:
						regs[c] = regs[a] & regs[b];
						break;
					case Opcode.bani:
						regs[c] = regs[a] & b;
						break;
					case Opcode.borr:
						regs[c] = regs[a] | regs[b];
						break;
					case Opcode.bori:
						regs[c] = regs[a] | b;
						break;
					case Opcode.setr:
						regs[c] = regs[a];
						break;
					case Opcode.seti:
						regs[c] = a;
						break;
					case Opcode.gtir:
						regs[c] = a > regs[b] ? 1 : 0;
						break;
					case Opcode.gtri:
						regs[c] = regs[a] > b ? 1 : 0;
						break;
					case Opcode.gtrr:
						regs[c] = regs[a] > regs[b] ? 1 : 0;
						break;
					case Opcode.eqir:
						regs[c] = a == regs[b] ? 1 : 0;
						break;
					case Opcode.eqri:
						regs[c] = regs[a] == b ? 1 : 0;
						break;
					case Opcode.eqrr:
						regs[c] = regs[a] == regs[b] ? 1 : 0;
						break;
					default:
						throw new Exception($"Unknown opcode {opcode}");
				}
				return regs;
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
		}

		protected override int Part1(string[] input)
		{
			var parts = input.GroupByEmptyLine().ToArray();
			var effects = parts
				.TakeWhile(p => p.Length == 3)
				.Select(p => new Effect(p))
				.ToArray();

			var n = 0;
			foreach (var e in effects)
			{
				var matches = 0;
				foreach (var opc in (Computer.Opcode[])Enum.GetValues(typeof(Computer.Opcode)))
				{
					var regs2 = Computer.Process(e.Before, opc, e.Ins[1], e.Ins[2], e.Ins[3]);
					if (regs2.SequenceEqual(e.After))
					{
						matches++;
					}
				}
				if (matches >= 3)
					n++;
			}

			// //var xx = parts.Skip(parts.Length - 5).ToArray();

			// var regs = new Dictionary<char, int>();

			return n;
		}

		protected override int Part2(string[] input)
		{





			return 0;
		}
	}
}
