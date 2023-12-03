using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day10.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Syntax Scoring";
		public override int Year => 2021;
		public override int Day => 10;

		public override void Run()
		{
			Run("test1").Part1(26397).Part2(288957);
			//Run("test1").Part2(288957);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(215229).Part2(1105996483);
		}

		protected override long Part1(string[] input)
		{
			var n = 0;
			foreach (var s in input)
			{
				var expect = new Stack<char>();
				var x = FirstIllegal(s, ref expect);
				//if (x != 0)
				//	Console.WriteLine(x);
				n += x;
			}
			//var n = input
			//	.Select(FirstIllegal)
			//	.Sum();

			return n;
		}

		private static int FirstIllegal(string s, ref Stack<char> expect)
		{
			foreach (var ch in s)
			{
				switch (ch)
				{
					case '(': expect.Push(')'); break;
					case '[': expect.Push(']'); break;
					case '{': expect.Push('}'); break;
					case '<': expect.Push('>'); break;
					default:
						{
							var e = expect.Peek();
							if (e != ch)
							{
								if (ch == ')') return 3;
								if (ch == ']') return 57;
								if (ch == '}') return 1197;
								if (ch == '>') return 25137;
								return 0;
							}
							expect.Pop();
						}
						break;
				}
			}
			return 0;
		}

		protected override long Part2(string[] input)
		{
			var lines = input
				.Select(s =>
				{
					var expect = new Stack<char>();
					var x = FirstIllegal(s, ref expect);
					if (x == 0)
					{
						//		Console.Write($"{s}: ");
						//var add = "";
						//expect.Pop();
						//expect.Pop();
						var score = 0L;
						while (expect.Count > 0)
						{
							var ch = expect.Pop();
							score *= 5;
							if (ch == ')') score += 1;
							if (ch == ']') score += 2;
							if (ch == '}') score += 3;
							if (ch == '>') score += 4;

							//add += expect.Pop();
						}
						//		Console.WriteLine(add);
						return score;
					}
					return 0;
				})
				.Where(x => x != 0)
				.OrderBy(x => x)
				.ToArray();
			var score = lines[lines.Length / 2];


			return score;
		}

	}
}