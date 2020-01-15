using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
			// My first successful attempt
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

			//// Shortest program found by brute force
			//program = @"
			//	NOT A J
			//	NOT C T
			//	OR T J
			//	AND D J
			//	WALK
			//".MultiLine();

			// Console.WriteLine(ExecuteSpringdroidProgramForDebug(program));

			var hullDamage = FindHullDamage(program);
			Console.WriteLine($"Day 21 Puzzle 1: {hullDamage}");
			Debug.Assert(hullDamage == 19354083);
		}

		private static void Puzzle2()
		{
			//// Solve by brute force
			//for (var i = 10; i > 6; i--)
			//{
			//	var sw = Stopwatch.StartNew();
			//	Console.Write($"Program of length {i} ...");
			//	var (damage, steps, prog) = FindHullDamageResultByBruteForce(i);
			//	Debug.Assert(damage == 1143499964);
			//	Console.WriteLine($"found in {sw.Elapsed} after {steps} programs:");
			//	Console.WriteLine(prog);
			//}

			//// Show last moments
			//foreach (var moment in LastMoments().Take(10))
			//{
			//	Console.WriteLine($"Last moment: {moment}");
			//}

			// Randomly picked short solution
			var program = @"
				NOT B J
				NOT C T
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

		private static (long, int, string) FindHullDamageResultByBruteForce(int length)
		{
			var md5 = MD5.Create();
			var memo = new HashSet<ulong>();
			var step = 0;
			while (true)
			{
				var program = RandomProgram.Generate(length);
				var signature = md5.ComputeHash(Encoding.ASCII.GetBytes(program)).Select(b => (ulong)b).Aggregate((s,v) => 3074457345618258799ul*s+v);
				if (memo.Contains(signature))
				{
					continue;
				}
				memo.Add(signature);
				var damage = FindHullDamage(program);
				if (damage > 0)
				{
					return (damage, step, program);
				}
				step++;
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
			private static readonly Random Rand = new Random();
			private static readonly string[] Ops = new string[] { "NOT", "AND", "OR" };
			private static readonly string[] Src = new string[] { "B", "C", "E", "F", "G", "H", "I", "T", "J" };
			private static readonly string[] Dst = new string[] { "T", "J" };
			private static string OneOf(string[] sa) => sa[Rand.Next(0, sa.Length)];

			public static string Generate(int length = 15)
			{
				var lastInstructions = new string[]
				{
					"OR T J",
					"AND D J",
					"NOT A T",
					"OR T J",
					"RUN"
				};
				var lastOpForDst = Dst.ToDictionary(x => x, _ => "");
				var sb = new StringBuilder();
				for (var step = 0; step < length - lastInstructions.Length; step++)
				{
					string op, src, dst;
					while (true)
					{
						op = OneOf(Ops);
						src = OneOf(Src);
						dst = OneOf(Dst);
						if (step == 0 && op == "AND") // AND x y as first step will always produce 0 because J/T are both 0
							continue;
						if (op != "NOT" && src == dst) // AND/OR x x will always produce x
							continue;
						if (op == "NOT" && lastOpForDst[dst] == "NOT") // Don't do NOT x J/T if last operation to J/T was also a NOT
							continue;
						break;
					}
					sb.Append($"{op} {src} {dst}\n");
					lastOpForDst[dst] = op;
				}
				foreach (var instruction in lastInstructions)
				{
					sb.AppendFormat($"{instruction}\n");
				}
				var program = sb.ToString();
				return program;
			}
		}
	}
}

