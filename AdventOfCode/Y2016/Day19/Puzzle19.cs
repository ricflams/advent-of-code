using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2016.Day19
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "An Elephant Named Joseph";
		public override int Year => 2016;
		public override int Day => 19;

		public void Run()
		{
			RunFor("test1", 3, 2);
			RunFor("input", 1834471, 1420064  );
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
			var N = int.Parse(input[0]);

			// Run the game in a number of passes. At each pass, mark the givers (players being
			// taken from) with 0 inside the array. When we reach the end, re-create the set of
			// players to weed out those 0s followed by the so-far untouched takers.
			// The formula for finding the giver halfway through the array is a bit tricky since
			// the array isn't resized. With resizing it would be:
			//   giver = taker + N/2
			// But because we don't remove the givers the desired position is too far off. It's
			// too far off by the number of givers we've neglected to remove, and also the size
			// of the array is too big by that same number. So let's correct for that:
			//   giver = taker + unremovedRemoveGivers + (N-unremovedRemoveGivers)/2
			// Now, for every taker we have an unremoved giver, ie they are exactly the same:
			//   giver = taker + taker + (N - takers)/2
			// Thus:
			static int TakeFrom(int taker, int N) =>  2*taker + (N-taker)/2;

			var players = Enumerable.Range(1, N).ToArray();
			for (var taker = 0;; taker++)
			{
				var giver = TakeFrom(taker, N);
				if (giver >= N)
				{
					// We're at the end, so compact the array. For the large input this will happen
					// about 35 times, which is acceptable. We're done when there's one player left.
					players = players[taker..].Where(x => x != 0).Concat(players[0..taker]).ToArray();
					N = players.Length;
					if (N == 1)
					{
						break;
					}
					taker = 0;
					giver = TakeFrom(taker, N);
				}
				players[giver] = 0;
			}

			var result = players[0]; // The one remaining player
			return result;
		}
	}
}
