using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
				return ContainsAbba(supernet) && !ContainsAbba(hypernet);

				static bool ContainsAbba(string s)
				{
					for (var i = 0; i < s.Length-3; i++)
					{
						if (s[i] != s[i+1] && s[i] == s[i+3] && s[i+1] == s[i+2])
						{
							return true;
						}
					}
					return false;
				}
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
				var abas = FindAba(supernet).Distinct();

				// Success if any ABA has a matching BAB in the hypernet part
				return abas.Any(x => hypernet.Contains($"{x[1]}{x[0]}{x[1]}"));
			}

			static IEnumerable<string> FindAba(string s)
			{
				for (var i = 0; i < s.Length-2; i++)
				{
					if (s[i] != s[i+1] && s[i] == s[i+2])
					{
						yield return s[i..(i+2)];
					}
				}
			}
		}

		private static (string, string) DecomposeIp7Addr(string ip)
		{
			// Extract (add up and remove) bracketed hypernet-parts from the supernet-part
			var supernet = new StringBuilder();
			var hypernet = new StringBuilder();
			bool hyper = false;
			foreach (var c in ip)
			{
				// When leaving supernet or hupernet, add an arbitrary delimiter to avoid
				// false matches across brackets on these consolidated strings afterwards
				if (c == '[')
				{
					hyper = true;
					supernet.Append('-');
				}
				else if (c == ']')
				{
					hyper = false;
					hypernet.Append('-');
				}
				else if (hyper)
				{
					hypernet.Append(c);
				}
				else
				{
					supernet.Append(c);
				}
			}
			return (supernet.ToString(), hypernet.ToString());
		}		
	}
}
