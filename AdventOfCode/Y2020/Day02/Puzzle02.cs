using AdventOfCode.Helpers;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2020.Day02
{
	internal class Puzzle : ComboParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Password Philosophy";
		protected override int Year => 2020;
		protected override int Day => 2;

		public void Run()
		{
			RunFor("test1", 2, 1);
			RunFor("input", 607, 321);
		}

		protected override (int, int) Part1And2(string[] input)
		{
			var result1 = 0;
			var result2 = 0;
			foreach (var line in input)
			{
				line.RegexCapture("%d-%d %c: %s")
					.Get(out int min)
					.Get(out int max)
					.Get(out char c)
					.Get(out string pwd);

				var n = pwd.Count(x => x == c);
				if (n >= min && n <= max)
				{
					result1++;
				}

				if (pwd[min - 1] == c ^ pwd[max - 1] == c)
				{
					result2++;
				}
			}

			return (result1, result2);
		}
	}
}
