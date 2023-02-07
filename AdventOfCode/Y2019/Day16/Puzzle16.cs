using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2019.Day16
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Flawed Frequency Transmission";
		public override int Year => 2019;
		public override int Day => 16;

		public void Run()
		{
			Run("input").Part1(58672132).Part2(91689380);
		}

		protected override int Part1(string[] input)
		{
			var rawinput = input[0];
			var signal = rawinput.Select(x => x - '0').ToArray();
			var result = Fft(100, signal).AsNumberFromDigits(8);
			return result;
		}

		protected override int Part2(string[] input)
		{
			var rawinput = input[0];
			var numinput = rawinput.Select(x => x - '0').ToArray();
			var N = rawinput.Length;

			var offset = int.Parse(rawinput.Substring(0, 7));
			var length = N * 10000 - offset;
			var input2 = new int[length];
			for (var i = 0; i < length; i++)
			{
				input2[i] = numinput[(offset + i) % N];
			}
			var result = FFt2(100, input2).AsNumberFromDigits(8);
			return result;
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

			static IEnumerable<int> FactorGenerator(int phase)
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
	}

	internal static class Extensions
	{
		public static int AsNumberFromDigits(this IEnumerable<int> input, int length)
		{
			return input.Take(length).Aggregate((sum, digit) => sum * 10 + digit);
		}
	}
}

