using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day23
{
	internal class Puzzle23
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private class Ins
		{
			public string Opcode { get; set; }
			public char Register { get; set; }
			public int Offset { get; set; }
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2015/Day23/input.txt");

			var code = input
				.Select(line =>
				{
					if (SimpleRegex.IsMatch(line, "%s %c, %d", out var val))
					{
						return new Ins { Opcode = val[0], Register = val[1][0], Offset =  int.Parse(val[2]) };
					}
					if (SimpleRegex.IsMatch(line, "%s %d", out val))
					{
						return new Ins { Opcode = val[0], Offset = int.Parse(val[1]) };
					}
					if (SimpleRegex.IsMatch(line, "%s %c", out val))
					{
						return new Ins { Opcode = val[0], Register = val[1][0] };
					}
					throw new Exception($"Unexpected line {line}");
				})
				.ToArray();

			var result1 = Run(code, 0, 0);
			Console.WriteLine($"Day 23 Puzzle 1: {result1}");
			Debug.Assert(result1 == 184);

			var result2 = Run(code, 1, 0);
			Console.WriteLine($"Day 23 Puzzle 2: {result2}");
			Debug.Assert(result2 == 231);
		}


		private static int Run(Ins[] code, int a, int b)
		{
			var regs = new Dictionary<char, int> { { 'a', a }, { 'b', b } };
			for (var ip = 0; ip < code.Length;)
			{
				var ins = code[ip];
				switch (ins.Opcode)
				{
					case "hlf":
						regs[ins.Register] /= 2;
						ip++;
						break;
					case "tpl":
						regs[ins.Register] *= 3;
						ip++;
						break;
					case "inc":
						regs[ins.Register]++;
						ip++;
						break;
					case "jmp":
						ip += ins.Offset;
						break;
					case "jie":
						ip += regs[ins.Register] % 2 == 0 ? ins.Offset : 1;
						break;
					case "jio":
						ip += regs[ins.Register] == 1 ? ins.Offset : 1;
						break;
					default:
						throw new Exception($"Bad opcode {ins.Opcode}");
				}
			}
			return regs['b'];
		}
	}
}
