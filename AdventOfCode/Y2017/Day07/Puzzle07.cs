using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2017.Day07
{
	internal class Puzzle : Puzzle<string, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Recursive Circus";
		public override int Year => 2017;
		public override int Day => 7;

		public void Run()
		{
			Run("test1").Part1("tknk").Part2(60);
			Run("input").Part1("mkxke").Part2(268);
		}

		protected override string Part1(string[] input)
		{
			var root = Disc.ReadFrom(input);
			return root.Name;
		}

		protected override int Part2(string[] input)
		{
			var root = Disc.ReadFrom(input);

			// Find the imbalance; it must be in the first 3+ set of childs that
			// are unbalanced. Don't bother coding for the general case, when all
			// known input has 3+ childs at the root.
			// The imbalance will make it easier to spot the right unbalanced disc
			// later on, even if it appear along with just one other disc, not 3+.
			if (root.Childs.Length < 3)
				throw new Exception("Can't handle the general case");
			var wrong = root.Childs.Single(c => root.Childs.Count(x => x.TotalWeight == c.TotalWeight) == 1);
			var right = root.Childs.First(c => c != wrong);
			var imbalance = right.TotalWeight - wrong.TotalWeight;

			// Find the first disc where all childs themselves are balanced; one of the disc's
			// child-discs must then itself be the cause of the imbalance.
			var disc = root;
			while (true)
			{
				var next = disc.Childs.SingleOrDefault(c => !c.IsBalanced);
				if (next == null)
					break;
				disc = next;
			}

			// Find the unbalanced child-disc; it's the one which will match any other disc's
			// weight if the imbalance is added to it. Then, add the imbalance to that child-disc.
			var unbalanced = disc.Childs.Single(c => disc.Childs.Any(x => c.TotalWeight + imbalance == x.TotalWeight));
			var weight = unbalanced.Weight + imbalance;

			return weight;
		}

		internal class Disc
		{
			private static readonly Disc[] NoChilds = new Disc[0];
			public string Name { get; set; }
			public int Weight { get; set; }
			public Disc Parent { get; set; } = null;
			public Disc[] Childs { get; set; } = NoChilds;
			public bool IsBalanced { get; private set; }
			public int TotalWeight { get; private set; }

			public static Disc ReadFrom(string[] input)
			{
				// First read all the disc's names to enlist them
				var discs = input.Select(line =>
				{
					var (name, weight) = line.RxMatch("%s (%d)").Get<string, int>();
					return new Disc {  Name = name, Weight = weight };
				})
				.ToDictionary(x => x.Name, x => x);

				// Next, setup the disc relations
				foreach (var line in input)
				{
					if (line.IsRxMatch("%s (%d) -> %*", out var captures))
					{
						var (name, _, childnames) = captures.Get<string,int,string>();
						var childs = discs[name].Childs = childnames.Replace(" ", "").Split(',').Select(c => discs[c]).ToArray();
						foreach (var c in childs)
						{
							c.Parent = discs[name];
						}
					}
				}

				// Find the root-disc and calculate balancing for all discs
				// (Balancing is really only needed for part 2, but cheap)
				var root = discs.Values.Single(d => d.Parent == null);
				root.CalculateBalance();

				return root;
			}			

			private void CalculateBalance()
			{
				IsBalanced = true;
				TotalWeight = Weight;
				if (Childs.Any())
				{
					foreach (var c in Childs)
					{
						c.CalculateBalance();
					}
					// Disc is balanced if all childs have exactly the same weight, ie 1/nth of their total weight
					var w = Childs.Select(c => c.TotalWeight).Sum();
					TotalWeight += w;
					IsBalanced = w % Childs.Length == 0 && Childs.All(c => c.TotalWeight == w / Childs.Length);
				}
			}
		}
	}
}
