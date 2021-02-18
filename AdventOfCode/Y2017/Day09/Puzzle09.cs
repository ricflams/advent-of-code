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
			var score = 0;
			var garbage = 0;

			var inGarbage = false;
			var nesting = 0;
			for (var i = 0; i < s.Length; i++)
			{
				var c = s[i];
				if (inGarbage)
				{
					if (c == '!')
						i++;
					else if (c == '>')
						inGarbage = false;
					else
						garbage++;
				}
				else if (c == '{')
					nesting++;
				else if (c == '}')
					score += nesting--;
				else if (c == '<')
					inGarbage = true;
			}

			return (score, garbage);
		}
	}
}
