using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day16
{
	internal static class Puzzle
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var rawinput = File.ReadAllText("Day16/input.txt")
				.ToArray().Select(x => x - '0').ToArray();

			var output = Fft(100, rawinput).Take(8);
			var output2 = string.Join("", output);
			Console.WriteLine(output2);
			Debug.Assert(output2 == "58672132");

			//for (var i = 0; i < 8; i++)
			//{
			//	var digit = Fft2(100, rawinput, i);
			//	Console.Write(digit);
			//}
			Console.WriteLine();

		}

		private static void Puzzle2()
		{
			var rawinput = File.ReadAllText("Day16/input.txt")
				.ToArray().Select(x => x - '0').ToArray();

			//var phase1 = FftTestUnitMatrix(1, rawinput).ToArray();
			//var phase2 = FftTestUnitMatrix(2, rawinput).ToArray();
			//var phase3 = FftTestUnitMatrix(3, rawinput).ToArray();
			//var phase4 = FftTestUnitMatrix(4, rawinput).ToArray();
			//var phase5 = FftTestUnitMatrix(5, rawinput).ToArray();
			//Debug.Assert(phase2.SequenceEqual(phase3));
			//Debug.Assert(phase3.SequenceEqual(phase4));
			//Debug.Assert(phase4.SequenceEqual(phase5));

			//Console.WriteLine(string.Join("", rawinput.Take(50)));
			//Console.WriteLine(string.Join("", phase1.Take(50)));
			//Console.WriteLine(string.Join("", phase2.Take(50)));
			//Console.WriteLine(string.Join("", phase3.Take(50)));
			//Console.WriteLine(string.Join("", phase4.Take(50)));
			//Console.WriteLine(string.Join("", phase5.Take(50)));


			Console.WriteLine(string.Join("", rawinput.Take(50)));
			for (var phases = 1; phases < 10; phases++)
			{
				var phase = FftTestDiagonal(phases, rawinput).ToArray();
				Console.WriteLine(string.Join("", phase.Take(50)));
			}

			//var output = Fft(100, rawinput).Take(8);
			//var output2 = string.Join("", output);
			//Console.WriteLine(output2);
			//Debug.Assert(output2 == "58672132");







			//var output = Fft2(2, rawinput).Take(8);
			//var output2 = string.Join("", output);
			//Console.WriteLine(output2);
			//Debug.Assert(output2 == "58672132");


			//var rawinput = File.ReadAllText("Day16/input.txt")
			//	.ToArray().Select(x => x - '0').ToArray();

		}

		private static int[] Fft2(int phases, int[] input)
		{
			var N = input.Length;
			var nfactors = Enumerable.Range(1, N)
				.Select(phase => FactorGenerator(phase).Skip(1).Take(N).ToArray())
				.Select((x, phase) => x.Select((v,idx) => idx < phase ? 0 : idx == phase ? 1 : v * phases).ToArray())
				.ToArray();

			//for (var phase = 0; phase < phases; phase++)
			//{
				var output = Enumerable.Range(0, N)
					.Select(i =>
					{
						//var factors = FactorGenerator(i).Skip(1).Take(input.Length).ToArray();
						return Math.Abs(input.Select((v, idx) => v * nfactors[i][idx]).Sum()) % 10;
					})
					.ToArray();
			//}
			return output;



			//long inputsum = 0;

			//var output = new List<int>();
			//for (var i = N; i-- > 0; )
			//{
			//	inputsum += input[i];
			//	output.Add((int)(Math.Abs(inputsum * nfactors[i]) % 10));
			//}
			//output.Reverse();
			//return output.ToArray();

			IEnumerable<int> FactorGenerator(int phase)
			{
				var pattern = new int[] { 0, 1, 0, -1 };
				while (true)
				{
					foreach (var p in pattern)
					{
						for (var i = 0; i < phase; i++)
						{
							yield return p;
						}
					}
				}
			}

			int modpow(int basex, int exp, int modulus)
			{
				basex %= modulus;
				int result = 1;
				while (exp > 0)
				{
					if (exp % 2 == 1) result = (result * basex) % modulus;
					basex = (basex * basex) % modulus;
					exp /= 2;
				}
				return result;
			}
		}


		private static int[] Fft(int phases, int[] input)
		{
			for (var phase = 0; phase < phases; phase++)
			{
				input = Enumerable.Range(1, input.Length)
					.Select(i =>
					{
						var factors = FactorGenerator(i).Skip(1).Take(input.Length).ToArray();
						return Math.Abs(input.Select((v, idx) => v * factors[idx]).Sum()) % 10;
					})
					.ToArray();
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



		private static int[] FftTestUnitMatrix(int phases, int[] input)
		{
			for (var phase = 0; phase < phases; phase++)
			{
				input = Enumerable.Range(1, input.Length)
					.Select(i =>
					{
						//var factors = FactorGenerator(i).Skip(1).Take(input.Length).ToArray();
						return input.Sum() % 10;
					})
					.ToArray();
			}
			return input;
		}

		private static int[] FftTestDiagonal(int phases, int[] input)
		{
			for (var phase = 0; phase < phases; phase++)
			{
				input = Enumerable.Range(1, input.Length)
					.Select(i =>
					{
						var factors = FactorGenerator(i).Skip(1).Take(input.Length).ToArray();
						return Math.Abs(input.Select((v, idx) => v * factors[idx]).Sum()) % 10;
					})
					.ToArray();
			}
			return input;

			IEnumerable<int> FactorGenerator(int phase)
			{
				for (var i = 0; i < phase; i++)
				{
					yield return 0;
				}
				while (true)
				{
					yield return 1;
				}
			}
		}



	}
}

