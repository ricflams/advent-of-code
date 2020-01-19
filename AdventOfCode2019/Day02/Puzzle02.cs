using System;
using System.Diagnostics;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day02
{
	internal class Puzzle02
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var result = new Engine()
				.WithMemoryFromFile("Day02/input.txt")
				.WithMemoryValueAt(1, 12)
				.WithMemoryValueAt(2, 2)
				.Execute()
				.ReadMemory(0);
			Console.WriteLine($"Day  2 Puzzle 1: {result}");
			Debug.Assert(result == 5866714);
		}

		private static void Puzzle2()
		{
			var memory = Engine.ReadMemoryFromFile("Day02/input.txt");
			var desiredOutput = 19690720;

			var result = 0;
			var engine = new Engine();
			for (var op1 = 0; op1 < 100; op1++)
			{
				for (var op2 = 0; op2 < 100; op2++)
				{
					var output = engine
						.WithMemory(memory)
						.WithMemoryValueAt(1, op1)
						.WithMemoryValueAt(2, op2)
						.Execute()
						.ReadMemory(0);
					if (output == desiredOutput)
					{
						result = op1 * 100 + op2;
						break;
					}
				}
			}
			Console.WriteLine($"Day  2 Puzzle 2: {result}");
			Debug.Assert(result == 5208);
		}
	}
}