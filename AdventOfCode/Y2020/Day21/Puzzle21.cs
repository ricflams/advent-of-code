using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day21
{
	internal class Puzzle : ComboParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Allergen Assessment";
		protected override int Year => 2020;
		protected override int Day => 21;

		public void Run()
		{
			RunFor("test1", "5", "mxmxvkd,sqjhc,fvjkl");
			RunFor("input", "2423", "jzzjz,bxkrd,pllzxb,gjddl,xfqnss,dzkb,vspv,dxvsp");
		}

		internal class Food
		{
			public HashSet<string> Ingredients { get; set; }
			public HashSet<string> Allergies { get; set; }
		}

		protected override (string,string) Part1And2(string[] input)
		{
			// mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
			// trh fvjkl sbzzf mxmxvkd (contains dairy)
			// sqjhc fvjkl (contains soy)
			// sqjhc mxmxvkd sbzzf (contains fish)
			var food = input
				.Select(line =>
				{
					line.RegexCapture(@"%* (contains %*)")
						.Get(out string ingredients)
						.Get(out string allergies);
					return new Food
					{
						Ingredients = new HashSet<string>(ingredients.Split(" ", StringSplitOptions.RemoveEmptyEntries)),
						Allergies = new HashSet<string>(allergies.Split(", ", StringSplitOptions.RemoveEmptyEntries))
					};
				})
				.ToArray();

			// All ingredients anda allergies
			var allIngredients = food.Aggregate(new HashSet<string>(), (set, f) => { set.UnionWith(f.Ingredients); return set; });
			var allAllergies = food.Aggregate(new HashSet<string>(), (set, f) => { set.UnionWith(f.Allergies); return set; });

			var allergyCandidates = allAllergies
				.Select(a =>
				{
					var foodopt = food
						.Where(f => f.Allergies.Contains(a))
						.ToArray();
					var ingopt = new HashSet<string>(foodopt.First().Ingredients);
					foreach (var f in foodopt.Skip(1))
					{
						ingopt.IntersectWith(f.Ingredients);
					}
					return (a, ingopt);
				})
				.ToDictionary(x => x.a, x => x.ingopt);

			var allergyIngredients = new Dictionary<string, string>();
			while (allergyCandidates.Any())
			{
				var a = allergyCandidates.OrderBy(x => x.Value.Count()).First();
				var ingredient = a.Value.First();
				allergyIngredients[a.Key] = ingredient;
				allergyCandidates.Remove(a.Key);
				foreach (var ac in allergyCandidates)
				{
					ac.Value.Remove(ingredient);
				}
			}

			var ingredientsWithoutAllergy = new HashSet<string>(allIngredients.Except(allergyIngredients.Values));
			var result1 = food
				.Sum(f => f.Ingredients.Count(i => ingredientsWithoutAllergy.Contains(i)))
				.ToString();

			var ingredientsWithAllergy = allergyIngredients
				.OrderBy(x => x.Key)
				.Select(x => x.Value);
			var result2 = string.Join(",", ingredientsWithAllergy);

			return (result1, result2);
		}
	}
}
