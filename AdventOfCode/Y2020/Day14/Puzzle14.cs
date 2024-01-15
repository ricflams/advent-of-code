using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day14
{
	internal class Puzzle : Puzzle<ulong, ulong>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Docking Data";
		public override int Year => 2020;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").Part1(165);
			Run("test2").Part2(208);
			Run("input").Part1(14553106347726).Part2(2737766154126);
			Run("extra").Part1(8566770985168).Part2(4832039794082);
		}

		protected override ulong Part1(string[] input)
		{
			var mem = new Dictionary<int, ulong>();

			var maskset = 0UL;
			var maskclear = 0UL;
			foreach (var line in input)
			{
				if (line.IsRxMatch("mask = %s", out var captures))
				{
					var mask = captures.Get<string>();
					maskset = Convert.ToUInt64(mask.Replace("X", "0"), 2);
					maskclear = Convert.ToUInt64(mask.Replace("X", "1"), 2);
				}
				else
				{
					var (addr, val) = line.RxMatch(@"mem[%d] = %d").Get<int, ulong>();
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
				if (line.IsRxMatch("mask = %s", out var captures))
				{
					var mask = captures.Get<string>();
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
					var (vaddr, val) = line.RxMatch(@"mem[%d] = %d").Get<ulong, ulong>();
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
