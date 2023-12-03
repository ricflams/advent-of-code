using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day25
{
	internal class Puzzle : Puzzle<string, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Full of Hot Air";
		public override int Year => 2022;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1("2=-1=0");
			Run("test9").Part1("2-=12=2-2-2-=0012==2");
			Run("input").Part1("122-2=200-0111--=200");
		}

		protected override string Part1(string[] input)
		{
			var digits = "=-012";

			var sum = 0L;
			foreach (var s in input)		
			{
				var val = 0L;
				foreach (var ch in s)
				{
					var digit = digits.IndexOf(ch) - 2;
					val = val*5 + digit;
				}
				sum += val;
			}

			var result = "";
			while (sum > 0)
			{
				var val = (int)((sum+2) % 5) - 2;
				result = digits[val+2] + result;
				sum = (sum - val) / 5;
			}
			return result;
		}

		protected override int Part2(string[] _) => 0;
	}
}
