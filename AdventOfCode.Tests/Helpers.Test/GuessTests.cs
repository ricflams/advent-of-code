using Xunit;
using AdventOfCode.Helpers;

namespace AdventOfCode.Helpers.UnitTests
{
	public class GuessTests
	{
		[Fact]
		public void CanGuess()
		{
			for (var i = 1; i < 100; i++)
			{
				var guess = Guess.FindLowest(1, x => x >= i);
				Assert.Equal(i, guess);
			}
			for (var x = 1; x < 10; x++)
			{
				for (var i = x; i < x+10; i++)
				{
					var guess = Guess.FindLowest(x, x => x >= i);
					Assert.Equal(i, guess);
				}
			}
		}
	}
}