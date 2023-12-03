using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day14.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 14";
		public override int Year => 2021;
		public override int Day => 14;

		public override void Run()
		{
			Run("test1").Part1(1588).Part2(2188189693529);

			//Run("test2").Part1(0).Part2(0);

			// 2158894777815 not right
			Run("input").Part1(2068).Part2(2158894777814);
		}

		protected override long Part1(string[] input)
		{
			var template = input.First();

			var rules = new Dictionary<string, string>();
			foreach (var s in input.Skip(2))
			{
				var (a, b) = s.RxMatch("%s -> %s").Get<string, string>();
				rules.Add(a, b);
			}

			for (var i = 0; i < 10; i++)
			{
			//	Console.WriteLine(template);
				var result = new StringBuilder();
				result.Append(template[0]);
				for (var p = 0; p < template.Length - 1; p++)
				{
					var pp = template.Substring(p, 2);
					//result.Append(pp[0]);
					result.Append(rules[pp]);
					result.Append(pp[1]);
				}
				template = result.ToString();
			}

			var occ = template.GroupBy(x => x);
			var xx = occ.OrderBy(x => x.Count()).ToArray();
			var aa = xx.First().Count();
			var bb = xx.Last().Count();

			return bb - aa;
		}

		protected override long Part2(string[] input)
		{
			var template = input.First();

			var rules = new Dictionary<string, string>();
			foreach (var s in input.Skip(2))
			{
				var (a, b) = s.RxMatch("%s -> %s").Get<string, string>();
				rules.Add(a, b);
			}

			var pairs = new SafeDictionary<string, long>();
			for (var p = 0; p < template.Length - 1; p++)
			{
				var pp = template.Substring(p, 2);
				pairs[pp]++;
			}


			for (var i = 0; i < 40; i++)
			{
				var pairs2 = new SafeDictionary<string, long>();
				foreach (var k in pairs)
				{
					pairs2[k.Key] = k.Value;
				}	
				foreach (var pp in pairs)
				{
					if (pp.Value == 0)
						continue;
					pairs2[pp.Key] -= pairs[pp.Key];
					var reduc = rules[pp.Key];
					var sb = new StringBuilder();
					sb.Append(pp.Key[0]);
					sb.Append(reduc[0]);
					var name1 = sb.ToString();
					pairs2[name1] += pp.Value;
					sb.Clear();
					sb.Append(reduc[0]);
					sb.Append(pp.Key[1]);
					var name2 = sb.ToString();
					pairs2[name2] += pp.Value;
				}
				pairs = pairs2;
			}

			var occur = new SafeDictionary<char, long>();
			foreach (var kk in pairs)
			{
				occur[kk.Key[0]] += kk.Value;
				occur[kk.Key[1]] += kk.Value;
			}
			var ord = occur
				.Select(x => 
				{
					var n = x.Value;
					if (x.Key == template.First()) n++;
					if (x.Key == template.Last()) n++;
					return n / 2;
				})
				.OrderBy(x => x).ToArray();

			var aa = ord.First();
			var bb = ord.Last();
			var uniq = bb - aa;

			return uniq;
		}

	}
}
