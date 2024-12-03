using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2024.Day03
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Mull It Over";
		public override int Year => 2024;
		public override int Day => 3;

		public override void Run()
		{
			Run("test1").Part1(161);
			Run("test2").Part2(48);
			Run("input").Part1(187825547).Part2(85508223);
			Run("extra").Part1(173419328).Part2(90669332);
		}

		protected override long Part1(string[] input)
		{
			var line = string.Concat(input);

			var muls = Regex.Matches(line, @"mul\((\d{1,3}),(\d{1,3})\)");
			var sum = muls.Sum(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var line = string.Concat(input);
			line = Regex.Replace(line, @"don't\(\).*?(do\(\)|$)", "");

			var muls = Regex.Matches(line, @"mul\((\d{1,3}),(\d{1,3})\)");
			var sum = muls.Sum(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));

			return sum;
		}
	}
}
