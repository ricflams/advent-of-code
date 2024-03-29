﻿using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System;

namespace AdventOfCode.Y2019.Day02
{
	internal class Puzzle : Puzzle<long, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "1202 Program Alarm";
		public override int Year => 2019;
		public override int Day => 2;

		public override void Run()
		{
			Run("input").Part1(5866714).Part2(5208);
			Run("extra").Part1(10566835).Part2(2347);
		}

		protected override long Part1(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithMemoryValueAt(1, 12)
				.WithMemoryValueAt(2, 2)
				.Execute()
				.ReadMemory(0);
			return result;
		}

		protected override int Part2(string[] input)
		{
			var intcode = input[0];
			var memory = Engine.ReadAsMemory(intcode);
			var desiredOutput = 19690720;

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
						return op1 * 100 + op2;
					}
				}
			}

			throw new Exception("No result found");
		}
	}
}