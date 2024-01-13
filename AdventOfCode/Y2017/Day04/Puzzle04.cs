using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2017.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "High-Entropy Passphrases";
		public override int Year => 2017;
		public override int Day => 4;

		public override void Run()
		{
			Run("input").Part1(337).Part2(231);
			Run("extra").Part1(451).Part2(223);
		}

		protected override int Part1(string[] input)
		{
			var n = input
				.Sum(line =>
				{
					var seen = new HashSet<string>();
					foreach (var w in line.Split(' '))
					{
						if (seen.Contains(w))
							return 0;
						seen.Add(w);
					}
					return 1;
				});
			return n;
		}

		protected override int Part2(string[] input)
		{
			var n = input
				.Sum(line =>
				{
					var seen = new HashSet<string>();
					foreach (var w in line.Split(' '))
					{
						var s = new string(w.ToCharArray().OrderBy(c => c).ToArray());
						if (seen.Contains(s))
							return 0;
						seen.Add(s);
					}
					return 1;
				});
			return n;
		}
	}
}
