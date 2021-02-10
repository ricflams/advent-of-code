using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2016.Day23
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Safe Cracking";
		public override int Year => 2016;
		public override int Day => 23;

		public void Run()
		{
			RunPart1For("test1", 3);
			//RunFor("test2", 0, 0);
			RunFor("input", 12762, 479009322);
		}

		protected override int Part1(string[] input)
		{
			var comp = new Computer(input);
			comp.Regs[0] = 7;
			comp.Run();
			return comp.Regs[0];
		}

		protected override int Part2(string[] input)
		{
			input[3] = "MX1"; //  cpy 0 a
			input[4] = "NOP"; //  cpy b c
			input[5] = "NOP"; //  inc a
			input[6] = "NOP"; //  dec c
			input[7] = "NOP"; //  jnz c -2
			input[8] = "NOP"; //  dec d
			input[9] = "NOP"; //  jnz d -5

			input[11] = "MX2"; // cpy b c
			input[12] = "NOP"; // cpy c d
			input[13] = "NOP"; // dec d
			input[14] = "NOP"; // inc c
			input[15] = "NOP"; // jnz d -2

			var comp = new Computer(input);
			comp.Regs[0] = 12;
			comp.Run();
			return comp.Regs[0];
		}


		internal class Computer
		{
			private enum OpCode { CpyRegReg, CpyRegVal, CpyValReg, CpyValVal, Inc, Dec, JnzRegReg, JnzRegVal, JnzValReg, JnzValVal, Tgl, MyNop, MyMult1, MyMult2};
			private class Ins
			{
				public Ins(OpCode opc, int op1, int op2)
				{
					OpCode = opc; Op1 = op1; Op2 = op2;
				}
				public OpCode OpCode { get; set; }
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
							// tgl x toggles the instruction x away (positive means forward; negative means backward)
							//     For one-argument instructions, inc becomes dec, and all other one-argument instructions become inc.
							//     For two-argument instructions, jnz becomes cpy, and all other two-instructions become jnz.
							//     The arguments of a toggled instruction are not affected.
							//     If an attempt is made to toggle an instruction outside the program, nothing happens.
							//     If toggling produces an invalid instruction (like cpy 1 2) and an attempt is later made to execute that instruction, skip it instead.
							"cpy" => char.IsLetter(p[1].First())
										? new Ins(OpCode.CpyRegReg, Reg(p[1]), Reg(p[2]))
										: new Ins(OpCode.CpyValReg, int.Parse(p[1]), Reg(p[2])),
							"inc" => new Ins(OpCode.Inc, Reg(p[1]), 0),
							"dec" => new Ins(OpCode.Dec, Reg(p[1]), 0),
							"jnz" => char.IsLetter(p[1].First())
										? char.IsLetter(p[2].First())
											? new Ins(OpCode.JnzRegReg, Reg(p[1]), Reg(p[2]))
											: new Ins(OpCode.JnzRegVal, Reg(p[1]), int.Parse(p[2]))
										: char.IsLetter(p[2].First())
											? new Ins(OpCode.JnzValReg, int.Parse(p[1]), Reg(p[2]))
											: new Ins(OpCode.JnzValVal, int.Parse(p[1]), int.Parse(p[2])),
							"tgl" => new Ins(OpCode.Tgl, Reg(p[1]), 0),
							"MX1" => new Ins(OpCode.MyMult1, 0, 0),
							"MX2" => new Ins(OpCode.MyMult2, 0, 0),
							"NOP" => new Ins(OpCode.MyNop, 0, 0),
							_ => throw new Exception($"Unknown instruction {p[0]}")
						};
					})
					.ToArray();
				_ip = 0;

				int Reg(string s) => s.First() - 'a';
			}

			//private string CodeId => $"{_ip}{new string(_code.Select(ins => (char)('a'+(int)ins.OpCode)).ToArray())}";

			public void Run()
			{
				var modified = new bool[_code.Length];
				while (_ip < _code.Length)
				{
					var ins = _code[_ip];
					switch (ins.OpCode)
					{
						case OpCode.CpyRegReg:
							Regs[ins.Op2] = Regs[ins.Op1];
							break;
						case OpCode.CpyRegVal:
							// Invalid, coming from toggling JnzRegVal
							break;
						case OpCode.CpyValReg:
							Regs[ins.Op2] = ins.Op1;
							break;
						case OpCode.CpyValVal:
							// Invalid, coming from toggling JnzValVal
							break;
						case OpCode.Inc:
							Regs[ins.Op1]++;
							break;
						case OpCode.Dec:
							Regs[ins.Op1]--;
							break;
						case OpCode.JnzRegReg:
							if (Regs[ins.Op1] != 0)
							{
								_ip += Regs[ins.Op2] - 1;
							}
							break;
						case OpCode.JnzRegVal:
							if (Regs[ins.Op1] != 0)
							{
								_ip += ins.Op2 - 1;
							}
							break;
						case OpCode.JnzValReg:
							if (ins.Op1 != 0)
							{
								_ip += Regs[ins.Op2] - 1;
							}
							break;
						case OpCode.JnzValVal:
							if (ins.Op1 != 0)
							{
								_ip += ins.Op2 - 1;
							}
							break;
						case OpCode.Tgl:
							var offset = Regs[ins.Op1];
							var ip = _ip + offset;
							if (ip >= 0 && ip < _code.Length)
							{
								var modins = _code[ip];
								modins.OpCode = modins.OpCode switch {
									OpCode.Inc => OpCode.Dec,
									OpCode.Dec => OpCode.Inc,
									OpCode.Tgl => OpCode.Inc,
									OpCode.JnzRegReg => OpCode.CpyRegReg,
									OpCode.JnzRegVal => OpCode.CpyRegVal,
									OpCode.JnzValReg => OpCode.CpyValReg,
									OpCode.JnzValVal => OpCode.CpyValVal,
									OpCode.CpyRegReg => OpCode.JnzRegReg,
									OpCode.CpyRegVal => OpCode.JnzRegVal,
									OpCode.CpyValReg => OpCode.JnzValReg,
									OpCode.CpyValVal => OpCode.JnzValVal,
									_ => throw new Exception($"Unexpected opcode {modins.OpCode}")
								};
								modified[ip] = true;
							}
							break;

						case OpCode.MyNop:
							// Filler-instruction, do nothing
							break;
						case OpCode.MyMult1:
							Regs[0] = Regs[1] * Regs[3]; // a = b*d
							Regs[2] = 0; // c = 0
							Regs[3] = 0; // d = 0
							break;
						case OpCode.MyMult2:
							Regs[2] = Regs[1] * 2; // c = b*2
							Regs[3] = 0; // d = 0
							break;

					}
					_ip++;
				}
			}
		}


		// Code looks like this.
		// Only lines marked by * are toggled at runtime, rest remaind fixed:
		//   cpy a b
		//   dec b
		//   cpy a d
		//   cpy 0 a
		//   cpy b c
		//   inc a
		//   dec c
		//   jnz c -2
		//   dec d
		//   jnz d -5
		//   dec b
		//   cpy b c
		//   cpy c d
		//   dec d
		//   inc c
		//   jnz d -2
		//   tgl c
		//   cpy -16 c
		// * jnz 1 c
		//   cpy 78 c
		// * jnz 99 d
		//   inc a
		// * inc d
		//   jnz d -2
		// * inc c
		//   jnz c -5		


///
/// b = a
/// b--
/// d = a
/// a = 0
/// do
///   c = b
///   do
///     a++
///     c--
///   while c != 0
///   d--
/// while d != 0
/// b--
/// c = b
/// d = c
/// do
///   d--
///   c++
/// while d != 0
/// tgl c
/// ........
/// 
/// 
/// a = 0
/// do
///   c = b
///   do
///     a++
///     c--
///   while c != 0
///   d--
/// while d != 0
/// 
/// equal to
/// 
/// a = b * d
/// c = 0
/// d = 0
/// 
/// 
/// c = b
/// d = c
/// do
///   d--
///   c++
/// while d != 0
/// 
/// equal to
/// 
/// c = b * 2
/// d = 0
/// 
/// 
/// 
/// 
/// 
/// 
	}




	//  internal class Puzzle : ComboParts<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//		public override string Name => "";
	//  	public override int Year => 2016;
	//  	public override int Day => 23;
	//  
	//  	public void Run()
	//  	{
	//  		//RunFor("test1", 0, 0);
	//  		//RunFor("test2", 0, 0);
	//  		RunFor("input", 0, 0);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}
