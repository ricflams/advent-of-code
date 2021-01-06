using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2016.Day07
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Internet Protocol Version 7";
		public override int Year => 2016;
		public override int Day => 7;

		public void Run()
		{
			RunPart1For("test1", 2);
			RunPart2For("test2", 3);
			RunFor("input", 110, 242);
		}

		protected override int Part1(string[] input)
		{
			return input.Count(SupportsTls);

			static bool SupportsTls(string s)
			{
				var (supernet, hypernet) = DecomposeIp7Addr(s);
				// The supernet MUST, and the hypernet must NOT, contain an abba sequence
				// Use positive-lookahead of first char to weed out cases of duplicates
				return Regex.IsMatch(supernet, @"(\w)(?!\1)(\w)\2\1") && !Regex.IsMatch(hypernet, @"(\w)(\w)\2\1");
			}
		}

		protected override int Part2(string[] input)
		{
			return input.Count(SupportsSsl);

			static bool SupportsSsl(string s)
			{
				var (supernet, hypernet) = DecomposeIp7Addr(s);

				// ABAs can be overlapping, so we need to search the entire supernet without
				// risk of missing some needed matches due to an unwanted overlapping match
				var abas = Enumerable.Range(0, supernet.Length - 3)
					.SelectMany(startAt => new Regex(@"(\w)(?!\1)(\w)\1").Matches(supernet, startAt))
					.Select(m => m.Value)
					.Distinct()
					.ToArray();

				// Success if any ABA has a matching BAB in the hypernet part
				return abas.Any(x => hypernet.Contains($"{x[1]}{x[0]}{x[1]}"));
			}
		}

		private static (string, string) DecomposeIp7Addr(string ip)
		{
			// Extract (add up and remove) bracketed hypernet-parts from the supernet-part
			var supernet = ip;
			var hypernet = "";
			while (supernet.MaybeRegexCapture("%*[%*]%*").Get(out string s1).Get(out string hyp).Get(out string s2).IsMatch)
			{
				// Add arbitrary delimiter to avoid false matches across bracketed strings
				supernet = s1 + "-" + s2;
				hypernet += hyp + "-";
			}
			return (supernet, hypernet);
		}
	}
}
