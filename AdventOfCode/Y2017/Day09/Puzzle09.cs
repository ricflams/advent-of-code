using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2017.Day09
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Stream Processing";
		public override int Year => 2017;
		public override int Day => 9;

		public override void Run()
		{
			Run("test1").Part1(1);
			Run("test2").Part1(6);
			Run("test3").Part1(5);
			Run("test4").Part1(16);
			Run("test5").Part1(1);
			Run("test6").Part1(9);
			Run("test7").Part1(9);
			Run("test8").Part1(3);
			Run("input").Part1(10050).Part2(4482);
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
