using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;

namespace AdventOfCode.Y2017.Day09
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Stream Processing";
		public override int Year => 2017;
		public override int Day => 9;

		public void Run()
		{
			RunPart1For("test1", 1);
			RunPart1For("test2", 6);
			RunPart1For("test3", 5);
			RunPart1For("test4", 16);
			RunPart1For("test5", 1);
			RunPart1For("test6", 9);
			RunPart1For("test7", 9);
			RunPart1For("test8", 3);
			RunFor("input", 10050, 4482);
		}

		protected override int Part1(string[] input)
		{
			var stream = input[0];
			var (score, _) = CalcScoreAndGarbage(stream);
			return score;
		}

		protected override int Part2(string[] input)
		{
			var stream = input[0];
			var (_, garbage) = CalcScoreAndGarbage(stream);
			return garbage;
		}

		private static (int, int) CalcScoreAndGarbage(string s)
		{
			// Easiest (to avoid initial special case of "no groups") to populate the
			// group-score stack with the always present outer group, that has a score
			// of 1.
			var groupscore = new Stack<int>();
			groupscore.Push(1);
			var score = 1;

			var garbage = 0;

			// Loop the inner part of the outer-most {}-group
			var N = s.Length - 1;
			for (var i = 1; i < N; i++)
			{
				switch (s[i])
				{
					case '{':
						groupscore.Push(groupscore.Peek() + 1);
						break;

					case '}':
						score += groupscore.Pop();
						break;

					case ',':
						break;
					
					case '<':
						i++;
						while (s[i] != '>')
						{
							// If looking at ! then repeat skipping it and then next char; else
							// we're looking at real garbage, which should be skipped and counted
							if (s[i] == '!')
							{
								while (s[i] == '!')
									i+=2;
							}
							else
							{
								i++;
								garbage++;
							}
						}
						break;
				}
			}

			return (score, garbage);
		}
	}
}
