using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Data;
using System.Xml.Schema;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace AdventOfCode.Y2023.Day19
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1(19114).Part2(167409079868000);
			//Run("test2").Part1(0).Part2(0);
			// TODO Run("input").Part1(406934).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var x = input.GroupByEmptyLine().ToArray();

			// ex{x>10:one,m<20:two,a>30:R,A}
			var wfs = x[0].Select(s =>
			{
				// case '%': sb.Append('%'); break;
				// case '*': sb.Append(@"(.+)"); break;
				// case 's': sb.Append(@"(\w+)"); break;
				// case 'c': sb.Append(@"(.)"); break;
				// case 'd': sb.Append(@"([-+]?\d+)"); break;
				// case 'D': sb.Append(@"\s*([-+]?\d+)"); break;					
				//var (name, ruledefs) = s.RxMatch("%s{%s}").Get<string, string>();
				var xx = s.Split('{').ToArray();
				var name = xx[0];
				var ruledefs = xx[1].TrimEnd('}');
				var rules = ruledefs.Split(',').ToArray();
				var conds = rules[..^1]
					.Select(x => x.RxMatch("%s%c%d:%s").Get<string, char, int, string>())
					.Select(x =>
					{
						var (cat, cond, val, res) = x;
						return (Cat: cat, Cond: cond, Val: val, Result: res);
					})
					.ToArray();
				var otherwise = rules[^1];
				return (Name: name, If: conds, Else: otherwise);
			})
			.ToDictionary(x => x.Name, x => x);

			var parts = x[1].Select(s =>
				{
					return s.TrimStart('{').TrimEnd('}').Split(',')
						.Select(x =>
						{
							var p = x.Split('=').ToArray();
							return (Name: p[0], Val: int.Parse(p[1]));
						})
						.ToDictionary(x => x.Name, x => x.Val);
				})
				.ToArray();


			var sum = parts.Sum(part =>
			{
				var wf = wfs["in"];
				while (true)
				{
					var result = wf.If.FirstOrDefault(x =>
					{
						var cat = x.Cat;
						var partcat = part[x.Cat];
						if (x.Cond == '<')
							return partcat < x.Val;
						else
							return partcat > x.Val;
					}).Result ?? wf.Else;
					if (result == "A")
						return part.Values.Sum();
					if (result == "R")
						return 0;
					wf = wfs[result];
				}
				throw new Exception();
			});

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var x = input.GroupByEmptyLine().ToArray();

			// ex{x>10:one,m<20:two,a>30:R,A}
			var wfs = x[0].Select(s =>
			{
				// case '%': sb.Append('%'); break;
				// case '*': sb.Append(@"(.+)"); break;
				// case 's': sb.Append(@"(\w+)"); break;
				// case 'c': sb.Append(@"(.)"); break;
				// case 'd': sb.Append(@"([-+]?\d+)"); break;
				// case 'D': sb.Append(@"\s*([-+]?\d+)"); break;					
				//var (name, ruledefs) = s.RxMatch("%s{%s}").Get<string, string>();
				var xx = s.Split('{').ToArray();
				var name = xx[0];
				var ruledefs = xx[1].TrimEnd('}');
				var rules = ruledefs.Split(',').ToArray();
				var conds = rules[..^1]
					.Select(x => x.RxMatch("%s%c%d:%s").Get<string, char, int, string>())
					.Select(x =>
					{
						var (cat, cond, val, res) = x;
						return (Cat: cat, Cond: cond, Val: val, Result: res);
					})
					.ToArray();
				var otherwise = rules[^1];
				return (Name: name, If: conds, Else: otherwise);
			})
			.ToDictionary(x => x.Name, x => x);


			var combi = 0;

			long Combi(Dictionary<string, int> parts, string[] cats)
			{
				if (parts.Count == 4)
				{
					return EvalPart(parts) ? 1 : 0;
				}

				var cat = cats[0];
				var rest = cats[1..];

				var conds = wfs.Values
					.SelectMany(wf => wf.If.Where(x => x.Cat == cat))
					.OrderBy(x => x.Val)
					.ToArray();

				var sum = 0L;
				var lastend = 0;
				foreach (var cond in conds)
				{
					var end = cond.Cond == '<' ? cond.Val - 1 : cond.Val;
					var combinations = end - lastend;
					var newparts = parts.ToDictionary(x => x.Key, x => x.Value);
					newparts[cat] = end;
					//parts[cat] = end;
					var acceptedcombis = combinations * Combi(newparts, rest);
					//var acceptedcombis = combinations * Combi(parts, rest);
					sum += acceptedcombis;
					lastend = end;
				}

				var end2 = 4000;
				//if (conds.Last().Cond == '>')
				//	lastend++;
				var combinations2 = end2 - lastend;
				var newparts2 = parts.ToDictionary(x => x.Key, x => x.Value);
				newparts2[cat] = end2;
				//parts[cat] = end2;
				var acceptedcombis2 = combinations2 * Combi(newparts2, rest);
				//var acceptedcombis2 = combinations2 * Combi(parts, rest);
				sum += acceptedcombis2;

				return sum;
			}

			var combis = Combi([], new string[] { "x", "m", "a", "s" });
			return combis;

			//var ss = wfs.Values
			//	.SelectMany(wf =>
			//	{
			//		var xx = wf.If.Where(x => x.Cat == "s");
			//		return xx;
			//	})
			//	.OrderBy(x => x.Val)
			//	.ToArray();
			;
			//foreach (var wf in wfs.Values)
			//{
			//	if (wf.If.Any(x => x.Cat == "s"))
			//}


			bool EvalPart(Dictionary<string, int> part)
			{
				var wf = wfs["in"];
				while (true)
				{
					var result = wf.If.FirstOrDefault(x =>
					{
						var cat = x.Cat;
						var partcat = part[x.Cat];
						if (x.Cond == '<')
							return partcat < x.Val;
						else
							return partcat > x.Val;
					}).Result ?? wf.Else;
					if (result == "A")
						return true;
					if (result == "R")
						return false;
					wf = wfs[result];
				}
			}


			return 0;
		}
	}
}
