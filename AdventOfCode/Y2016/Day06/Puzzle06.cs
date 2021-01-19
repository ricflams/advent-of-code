using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2016.Day06
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Signals and Noise";
		public override int Year => 2016;
		public override int Day => 6;

		public void Run()
		{
			RunFor("test1", "easter", "advent");
			RunFor("input", "mshjnduc", "apfeeebz");
		}

		protected override string Part1(string[] input)
		{
			return Decode(input, true);
		}

		protected override string Part2(string[] input)
		{
			return Decode(input, false);
		}

		private string Decode(string[] input, bool takeMostFrequentChar)
		{
			// Rotate input, then find the most frequent (or infrequent) letters in each line
			var lines = input.ToCharMatrix().RotateClockwise(90).ToStringArray();
			var letters = takeMostFrequentChar
				? lines.Select(line => line.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First())
				: lines.Select(line => line.GroupBy(x => x).OrderBy(x => x.Count()).Select(x => x.Key).First());
			return new string(letters.ToArray());
		}
	}
}
