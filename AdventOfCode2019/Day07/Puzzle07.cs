using AdventOfCode2019.Helpers;
using System;
using System.Linq;
using System.Diagnostics;
using AdventOfCode2019.Intcode;

namespace AdventOfCode2019.Day07
{
	internal class Puzzle07
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var memory = Engine.ReadMemoryFromFile("Day07/input.txt");
			var maxSignal = MathHelper.Permute(Enumerable.Range(0, 5))
				.Max(phases =>
				{
					var signal = 0L;
					foreach (var phase in phases)
					{
						var engine = new Engine();
						engine
							.WithMemory(memory)
							.WithInput(phase, signal)
							.Execute();
						signal = engine.Output.Take();
					}
					return signal;
				});
			Console.WriteLine($"Day  7 Puzzle 1: {maxSignal}");
			Debug.Assert(maxSignal == 19650);
		}

		private static void Puzzle2()
		{
			var memory = Engine.ReadMemoryFromFile("Day07/input.txt");
			var maxSignal = MathHelper.Permute(Enumerable.Range(5, 5))
				.Max(phases =>
				{
					var engines = phases.Select(phase => new Engine().WithMemory(memory).WithInput(phase)).ToList();
					var n = engines.Count;
					for (var i = 0; i < n; i++)
					{
						engines[(i + n - 1) % n].Output = engines[i].Input;
					}
					engines[0].WithInput(0);
					return engines
						.AsParallel()
						.WithDegreeOfParallelism(n)
						.Select(e => e.Execute())
						.AsSequential()
						.First(e => e == engines.Last())
						.Output
						.Take();
				});
			Console.WriteLine($"Day  7 Puzzle 2: {maxSignal}");
			Debug.Assert(maxSignal == 35961106);
		}
	}
}