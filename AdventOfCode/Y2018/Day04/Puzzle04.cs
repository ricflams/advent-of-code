using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Repose Record";
		public override int Year => 2018;
		public override int Day => 4;

		public void Run()
		{
			Run("test1").Part1(240).Part2(4455);
			Run("input").Part1(12169).Part2(16164);
		}

		protected override int Part1(string[] input)
		{
			var guards = ReadGuards(input);

			// Sleepiest guard is the one with the highest total sleep
			var sleepiestGuard = guards
				.OrderByDescending(x => x.Duties.Sum(z => z.TotalSleep))
				.First();

			// For every minute find out how many times the guard was asleep at
			// the duties. For the highest number seen, remember the minute.
			var maxsleep = 0;
			var minute = 0;
			var duties = sleepiestGuard.Duties;
			for (var t = 0; t < 60; t++)
			{
				var sleep = duties.Sum(g => g.SleepAtMinute[t]);
				if (sleep > maxsleep)
				{
					maxsleep = sleep;
					minute = t;
				}
			}

			var result = sleepiestGuard.Id * minute;
			return result;
		}


		protected override int Part2(string[] input)
		{
			var guards = ReadGuards(input);

			// For every minute examine all guards, whecking how many times each
			// one of them is asleep at that minute. For the highest number of times
			// seen, remember the guard id and minute.
			var maxsleep = 0;
			var id = 0;
			var minute = 0;
			for (var t = 0; t < 60; t++)
			{
				foreach (var g in guards)
				{
					var sleep = g.Duties.Sum(x => x.SleepAtMinute[t]);
					if (sleep > maxsleep)
					{
						maxsleep = sleep;
						id = g.Id;
						minute = t;
					}
				}
			}

			var result = id * minute;
			return result;
		}

		internal class Guard
		{
			public Guard(int id, Duty[] duties) => (Id, Duties) = (id, duties);
			public int Id { get; }
			public Duty[] Duties { get; }
		}

		internal class Duty
		{
			private int _sleeptime;
			public int TotalSleep { get; private set; }
			public int[] SleepAtMinute { get; } = new int[60];
			public void DoFallAsleep(int time) => _sleeptime = time;
			public void DoWakeUp(int time)
			{
 				TotalSleep += time - _sleeptime;
				for (var t = _sleeptime; t < time; t++) 
				{
					SleepAtMinute[t] = 1;
				}
			}
		}

		private static Guard[] ReadGuards(string[] input)
		{
			// Loop through all input lines in sorted order (that's the time order, too)
			// and build up the list of guard-duties. Afterwardds, convert it into an
			// array of guards, each with the id and that guard's duties.
			var duties = new List<(int, Duty)>();
			foreach (var line in input.OrderBy(x => x))
			{
				// [1518-11-01 00:00] Guard #10 begins shift
				// [1518-11-01 00:05] falls asleep
				// [1518-11-01 00:25] wakes up
				var p = line.Split("] ");
				var (hour, min) = p[0].RxMatch(" %d:%d").Get<int, int>();
				var time = hour == 23 ? 0 : min;
				var action = p[1];

				if (action == "falls asleep")
				{
					duties.Last().Item2.DoFallAsleep(time);
				}
				else if (action == "wakes up")
				{
					duties.Last().Item2.DoWakeUp(time);
				}
				else
				{
					var id = action.RxMatch("Guard #%d").Get<int>();
					duties.Add((id, new Duty()));
				}
			}
			var guards = duties
				.GroupBy(x => x.Item1)
				.Select(x => new Guard(x.Key, x.Select(z => z.Item2).ToArray()))
				.ToArray();
			return guards;
		}
	}
}
