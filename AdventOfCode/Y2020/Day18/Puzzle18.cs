using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2020.Day18
{
	internal class Puzzle : SoloParts<ulong>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 18;

		public void Run()
		{
			RunFor("test1", 71, 231);
			RunFor("test2", 51, 51);
			RunFor("test3", 26, 46);
			RunFor("test4", 437, 1445);
			RunFor("test5", 12240, 669060);
			RunFor("test6", 13632, 23340);
			RunFor("input", 69490582260, 362464596624526);
		}

		//internal class IExpr
		//{

		//}
		//internal class IExpr
		//{
		//	public IExpr Left { get; set; }
		//	public IExpr Right { get; set; }
		//	public char Op { get; set; }
		//}

		internal int EvalLine(string line)
		{
			//Console.WriteLine($"Eval: {line}");
			// 1 + (2 * 3) + (4 * (5 + 6))
			line = line.Replace(" ", "");
			while (true)
			{
				var org = line;
				line = Regex.Replace(line, @"(\d+)(.)(\d+)", match =>
				{
					var val = match.Groups.Values.Skip(1).Select(g => g.Value).ToArray();
					var op1 = int.Parse(val[0]);
					var op2 = int.Parse(val[2]);
					var eval = val[1] == "+"
						? op1 + op2
						: op1 * op2;
					return eval.ToString();
				});
				line = Regex.Replace(line, @"\((\d+)\)", match =>
				{
					var val = match.Groups.Values.Skip(1).Select(g => g.Value).ToArray();
					return val[0];
				});
				//Console.WriteLine($"  line: {line}");
				if (line == org)
					break;
			}
			Console.WriteLine($"  done: {line}");
			return int.Parse(line);
		}

		internal ulong EvalLine2(string line)
		{
			//Console.WriteLine($"Eval: {line}");
			var pos = 0;
			line = line.Replace(" ", "");
			var value = EvalLine3(line, ref pos);
			//Console.WriteLine($"  done: {value}");
			return value;
		}

		internal ulong EvalLine3(string line, ref int pos)
		{
			// 1 + (2 * 3) + (4 * (5 + 6))

			//((9 * 2 + 4) * (6 * 2 * 2 * 2 + 2) + 5 * 3 + 2) + 7 * 7 + 9
			//6 + 5 * 7 * 9
			//(9 * 7 * (2 * 4 + 7 * 7 + 6) * 5) + 7

			ulong? value = null;
			char? op = null;
			while (pos < line.Length)
			{
				ulong? v = null;
				char? o = null;
				var ch = line[pos++];
				if (ch == ')')
					return value.Value;
				if (ch == '(')
				{
					v = EvalLine3(line, ref pos);
				}
				else if (Char.IsDigit(ch))
				{
					v = (ulong)(ch - '0');
				}
				else if (ch == '+' || ch == '*')
				{
					op = ch;
				}
				if (v.HasValue)
				{
					if (value.HasValue)
					{
						value = op == '+'
							? v.Value + value.Value
							: v.Value * value.Value;
					}
					else
					{
						value = v;
					}
				}
			}
			return value.Value;

			//while (true)
			//{
			//	var org = line;
			//	line = Regex.Replace(line, @"(\d+)(.)(\d+)", match =>
			//	{
			//		var val = match.Groups.Values.Skip(1).Select(g => g.Value).ToArray();
			//		var op1 = int.Parse(val[0]);
			//		var op2 = int.Parse(val[2]);
			//		var eval = val[1] == "+"
			//			? op1 + op2
			//			: op1 * op2;
			//		return eval.ToString();
			//	});
			//	line = Regex.Replace(line, @"\((\d+)\)", match =>
			//	{
			//		var val = match.Groups.Values.Skip(1).Select(g => g.Value).ToArray();
			//		return val[0];
			//	});
			//	Console.WriteLine($"  line: {line}");
			//	if (line == org)
			//		break;
			//}
			//Console.WriteLine($"  done: {line}");
			//return int.Parse(line);
		}


		protected override ulong Part1(string[] input)
		{
			return input.Select(EvalLine2).Sum();


		}


		internal ulong EvalLinePart2(string line)
		{
			//Console.WriteLine($"Eval: {line}");
			var pos = 0;
			line = line.Replace(" ", "");
			var value = EvalLinePart2X(line, ref pos);
			//Console.WriteLine($"  done: {value}");
			return value;
		}

		internal ulong EvalLinePart2X(string line, ref int pos)
		{
			// 1 + (2 * 3) + (4 * (5 + 6))

			//((9 * 2 + 4) * (6 * 2 * 2 * 2 + 2) + 5 * 3 + 2) + 7 * 7 + 9
			//6 + 5 * 7 * 9
			//(9 * 7 * (2 * 4 + 7 * 7 + 6) * 5) + 7

			ulong? value = null;
			ulong? subvalue = null;
			char? op = null;
			while (pos < line.Length)
			{
				ulong? v = null;
				var ch = line[pos++];
				if (ch == ')')
					break;
				if (ch == '(')
				{
					v = EvalLinePart2X(line, ref pos);
				}
				else if (Char.IsDigit(ch))
				{
					v = (ulong)(ch - '0');
				}
				else if (ch == '+' || ch == '*')
				{
					op = ch;
				}
				if (v.HasValue)
				{
					if (subvalue.HasValue)
					{
						if (op == '+')
						{
							subvalue = v.Value + subvalue.Value;
						}
						else
						{
							if (value.HasValue)
							{
								value *= subvalue;
							}
							else
							{
								value = subvalue;
							}
							subvalue = v;
						}
					}
					else
					{
						subvalue = v;
					}
				}
			}
			if (subvalue.HasValue)
			{
				if (value.HasValue)
				{
					value *= subvalue;
				}
				else
				{
					value = subvalue;
				}
			}
			return value.Value;

		}




		protected override ulong Part2(string[] input)
		{

			return input.Select(EvalLinePart2).Sum();



			return 0;
		}
	}




	//  internal class Puzzle : ComboPart<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//  	protected override int Year => 2020;
	//  	protected override int Day => 18;
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
