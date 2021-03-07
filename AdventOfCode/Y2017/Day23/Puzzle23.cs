using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace AdventOfCode.Y2017.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Coprocessor Conflagration";
		public override int Year => 2017;
		public override int Day => 23;

		public void Run()
		{
			//RunPart2For("test1", 0);
			//RunFor("test1", 0, 0);
			//RunFor("test2", 0, 0);
			RunFor("input", 4225, 905);
		}

		protected override long Part1(string[] input)
		{
			var tablet = new Tablet(input);
			tablet.Run();
			return tablet.MulCount;
		}

		protected override long Part2(string[] input)
		{
			// var tablet = new Tablet(input);
			// tablet.Regs['a'] = 1;
			// tablet.Run();
			// return tablet.Regs['h'];


			var d = 0;
			var e = 0;
			var f = 0;
			var h = 0;
			for (var b = 100000+6700; b <= 100000+6700+17000; b += 17)
			{
				f = 1;
				d = 2;
				do
				{

					// e = 2;
					// do
					// {
					// 	if (d*e == b)
					// 	{
					// 		f = 0;
					// 	}
					// 	e++;
					// }
					// while (e != b);

					if (b%d == 0)
					{
						f = 0;
						break;
					}



					d++;
				}
				while (d != b);
				if (f == 0)
				{
					h++;
				}
			}

			return h;

		}

// 925  too high
// 904 too low


		public class Tablet
		{
			private readonly Ins[] _code;
			private int _ip;

			public SafeDictionary<char, long> Regs = new SafeDictionary<char, long>();

			public bool Halt { get; set; }

			public Tablet(string[] code)
			{
				_code = code
					.Select(line =>
					{
						var p = line.Split(' ');
						return p[0] switch
						{
							// set X Y sets register X to the value of Y.
							// sub X Y decreases register X by the value of Y.
							// mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
							// jnz X Y jumps with an offset of the value of Y, but only if the value of X is not zero. (An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)

							// // snd X plays a sound with a frequency equal to the value of X.
							// // set X Y sets register X to the value of Y.
							// // add X Y increases register X by the value of Y.
							// // mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
							// // mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y (that is, it sets X to the result of X modulo Y).
							// // rcv X recovers the frequency of the last sound played, but only when the value of X is not zero. (If it is zero, the command does nothing.)
							// // jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero. (An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
							// //
							// // Part 2:
							// // snd X sends the value of X to the other program.
							// // rcv X receives the next value and stores it in register X.
							//"snd" => new Ins(OpCode.Snd, GetOp(p[1])),
							"set" => new Ins(OpCode.Set, GetOp(p[1]), GetOp(p[2])),
							"sub" => new Ins(OpCode.Sub, GetOp(p[1]), GetOp(p[2])),
							// "add" => new Ins(OpCode.Add, GetOp(p[1]), GetOp(p[2])),
							"mul" => new Ins(OpCode.Mul, GetOp(p[1]), GetOp(p[2])),
							// "mod" => new Ins(OpCode.Mod, GetOp(p[1]), GetOp(p[2])),
							// "rcv" => new Ins(OpCode.Rcv, GetOp(p[1])),
							//"jgz" => new Ins(OpCode.Jgz, GetOp(p[1]), GetOp(p[2])),
							"jnz" => new Ins(OpCode.Jnz, GetOp(p[1]), GetOp(p[2])),
							_ => throw new Exception($"Unknown instruction {p[0]}")
						};
					})
					.ToArray();
				_ip = 0;

				(long, bool) GetOp(string op) => char.IsLetter(op.First()) ? (op.First(), true) : (long.Parse(op), false);
			}

			private enum OpCode { Set, Sub, Mul, Jnz };
			private class Operand
			{
				public long Value { get; set; }
				public bool IsRegister { get; set; }
			}
			private class Ins
			{
				public Ins(OpCode opc, params (long, bool)[] ops)
				{
					OpCode = opc;
					Ops = ops.Select(x => new Operand { Value = x.Item1, IsRegister = x.Item2 }).ToArray();
				}
				public OpCode OpCode { get; set; }
				public Operand[] Ops { get; set; }
			}

			private long ValueOf(Operand op) => op.IsRegister ? Regs[(char)op.Value] : op.Value;
			private char Reg(Operand op) => (char)op.Value;

			public long MulCount { get; private set; }

			public void Run()
			{
				MulCount = 0;
				_ip = 0;

				// Regs['a'] = 0;
				// Regs['b'] = 0;
				// Regs['c'] = 0;
				// Regs['d'] = 0;
				// Regs['e'] = 0;
				// Regs['f'] = 0;
				// Regs['g'] = 0;
				// Regs['h'] = 0;

				// var states = new HashSet<string>();



				while (_ip >= 0 && _ip < _code.Length && !Halt)
				{
					// var state = $"{_ip}:{string.Join(',', Regs.OrderBy(x=>x.Key).Select(x=>x.Value))}";
					// Console.WriteLine(state);
					// if (states.Contains(state))
					// {
					// 	//break;
					// }
					// states.Add(state);




					var ins = _code[_ip];
					var ops = ins.Ops;
					switch (ins.OpCode)
					{
						case OpCode.Set:
							Regs[Reg(ops[0])] = ValueOf(ops[1]);
							break;
						case OpCode.Sub:
							Regs[Reg(ops[0])] -= ValueOf(ops[1]);
							break;
						case OpCode.Mul:
							Regs[Reg(ops[0])] *= ValueOf(ops[1]);
							MulCount++;
							break;
						case OpCode.Jnz:
							if (ValueOf(ops[0]) != 0)
							{
								var offset = (int)ValueOf(ops[1]);
								//_ip = (_ip + offset + _code.Length) % _code.Length;
								//_ip--;
								_ip += offset - 1;
							}
							break;

						default:
							throw new Exception($"Unhandled opcode {ins.OpCode}");
					}
					_ip++;
				}
			}
		}


	}
}


#if false

b = 100067
c = 117067
loop
  f = 1
  d = 2
  do
    e = 2
    do
      g = d
      g *= e
      g -= b
      if (g == 0)
        f = 0
        e++
      g = e-b
    while g != 0
    d++
    g = d-b
  while (g != 0)
  if (f == 0)
    h++
    g = b
  g -= c
  if (g == 0)
    exit
  b += 17


b = 100067
const c = 117067
loop
  f = 1
  d = 2
  do
    e = 2
    do
      g = d*e-b
      if (g == 0)
        f = 0
        e++
      g = e-b
    while g != 0
    d++
    g = d-b
  while (g != 0)
  if (f == 0)
    h++
    g = b
  g -= c
  if (g == 0)
    exit
  b += 17

#endif