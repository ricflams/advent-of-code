using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2024.Day11.Raw
{
	internal class Puzzle : PuzzleWithParameter<int, long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").WithParameter(1).Part1(7).Part2(0);
			Run("test2").WithParameter(1).Part1(3).Part2(0);
			Run("test2").WithParameter(2).Part1(4).Part2(0);
			Run("test2").WithParameter(3).Part1(5).Part2(0);
			Run("test2").WithParameter(4).Part1(9).Part2(0);
			Run("test2").WithParameter(5).Part1(13).Part2(0);
			Run("test2").WithParameter(6).Part1(22).Part2(0);
			Run("test2").WithParameter(25).Part1(55312).Part2(0);
			Run("input").WithParameter(25).Part1(0).Part2(0);
			Run("input").WithParameter(75).Part1(0).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{

			// If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
			// If the stone is engraved with a number that has an even number of digits, it is replaced by two stones. The left half of the digits are engraved on the new left stone, and the right half of the digits are engraved on the new right stone. (The new numbers don't keep extra leading zeroes: 1000 would become stones 10 and 0.)
			// If none of the other rules apply, the stone is replaced by a new stone; the old stone's number multiplied by 2024 is engraved on the new stone.


			var blinks = PuzzleParameter;
			var stones = input[0].SplitSpace();

			var memo = new Dictionary<string,long>();

			long Blink(string s, int blinks)
			{
				if (blinks == 0)
					return 1;

				var key = $"{s}-{blinks}";
				if (memo.TryGetValue(key, out var cn))
					return cn;

				if (s == "0")
				{
					var n = Blink("1", blinks - 1);
					memo[key] = n;
					return n;
				}
				if (s.Length % 2 == 0)
				{
					var half = s.Length / 2;
					// var s1 = s[^half..].TrimStart('0');
					// var s2 = s[..half].TrimStart('0');

					var s1 = s.Substring(0, half);
					var s2 = s.Substring(half).TrimStart('0');
					if (s2 == "")
						s2 = "0";

					var n = Blink(s1, blinks - 1) + Blink(s2, blinks - 1);
					memo[key] = n;
					return n;
				}
				var val = (long.Parse(s) * 2024).ToString();
				var n3 = Blink(val, blinks - 1);
				memo[key] = n3;
				return n3;
			}

			var len = stones.Sum(s => Blink(s, blinks));

			return len;
		}



		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
