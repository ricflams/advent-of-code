using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day07
{
	internal class Puzzle : PuzzleWithParam<(int,int), string, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Sum of Its Parts";
		public override int Year => 2018;
		public override int Day => 7;

		public void Run()
		{
			Run("test1").Part1("CABDFE");
			Run("input").Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);

			Run("test1").Part1("CABDFE");
			Run("input").Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);

			Run("test1").Part1("CABDFE");
			Run("input", (5, 1)).Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);
	//		RunParamOnly("input", (5, 1)).Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);

			// RunWithParam((2, 15)).Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);
			// RunWithParam((2, 15)).Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ");
			// RunWithParam((2, 15)).Part2(920);
			// Run("input", "GKPTSLUXBIJMNCADFOVHEWYQRZ", 920);
			// Run("input").WithParam((2, 15)).Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);
			// Run("input").Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);
			// Run("input").Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ");
			// Run("input").Part2(920);

			// For("input").Run("GKPTSLUXBIJMNCADFOVHEWYQRZ", 920);

			// , (2, 6)
			// RunFor("input").WithParams().Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(, 920).Run();
		}

		protected override string Part1(string[] input)
		{
			var work = ReadWorks(input);
			
			var order = "";
			while (work.Any())
			{
				var w = work.Values.Where(w => w.ReadyIn == 0).OrderBy(w => w.Letter).First();
				order += w.Letter;
				work.Remove(w.Letter);
				foreach (var dep in w.WillMakeAvailable)
				{
					work[dep].ReadyIn--;
				}
			}
			return order;
		}

		protected override int Part2(string[] input)
		{
			Console.WriteLine(Param);
			var work = ReadWorks(input);
			
			var order = "";

			var ongoing = new Dictionary<char, int>();
			var N = 6;

			var seconds = 0;

			while (true)
			{
				foreach (var o in ongoing.Keys.ToArray()) // todo - this is annoying
				{
					if (--ongoing[o] == 0)
					{
						order += o;
						ongoing.Remove(o);
						var w = work[o];
						foreach (var dep in w.WillMakeAvailable)
						{
							work[dep].ReadyIn--;
						}						
						work.Remove(w.Letter);
					}
				}

				if (!work.Any())
				{
					break;
				}
				
				while (ongoing.Count < N)
				{
					var w = work.Values
						.Where(w => w.ReadyIn == 0)
						.Where(w => !ongoing.ContainsKey(w.Letter))
						.OrderBy(w => w.Letter)
						.FirstOrDefault();
					if (w == null)
						break;
					ongoing[w.Letter] = 60 + w.Letter - 'A' + 1;
				}

				seconds++;
			}
			return seconds;
		}

		internal class Work
		{
			public Work(char letter, char[] canBegin) => (Letter, WillMakeAvailable) = (letter, canBegin);
			public char Letter { get; }
			public int ReadyIn { get; set; }
			public char[] WillMakeAvailable { get; }
		}

		public static Dictionary<char, Work> ReadWorks(string[] input)
		{
			var works = input
				.Select(line => line.RxMatch("Step %c must be finished before step %c can begin").Get<char, char>())
				.GroupBy(x => x.Item1)
				.ToDictionary(x => x.Key, x => new Work(x.Key, x.Select(z => z.Item2).ToArray()));

			var produces = works.Values.SelectMany(w => w.WillMakeAvailable).Distinct();
			var missing = produces.Except(works.Keys).ToArray();
			foreach (var leaf in missing)
			{
				works[leaf] = new Work(leaf, new char[0]);
			}

			foreach (var (c, work) in works)
			{
				work.ReadyIn = works.Count(x => x.Value.WillMakeAvailable.Contains(c));
			}

			return works;
		}


	}
}
