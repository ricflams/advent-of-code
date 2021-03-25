using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Y2019.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Springdroid Adventure";
		public override int Year => 2019;
		public override int Day => 21;

		public void Run()
		{
			Run("input").Part1(19354083).Part2(1143499964);
		}


		private long[] _springdroidMemory;

		protected override long Part1(string[] input)
		{
			var intcode = input[0];
			_springdroidMemory = Engine.ReadAsMemory(intcode);

			// My first successful attempt, so let's keep that
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
			//Console.WriteLine(ExecuteSpringdroidProgramForDebug(program));

			var hullDamage = FindHullDamage(program);
			return hullDamage;
		}

		protected override long Part2(string[] input)
		{
			var intcode = input[0];
			_springdroidMemory = Engine.ReadAsMemory(intcode);


			//// Solve by brute force
			//for (var i = 12; i > 6; i--)
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
			return hullDamage;
		}

		private long FindHullDamage(string program)
		{
			return ExecuteSpringdroidProgram(program)
				.FirstOrDefault(x => x > 255);
		}

		private (long, int, string) FindHullDamageResultByBruteForce(int length)
		{
			var memo = new HashSet<uint>();
			var step = 0;
			while (true)
			{
				var program = RandomProgram.Generate(length);
				var signature = Hashing.Hash(program);
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

		private IEnumerable<string> LastMoments()
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

		private long[] ExecuteSpringdroidProgram(string program)
		{
			return new Engine()
				.WithMemory(_springdroidMemory)
				.WithInput(program.Select(c => (long)c).ToArray())
				.Execute()
				.Output.TakeAll().ToArray();
		}

		private string ExecuteSpringdroidProgramForDebug(string program)
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

