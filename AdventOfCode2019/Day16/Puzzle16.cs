using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Day16
{
	internal static class Puzzle16
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var input = File.ReadAllText("Day16/input.txt")
				.ToArray().Select(x => x - '0').ToArray();

			var result = Fft(100, input).AsNumberFromDigits(8);
			Console.WriteLine($"Day 16 Puzzle 1: {result}");
			Debug.Assert(result == 58672132);
		}

		private static void Puzzle2()
		{
			var rawinput = File.ReadAllText("Day16/input.txt");
			var numinput = rawinput.ToArray().Select(x => x - '0').ToArray();
			var N = rawinput.Length;

			var offset = int.Parse(rawinput.Substring(0, 7));
			var length = N * 10000 - offset;
			var input = new int[length];
			for (var i = 0; i < length; i++)
			{
				input[i] = numinput[(offset + i) % N];
			}
			var result = FFt2(100, input).AsNumberFromDigits(8);
			Console.WriteLine($"Day 16 Puzzle 2: {result}");
			Debug.Assert(result == 91689380);
		}

		private static int[] Fft(int phases, int[] input)
		{
			var N = input.Length;

			// Generate factors up front just once
			var factors = Enumerable.Range(1, N)
				.Select(i => FactorGenerator(i).Skip(1).Take(input.Length).ToArray())
				.ToArray();

			for (var phase = 0; phase < phases; phase++)
			{
				var next = new int[N];
				for (var i = 0; i < N; i++)
				{
					next[i] = Math.Abs(input.Select((v, idx) => v * factors[i][idx]).Sum()) % 10;
				}
				input = next;
			}
			return input;

			IEnumerable<int> FactorGenerator(int phase)
			{
				var pattern = new int[] { 0, 1, 0, -1 };
				while (true)
				{
					foreach (var value in pattern)
					{
						for (var i = 0; i < phase; i++)
						{
							yield return value;
						}
					}
				}
			}
		}

		private static int[] FFt2(int phases, int[] input)
		{
			var N = input.Length;
			for (var phase = 0; phase < phases; phase++)
			{
				var next = new int[N];
				next[N - 1] = input[N - 1];
				for (var i = N - 1; i-- > 0; )
				{
					next[i] = (input[i] + next[i + 1]) % 10;
				}
				input = next;
			}
			return input;
		}

		private static int AsNumberFromDigits(this IEnumerable<int> input, int length)
		{
			return input.Take(length).Aggregate((sum, digit) => sum * 10 + digit);
		}
	}
}

