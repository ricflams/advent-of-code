using System;

namespace AdventOfCode.Helpers
{
    public static class Guess
    {
		public enum ValueIs
		{
			ExactOrHigherThan,
			ExactOrLowerThan,
			Exactly
		};

		public static int Find(ValueIs guessIs, int target, Func<int, int> function)
		{
			return (int)Find(guessIs, target, (long x) => function((int)x));
		}

		public static long Find(ValueIs guessIs, long target, Func<long, long> function)
		{
			for (var scale = 1L; scale > 0; scale *= 10)
			{
				var result = function(scale);
				if (result == target)
					return target; // quite unlikely
				if (result > target)
					return Find(scale / 10, scale);
			}
			throw new Exception("Number too high to guess");

			long Find(long begin, long end)
			{
				//Console.WriteLine($"Guess: Find {begin} {end}");
				var guess = (end + begin) / 2;
				var result = function(guess);
				if (result == target)
				{
					return guess; // bingo
				}
				if (guess == begin)
				{
					// No more guesses - we didn't find it
					switch (guessIs)
					{
						case ValueIs.Exactly:
							throw new Exception("No exact match");
						case ValueIs.ExactOrHigherThan:
							while (result < target)
							{
								result = function(++guess);
							}
							return guess;
						case ValueIs.ExactOrLowerThan:
							while (result > target)
							{
								result = function(--guess);
							}
							return guess;
					}
				}
				return result > target
					? Find(begin, guess)
					: Find(guess, end);
			}
		}

		public static int FindLowest(int start, Func<int, bool> function)
		{
			return (int)FindLowest(start, (long x) => function((int)x));
		}

		public static long FindLowest(long start, Func<long, bool> function)
		{
			while (true)
			{
				if (function(start))
					break;
				start *= 2;
			}

			var highEnough = start;
			var tooLow = start / 2;
			while (true)
			{
				var v = (highEnough + tooLow) / 2;
				if (function(v))
				{
					highEnough = v;
				}
				else
				{
					tooLow = v;
				}
				if (tooLow + 1 == highEnough)
					break;
			}
			return highEnough;
		}

	}
}
