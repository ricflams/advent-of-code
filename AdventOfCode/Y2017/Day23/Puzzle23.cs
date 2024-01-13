using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2017.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Coprocessor Conflagration";
		public override int Year => 2017;
		public override int Day => 23;

		public override void Run()
		{
			Run("input").Part1(4225).Part2(905);
			Run("extra").Part1(8281).Part2(911);
		}

		protected override long Part1(string[] input)
		{
			var tablet = new Tablet(input);
			tablet.Run();
			return tablet.MulCount;
		}

		protected override long Part2(string[] input)
		{
			// 1-1 translation of input into pseudocode:
			//	b = 67;
			//	b = (b*100)+100000;
			//	c = b + 17000
			//	loop
			//		f = 1
			//		d = 2
			//		do
			//			e = 2
			//			do
			//				g = d
			//				g *= e
			//				g -= b
			//				if (g == 0)
			//					f = 0
			//				e++
			//				g = e-b
			//			while g != 0
			//			d++
			//			g = d-b
			//		while (g != 0)
			//		if (f == 0)
			//			h++
			//		g = b
			//		g -= c
			//		if (g == 0)
			//			exit
			//		b += 17
			//
			// g is always just used for calculating conditions so
			// it can be replaced by the calculations themselves. Also,
			// let's write loops as they would appear for real:
			//
			//	while (true)
			//	{
			// 		f = 1;
			//		d = 2;
			//		do
			//		{
			//			e = 2;
			//			do
			//			{
			//				if (d*e == b)
			//					f = 0;
			//				e++;
			//			}
			//			while (e != b);
			//			d++;
			//		}
			//		while (d != b);
			//		if (f == 0)
			//			h++;
			//		if (b == c)
			//			break;
			//		b += 17
			//	}
			//
			// The do-while (e != b) loop simply test for whether b%d == 0 (by
			// doing e*d==b testing for e-values from 2 to b) so it can be replaced
			// by a modulus-test. The outer loop is counting up b to c. The inner
			// loop for d is counting from 2 to b.
			//
			//	for (; b <= c; b += 17)
			//	{
			// 		f = 1;
			//		for (var d = 2; d < b; d++)
			//		{
			// 			if (b%d == 0)
			//			{
			//				f = 0;
			//				break;
			//			}
			//		}
			//		if (f == 0)
			//			h++;
			//	}
			//
			// The test-flag f isn't necessary. Algorithmically, d doesn't need to
			// count all the way up to b but only to sqrt(b).
			//
			//	for (; b <= c; b += 17)
			//	{
			//		for (var d = 2; d*d <= b; d++)
			//		{
			// 			if (b%d == 0)
			//			{
			//				h++;
			//				break;
			//			}
			//		}
			//	}

			var b = input[0].RxMatch("set b %d").Get<int>();
			
			b = (b*100)+100000;
			var c = b + 17000;
			var h = 0;
			for (; b <= c; b += 17)
			{
				for (var d = 2; d*d <= b; d++)
				{
					if (b % d == 0)
					{
						h++;
						break;
					}
				}
			}
			return h;
		}

		public class Tablet
		{
			private readonly Ins[] _code;
			private int _ip;

			public SafeDictionary<char, long> Regs = new SafeDictionary<char, long>();

			public long MulCount { get; private set; }

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
							"set" => new Ins(OpCode.Set, GetOp(p[1]), GetOp(p[2])),
							"sub" => new Ins(OpCode.Sub, GetOp(p[1]), GetOp(p[2])),
							"mul" => new Ins(OpCode.Mul, GetOp(p[1]), GetOp(p[2])),
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

			public void Run()
			{
				MulCount = 0;
				_ip = 0;

				while (_ip < _code.Length)
				{
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

b = 67;
b = (b*100)+100000;
c = b + 17000
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