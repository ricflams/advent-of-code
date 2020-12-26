using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day15
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2015;
		protected override int Day => 15;

		public void Run()
		{
			// TODO, fails: RunFor("test1", 62842880, 57600000);
			RunFor("input", 13882464, 11171160);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var ingredients = input.Select(Ingredient.ParseFrom).ToArray();

			var score1 = FindMaximumScore(ingredients, _ => true);
			var score2 = FindMaximumScore(ingredients, calories => calories == 500);
			return (score1, score2);
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
				var val = SimpleRegex.Match(line, "%s: capacity %d, durability %d, flavor %d, texture %d, calories %d");
				return new Ingredient
				{
					Name = val[0],
					Scores = new int[ScoresCount] { int.Parse(val[1]), int.Parse(val[2]), int.Parse(val[3]), int.Parse(val[4]) },
					Calories = int.Parse(val[5])
				};
			}
		}
	}
}
