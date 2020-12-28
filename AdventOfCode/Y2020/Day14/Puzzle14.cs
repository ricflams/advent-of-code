using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day14
{
	internal class Puzzle : SoloParts<ulong>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Docking Data";
		protected override int Year => 2020;
		protected override int Day => 14;

		public void Run()
		{
			RunPart1For("test1", 165);
			RunPart2For("test2", 208);
			RunFor("input", 14553106347726, 2737766154126);
		}

		protected override ulong Part1(string[] input)
		{
			var mem = new Dictionary<int, ulong>();

			var maskset = 0UL;
			var maskclear = 0UL;
			foreach (var line in input)
			{
				if (line.MaybeRegexCapture("mask = %s").Get(out string mask).IsMatch)
				{
					maskset = Convert.ToUInt64(mask.Replace("X", "0"), 2);
					maskclear = Convert.ToUInt64(mask.Replace("X", "1"), 2);
				}
				else
				{
					line.RegexCapture(@"mem[%d] = %d").Get(out int addr).Get(out ulong val);
					mem[addr] = (val & maskclear) | maskset;
				}
			}

			return mem.Values.Sum();
		}

		protected override ulong Part2(string[] input)
		{
			var mem = new Dictionary<ulong, ulong>();

			var maskset = 0UL;
			var floatingbits = new ulong[0];
			foreach (var line in input)
			{
				if (line.MaybeRegexCapture("mask = %s").Get(out string mask).IsMatch)
				{
					maskset = Convert.ToUInt64(mask.Replace("X", "0"), 2);
					var maskfloat = Convert.ToUInt64(mask.Replace("1", "0").Replace("X", "1"), 2);

					floatingbits = new ulong[mask.Count(x => x == 'X')];
					var bit = 1UL;
					for (var index = 0; index < floatingbits.Length; )
					{
						if ((maskfloat & bit) != 0)
						{
							floatingbits[index++] = bit;
						}
						bit <<= 1;
					}
				}
				else
				{
					line.RegexCapture(@"mem[%d] = %d").Get(out ulong vaddr).Get(out ulong val);
					var addr = vaddr | maskset;
					SetAllFloatingValues(addr, val, floatingbits);
				}
			}

			void SetAllFloatingValues(ulong addr, ulong val, ulong[] bits)
			{
				if (bits.Length == 0)
				{
					return;
				}

				var addr1 = addr | bits[0];
				var addr2 = addr & ~bits[0];
				mem[addr1] = val;
				mem[addr2] = val;
				SetAllFloatingValues(addr1, val, bits[1..]);
				SetAllFloatingValues(addr2, val, bits[1..]);
			}

			return mem.Values.Sum();
		}
	}
}
