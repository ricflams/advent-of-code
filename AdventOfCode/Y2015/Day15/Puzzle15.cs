using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day15
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Science for Hungry People";
		public override int Year => 2015;
		public override int Day => 15;

		public void Run()
		{
			// TODO, fails: Run("test1").Part1(62842880).Part2(57600000);
			Run("input").Part1(13882464).Part2(11171160);
		}

		protected override int Part1(string[] input)
		{
			var ingredients = input.Select(Ingredient.ParseFrom).ToArray();
			var score1 = FindMaximumScore(ingredients, _ => true);
			return score1;
		}

		protected override int Part2(string[] input)
		{
			var ingredients = input.Select(Ingredient.ParseFrom).ToArray();
			var score2 = FindMaximumScore(ingredients, calories => calories == 500);
			return score2;
		}

		private static int FindMaximumScore(Ingredient[] ingredients, Func<int,bool> calorieCondition)
		{
			var vicinity = new VicinityExplorer();
			var maxscore = 0;

			var seed = MathHelper.DivideEvenly(100, ingredients.Length);
			var queue = new Queue<int[]>();
			queue.Enqueue(seed);

			while (queue.Any())
			{
				var currentSpoons = queue.Dequeue();
				foreach (var spoons in vicinity.Explore(currentSpoons))
				{
					var scores = new int[currentSpoons.Length];
					for (var spoon = 0; spoon < currentSpoons.Length; spoon++)
					{
						for (var i = 0; i < ingredients.Length; i++)
						{
							scores[spoon] += spoons[i] * ingredients[i].Scores[spoon];
						}
					}
					if (scores.Any(x => x <= 0))
					{
						continue;
					}

					var calories = 0;
					for (var i = 0; i < ingredients.Length; i++)
					{
						calories += spoons[i] * ingredients[i].Calories;
					}

					var fullscore = scores.Aggregate(1, (sum, v) => sum * v);
					if (fullscore > maxscore && calorieCondition(calories))
					{
						maxscore = fullscore;
						//Console.WriteLine($"[{string.Join(" ", walk)}: {fullscore}]");
					}
					if (fullscore > maxscore * .95)
					{
						queue.Enqueue(spoons);
					}
				}
			}

			return maxscore;
		}

		private class Ingredient
		{
			private const int ScoresCount = 4;
			public string Name { get; set; }
			public int[] Scores { get; set; }
			public int Calories { get; set; }

			public static Ingredient ParseFrom(string line)
			{
				// Example:
				// Frosting: capacity 0, durability -1, flavor 4, texture 0, calories 6
				var (name, capacity, durability, flavor, texture, calories) = line
					.RxMatch("%s: capacity %d, durability %d, flavor %d, texture %d, calories %d")
					.Get<string, int, int, int, int, int>();
				return new Ingredient
				{
					Name = name,
					Scores = new int[ScoresCount] { capacity, durability, flavor, texture },
					Calories = calories
				};
			}
		}
	}
}
