using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day07
{
	internal class Puzzle : PuzzleWithParameter<(int,int), string, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Sum of Its Parts";
		public override int Year => 2018;
		public override int Day => 7;

		public void Run()
		{
			// For test: 1 helper and +0 sec
			// For real input: 5 helpers and +60 sec
			Run("test1").WithParameter((1,  0)).Part1("CABDFE").Part2(15);
			Run("input").WithParameter((5, 60)).Part1("GKPTSLUXBIJMNCADFOVHEWYQRZ").Part2(920);
		}

		protected override string Part1(string[] input)
		{
			var work = ReadWorks(input);
			
			// Pick the first letter (alphabetically) that is available, add it to the
			// result and remove it from the work-set, and then countdown each of the
			// letters it enables so they too eventually wil become eligible for picking
			var result = "";
			while (work.Any())
			{
				var w = work.Values.Where(w => w.CountdownForReady == 0).OrderBy(w => w.Letter).First();
				result += w.Letter;
				work.Remove(w.Letter);
				foreach (var dep in w.WillEnable)
				{
					work[dep].CountdownForReady--;
				}
			}
			return result;
		}

		protected override int Part2(string[] input)
		{
			var work = ReadWorks(input);
			var (helpers, extraSeconds) = PuzzleParameter;
			
			// Keep track of the ongoing work,time here
			var ongoing = new Dictionary<char, int>();
			var workers = 1 + helpers;

			// Let the seconds tick by. For each second, process any ongoing work and
			// get rid of it once its time has elapsed; meaning remove it from the ongoing
			// tasks, countdown the work that it enables, and remove it from the work-set.
			var seconds = 0;
			while (true)
			{
				foreach (var o in ongoing.Keys.ToArray())
				{
					if (--ongoing[o] == 0) // time's up
					{
						ongoing.Remove(o);
						foreach (var c in work[o].WillEnable)
						{
							work[c].CountdownForReady--;
						}						
						work.Remove(o);
					}
				}

				// If there's no more work then we're done, so bail before checking for more
				// work to do and before counting up the seconds
				if (!work.Any())
				{
					break;
				}
				
				// As long as there is room for more workers and there are letters ready that
				// are not yet being worked on then keep on taking up new ongoing work
				while (ongoing.Count < workers)
				{
					var w = work.Values
						.Where(w => w.CountdownForReady == 0)
						.Where(w => !ongoing.ContainsKey(w.Letter))
						.OrderBy(w => w.Letter)
						.FirstOrDefault();
					if (w == null)
						break;
					ongoing[w.Letter] = extraSeconds + w.Letter - 'A' + 1;
				}

				// Pass the time
				seconds++;
			}

			return seconds;
		}

		internal class Work
		{
			public Work(char letter, char[] willEnable) => (Letter, WillEnable) = (letter, willEnable);
			public char Letter { get; }
			public int CountdownForReady { get; set; }
			public char[] WillEnable { get; }
		}

		public static Dictionary<char, Work> ReadWorks(string[] input)
		{
			// Read all the work-items and add the missing leafs (there seem to just be one
			// for both test and real input, but let's just do it right)
			var works = input
				.Select(line => line.RxMatch("Step %c must be finished before step %c can begin").Get<char, char>())
				.GroupBy(x => x.Item1)
				.ToDictionary(x => x.Key, x => new Work(x.Key, x.Select(z => z.Item2).ToArray()));
			var willEnable = works.Values.SelectMany(w => w.WillEnable).Distinct();
			var leafs = willEnable.Except(works.Keys).ToArray();
			foreach (var letter in leafs)
			{
				works[letter] = new Work(letter, new char[0]); // will enable nothing`
			}

			// Count how many "enablings" it takes for each work-node to be fully enabled
			foreach (var (c, work) in works)
			{
				work.CountdownForReady = works.Count(x => x.Value.WillEnable.Contains(c));
			}

			return works;
		}
	}
}
