using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2016.Day12
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Leonardo's Monorail";
		public override int Year => 2016;
		public override int Day => 12;

		public void Run()
		{
			RunPart1For("test1", 42);
			RunFor("input", 318003, 9227657);
		}

		protected override int Part1(string[] input)
		{
			var comp = new Computer(input);
			comp.Run();
			return comp.Regs[0];
		}

		protected override int Part2(string[] input)
		{
			var comp = new Computer(input);
			comp.Regs[2] = 1;
			comp.Run();
			return comp.Regs[0];
		}

		internal class Computer
		{
			private enum OpCode { CpyReg, CpyVal, Inc, Dec, JnzReg, JnzVal };
			private class Ins
			{
				public Ins(OpCode opc, int op1, int op2)
				{
					OpCode = opc; Op1 = op1; Op2 = op2;
				}
				public OpCode OpCode { get; private set; }
				public int Op1 { get; private set; }
				public int Op2 { get; private set; }
			}
			private readonly Ins[] _code;
			private int _ip;

			public int[] Regs { get; } = new int[4];

			public Computer(string[] code)
			{
				Regs[0] = Regs[1] = Regs[2] = Regs[3] = 0;
				_code = code
					.Select(line =>
					{
						var p = line.Split(' ');
						return p[0] switch
						{
							// cpy x y copies x (either an integer or the value of a register) into register y.
							// inc x increases the value of register x by one.
							// dec x decreases the value of register x by one.
							// jnz x y jumps to an instruction y away (positive means forward; negative means backward), but only if x is not zero.
							"cpy" => char.IsLetter(p[1].First())
										? new Ins(OpCode.CpyReg, Reg(p[1]), Reg(p[2]))
										: new Ins(OpCode.CpyVal, int.Parse(p[1]), Reg(p[2])),
							"inc" => new Ins(OpCode.Inc, Reg(p[1]), 0),
							"dec" => new Ins(OpCode.Dec, Reg(p[1]), 0),
							"jnz" => char.IsLetter(p[1].First())
										? new Ins(OpCode.JnzReg, Reg(p[1]), int.Parse(p[2]))
										: new Ins(OpCode.JnzVal, int.Parse(p[1]), int.Parse(p[2])), // subtract 1 for easier jumps
							_ => throw new Exception($"Unknown instruction {p[0]}")
						};
					})
					.ToArray();
				_ip = 0;

				int Reg(string s) => s.First() - 'a';
			}

			public void Run()
			{
				while (_ip < _code.Length)
				{
					var ins = _code[_ip];
					switch (ins.OpCode)
					{
						case OpCode.CpyReg:
							Regs[ins.Op2] = Regs[ins.Op1];
							break;
						case OpCode.CpyVal:
							Regs[ins.Op2] = ins.Op1;
							break;
						case OpCode.Inc:
							Regs[ins.Op1]++;
							break;
						case OpCode.Dec:
							Regs[ins.Op1]--;
							break;
						case OpCode.JnzReg:
							if (Regs[ins.Op1] != 0)
							{
								_ip += ins.Op2 - 1;
							}
							break;
						case OpCode.JnzVal:
							if (ins.Op1 != 0)
							{
								_ip += ins.Op2 - 1;
							}
							break;
					}
					_ip++;
				}
			}
		}
	}
}
