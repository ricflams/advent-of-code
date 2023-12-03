using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day14
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Extended Polymerization";
		public override int Year => 2021;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").Part1(1588).Part2(2188189693529);
			Run("test9").Part1(3048).Part2(3288891573057);
			Run("input").Part1(2068).Part2(2158894777814);
		}

		protected override long Part1(string[] input)
		{
			var (template, rules) = ReadInput(input);

			var polymer = template;
			for (var step = 0; step < 10; step++)
			{
				var sb = new StringBuilder();
				sb.Append(polymer[0]);
				for (var i = 0; i < polymer.Length - 1; i++)
				{
					var pair = polymer[i..(i+2)];
					sb.Append(rules[pair]);
					sb.Append(pair[1]);
				}
				polymer = sb.ToString();
			}

			var elements = polymer
				.GroupBy(x => x)
				.OrderByDescending(x => x.Count())
				.ToArray();
			var mostCommon = elements.First().Count();
			var leastCommon = elements.Last().Count();
			var quantity = mostCommon - leastCommon;

			return quantity;
		}

		protected override long Part2(string[] input)
		{
			var (template, rules) = ReadInput(input);

			var pairs = new SafeDictionary<string, long>();
			for (var i = 0; i < template.Length - 1; i++)
			{
				var pp = template[i..(i + 2)];
				pairs[pp]++;
			}

			for (var i = 0; i < 40; i++)
			{
				var next = new SafeDictionary<string, long>();
				foreach (var p in pairs)
				{
					next[p.Key] = p.Value;
				}	
				foreach (var p in pairs)
				{
					var reduc = rules[p.Key];
					next[p.Key] -= pairs[p.Key];
					next[$"{p.Key[0]}{reduc[0]}"] += p.Value;
					next[$"{reduc[0]}{p.Key[1]}"] += p.Value;
				}
				pairs = next;
			}

			var occurences = new SafeDictionary<char, long>();
			foreach (var p in pairs)
			{
				occurences[p.Key[0]] += p.Value;
				occurences[p.Key[1]] += p.Value;
			}

			var first = template.First();
			var last = template.Last();
			var ordered = occurences
				.Select(x => 
				{
					var n = x.Value;
					if (x.Key == first || x.Key == last) n++;
					return n / 2;
				})
				.OrderByDescending(x => x)
				.ToArray();

			var mostCommon = ordered.First();
			var leastCommon = ordered.Last();
			var result = mostCommon - leastCommon;

			return result;
		}

		private static (string, Dictionary<string, string>) ReadInput(string[] input)
		{
			var template = input.First();

			var rules = new Dictionary<string, string>();
			foreach (var s in input.Skip(2))
			{
				var (a, b) = s.RxMatch("%s -> %s").Get<string, string>();
				rules.Add(a, b);
			}

			return (template, rules);
		}
	}
}
