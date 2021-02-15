using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2017.Day01
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Inverse Captcha";
		public override int Year => 2017;
		public override int Day => 1;

		public void Run()
		{
			RunPart1For("test1", 3);
			RunPart1For("test2", 4);
			RunPart1For("test3", 0);
			RunPart1For("test4", 9);
			RunFor("input", 1175, 1166);
		}

		protected override int Part1(string[] input)
		{
			var captcha = input[0];
			return SequenceSum(captcha, 1);
		}

		protected override int Part2(string[] input)
		{
			var captcha = input[0];
			return SequenceSum(captcha, captcha.Length / 2);
		}

		private static int SequenceSum(string s, int dist)
		{
			var n = 0;
			for (var i = 0; i < s.Length; i++)
			{
				if (s[i] == s[(i+dist) % s.Length])
				{
					n += s[i] - '0';
				}
			}
			return n;
		}
	}
}
