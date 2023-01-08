using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day25.Raw
{
	internal class Puzzle : Puzzle<string, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 25";
		public override int Year => 2022;
		public override int Day => 25;

		public void Run()
		{
			Run("test1").Part1("2=-1=0").Part2(0);
			//Run("test2").Part1(0).Part2(0);
			
			Run("input").Part1("122-2=200-0111--=200").Part2(0);
		}

		protected override string Part1(string[] input)
		{
			// =-012

			// 1=-0-2
			// 12111
			// 2=0=
			// 21
			// 2=01
			// 111
			// 20012
			// 112
			// 1=-1=
			// 1-12
			// 12
			// 1=
			// 122	


			var digits = "=-012";
			var sum = 0L;
			foreach (var s in input)		
			{
				var v = 0L;
				foreach (var ch in s)
				{
					var x = digits.IndexOf(ch)-2;
					v = v*5 + x;
				}
				//Console.WriteLine(v);
				sum += v;
			}

			var result = "";
	//		var exp = 1;
			while (sum > 0)
			{
				var dig = (int)( ((sum+2) % 5) - 2 );
				result = digits[dig+2] + result;
				sum = (sum -dig) / 5;// + dig*exp;
				//exp*=5;
			}
			return result;
		}

		protected override int Part2(string[] _) => 0;
	}
}
