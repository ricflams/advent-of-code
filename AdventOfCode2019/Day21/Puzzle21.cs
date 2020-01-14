using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AdventOfCode2019.Helpers;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day21
{
	internal static class Puzzle21
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var program = @"
				NOT A T
				NOT T T
				AND B T
				AND C T
				NOT D J
				OR J T
				NOT T J
				WALK
			".MultiLine();
			// Console.WriteLine(ExecuteSpringdroidProgramForDebug(program));

			var hullDamage = FindHullDamage(program);
			Console.WriteLine($"Day 21 Puzzle 1: {hullDamage}");
			Debug.Assert(hullDamage == 19354083);
		}

		private static void Puzzle2()
		{
			//// Solve by brute force
			//Console.WriteLine("By brute force: " + FindHullDamageResultByBruteForce());

			//// Show last moments
			//foreach (var moment in LastMoments().Take(10))
			//{
			//	Console.WriteLine($"Last moment: {moment}");
			//}

			// Randomly picked solution
			var program = @"
				AND E J
				NOT I J
				OR I T
				NOT B J
				OR H T
				AND B T
				OR E T
				NOT C T
				AND J J
				AND H T
				OR T J
				AND D J
				NOT A T
				OR T J
				RUN
			".MultiLine();
			// Console.WriteLine(ExecuteSpringdroidProgramForDebug(program));

			var hullDamage = FindHullDamage(program);
			Console.WriteLine($"Day 21 Puzzle 2: {hullDamage}");
			Debug.Assert(hullDamage == 1143499964);
		}

		private static long FindHullDamage(string program)
		{
			return ExecuteSpringdroidProgram(program)
				.FirstOrDefault(x => x > 255);
		}

		private static long FindHullDamageResultByBruteForce()
		{
			while (true)
			{
				var program = RandomProgram.Generate();
				var damage = FindHullDamage(program);
				if (damage > 0)
				{
					return damage;
				}
			}
		}

		private static IEnumerable<string> LastMoments()
		{
			var lastMoments = new HashSet<string>();
			while (true)
			{
				var program = RandomProgram.Generate();
				var output = ExecuteSpringdroidProgram(program);
				if (!output.Any(c => c > 255))
				{
					var lines = new string(output.Select(v => (char)v).ToArray()).Split('\n');
					var lastMoment = lines.First(line => "#@".All(line.Contains));
					if (!lastMoments.Contains(lastMoment))
					{
						lastMoments.Add(lastMoment);
						yield return lastMoment;
					}
				}
			}
		}

		private static long[] ExecuteSpringdroidProgram(string program)
		{
			return new Engine()
				.WithMemoryFromFile("Day21/input.txt")
				.WithInput(program.Select(c => (long)c).ToArray())
				.Execute()
				.Output.TakeAll().ToArray();
		}

		private static string ExecuteSpringdroidProgramForDebug(string program)
		{
			var output = ExecuteSpringdroidProgram(program);
			return new string(output.Select(v => (char)v).ToArray());
		}

		private static class RandomProgram
		{
			private static readonly string[] Ops = new string[] { "NOT", "AND", "OR" };
			private static readonly string[] Src = new string[] { "B", "C", "E", "F", "G", "H", "I", "T", "J" };
			private static readonly string[] Dst = new string[] { "T", "J" };
			private static readonly Random Rand = new Random();
			private static string OneOf(string[] sa) => sa[Rand.Next(0, sa.Length)];

			public static string Generate()
			{
				var sb = new StringBuilder();
				for (var i = 0; i < 10; i++)
				{
					sb.Append($"{OneOf(Ops)} {OneOf(Src)} {OneOf(Dst)}\n");
				}
				sb.Append("OR T J\n");
				sb.Append("AND D J\n");
				sb.Append("NOT A T\n");
				sb.Append("OR T J\n");
				sb.Append("RUN\n");
				var program = sb.ToString();
				return program;
			}
		}
	}
}

