using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2020.Day19
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Monster Messages";
		public override int Year => 2020;
		public override int Day => 19;

		public void Run()
		{
			Run("test1").Part1(2); 
			Run("test2").Part2(12);
			Run("input").Part1(122).Part2(287);
		}

		protected override int Part1(string[] input)
		{
			return CalcValidMessages(input);
		}

		protected override int Part2(string[] input)
		{
			var modified = input
				.Select(line => line switch
					{
						"8: 42" => "8: 42 | 42 8",
						"11: 42 31" => "11: 42 31 | 42 11 31",
						_ => line
					});
			return CalcValidMessages(modified);
		}
	
		private int CalcValidMessages(IEnumerable<string> input)
		{
			var groups = input.GroupByEmptyLine().ToArray();
			var rawrules = groups[0];
			var messages = groups[1];

			var rules = new Rules(rawrules);
			var regex = rules.GetRegex();
			var rx = new Regex(regex, RegexOptions.Compiled);
			var valid = messages.Count(message => rx.IsMatch(message));
			return valid;
		}

		internal class Rules
		{
			private const int SpecialRule11MaxRecurse = 5;
			private readonly Dictionary<int, Func<string>> _map = new Dictionary<int, Func<string>>();

			public Rules(IEnumerable<string> rawrules)
			{
				foreach (var rawrule in rawrules)
				{
					//  43: "a"
					//  94: 39 43 | 75 35
					//  70: 55 43 | 12 35
					//  55: 43 119 | 35 71
					//  84: 7 35 | 76 43
					//  128: 34 35 | 23 43
					//  34: 63 63
					var parts = rawrule.Split(':', 2);
					var id = int.Parse(parts[0]);
					var ruleexpr = parts[1];
					var rule =
						rawrule == "8: 42 | 42 8" ? SpecialRule8 :
						rawrule == "11: 42 31 | 42 11 31" ? SpecialRule11 :
						(Func<string>)(() => Alternate(ruleexpr));
					_map[id] = rule;
				}
			}

			private string Alternate(string expr)
			{
				var rxs = expr.Trim().Split('|').Select(e => Sequence(e)).ToArray();
				return rxs.Length == 1
					? rxs.First()
					: $"(?:{string.Join('|', rxs)})";
			}

			private string Sequence(string expr)
			{
				var rxs = expr.Trim().Split(' ').Select(e => Value(e));
				return string.Concat(rxs);
			}

			private string Value(string expr)
			{
				return expr[0] == '"'
					? expr[1..^1]
					: _map[int.Parse(expr)]();
			}
			
			private string SpecialRule8()
			{
				var rx42 = _map[42]();
				return $"(?:{rx42})+";
			}

			private string SpecialRule11()
			{
				var rx42 = _map[42]();
				var rx31 = _map[31]();
				var regexs = new List<string>();
				for (var i = 1; i < SpecialRule11MaxRecurse; i++)
				{
					var s = "";
					for (var n = 0; n < i; n++)
					{
						s = rx42 + s + rx31;
					}
					regexs.Add($"(?:{s})");
				}
				var ors = string.Join("|", regexs);
				return $"(?:{ors})";
			}

			public string GetRegex()
			{
				var rx = _map[0]();
				return $"^{rx}$";
			}
		}
	}
}
 