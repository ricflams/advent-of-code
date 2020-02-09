using AdventOfCode.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2015.Day20
{
	internal class Puzzle20
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = 36000000;

			int PacketsForHouseNumber(int house)
			{
				var n = Enumerable.Range(1, house).Sum(i => house % i == 0 ? house / i : 0);
				return n * 10;
			}

			var sq = (int)Math.Ceiling(Math.Sqrt(input));
			var packetsForHouse = Enumerable.Range(1, sq).Select(PacketsForHouseNumber).ToList();

			var house = -1;
			for (var i = 0; i < input; i++)
			{
				var index = i % sq;
				var p0 = packetsForHouse[index];
				var fact = i / sq + 1;
				var presents1 = fact * p0;
				var presents = PacketsForHouseNumber(i+1);
				if (presents != presents1)
					;
				if (presents >= input)
				{
					house = i;// + 1;
					var n = PacketsForHouseNumber(house);
					break;
				}
			} 

			//Console.WriteLine(PacketsForHouseNumber(2113823));
			//Console.WriteLine(PacketsForHouseNumber(2113824));

			//var house = Guess.Find(Guess.ValueIs.ExactOrHigherThan, input, PacketsForHouseNumber);

			//var res = 2113824;
			//var res = (int)Math.Log2(input);

			//var minhouse = int.MaxValue;
			//for (var i = 0; i < 1000; i++)
			//{
			//	var h = res - i;
			//	var packets = PacketsForHouseNumber(h);
			//	if (packets > input && h < minhouse)
			//	{
			//		Console.WriteLine($"Found {h} => {packets}");
			//		minhouse = h;
			//	}
			//	h = res + i;
			//	packets = PacketsForHouseNumber(h);
			//	if (packets > input && h < minhouse)
			//	{
			//		Console.WriteLine($"Found {h} => {packets}");
			//		minhouse = h;
			//	}
			//}


			Console.WriteLine($"Day 20 Puzzle 1: {house}");
			Debug.Assert(house == 831600);
		}

		private static void Puzzle2()
		{

			//Console.WriteLine($"Day 20 Puzzle 2: {result}");
			//Debug.Assert(result == );
		}
	}
}
