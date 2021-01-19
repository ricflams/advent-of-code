using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day08
{
	internal class Puzzle : Puzzle<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Handheld Halting";
		public override int Year => 2020;
		public override int Day => 8;

		public void Run()
		{
			RunFor("test1", 5, 8);
			RunFor("input", 2034, 672);
		}

		protected override int Part1(string[] input)
		{
			var code = GetCode(input);
			RunToCompletion(code, out var acc);
			return acc;
		}

		protected override int Part2(string[] input)
		{
			var code = GetCode(input);
			var acc = 0;
			foreach (var ins in code)
			{
				var opcode = ins.Opcode;
				ins.Opcode =
					opcode == "jmp" ? "nop" :
					opcode == "nop" ? "jmp" :
					opcode;
				if (RunToCompletion(code, out acc))
				{
					break;
				}
				ins.Opcode = opcode;
			}
			return acc;
		}

		private class Ins
		{
			public string Opcode { get; set; }
			public int Val { get; set; }
		}

		private static Ins[] GetCode(string[] input)
		{
			var code = input
				.Select(line =>
				{
					var (opcode, sign, val) = line.RxMatch("%s %c%d").Get<string, char, int>();
					return new Ins
					{
						Opcode = opcode,
						Val = val * (sign == '+' ? 1 : -1)
					};
				})
				.ToArray();
			return code;
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
