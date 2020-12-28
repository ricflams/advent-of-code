using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day23
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Opening the Turing Lock";
		protected override int Year => 2015;
		protected override int Day => 23;

		public void Run()
		{
			RunFor("input", 184, 231);
		}

		private class Ins
		{
			public string Opcode { get; set; }
			public char Register { get; set; }
			public int Offset { get; set; }
		}

		protected override (int, int) Part1And2(string[] input)
		{
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
			var result2 = Run(code, 1, 0);

			return (result1, result2);
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
