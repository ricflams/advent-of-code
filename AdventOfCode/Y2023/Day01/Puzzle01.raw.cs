using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day01.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(0).Part2(0);
			Run("test2").Part2(0);
			Run("input").Part1(0).Part2(0);


		}

		protected override long Part1(string[] input)
		{
			var sum = 0;
			foreach (var s in input)
			{
				var d1 = s.First(c => char.IsDigit(c));
				var d2 = s.Last(c => char.IsDigit(c));
				var d = (d1-'0')*10 + d2-'0';
				sum += d;
			}

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var sum = 0;
			var digitnames = new string[] {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};
			foreach (var s in input)
			{
				var d1 = 0;
				var d2 = 0;
				for (var i = 0; i < s.Length; i++)
				{
					if (char.IsDigit(s[i]))
					{
						d1 = s[i]-'0';
						break;
					}
					var idx = digitnames.FirstOrDefault(dn => s[i..].StartsWith(dn));
					if (idx != null) {
						d1 = digitnames.IndexOf(name => name == idx);
						break;
					}
				}
				for (var i = s.Length-1; i >= 0; i--)
				{
					if (char.IsDigit(s[i]))
					{
						d2 = s[i]-'0';
						break;
					}
					var idx = digitnames.FirstOrDefault(dn => s[i..].StartsWith(dn));
					if (idx != null) {
						d2 = digitnames.IndexOf(name => name == idx);
						break;
					}
				}
				// var d1 = s.First(c => char.IsDigit(c));
				// var d2 = s.Last(c => char.IsDigit(c));
				var d = d1*10 + d2;
				sum += d;
			}

			return sum;
		}
	}
}
