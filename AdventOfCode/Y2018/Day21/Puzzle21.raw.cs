using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2018.Day21.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Chronal Conversion";
		public override int Year => 2018;
		public override int Day => 21;

		public void Run()
		{
			//Run("test1").Part1(0).Part2(0);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(0).Part2(0);
		}


		// #ip 1
		// seti 123 0 3        C = 123
		// bani 3 456 3        C = C & 456
		// eqri 3 72 3         C = C==72 ? 1 : 0
		// addr 3 1 1          A = C + A
		// seti 0 0 1          
		// seti 0 9 3
		// bori 3 65536 5
		// seti 15028787 4 3
		// bani 5 255 2
		// addr 3 2 3
		// bani 3 16777215 3
		// muli 3 65899 3
		// bani 3 16777215 3
		// gtir 256 5 2
		// addr 2 1 1
		// addi 1 1 1
		// seti 27 3 1
		// seti 0 9 2
		// addi 2 1 4
		// muli 4 256 4
		// gtrr 4 5 4
		// addr 4 1 1
		// addi 1 1 1
		// seti 25 1 1
		// addi 2 1 2
		// seti 17 8 1
		// setr 2 4 5
		// seti 7 3 1
		// eqrr 3 0 2
		// addr 2 1 1
		// seti 5 3 1		

		private void Test()
		{
			var A = 8499279;
			A = 13270004;
			A = -1;
			var D = 0;
			var fseen = new HashSet<int>();

			do {
				var F = D | 65536;
				if (fseen.Contains(F))
					break;
				fseen.Add(F);				
				D = 15028787;// # prime
				while (true) {
//					Console.WriteLine($"D={D}  F={F}");
					D = ((D + (F&0xff) & 0xffffff) * 65899) & 0xffffff;
//					Console.WriteLine($"  D={D}");
					if (F < 256)
						break;
					F = F / 256;
//					Console.WriteLine($"  F={F}");
				}
				Console.WriteLine($"  D={D}");
			} while (D != A);

			Console.WriteLine($"  D={D}");
;

// 13646422 too high
// 12879142 <---- right!
// 8499279 not correct
// 62116 too low


			// do {
			// 	var F = D | 65536;
			// 	D = 15028787;// # prime
			// 	while (true) {
			// 		Console.WriteLine($"D={D}  F={F}");
			// 		var C = F & 255;
			// 		D += C;
			// 		D &= 16777215;
			// 		D *= 65899; //# prime
			// 		D &= 16777215;
			// 		Console.WriteLine($"  D={D}");
			// 		if (F < 256)
			// 			break;
			// 		C = 0;
			// 		var E = 0;
			// 		do {
			// 			E = C + 1;
			// 			E *= 256;
			// 			if (E <= F)
			// 				C++;
			// 		} while (E <= F);
			// 		F = C;
			// 		Console.WriteLine($"  F={F}");
			// 	}
			// } while (D != A);


		}

		protected override long Part1(string[] input)
		{
			var computer = new Computer(input);
			var line = 0;
			Console.WriteLine($"IpReg = {(char)('A' + computer.IpRegister)}");
			foreach (var dis in computer.Disassembly())
			{
				Console.WriteLine($"{line:D2}   {dis}");
				line++;
			}

			Test();

			return 0;

			// var desiredReg0 = 0;
			// computer.Run(ref desiredReg0);

// 254 too low

			// return desiredReg0;

			// computer = new Computer(input);
			// computer.Regs[0] = desiredReg0;


			// var ip = computer.IpRegister;
			// for (var reg0 = 0; reg0 < int.MaxValue; reg0++)
			// {
			// 	//Console.WriteLine(reg0);
			// 	computer.Regs = new int[6];
			// 	computer.IpRegister = ip;
			// 	computer.Regs[0] = reg0;
			// 	computer.NumberOfExecutedInstructions = 0;
			// 	if (computer.Run())
			// 		return reg0;
			// 	reg0++;
			// }

			// return 0;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}


		/////////////////////////////////////////////////////////////////////////////////////////////////////




		internal class Computer
		{
			public enum Opcode
			{
				addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
			}

			public /*readonly*/ int[] Regs = new int[6];
			public /*readonly*/ int IpRegister;
			public readonly Ins[] Instructions;

			public int NumberOfExecutedInstructions { get; set; }

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

			public IEnumerable<string> Disassembly()
			{
				static char Reg(int regno) => (char)('A' + regno);
				static string Val(int val) => $"{val}/{Convert.ToString(val, 2)}";

				foreach (var ins in Instructions)
				{
					var (a, b, c) = (ins.A, ins.B, ins.C);
					switch (ins.Opcode)
					{
						case Opcode.addr: yield return $"{Reg(c)} = {Reg(a)} + {Reg(b)}"; break;
						case Opcode.addi: yield return $"{Reg(c)} = {Reg(a)} + {Val(b)}"; break;
						case Opcode.mulr: yield return $"{Reg(c)} = {Reg(a)} * {Reg(b)}"; break;
						case Opcode.muli: yield return $"{Reg(c)} = {Reg(a)} * {Val(b)}"; break;
						case Opcode.banr: yield return $"{Reg(c)} = {Reg(a)} & {Reg(b)}"; break;
						case Opcode.bani: yield return $"{Reg(c)} = {Reg(a)} & {Val(b)}"; break;
						case Opcode.borr: yield return $"{Reg(c)} = {Reg(a)} | {Reg(b)}"; break;
						case Opcode.bori: yield return $"{Reg(c)} = {Reg(a)} | {Val(b)}"; break;
						case Opcode.setr: yield return $"{Reg(c)} = {Reg(a)}"; break;
						case Opcode.seti: yield return $"{Reg(c)} = {Val(a)}"; break;
						case Opcode.gtir: yield return $"{Reg(c)} = {Val(a)} > {Reg(b)} ? 1 : 0"; break;
						case Opcode.gtri: yield return $"{Reg(c)} = {Reg(a)} > {Val(b)} ? 1 : 0"; break;
						case Opcode.gtrr: yield return $"{Reg(c)} = {Reg(a)} > {Reg(b)} ? 1 : 0"; break;
						case Opcode.eqir: yield return $"{Reg(c)} = {Val(a)} == {Reg(b)} ? 1 : 0"; break;
						case Opcode.eqri: yield return $"{Reg(c)} = {Reg(a)} == {Val(b)} ? 1 : 0"; break;
						case Opcode.eqrr: yield return $"{Reg(c)} = {Reg(a)} == {Reg(b)} ? 1 : 0"; break;
						default:
							throw new Exception($"Unknown opcode {ins.Opcode}");
					}
				}
			}

			public class Ins
			{
				public Opcode Opcode { get; init; }
				public int A { get; init; }
				public int B { get; init; }
				public int C { get; init; }
				public override string ToString() => $"{Opcode} {A} {B} {C}";
			}

			public List<(int,int)> Matches = new List<(int,int)>();


			public bool Run(ref int desiredA)
			{
				var dseen = new HashSet<int>();
				//var lastd = 0;

				for (var ip = 0; ip >= 0 && ip < Instructions.Length; ip++)
				{
					var ins = Instructions[ip];
					var (opcode, a, b, c) = (ins.Opcode, ins.A, ins.B, ins.C);

					// if (ip == 28)
					// {
						// Console.WriteLine(Regs[3]);
						// desiredA = Regs[3];
						// return true;
					// }

					if (ip == 7)
					{
						var d = Regs[3];
						if (dseen.Contains(d))
						{
							Console.WriteLine(d);
							desiredA = d;
							return true;
						}
						dseen.Add(d);
					}

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

					NumberOfExecutedInstructions++;
					// 	return false;

					// var state = $"{ip}-{Regs[0]}-{Regs[1]}-{Regs[2]}-{Regs[3]}-{Regs[4]}-{Regs[5]}";
					// if (states.Contains(state))
					// 	return false;
					// states.Add(state);
				}

				return true;
			}
		}




	}

}

