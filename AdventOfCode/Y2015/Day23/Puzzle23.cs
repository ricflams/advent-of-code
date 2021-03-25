using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day23
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Opening the Turing Lock";
		public override int Year => 2015;
		public override int Day => 23;
 
		public void Run()
		{
			Run("input").Part1(184).Part2(231);
		}

		private class Ins
		{
			public string Opcode { get; set; }
			public char Register { get; set; }
			public int Offset { get; set; }
		}

		protected override int Part1(string[] input)
		{
			var code = GetCode(input);
			var result = Run(code, 0, 0);
			return result;
		}

		protected override int Part2(string[] input)
		{
			var code = GetCode(input);
			var result = Run(code, 1, 0);
			return result;
		}

		private Ins[] GetCode(string[] input)
		{
			var code = input
				.Select(line =>
				{
					if (line.IsRxMatch("%s %c, %d", out var captures))
					{
						var (opc, reg, offset) = captures.Get<string, char, int>();
						return new Ins { Opcode = opc, Register = reg, Offset = offset };
					}
					if (line.IsRxMatch("%s %d", out captures))
					{
						var (opc, offset) = captures.Get<string, int>();
						return new Ins { Opcode = opc, Offset = offset };
					}
					if (line.IsRxMatch("%s %c", out captures))
					{
						var (opc, reg) = captures.Get<string, char>();
						return new Ins { Opcode = opc, Register = reg };
					}
					throw new Exception($"Unexpected line {line}");
				})
				.ToArray();
			return code;
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
