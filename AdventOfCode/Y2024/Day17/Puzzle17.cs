using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Runtime.InteropServices.Marshalling;
using MathNet.Numerics;

namespace AdventOfCode.Y2024.Day17
{
	internal class Puzzle : Puzzle<string, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 17;

		public override void Run()
		{
			//Run("test1").Part1("4,6,3,5,6,3,5,2,1,0");
			Run("test2").Part2(117440);
			//Run("test2").Part1(0).Part2(0);

			// not 0,2,4,3,5,3,5,3,3

			Run("input").Part1("7,4,2,0,5,0,5,3,7").Part2(202991746427434);
			// 395247289637 too low


			//Run("extra").Part1(0).Part2(0);
		}

		protected override string Part1(string[] input)
		{
			var regA = input[0].RxMatch("Register A: %d").Get<int>();
			var regB = input[1].RxMatch("Register B: %d").Get<int>();
			var regC = input[2].RxMatch("Register C: %d").Get<int>();
			var prog = input[4].Split(':')[1].Split(',').Select(int.Parse).ToArray();

			var output = Run(0).ToArray();
			var result = string.Join(',', output);

			IEnumerable<int> Run(int ip)
			{
				while (ip >= 0 && ip < prog.Length)
				{
					switch (prog[ip++])
					{
						case 0:
							regA /= 1 << Combo(prog[ip++]);
							break;
						case 1: //
							regB ^= prog[ip++];
							break;
						case 2: //
							regB = Combo(prog[ip++]) % 8;
							break;
						case 3: //
							if (regA != 0)
								ip = prog[ip];
							else
								ip++;
							break;
						case 4: //
							regB ^= regC;
							ip++;
							break;
						case 5: //
								//Console.Write($"{Combo(prog[ip]) % 8} ");
							yield return Combo(prog[ip++]) % 8;
							break;
						case 6: //
							regB = regA / (1 << Combo(prog[ip++]));
							break;
						case 7: //
							regC = regA / (1 << Combo(prog[ip++]));
							break;
					}
				}


				int Combo(int val)
				{
					if (val >= 0 && val <= 3)
						return val;
					return val switch
					{
						4 => regA,
						5 => regB,
						6 => regC,
						_ => throw new Exception(),
					};
				}
			}

			return result;
		}

		protected override long Part2(string[] input)
		{
			var regA0 = input[0].RxMatch("Register A: %d").Get<long>();
			// var regB0 = input[1].RxMatch("Register B: %d").Get<long>();
			// var regC0 = input[2].RxMatch("Register C: %d").Get<long>();
			var prog = input[4].Split(':')[1].Split(',').Select(int.Parse).ToArray();

			// var regA = 0L;
			// var regB = regB0;
			// var regC = regC0;



			// Program: 2,4,1,1,7,5,4,4,1,4,0,3,5,5,3,0
			// B = A % 8
			// B ^= 1
			// C = A / 2^B
			// B ^= C
			// B ^= A
			// A /= 8
			// out B % 8
			// while A

			// var testa = new long[] { 0, 1, 2, 3, 4, 5, 6, 7 };
			// foreach (var digit in prog.Reverse())
			// {
			// 	var found = false;
			// 	foreach (var a in testa)
			// 	{
			// 		var dig = Run(smallprog, a).Single();
			// 		Console.Write($"{dig} ");
			// 		if (dig == digit)
			// 		{
			// 			mina = a;
			// 			for (var i = 0; i < 8; i++)
			// 				testa[i] = a * 8 + i;
			// 			found = true;
			// 			Console.WriteLine($"digit {digit}: a={a} code={string.Join(',', Run(prog, a))}");
			// 			break;
			// 		}
			// 	}
			// 	if (!found)
			// 		;
			// }

			var smallprog = prog[..^2];
			var mina = 0L;

			MinA(prog.Length - 1, [0, 1, 2, 3, 4, 5, 6, 7]);

			long? MinA(int progpos, long[] maybeA)
			{
				var lookfor = prog[progpos];
				foreach (var a in maybeA)
				{
					var dig = Run(smallprog, a).Single();
					if (dig == lookfor)
					{
						if (progpos == 0)
						{
							mina = a;
							return a;
						}
						var maybeAmore = new long[8];
						for (var i = 0; i < 8; i++)
							maybeAmore[i] = a * 8 + i;
						var aa = MinA(progpos - 1, maybeAmore);
						if (aa.HasValue)
							return aa.Value;
					}
				}
				return null;
			}

			var code = Run(prog, mina);
			if (code.SequenceEqual(prog))
				return mina;

			return 0;

			// for (var i = 7L * (long)Math.Pow(8L, 15); i < long.MaxValue; i++)
			// //for (var i = 117440L; i < long.MaxValue; i++)
			// {
			// 	regA = i;
			// 	regB = regB0;
			// 	regC = regC0;

			// 	if (i % 1000000 == 0) Console.Write('.');

			// 	var result = Run(0);
			// 	try
			// 	{
			// 		var code = result.ToArray();
			// 		if (code.Length != prog.Length)
			// 			;
			// 		if (code.SequenceEqual(prog))
			// 			return i;
			// 	}
			// 	catch
			// 	{

			// 	}
			// }
			return 0;

			IEnumerable<int> Run(int[] prog, long regA)
			{
				var regB = 0L;
				var regC = 0L;
				var ip = 0;
				while (ip >= 0 && ip < prog.Length)
				{
					switch (prog[ip++])
					{
						case 0:
							regA /= 1U << (int)Combo(prog[ip++]);
							break;
						case 1: //
							regB ^= prog[ip++];
							break;
						case 2: //
							regB = Combo(prog[ip++]) % 8;
							break;
						case 3: //
							if (regA != 0)
								ip = prog[ip];
							else
								ip++;
							break;
						case 4: //
							regB ^= regC;
							ip++;
							break;
						case 5:
							//Console.Write($"{Combo(prog[ip]) % 8} ");
							yield return (int)(Combo(prog[ip++]) % 8);
							break;
						case 6: //
							regB = regA / (1U << (int)Combo(prog[ip++]));
							break;
						case 7: //
							regC = regA / (1U << (int)Combo(prog[ip++]));
							break;
					}
				}

				long Combo(int val)
				{
					if (val >= 0 && val <= 3)
						return val;
					return val switch
					{
						4 => regA,
						5 => regB,
						6 => regC,
						_ => throw new Exception(),
					};
				}
			}
		}
	}
}