using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2018.Day19
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Go With The Flow";
		public override int Year => 2018;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(6);
			Run("input").Part1(1056).Part2(10915260);
		}

		protected override int Part1(string[] input)
		{
			var computer = new Computer(input);

			computer.Run();
			var reg0 = computer.Regs[0];

			return reg0;
		}

		protected override int Part2(string[] input)
		{
			var computer = new Computer(input);

			// The input program first does some init and then jump back to
			// the main loop-code in line 1-16, annotated as pseuode-code:
			//  0 addi 3 16 3  goto 17
			//  1 seti 1 2 5     f = 1
			//  2 seti 1 3 2     c = 1
			//  3 mulr 5 2 1     b = c * f
			//  4 eqrr 1 4 1     b = b==e?1:0
			//  5 addr 1 3 3     ip += b       # goto 7 if b==e
			//  6 addi 3 1 3     ip++          # else goto 8
			//  7 addr 5 0 0     a += f
			//  8 addi 2 1 2     c++
			//  9 gtrr 2 4 1     b = c>e?1:0
			// 10 addr 3 1 3     ip += b       # goto 12 if c>e
			// 11 seti 2 5 3     ip = 2        # else goto 3
			// 12 addi 5 1 5     f++
			// 13 gtrr 5 4 1     b = f>e?1:0
			// 14 addr 1 3 3     ip += b       # goto 16 if f>e
			// 15 seti 1 2 3     ip = 2        # else goto 2
			// 16 mulr 3 3 3     ip = 256      # exit
			// 17 addi 4 2 4 init stuff
			//    mulr 4 4 4  more etc ...
			//    .....
			//    seti 0 7 3 goto 1
			//
			// The pseudo-code can be rewritten as:
			//   f = 1
			//   do
			//       do
			//           c = 1
			//           if (c * f == e)
			//               a += f
			//           c++
			//       while c <= e
			//       f++
			//   while f <= e
			//
			// which can be rewritten as
			//   for (f = 1; f <= e; f++)
			//       for (c = 1; c <= e; c++)
			//           if (c * f == e)
			//               a += f
			//
			// which can be reduced to
			//   for (f = 1; f <= e; f++)
			//       if (e % f == 0)
			//           a += f
			//
			// So we will simply run the init-code and then do the above as
			// real code. Modify instriction 1 to set ip to something invalid
			// so it will bail after the init-code.

			computer.Regs[0] = 1;
			computer.Instructions[1] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 999, B = 0, C = computer.IpRegister };
			computer.Run();

			var a = computer.Regs[0];
			var e = computer.Regs[4];

			for (var f = 1; f <= e; f++)
			{
				if (e % f == 0)
				{
					a += f;
				}
			}

			return a;
		}

		internal class Computer
		{
			public enum Opcode
			{
				addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
			}

			public readonly int[] Regs = new int[6];
			public readonly int IpRegister;
			public readonly Ins[] Instructions;

			public Computer(string[] input)
			{
				// Example:
				// #ip 3
				// addi 3 16 3
				// seti 1 2 5
				IpRegister = input[0].RxMatch("#ip %d").Get<int>();
				Instructions = input
					.Skip(1)
					.Select(s =>
					{
						var (opcode, a, b, c) = s.RxMatch("%s %d %d %d").Get<string, int, int, int>();
						return new Ins
						{
							Opcode = Enum.Parse<Opcode>(opcode),
							A = a,
							B = b,
							C = c
						};
					})
					.ToArray();
			}

			public class Ins
			{
				public Opcode Opcode { get; init; }
				public int A { get; init; }
				public int B { get; init; }
				public int C { get; init; }
				public override string ToString() => $"{Opcode} {A} {B} {C}";
			}

			public void Run()
			{
				for (var ip = 0; ip >= 0 && ip < Instructions.Length; ip++)
				{
					var ins = Instructions[ip];
					var (opcode, a, b, c) = (ins.Opcode, ins.A, ins.B, ins.C);

					Regs[IpRegister] = ip;

					switch (opcode)
					{
						case Opcode.addr: Regs[c] = Regs[a] + Regs[b]; break;
						case Opcode.addi: Regs[c] = Regs[a] + b; break;
						case Opcode.mulr: Regs[c] = Regs[a] * Regs[b]; break;
						case Opcode.muli: Regs[c] = Regs[a] * b; break;
						case Opcode.banr: Regs[c] = Regs[a] & Regs[b]; break;
						case Opcode.bani: Regs[c] = Regs[a] & b; break;
						case Opcode.borr: Regs[c] = Regs[a] | Regs[b]; break;
						case Opcode.bori: Regs[c] = Regs[a] | b; break;
						case Opcode.setr: Regs[c] = Regs[a]; break;
						case Opcode.seti: Regs[c] = a; break;
						case Opcode.gtir: Regs[c] = a > Regs[b] ? 1 : 0; break;
						case Opcode.gtri: Regs[c] = Regs[a] > b ? 1 : 0; break;
						case Opcode.gtrr: Regs[c] = Regs[a] > Regs[b] ? 1 : 0; break;
						case Opcode.eqir: Regs[c] = a == Regs[b] ? 1 : 0; break;
						case Opcode.eqri: Regs[c] = Regs[a] == b ? 1 : 0; break;
						case Opcode.eqrr: Regs[c] = Regs[a] == Regs[b] ? 1 : 0; break;
						default:
							throw new Exception($"Unknown opcode {opcode}");
					}

					ip = Regs[IpRegister];
				}
			}
		}
	}
}
