using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day21
{
	internal class Puzzle : SoloParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Allergen Assessment";
		public override int Year => 2020;
		public override int Day => 21;

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

		protected override string Part1(string[] input)
		{
			var (food, allIngredients, allergyIngredients) = GetIngredientsAndAllergies(input);

			var ingredientsWithoutAllergy = new HashSet<string>(allIngredients.Except(allergyIngredients.Values));
			var result = food
				.Sum(f => f.Ingredients.Count(i => ingredientsWithoutAllergy.Contains(i)))
				.ToString();
			return result;
		}

		protected override string Part2(string[] input)
		{
			var (_, _, allergyIngredients) = GetIngredientsAndAllergies(input);

			var ingredientsWithAllergy = allergyIngredients
				.OrderBy(x => x.Key)
				.Select(x => x.Value);
			var result = string.Join(",", ingredientsWithAllergy);
			return result;
		}

		private static (Food[], HashSet<string>, Dictionary<string, string>) GetIngredientsAndAllergies(string[] input)
		{
			// mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
			// trh fvjkl sbzzf mxmxvkd (contains dairy)
			// sqjhc fvjkl (contains soy)
			// sqjhc mxmxvkd sbzzf (contains fish)
			var food = input
				.Select(line =>
				{
					var (ingredients, allergies) = line.RxMatch(@"%* (contains %*)").Get<string, string>();
					return new Food
					{
						Ingredients = new HashSet<string>(ingredients.Split(" ", StringSplitOptions.RemoveEmptyEntries)),
						Allergies = new HashSet<string>(allergies.Split(", ", StringSplitOptions.RemoveEmptyEntries))
					};
				})
				.ToArray();

			// All ingredients and allergies
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

			return (food, allIngredients, allergyIngredients);
		}
	}
}
