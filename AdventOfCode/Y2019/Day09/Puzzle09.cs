﻿using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;

namespace AdventOfCode.Y2019.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Sensor Boost";
		public override int Year => 2019;
		public override int Day => 9;

		public override void Run()
		{
			Run("input").Part1(2682107844).Part2(34738);
			Run("extra").Part1(2406950601).Part2(83239);
		}

		protected override long Part1(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithInput(1)
				.Execute()
				.Output.Take();
			return result;
		}

		protected override long Part2(string[] input)
		{
			var intcode = input[0];
			var result = new Engine()
				.WithMemory(intcode)
				.WithInput(2)
				.Execute()
				.Output.Take();
			return result;
		}
	}
}