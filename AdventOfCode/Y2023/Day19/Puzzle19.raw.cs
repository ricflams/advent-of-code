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
using System.Runtime.CompilerServices;
using static AdventOfCode.Y2018.Day15.Puzzle;

namespace AdventOfCode.Y2023.Day19.Raw
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
			Run("test2").Part1(0).Part2(0);
			Run("input").Part1(406934).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var x = input.GroupByEmptyLine().ToArray();

			// ex{x>10:one,m<20:two,a>30:R,A}
			var wfs = x[0].Select(s => {
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

		private record Rule
		{
			public string Name { get; set; }
			public Condition[] If { get; set; }
			public string Else { get; set; }
			public record Condition
			{
				public Condition(char cat, char op, int val, string result)
				{
					(Cat, Op, Val, Result) = (cat, op, val, result);
				}
				public char Cat { get; set; }
				public char Op { get; set; }
				public int Val { get; set; }
				public string Result { get; set; }
				public override string ToString() => $"{Cat}{Op}{Val}:{Result}";
			}
			public override string ToString() => $"{Name} {{ {string.Join(',', If.Select(x=>x.ToString()))} ,{Else} }}";
		}


		protected override long Part2(string[] input)
		{
			var x = input.GroupByEmptyLine().ToArray();

			// ex{x>10:one,m<20:two,a>30:R,A}
			var wfs = x[0].Select(s => {
				var xx = s.Split('{').ToArray();
				var name = xx[0];
				var ruledefs = xx[1].TrimEnd('}');
				var rules = ruledefs.Split(',').ToArray();
				var conds = rules[..^1]
					.Select(x => x.RxMatch("%c%c%d:%s").Get<char, char, int, string>())
					.Select(x =>
					{
						var (cat, op, val, res) = x;
						return new Rule.Condition(cat, op, val, res);
					})
					.ToArray();
				var otherwise = rules[^1];
				return new Rule { Name = name, If = conds, Else = otherwise };
			})
			.ToDictionary(x => x.Name, x => x);

			var wfsA = wfs.Values.Where(w => w.If.All(i => i.Result == "A") && w.Else == "A").ToArray();
			var nwfsA = wfsA.Length;
			var wfsR = wfs.Values.Where(w => w.If.All(i => i.Result == "R") && w.Else == "R").ToArray();
			var nwfsR = wfsR.Length;

			
			var values = "xmas".ToCharArray().ToDictionary(x => x, _ => new Interval(1, 4001));

			var combi = Combinations(0, wfs["in"], values);
			return combi;

			long Combinations(int indent, Rule rule, Dictionary<char, Interval> ranges)
			{
				var indents = new string(' ', indent);
				//Console.Write($"{indents}{rule} ");
				//Console.WriteLine(string.Join(' ', ranges.Select(r => $"{r.Key}={r.Value}")));
				//if (rule.If.All(i => i.Result == "A") && rule.Else == "A")
				//{
				//	return ranges.Values.Select(cr => (long)cr.Length).ToArray().Prod();
				//}

				var sum = 0L;


				ranges = ranges.ToDictionary();
				//var otherwise = ranges.ToDictionary();

				foreach (var cond in rule.If)
				{
					var cat = cond.Cat;
					var condrange = ranges[cat];
					var n = cond.Val;

					var (result, approved, remain) = (cond.Result, cond.Op) switch
					{
						("A", '>') => ("A", new Interval(n + 1, condrange.End), new Interval(condrange.Start, n + 1)),
						("A", '<') => ("A", new Interval(condrange.Start, n), new Interval(n, condrange.End)),
						("R", '>') => ("R", Interval.Empty, new Interval(condrange.Start, n + 1)),
						("R", '<') => ("R", Interval.Empty, new Interval(n, condrange.End)),
						(_, '>') => (cond.Result, new Interval(n + 1, condrange.End), new Interval(condrange.Start, n + 1)),
						(_, '<') => (cond.Result, new Interval(condrange.Start, n), new Interval(n, condrange.End)),
						_ => throw new Exception()
					};

					if (approved.End < approved.Start)
						throw new Exception();
					if (remain.End < remain.Start)
						throw new Exception();

					if (result == "A")
					{
						sum += approved.Length * ranges.Where(r => r.Key != cat).Select(r => (long)r.Value.Length).ToArray().Prod();
						//			Console.WriteLine($"{indents}sum += {approved} (A)");
					}
					else if (result == "R")
						{ } // nothing
					else if (approved.Length > 0)
					{
						//var r2 = ranges.ToDictionary();
						ranges[cat] = approved;
						var combos = Combinations(indent + 4, wfs[cond.Result], ranges.ToDictionary());
						sum += combos;
			//			Console.WriteLine($"{indents}sum += {combos} (rule)");
					}

					//otherwise[cat] = remain;
					ranges[cat] = remain;
					if (approved.Overlaps(remain))
						throw new Exception();
				}

				if (rule.Else == "A")
				{
					//sum += otherwise.Select(x => (long)x.Value.Length).ToArray().Prod();
					var combos = ranges.Select(x => (long)x.Value.Length).ToArray().Prod();
					sum += combos;
					//		Console.WriteLine($"{indents}sum += {combos} ({string.Join('*', ranges.Select(x => x.Value))}) (else A)");
				}
				else if (rule.Else == "R")
					{ } // nothing
				else
				{
					//sum += Combinations(indent + 4, wfs[rule.Else], otherwise);
					var combos = Combinations(indent + 4, wfs[rule.Else], ranges);
					sum += combos;
			//		Console.WriteLine($"{indents}sum += {combos} (else rule)");
				}

			//	Console.WriteLine($"{indents}sum={sum}");
				return sum;
			}


			//long Combi(Dictionary<string, int> parts, string[] cats)
			//{
			//	bool showProgress = parts.Count == 0;

			//	if (cats.Length == 0)
			//	{
			//		return EvalPart(parts) ? 1 : 0;
			//	}

			//	var cat = cats[0];
			//	var rest = cats[1..];

			//	var conds = wfs.Values
			//		.SelectMany(wf => wf.If.Where(x => x.Cat == cat))
			//		.OrderBy(x => x.Val)
			//		.ToArray();

			//	var sum = 0L;
			//	var lastend = 0;
			//	foreach (var cond in conds)
			//	{
			//		if (showProgress) Console.Write('.');
			//		var end = cond.Cond == '<' ? cond.Val - 1 : cond.Val;
			//		var combinations = end - lastend;

			//		//var newparts = parts.ToDictionary(x => x.Key, x => x.Value);
			//		//newparts[cat] = end;
			//		//var acceptedcombis = combinations * Combi(newparts, rest);

			//		parts[cat] = end;
			//		var acceptedcombis = combinations * Combi(parts, rest);

			//		sum += acceptedcombis;
			//		lastend = end;
			//	}

			//	var end2 = 4000;
			//	if (conds.Last().Cond == '>')
			//		lastend++;
			//	var combinations2 = end2 - lastend + 1;

			//	//var newparts2 = parts.ToDictionary(x => x.Key, x => x.Value);
			//	//newparts2[cat] = end2;
			//	//var acceptedcombis2 = combinations2 * Combi(newparts2, rest);

			//	parts[cat] = end2;
			//	var acceptedcombis2 = combinations2 * Combi(parts, rest);

			//	sum += acceptedcombis2;

			//	return sum;
			//}

			//var combis = Combi([], new string[] { "x", "m", "a", "s" });
			//return combis;

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


			//bool EvalPart(Dictionary<string, int> part)
			//{
			//	var wf = wfs["in"];
			//	while (true)
			//	{
			//		var result = wf.If.FirstOrDefault(x =>
			//		{
			//			var cat = x.Cat;
			//			var partcat = part[x.Cat];
			//			if (x.Cond == '<')
			//				return partcat < x.Val;
			//			else
			//				return partcat > x.Val;
			//		}).Result ?? wf.Else;
			//		if (result == "A")
			//			return true;
			//		if (result == "R")
			//			return false;
			//		wf = wfs[result];
			//	}
			//}


			//return 0;
		}
	}
}
