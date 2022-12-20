using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day20
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 20";
		public override int Year => 2022;
		public override int Day => 20;

		public void Run()
		{
			Run("test1").Part1(3).Part2(0);
			Run("input").Part1(0).Part2(0);

			// 11621 too high
		}

		protected override long Part1(string[] input)
		{
			return SumAfterRounds(input, 1);
		}

		protected override long Part2(string[] input)
		{
			return SumAfterRounds(input, 1);
		}

		private class Number
		{
			public int Value;
			public int Prev;
			public int Next;
		}

		private static int SumAfterRounds(string[] input, int rounds)
		{
			var numbers = input.Select(int.Parse).Select(x => new Number { Value = x }).ToArray();
			var N = numbers.Length;

			for (var i = 0; i < N; i++)
			{
				numbers[i].Prev = i-1;
				numbers[i].Next = i+1;
			}
			numbers[^1].Next = 0;
			numbers[0].Prev = N-1;

			//var startpos = 0;

			for (var j = 0; j < N*rounds; j++)
			{
				var k = j % N;
				var cur = numbers[k];
				var moveby = cur.Value % N;
				if (moveby == 0)
				{
					//Console.WriteLine($"{moveby} does not moves");
				}
				else
				{
					// if (k == startpos)
					// {
					// 	startpos = cur.Next;
					// 	Console.WriteLine($"New startpos at {startpos}");
					// }
					var pos = k;
					if (moveby < 0)
					{
						for (var i = 0; i < -moveby+1; i++)
						{
							pos = numbers[pos].Prev;
						}
					}
					else
					{
						for (var i = 0; i < moveby; i++)
						{
							pos = numbers[pos].Next;
						}
					}
					// if (pos == numbers[startpos].Prev)
					// {
					// 	Console.WriteLine($"  Move startpos from {startpos} to {pos}");
					// 	//startpos = pos;
					// }

					var prev = numbers[pos];
					//Console.WriteLine($"{cur.Value} moves between {prev.Value} and {numbers[prev.Next].Value}");

					numbers[cur.Prev].Next = cur.Next;
					numbers[cur.Next].Prev = cur.Prev;

					cur.Prev = pos;
					cur.Next = prev.Next;
					numbers[prev.Next].Prev = k;
					prev.Next = k;
				}

				// PrintNumbers();
				// Console.WriteLine();
			}

			// void PrintNumbers()
			// {
			// 	var pos = startpos;
			// 	for (var i = 0; i < N; i++)
			// 	{
			// 		Console.Write($"{numbers[pos].Value} ");
			// 		pos = numbers[pos].Next;
			// 	}
			// 	Console.WriteLine();
			// 	var reversed = new List<int>();
			// 	pos = numbers[startpos].Prev;
			// 	for (var i = 0; i < N; i++)
			// 	{
			// 		reversed.Add(numbers[pos].Value);
			// 		pos = numbers[pos].Prev;
			// 	}
			// 	reversed.Reverse();
			// 	foreach (var v in reversed)
			// 	{
			// 		Console.Write($"{v} ");
			// 	}
			// 	Console.WriteLine();
			// }

			var posi = numbers.IndexOf(x => x.Value == 0);
			var sum = 0;
			Console.WriteLine($"pos={posi}");
			for (var i = 0; i < 3; i++)
			{
				for (var j = 0; j < 1000; j++)
					posi = numbers[posi].Next;
				Console.WriteLine($"  found {numbers[posi].Value} at {posi}");
				sum += numbers[posi].Value;
			}

			return sum;			
		}
	}
}
