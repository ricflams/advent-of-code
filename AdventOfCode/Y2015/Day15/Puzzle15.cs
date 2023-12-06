using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Diagnostics;

namespace AdventOfCode.Y2015.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Science for Hungry People";
		public override int Year => 2015;
		public override int Day => 15;

		public override void Run()
		{
			Run("input").Part1(13882464).Part2(11171160);
			Run("extra").Part1(222870).Part2(117936);
		}

		protected override int Part1(string[] input)
		{
			var (_, scores) = Parse(input);

			var maxscore = 0;
			for (var a = 0; a < 100; a++)
			{
				var maxb = 100 - a;
				for (var b = 0; b < maxb; b++)
				{
					var maxc = 100 - (a + b);
					for (var c = 0; c < maxc; c++)
					{
						var d = 100 - (a + b + c);
						if (ScoreFor(0, out var s0) && ScoreFor(1, out var s1) && ScoreFor(2, out var s2) && ScoreFor(3, out var s3))
						{
							var fullscore = s0 * s1 * s2 * s3;
							if (fullscore > maxscore)
							{
								maxscore = fullscore;
							}
						}

						bool ScoreFor(int i, out int v)
						{
							v = a * scores[0, i] + b * scores[1, i] + c * scores[2, i] + d * scores[3, i];
							return v > 0;
						}
					}
				}
			}

			return maxscore;
		}

		protected override int Part2(string[] input)
		{
			var (cal, scores) = Parse(input);

			var maxscore = 0;
			for (var a = 0; a < 100; a++)
			{
				var maxb = 100 - a;
				for (var b = 0; b < maxb; b++)
				{
					var maxc = 100 - (a + b);
					for (var c = 0; c < maxc; c++)
					{
						var d = 100 - (a + b + c);
						var calories = a * cal[0] + b * cal[1] + c * cal[2] + d * cal[3];
						if (calories != 500)
							continue;
						if (ScoreFor(0, out var s0) && ScoreFor(1, out var s1) && ScoreFor(2, out var s2) && ScoreFor(3, out var s3))
						{
							var fullscore = s0 * s1 * s2 * s3;
							if (fullscore > maxscore)
							{
								maxscore = fullscore;
							}
						}

						bool ScoreFor(int i, out int v)
						{
							v = a * scores[0, i] + b * scores[1, i] + c * scores[2, i] + d * scores[3, i];
							return v > 0;
						}
					}
				}
			}

			return maxscore;
		}


		private static (int[] Calories, int[,] Scores) Parse(string[] input)
		{
			var N = input.Length;
			Debug.Assert(N == 4);
			var calories = new int[N];
			var scores = new int[N, 4];
			for (var i = 0; i < N; i++)
			{
				// Example:
				// Frosting: capacity 0, durability -1, flavor 4, texture 0, calories 6
				var (name, capacity, durability, flavor, texture, cals) = input[i]
					.RxMatch("%s: capacity %d, durability %d, flavor %d, texture %d, calories %d")
					.Get<string, int, int, int, int, int>();
				calories[i] = cals;
				scores[i,0] = capacity;
				scores[i,1] = durability;
				scores[i,2] = flavor;
				scores[i,3] = texture;
			}

			return (calories, scores);
		}
	}
}
