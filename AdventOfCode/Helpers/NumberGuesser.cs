using System;

namespace AdventOfCode.Helpers
{
    internal class NumberGuesser
    {
		public enum GuessIs
		{
			TooLow,
			TooHigh,
			Correct
		};

		public static long Find(Func<long, GuessIs> evaluator)
		{
			for (var scale = 1L; scale > 0; scale *= 10)
			{
				switch (evaluator(scale))
				{
					case GuessIs.Correct: return scale; // quite unlikely
					case GuessIs.TooHigh: return Find(scale / 10, scale);
				}
			}
			throw new Exception("Number too high to guess");

			long Find(long begin, long end)
			{
				//Console.WriteLine($"Guess: Find {begin} {end}");
				var guess = (end + begin) / 2;
				var hint = evaluator(guess);
				if (hint == GuessIs.Correct || guess == begin)
				{
					// Will return "closest number below" if we can't guess exactly; improve if needed
					return guess;
				}
				return hint == GuessIs.TooHigh
					? Find(begin, guess)
					: Find(guess, end);
			}
		}
	}
}
