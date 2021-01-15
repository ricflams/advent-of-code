using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2016.Day19
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "An Elephant Named Joseph";
		public override int Year => 2016;
		public override int Day => 19;

		public void Run()
		{
			RunFor("test1", 3, 2);
			RunFor("input", 1834471, 0);
		}

		protected override int Part1(string[] input)
		{
			var N = int.Parse(input[0]);

			var vacant = new bool[N];

			int NextPlayer(int i)
			{
				// Search for up to N and then from 0 again, to avoid doing modulus
				// for every increment; it's much, much faster.
				while (true)
				{
					while (i < N)
					{
						if (!vacant[i])
						{
							return i;
						}
						i = i + 1;
					}
					i = 0;
				}
			}

			// Run all N steps of the game so the winning player will end up
			// as the last giver; the math works easiest that way.
			// We can look for takers and givers in increasing steps of 2^n;
			// the first round will take from every second player, then from every
			// 4th player, every 8th, etc. Every time we arrive at a situation
			// where the player has left or the giver has left, we simply find the
			// next player and bump the step by 2x. There wasn't any clear pattern
			// in those increments so just search linearly; it only happens a few
			// times, like about 20 times for the real input.
			int taker = 0, giver = 0, step = 1;
			for (var k = 0; k < N; k++)
			{
				if (vacant[taker])
				{
					taker = NextPlayer(taker);
					step *= 2;
				}
				giver = (taker + step) % N;

				if (vacant[giver])
				{
					giver = NextPlayer(giver);
					step *= 2;
				}
				taker = (giver + step) % N;

				vacant[giver] = true;
			}

			var result = giver + 1; // Game is 1-based
			return result;
		}

		protected override int Part2(string[] input)
		{

			// var N = int.Parse(input[0]);

			// var players = Enumerable.Range(0, );

			// int NextPlayer(int i)
			// {
			// 	// Search for up to N and then from 0 again, to avoid doing modulus
			// 	// for every increment; it's much, much faster.
			// 	while (true)
			// 	{
			// 		while (i < N)
			// 		{
			// 			if (!vacant[i])
			// 			{
			// 				return i;
			// 			}
			// 			i = i + 1;
			// 		}
			// 		i = 0;
			// 	}
			// }

			// int taker = 0, giver = 0, step = 1;
			// while (N > 1)
			// {
			// 	if (vacant[taker])
			// 	{
			// 		taker = NextPlayer(taker);
			// 		//step *= 2;
			// 	}
			// 	giver = (taker + N/2) % N;

			// 	if (vacant[giver])
			// 	{
			// 		giver = NextPlayer(giver);
			// 		//step *= 2;
			// 	}
			// 	taker = (giver + step) % N;

			// 	var vacant2 = new bool[N-1];
			// 	for (int i = 0, j = 0; i < N; i++, j++)
			// 	{
			// 		if (i == giver)
			// 			i++;
			// 		vacant2[j] = vacant[i];
			// 	}
			// 	vacant = vacant2;

			// 	N--;
			// 	taker = taker % N;
			// 	//giver = giver % N;
			// }
			// var result = giver + 1; // Game is 1-based


			// return result;

			return 0;
		}
	}
}
