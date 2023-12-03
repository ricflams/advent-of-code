using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day03.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Rucksack Reorganization";
		public override int Year => 2022;
		public override int Day => 3;

		public override void Run()
		{
			Run("test1").Part1(157).Part2(70);
			Run("input").Part1(7701).Part2(2644);
		}

		protected override long Part1(string[] input)
		{
			var sum = 0;
			foreach (var i in input)
			{
				var len = i.Length/2;
				var a1 = i[0..len];
				var a2 = i[len..];
				foreach (var p in a1)
				{
					if (a2.Contains(p))
					{
						if ('a' <= p && p <= 'z')
							sum += (p - 'a' + 1);
						else if ('A' <= p && p <= 'Z')
							sum += (p - 'A' + 27);
						else
							throw new Exception();
						//Console.WriteLine($"{p} {sum}");
						break;
					}
				}
				// var xx = i.ToCharArray().GroupBy(x => x).Where(x => x.Count() > 1);
				// var xxx = xx.First();
				// var ch =xxx.First();
				// Console.WriteLine(ch);
			}

			return sum;
		}

		private int FindMatch(string[] input, int i)
		{
			foreach (var a1 in input[i])
			{
				foreach (var a2 in input[i+1])
				{
					if (a1 != a2)
						continue;
					if (input[i+2].Contains(a1))
					{
						var p = a1;
						if ('a' <= p && p <= 'z')
							return (p - 'a' + 1);
						else if ('A' <= p && p <= 'Z')
							return (p - 'A' + 27);									
						throw new Exception();
					}
				}				
			}		
			return 0;
		}

		protected override long Part2(string[] input)
		{
			var sum = 0;
			var n = input.Length;
			var groupts = new List<string[]>();

			for (var i = 0; i < n; i += 3)
			{
				var val = FindMatch(input, i);
				sum += val;
			}

			return sum;
		}
	}
}
