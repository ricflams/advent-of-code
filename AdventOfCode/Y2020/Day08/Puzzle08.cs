using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2020.Day08
{
	internal class Puzzle08
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private class Ins
		{
			public string Opcode { get; set; }
			public int Val { get; set; }
		}

		private static void Puzzle1And2()
		{
			var input = File.ReadAllLines("Y2020/Day08/input.txt");

			var code = input
				.Select(line =>
				{
					line.RegexCapture("%s %c%d").Get(out string opcode).Get(out char sign).Get(out int val);
					return new Ins
					{
						Opcode = opcode,
						Val = val * (sign == '+' ? 1 : -1)
					};
				})
				.ToArray();

			RunToCompletion(code, out var result1);
			Console.WriteLine($"Day 08 Puzzle 1: {result1}");
			Debug.Assert(result1 == 2034);

			var result2 = 0;
			foreach (var ins in code)
			{
				var opcode = ins.Opcode;
				ins.Opcode =
					opcode == "jmp" ? "nop" :
					opcode == "nop" ? "jmp" :
					opcode;
				if (RunToCompletion(code, out result2))
				{
					break;
				}
				ins.Opcode = opcode;
			}
			Console.WriteLine($"Day 08 Puzzle 2: {result2}");
			Debug.Assert(result2 == 672);
		}

		private static bool RunToCompletion(Ins[] code, out int acc)
		{
			acc = 0;
			var seen = new HashSet<int>();
			for (var ip = 0; ip < code.Length; )
			{
				if (seen.Contains(ip)) // infinite loop
				{
					return false;
				}
				seen.Add(ip);
				var ins = code[ip];
				switch (ins.Opcode)
				{
					case "nop":
						ip++;
						break;
					case "acc":
						acc += ins.Val;
						ip++;
						break;
					case "jmp":
						ip += ins.Val;
						break;
					default:
						throw new Exception($"bad opcode {ins.Opcode}");
				}
			}
			return true;
		}
	}
}
