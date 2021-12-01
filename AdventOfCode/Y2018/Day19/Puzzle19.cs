using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Go With The Flow";
		public override int Year => 2018;
		public override int Day => 19;

		public void Run()
		{
			//Run("test1").Part1(6);
			//Run("input").Part1(1056).Part2(0);
			Run("input").Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var computer = new Computer(input);

			computer.Run();
			var reg0 = computer.Regs[0];

			return reg0;
		}

		protected override long Part2(string[] input)
		{
			var computer = new Computer(input);

			computer.Regs[0] = 1;

			//computer._ins[3] = new Computer.Ins { Opcode = Computer.Opcode.addr, A = 5, B = 0, C = 0 };

			computer._ins[3] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[4] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[5] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[6] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			//computer._ins[7] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[8] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[9] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[10] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer._ins[11] = new Computer.Ins { Opcode = Computer.Opcode.seti, A = 0, B = 0, C = 1 };
			computer.Run();
			var reg0 = computer.Regs[0];

			return reg0;
		}

		internal class Computer
		{
			public Ins[] _ins;
			private readonly int _ipreg;
			private int _ip;

			public readonly long[] Regs;

			public enum Opcode
			{
				addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
			}

			public Computer(string[] input)
			{
				//#ip 3
				//addi 3 16 3
				//seti 1 2 5
				_ipreg = input[0].RxMatch("#ip %d").Get<int>();
				_ins = input
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
				Regs = new long[6];
				_ip = 0;

				IpSeen = new int[_ins.Length];
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
				_ip = 0;
				while (_ip >= 0 && _ip < _ins.Length)
				{
					Step();
				}
			}

			private int[] IpSeen;

			private void Step()
			{
				IpSeen[_ip]++;

				Console.WriteLine(string.Join(" ", IpSeen.Select(x => x.ToString("D3"))));

				var ins = _ins[_ip];
				var (opcode, a, b, c) = (ins.Opcode, ins.A, ins.B, ins.C);

				//Console.Write($"ip={_ip} [{string.Join(",", _regs.Select(x=>x.ToString()))}] {opcode} {a} {b} {c}");

				Regs[_ipreg] = _ip;

				switch (ins.Opcode)
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

				_ip = (int)Regs[_ipreg];
				_ip++;

				//Console.WriteLine($" [{string.Join(",", _regs.Select(x => x.ToString()))}]");
			}
		}

	}
}
