using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day10
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
			Run("input").Part1(215229).Part2(1105996483);
		}

		protected override long Part1(string[] input)
		{
			// Find the sum of all invalid lines' syntax-error-scores
			var score = input
				.Select(s =>
				{
					var (x, _) = FindSyntaxErrorScoreAndExpectedTail(s);
					return x;
				})
				.Sum();
			return score;
		}

		protected override long Part2(string[] input)
		{
			// Find the scores of all non-invalid lines' missing parts
			var scores = input
				.Select(s =>
				{
					var (syntaxScore, expect) = FindSyntaxErrorScoreAndExpectedTail(s);
					var score = 0L;
					if (syntaxScore == 0)
					{
						while (expect.Any())
						{
							var ch = expect.Pop();
							score *= 5;
							if (ch == ')') score += 1;
							if (ch == ']') score += 2;
							if (ch == '}') score += 3;
							if (ch == '>') score += 4;
						}
					}
					return score;
				})
				.Where(x => x != 0)
				.OrderBy(x => x)
				.ToArray();
			var score = scores[scores.Length / 2];

			return score;
		}

		private static (int, Stack<char>) FindSyntaxErrorScoreAndExpectedTail(string s)
		{
			var expect = new Stack<char>();

			foreach (var ch in s)
			{
				switch (ch)
				{
					case '(': expect.Push(')'); break;
					case '[': expect.Push(']'); break;
					case '{': expect.Push('}'); break;
					case '<': expect.Push('>'); break;
					default:
						if (expect.Peek() != ch)
						{
							var score = ch switch
							{
								')' => 3,
								']' => 57,
								'}' => 1197,
								'>' => 25137,
								_ => throw new Exception($"Unexpected {ch}")
							};
							return (score, expect);
						}
						expect.Pop();
						break;
				}
			}

			// Line is valid so syntax-error-score is 0
			return (0, expect);
		}
	}
}
