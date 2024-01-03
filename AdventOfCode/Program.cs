using AdventOfCode.Helpers.Puzzles;
using System;
using System.Diagnostics;

namespace AdventOfCode
{
	public class Program
	{
		static void Main(string[] _)
		{
			//AdventOfCode.Helpers.PuzzleDay.GeneratePuzzles.Generate(2023); return;

			var runner = new PuzzleRunner();

			runner.Options.RunOnly((year, day) =>
				false
				// || (year, day) == (2019, 18)
				// || (year, day) == (2016, 13)
				// || (year, day) == (2021, 12)
				// || (year, day) == (2022, 16)
				// || (year, day) == (2017, 12)
				//|| year == 2016 && day == 15
				|| (year, day) == (2023, 25)
			);

//runner.Options.OnlyRunForInputs = true;
			var iterations = 1;
			//iterations = 3;

			if (iterations == 1)
			{
				runner.Options.Iterations = 1;
				var sw = Stopwatch.StartNew();
				runner.RunAll();
				Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
			}
			else
			{
				Console.WriteLine($"Running {iterations} iterations...");
				//runner.Options..OnlyRunForInputs = true;
				runner.Options.Silent = true;
				runner.Options.Iterations = 1;
				runner.RunAll();
				runner.Options.Silent = false;
				runner.Options.Iterations = iterations;
				var sw = Stopwatch.StartNew();
				runner.RunAll();
				var elapsedMsec = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;
				Console.WriteLine($"Elapsed: {elapsedMsec / iterations:F2} ms");
			}

			//if (Debugger.IsAttached)
			//{
			//	Console.Write("Press any key to close ");
			//	Console.ReadKey();
			//}
		}
	}
}
