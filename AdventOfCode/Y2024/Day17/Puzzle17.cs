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
			Run("test1").Part1("4,6,3,5,6,3,5,2,1,0");
			Run("test2").Part2(117440);
			//Run("test2").Part1(0).Part2(0);

			// not 0,2,4,3,5,3,5,3,3

			Run("input").Part1("7,4,2,0,5,0,5,3,7").Part2(0);
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
							regA /= 2 << (int)(Combo(prog[ip++]) - 1);
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
							regB = regA / (2 << (int)(Combo(prog[ip++]) - 1));
							break;
						case 7: //
							regC = regA / (2 << (int)(Combo(prog[ip++]) - 1));
							break;
					}
				}


				int Combo(int val)
				{
					if (val >= 0 && val <= 3)
						return val;
					return val switch {
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
			var regB0 = input[1].RxMatch("Register B: %d").Get<long>();
			var regC0 = input[2].RxMatch("Register C: %d").Get<long>();
			var prog = input[4].Split(':')[1].Split(',').Select(int.Parse).ToArray();

			var regA = 0L;
			var regB = regB0;
			var regC = regC0;

			for (var i = 10L; i < long.MaxValue; i++)
			{
				regA = i;
				regB = regB0;
				regC = regC0;

				if (i%1000000==0) Console.Write('.');

				var result = Run(0);
				//try
				{
					if (result.Take(prog.Length).SequenceEqual(prog))
						return i;
				}
				//catch {}
			}
			return 0;

			IEnumerable<int> Run(int ip)
			{
				while (ip >= 0 && ip < prog.Length)
				{
					switch (prog[ip++])
					{
						case 0:
							{
								var denom = 2L << (int)(Combo(prog[ip++]) - 1);
								if (denom == 0)
									yield break;
								regA /= denom;
							}
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
							yield return (int)(Combo(prog[ip++]) % 8);
							break;
						case 6: //
							{
								var denom = 2L << (int)(Combo(prog[ip++]) - 1);
								if (denom == 0)
									yield break;
								regB = regA / denom;
							}
							break;
						case 7: //
							{
								var denom = 2L << (int)(Combo(prog[ip++]) - 1);
								if (denom == 0)
									yield break;
								regC = regA / denom;
							}
							break;
					}
				}

				long Combo(int val)
				{
					if (val >= 0 && val <= 3)
						return val;
					return val switch {
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