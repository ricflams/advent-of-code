using System;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2023.Day06
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Wait For It";
		public override int Year => 2023;
		public override int Day => 6;

		public override void Run()
		{
			Run("test1").Part1(288).Part2(71503);
			Run("input").Part1(4811940).Part2(30077773);
			Run("extra").Part1(6209190).Part2(28545089);
		}

		protected override long Part1(string[] input)
		{
			var time = input[0].Split(':')[1].ToIntArray();
			var dist = input[1].Split(':')[1].ToIntArray();

			var result = 1;
			for (var race = 0; race < time.Length; race++)
			{
				var t = time[race];
				var d = dist[race];
				result *= Wins(t, d);
			}

			return result;
		}

		protected override long Part2(string[] input)
		{
			var t = int.Parse(input[0].Split(':')[1].Replace(" ", ""));
			var d = long.Parse(input[1].Split(':')[1].Replace(" ", ""));

			return Wins(t, d);
		}

		private static int Wins(int t, long d)
		{
			// Using https://en.wikipedia.org/wiki/Quadratic_formula, ax² + bx + c = 0
			// Distance travelled after waiting x is t-x (the travel time) times x (the speed),
			// => (t-x)*x > d to win the race
			// => -x² + tx -d > 0
			// Use a=-1 b=t c=-d in the equation to solve as
			//    x = (-b ± √(b²-4ac)) / (2a)
			// => x = (-t ± √(t² - 4d)) / -2
			// => x = (t ± √(t² - 4d)) / 2
			var det = Math.Sqrt((long)t*t - 4*d);
			var x1 = (t - det) / 2;
			var x2 = (t + det) / 2;

			// x1,x2 is when the travelled distance is equal to d and we're looking for
			// values that exceeds d. It does that one step after x1 and one step before
			// x2, rounded down/up respectively. The wins is that range, ie t2-t1+1.
			var t1 = (int)Math.Floor(x1 + 1);
			var t2 = (int)Math.Ceiling(x2 - 1);
			var wins = t2 - t1 + 1;

			return wins;
		}
	}
}
