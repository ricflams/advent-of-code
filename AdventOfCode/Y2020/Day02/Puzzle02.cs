using AdventOfCode.Helpers;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day02
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Password Philosophy";
		public override int Year => 2020;
		public override int Day => 2;

		public void Run()
		{
			RunFor("test1", 2, 1);
			RunFor("input", 607, 321);
		}

		protected override int Part1(string[] input)
		{
			var result = 0;
			foreach (var line in input)
			{
				var (min, max, c, pwd) = line.RxMatch("%d-%d %c: %s").Get<int, int, char, string>();
				var n = pwd.Count(x => x == c);
				if (n >= min && n <= max)
				{
					result++;
				}
			}
			return result;
		}
		
		protected override int Part2(string[] input)
		{
			var result = 0;
			foreach (var line in input)
			{
				var (min, max, c, pwd) = line.RxMatch("%d-%d %c: %s").Get<int, int, char, string>();
				if (pwd[min - 1] == c ^ pwd[max - 1] == c)
				{
					result++;
				}
			}
			return result;
		}
	}
}
