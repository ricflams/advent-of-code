using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2018.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Marble Mania";
		public override int Year => 2018;
		public override int Day => 9;

		public void Run()
		{
			Run("test1").Part1(8317);
			Run("test2").Part1(146373);
			Run("test3").Part1(2764);
			Run("test4").Part1(54718);
			Run("test5").Part1(37305);
			Run("input").Part1(371284).Part2(3038972494);
		}

		protected override long Part1(string[] input)
		{
			var (N, worth) = input[0].RxMatch("%d players; last marble is worth %d").Get<int, int>();
			return CalcScore(N, worth);
		}

		protected override long Part2(string[] input)
		{
			var (N, worth) = input[0].RxMatch("%d players; last marble is worth %d").Get<int, int>();
			return CalcScore(N, worth * 100);
		}

		private static long CalcScore(int players, int marbles)
		{
			// Keep track of scores for each player, 0-N
			var scores = new long[players];

			// Low-overhead linked list:
			// Every entry for x in both prev and next are pointers to what goes
			// before/after the value x. Initially all points to 0, including 0
			// itself so all is fine from the start.
			var next = new int[marbles+1];
			var prev = new int[marbles+1];

			var current = 0;
			for (var m = 1; m <= marbles; m++)
			{
				if (m % 23 == 0)
				{
					// Find the 7th previous marble and add that value, plus the value
					// of m (we discard that marble entirely, btw) to the player's score.
					for (var i = 0; i < 7; i++)
					{
						current = prev[current];
					}
					var player = m % players;
					scores[player] += m + current;

					// Delete the now current marble, simply by letting its prev/next
					// marbles bypass it by setting their next/prev pointers to that
					// of the marble. Then, set the current marble to the next one.
					var mprev = prev[current];
					var mnext = next[current];
					next[mprev] = mnext;
					prev[mnext] = mprev;
					current = mnext;
				}
				else
				{
					// We know for sure that m will always be a new marble. Pick the
					// two next marbles (next1 and next2) from the current one; then
					// set the current marble to m and insert it into the linked list
					// between next1 and next2.
					var next1 = next[current];
					var next2 = next[next1];
					current = m;
					prev[current] = next1;
					next[current] = next2;
					prev[next2] = current;
					next[next1] = current;
				}
			}

			// Return the highest score recorded for any player
			var score = scores.Max();
			return score;			
		}
	}
}
