using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2016.Day05
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "How About a Nice Game of Chess?";
		public override int Year => 2016;
		public override int Day => 5;

		public override void Run()
		{
			Run("test1").Part1("18f47a30").Part2("05ace8e3");
			Run("input").Part1("801b56a7").Part2("424a0197");
			Run("extra").Part1("c6697b55").Part2("8c35d1ab");
		}

		protected override string Part1(string[] input)
		{
			var secret = input[0];
			var finder = new Md5HashFinder(Md5HashFinder.Condition5x0);
			var result = finder.FindMatches(secret, 0)
				.Take(8)
				.Select(x => x.Hash[2])
				.Aggregate(0U, (sum, v) => sum << 4 | v);
			return result.ToString("x");
		}

		protected override string Part2(string[] input)
		{
			var N = 8;
			var secret = input[0];
			var finder = new Md5HashFinder(Md5HashFinder.Condition5x0);
			var result = new char[N];
			var found = 0;
			foreach (var match in finder.FindMatches(secret, 0))
			{
				var pos = match.Hash[2];
				var val = (byte)(match.Hash[3]>>4);
				if (pos < N && result[pos] == 0)
				{
					result[pos] = val.ToString("x").First();
					if (++found == N)
					{
						break;
					}
				}
			}

			return new string(result);
		}
	}
}