#if false
IpReg = B
00   D = 123/1111011
01   D = D & 456/111001000
02   D = D == 72/1001000 ? 1 : 0
03   B = D + B
04   B = 0/0
05   D = 0/0
06   F = D | 65536/10000000000000000
07   D = 15028787/111001010101001000110011
08   C = F & 255/11111111
09   D = D + C
10   D = D & 16777215/111111111111111111111111
11   D = D * 65899/10000000101101011
12   D = D & 16777215/111111111111111111111111
13   C = 256/100000000 > F ? 1 : 0
14   B = C + B
15   B = B + 1/1
16   B = 27/11011
17   C = 0/0
18   E = C + 1/1
19   E = E * 256/100000000
20   E = E > F ? 1 : 0
21   B = E + B
22   B = B + 1/1
23   B = 25/11001
24   C = C + 1/1
25   B = 17/10001
26   F = C
27   B = 7/111
28   C = D == A ? 1 : 0
29   B = C + B
30   B = 5/101


D = 0
6:
F = D | 10000000000000000
D = 111001010101001000110011
08:
C = F & 11111111
D += C
D &= 111111111111111111111111
D *= 65899
D &= 111111111111111111111111
if (256 > F)
  goto 28
C = 0
:18
  E = C + 1
  E *= 256
  if (E <= F)
    C++
	goto 18
F = C
goto 08
28:
if (D != A)
  goto 6



do {
  F = D | 10000000000000000
  D = 15028787 # prime
  while (true) {
    C = F & 11111111
    D += C
    D &= 111111111111111111111111
    D *= 65899 # prime
    D &= 111111111111111111111111
	if (F < 256)
	  break;
	C = 0
    do {
      E = C + 1
      E *= 256
      if (E <= F)
	    C++
	} while (E <= F)
    F = C
  }
} while (D != A)

  


	C = 0
    do {
      E = C + 1
      E *= 256
      if (E <= F)
	    C++
	} while (E <= F)
<=>
    for (C = 0; (C+1)*256 <= F; C++)
	  ;
<=>
	C = F / 256 - 1
	
	  
D = 0
do {
  F = D | 1.0000.0000.0000.0000
  D = 15028787 # prime
  while (true) {
    C = F & 1111.1111
    D += C
    D &= 1111.1111.1111.1111.1111.1111
    D *= 65899 # prime
    D &= 1111.1111.1111.1111.1111.1111
	if (F < 256)
	  break;
	F = F / 256 - 1
  }
} while (D != A)


    C = F & 1111.1111
    D += C
    D &= 1111.1111.1111.1111.1111.1111
    D *= 65899 # prime
    D &= 1111.1111.1111.1111.1111.1111
<=>	
	D = ((D + (F&0xff) & 0xffffff) * 65899) & 0xffffff
	


D = 0
do {
  F = D | 1.0000.0000.0000.0000
  D = 15028787 # prime
  while (true) {
	D = ((D + (F&0xff) & 0xffffff) * 65899) & 0xffffff
	if (F < 256)
	  break;
	F = F / 256 - 1
  }
} while (D != A)

#endif







