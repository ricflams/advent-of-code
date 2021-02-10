using System;
using System.Linq;

namespace AdventOfCode.Y2016.Assembunny
{
    public class Computer
	{
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
						// tgl x toggles the instruction x away (positive means forward; negative means backward)
						//     For one-argument instructions, inc becomes dec, and all other one-argument instructions become inc.
						//     For two-argument instructions, jnz becomes cpy, and all other two-instructions become jnz.
						//     The arguments of a toggled instruction are not affected.
						//     If an attempt is made to toggle an instruction outside the program, nothing happens.
						//     If toggling produces an invalid instruction (like cpy 1 2) and an attempt is later made to execute that instruction, skip it instead.
						// out x transmits x (either an integer or the value of a register) to out
						// NEW:
						// nop do nothing
						// mul x y z copies x*y (values or registers) into register z
						"cpy" => new Ins(OpCode.Cpy, GetOp(p[1]), GetOp(p[2])),
						"inc" => new Ins(OpCode.Inc, GetOp(p[1])),
						"dec" => new Ins(OpCode.Dec, GetOp(p[1])),
						"jnz" => new Ins(OpCode.Jnz, GetOp(p[1]), GetOp(p[2])),
						"tgl" => new Ins(OpCode.Tgl, GetOp(p[1])),
						"out" => new Ins(OpCode.Out, GetOp(p[1])),
						"mul" => new Ins(OpCode.Mul, GetOp(p[1]), GetOp(p[2]), GetOp(p[3])),
						"nop" => new Ins(OpCode.Nop),
						_ => throw new Exception($"Unknown instruction {p[0]}")
					};
				})
				.ToArray();
			_ip = 0;

			(int, bool) GetOp(string op) => char.IsLetter(op.First()) ? (op.First() - 'a', true) : (int.Parse(op), false);
		}

		private enum OpCode { Cpy, Inc, Dec, Jnz, Tgl, Out, Nop, Mul };
		private class Operand
		{
			public int Value { get; set; }
			public bool IsRegister { get; set; }
		}
		private class Ins
		{
			public Ins(OpCode opc, params (int, bool)[] ops)
			{
				OpCode = opc;
				Ops = ops.Select(x => new Operand { Value = x.Item1, IsRegister = x.Item2 }).ToArray();
			}
			public OpCode OpCode { get; set; }
			public Operand[] Ops { get; set; }
		}

		private int ValueOf(Operand op) => op.IsRegister ? Regs[op.Value] : op.Value;
		
		public static void OptimizeFragment(string[] code, string[] fragment, string[] replacement)
		{
			var len = fragment.Length;
			for (var i = 0; i < code.Length - len; i++)
			{
				if (code[i..(i+len)].SequenceEqual(fragment))
				{
					for (var j = 0; j < len; j++)
					{
						code[i+j] = j < replacement.Length ? replacement[j] : "nop";
					}
					break;
				}
			}
		}

		public Func<int, bool> OnOutShouldHalt { get; set; }

		public void Run()
		{
			//var modified = new bool[_code.Length];
			_ip = 0;
			while (_ip < _code.Length)
			{
				var ins = _code[_ip];
				var ops = ins.Ops;
				switch (ins.OpCode)
				{
					case OpCode.Cpy:
						if (ops[1].IsRegister)
						{
							Regs[ops[1].Value] = ValueOf(ops[0]);
						}
						break;
					case OpCode.Inc:
						Regs[ops[0].Value]++;
						break;
					case OpCode.Dec:
						Regs[ops[0].Value]--;
						break;
					case OpCode.Jnz:
						if (ValueOf(ops[0]) != 0)
						{
							_ip += ValueOf(ops[1]) - 1;
						}
						break;
					case OpCode.Tgl:
						var ip = _ip + ValueOf(ops[0]);
						if (ip >= 0 && ip < _code.Length)
						{
							var modins = _code[ip];
							modins.OpCode = modins.OpCode switch {
								OpCode.Inc => OpCode.Dec,
								OpCode.Dec => OpCode.Inc,
								OpCode.Tgl => OpCode.Inc,
								OpCode.Jnz => OpCode.Cpy,
								OpCode.Cpy => OpCode.Jnz,
								_ => throw new Exception($"Unexpected tgl opcode {modins.OpCode}")
							};
							//modified[ip] = true;
						}
						break;
					case OpCode.Out:
						var v = ValueOf(ops[0]);
						if (OnOutShouldHalt(v))
							return;
						break;

					case OpCode.Nop:
						break;
					case OpCode.Mul:
						Regs[ops[2].Value] = ValueOf(ops[0]) * ValueOf(ops[1]);
						break;

					default:
						throw new Exception($"Unhandled opcode {ins.OpCode}");
				}
				_ip++;
			}
		}
	}
}