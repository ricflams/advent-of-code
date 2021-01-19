using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System.Linq;

namespace AdventOfCode.Y2019.Day07
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Amplification Circuit";
		public override int Year => 2019;
		public override int Day => 7;

		public void Run()
		{
			RunFor("input", 19650, 35961106);
		}

		protected override long Part1(string[] input)
		{
			var memory = Engine.ReadAsMemory(input[0]);
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
			return maxSignal;
		}

		protected override long Part2(string[] input)
		{
			var memory = Engine.ReadAsMemory(input[0]);
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
			return maxSignal;
		}
	}
}