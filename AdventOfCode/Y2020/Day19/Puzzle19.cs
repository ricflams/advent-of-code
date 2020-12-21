using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2020.Day19
{
	// TODO: Cleanup
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 19;

		public void Run()
		{
			//RunFor("test1", 2, 0);
			
			//RunFor("test2", 3, 12);
			RunPart2For("test2", 12);

			//RunFor("test2", null, null);
			RunFor("input", 122, 287);
		}

		internal class RuleMap : Dictionary<int, IRule> {};
		internal class Rules
		{
			private readonly RuleMap _rulemap = new RuleMap();

			public Rules(IEnumerable<string> rawrules)
			{
				foreach (var rawrule in rawrules)
				{
					AddRule(rawrule);
				}
			}

			public void AddRule(string rawrule)
			{
				if (rawrule == "8: 42 | 42 8")
				{
					_rulemap[8] = new Rule8();
					return;
				}
				if (rawrule == "11: 42 31 | 42 11 31")
				{
					_rulemap[11] = new Rule11();
					return;
				}
				//  43: "a"
				//  94: 39 43 | 75 35
				//  70: 55 43 | 12 35
				//  55: 43 119 | 35 71
				//  84: 7 35 | 76 43
				//  128: 34 35 | 23 43
				//  34: 63 63
				var pp = rawrule.Split(':', 2);
				var id = int.Parse(pp[0]);
				var rule = pp[1].Trim();

				if (rule.MaybeRegexCapture("\"%c\"").Get(out char ch).IsMatch)
				{
					_rulemap[id] = new RuleLiteral(ch);
				}
				else if (rule.MaybeRegexCapture(@"%d %d | %d %d").Get(out int r11).Get(out int r12).Get(out int r21).Get(out int r22).IsMatch)
				{
					_rulemap[id] = new RuleAltSequence(r11, r12, r21, r22);
				}
				else if (rule.MaybeRegexCapture(@"%d %d").Get(out int r1).Get(out int r2).IsMatch)
				{
					_rulemap[id] = new RuleSequence(r1, r2);
				}
				else if (rule.MaybeRegexCapture(@"%d | %d").Get(out int a1).Get(out int a2).IsMatch)
				{
					_rulemap[id] = new RuleAlt(a1, a2);
				}
				else if (rule.MaybeRegexCapture(@"%d").Get(out int s1).IsMatch)
				{
					_rulemap[id] = new RuleSubst(s1);
				}
				else
				{
					throw new Exception($"Unexpected rule {rawrule}");
				}
			}

			public bool IsValidRule(string message)
			{
				var pos = 0;
				return _rulemap[0].Match(_rulemap, message, ref pos) && pos == message.Length;
			}

			public string AsRegex()
			{
				return $"^{_rulemap[0].AsRegex(_rulemap)}$";
			}
		}

		internal interface IRule
		{
			bool Match(RuleMap map, string s, ref int pos);
			string AsRegex(RuleMap map);
		}

		class RuleSubst : IRule
		{
			private int _rule;
			public RuleSubst(int rule) { _rule = rule; }
			public bool Match(RuleMap map, string s, ref int pos)
			{
				var p = pos;
				if (map[_rule].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				return false;
			}
			public string AsRegex(RuleMap map) => map[_rule].AsRegex(map);
		}

		class RuleLiteral : IRule
		{
			private readonly char _ch;
			public RuleLiteral(char ch) { _ch = ch; }
			public bool Match(RuleMap map, string s, ref int pos)
			{
				if (pos >= s.Length || s[pos] != _ch)
					return false;
				pos++;
				return true;
			}
			public string AsRegex(RuleMap map) => _ch.ToString();
		}

		class RuleSequence : IRule
		{
			private int _rule1;
			private int _rule2;
			public RuleSequence(int rule1, int rule2) { _rule1 = rule1; _rule2 = rule2; }
			public bool Match(RuleMap map, string s, ref int pos)
			{
				var p = pos;
				if (map[_rule1].Match(map, s, ref p) && map[_rule2].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				return false;
			}
			public string AsRegex(RuleMap map) => $"(?:{map[_rule1].AsRegex(map)}{map[_rule2].AsRegex(map)})";
		}

		class RuleAlt : IRule
		{
			private int _rule1;
			private int _rule2;
			public RuleAlt(int rule1, int rule2) { _rule1 = rule1; _rule2 = rule2; }
			public bool Match(RuleMap map, string s, ref int pos)
			{
				var p = pos;
				if (map[_rule1].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				p = pos;
				if (map[_rule2].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				return false;
			}
			public string AsRegex(RuleMap map) => $"(?:{map[_rule1].AsRegex(map)}|{map[_rule2].AsRegex(map)})";
		}

		class RuleAltSequence : IRule
		{
			private int _rule11;
			private int _rule12;
			private int _rule21;
			private int _rule22;
			public RuleAltSequence(int rule11, int rule12, int rule21, int rule22) { _rule11 = rule11; _rule12 = rule12; _rule21 = rule21; _rule22 = rule22; }
			public bool Match(RuleMap map, string s, ref int pos)
			{
				var p = pos;
				if (map[_rule11].Match(map, s, ref p) && map[_rule12].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				p = pos;
				if (map[_rule21].Match(map, s, ref p) && map[_rule22].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				return false;
			}
			public string AsRegex(RuleMap map) => $"(?:(?:{map[_rule11].AsRegex(map)}{map[_rule12].AsRegex(map)})|(?:{map[_rule21].AsRegex(map)}{map[_rule22].AsRegex(map)}))";
		}

		class Rule8 : IRule
		{
			public bool Match(RuleMap map, string s, ref int pos)
			{
				var p = pos;
				if (map[42].Match(map, s, ref p))
				{
					var p2 = p;
					if (map[8].Match(map, s, ref p2))
					{
						p = p2;
					}
					pos = p;
					return true;
				}
				return false;
			}
			public string AsRegex(RuleMap map) => $"(?:{map[42].AsRegex(map)})+";
		}

		internal static int Backref = 0;

		class Rule11 : IRule
		{
			public bool Match(RuleMap map, string s, ref int pos)
			{
				var p = pos;
				if (map[42].Match(map, s, ref p) && map[31].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				p = pos;
				if (map[42].Match(map, s, ref p) && map[11].Match(map, s, ref p) && map[31].Match(map, s, ref p))
				{
					pos = p;
					return true;
				}
				return false;
			}
			public string AsRegex(RuleMap map)
			{
				var expr42 = map[42].AsRegex(map);
				var expr31 = map[31].AsRegex(map);
				var regexs = new List<string>();
				for (var i = 1; i < 5; i++)
				{
					var s = "";
					for (var n = 0; n < i; n++)
					{
						s = expr42 + s + expr31;
					}
					regexs.Add($"(?:{s})");
				}
				var ors = string.Join("|", regexs);
				return $"(?:{ors})";



				//var name1 = $"back{Backref++}";
				//var name2 = $"back{Backref++}";
				//return $@"((?<{name}>{map[42].AsRegex(map)}{map[31].AsRegex(map)})\k<{name}>)+";
			}
		}

		protected override int Part1(string[] input)
		{
			var groups = input.GroupByEmptyLine().ToArray();
			var rawrules = groups[0];
			var messages = groups[1];

			var rules = new Rules(rawrules);

			var valid = messages.Count(message => rules.IsValidRule(message));



			return valid;
		}

		protected override int Part2(string[] input)
		{

			var groups = input.GroupByEmptyLine().ToArray();
			var rawrules = groups[0];
			var messages = groups[1];

			rawrules[rawrules.TakeWhile(m => m != "8: 42").Count()] = "8: 42 | 42 8";
			rawrules[rawrules.TakeWhile(m => m != "11: 42 31").Count()] = "11: 42 31 | 42 11 31";

			var rules = new Rules(rawrules);

			var regex = rules.AsRegex();
			var rx = new Regex(regex, RegexOptions.Compiled);
//			Console.WriteLine(regex);

			var valid = messages.Count(message => rx.IsMatch(message));



			return valid;

		}
	}




	//  internal class Puzzle : ComboPart<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//  	protected override int Year => 2020;
	//  	protected override int Day => 19;
	//  
	//  	public void Run()
	//  	{
	//  		RunFor("test1", null, null);
	//  		//RunFor("test2", null, null);
	//  		//RunFor("input", null, null);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}
