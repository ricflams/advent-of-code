using System;

namespace AdventOfCode2019.Helpers
{
    internal class NumberGuesser
    {
		private static readonly int MaxExp = (int)Math.Log10(long.MaxValue);

		public enum GuessIs
		{
			TooLow,
			TooHigh,
			Correct
		};

		public static long[] Find(Func<long, GuessIs> evaluator)
		{
			var guess = 1;
			for (var i = 0; i < MaxExp; i++)
			{
				guess *= 10;
				switch (evaluator(guess))
				{
					case GuessIs.Correct: return new long[] { guess };
					case GuessIs.TooHigh: return Find(guess / 10, guess / 10, evaluator);
				}
			}
			throw new Exception("Number is too high");
		}

		private static long[] Find(long guessbase, long delta, Func<long, GuessIs> evaluator)
		{
			if (delta == 0)
			{
				return new long[] { guessbase, guessbase + 1 };
			}
			for (var i = 0; i < 10; i++)
			{
				var guess = guessbase + delta * i;
				switch (evaluator(guess))
				{
					case GuessIs.Correct: return new long[] { guess };
					case GuessIs.TooHigh: return Find(guess - delta, delta / 10, evaluator);
				}
			}
			throw new Exception("Error in guesser");
		}
	}
}
