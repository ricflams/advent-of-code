using System;
using System.Linq;
using System.Diagnostics;

namespace AdventOfCode2019.Day02
{
	internal class Puzzle
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var result = new Intcode.Engine()
				.WithMemoryFromFile("Day02/input.txt")
				.WithMemoryValueAt(1, 12)
				.WithMemoryValueAt(2, 2)
				.Execute()
				.Memory[0];
			Console.WriteLine($"Day  2 Puzzle 1: mem[0] = {result}");
			Debug.Assert(result == 5866714);
		}

		private static void Puzzle2()
		{
			var engine = new Intcode.Engine()
				.WithMemoryFromFile("Day02/input.txt");

			// Store initial memory in array for much faster re-initialization in loop
			var mem = engine.Memory.Values.ToArray();

			var result = 0;
			for (var op1 = 0; op1 < 100; op1++)
			{
				for (var op2 = 0; op2 < 100; op2++)
				{
					var output = engine
						.WithMemory(mem)
						.WithMemoryValueAt(1, op1)
						.WithMemoryValueAt(2, op2)
						.Execute()
						.Memory[0];
					if (output == 19690720)
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