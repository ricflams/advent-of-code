using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Linq;

namespace AdventOfCode.Y2025.Day02
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Gift Shop";
		public override int Year => 2025;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(1227775554).Part2(4174379265);
			Run("input").Part1(28844599675).Part2(48778605167);
		}

		protected override long Part1(string[] input)
		{
			var ranges= string.Join("", input).SplitByComma();

			var n = 0L;
			foreach (var range in ranges)
			{
				var x = range.SplitByAny("-").Select(long.Parse).ToArray();
				var (a, b) = (x[0], x[1]);
				for (var val = a; val <= b; val++)
				{
					var s = val.ToString().ToCharArray();
					if (IsInvalidId(s, 2))
						n += val;
				}
			}

			return n;
		}

		protected override long Part2(string[] input)
		{
			var ranges = string.Join("", input).SplitByComma();

			var n = 0L;
			foreach (var range in ranges)
			{
				var x = range.SplitByAny("-").Select(long.Parse).ToArray();
				var (a, b) = (x[0], x[1]);
				for (var val = a; val <= b; val++)
				{
					var s = val.ToString().ToCharArray();
					var len = s.Length;
					for (var split = 2; split <= len; split++)
					{
						if (IsInvalidId(s, split))
						{
							n += val;
							break;
						}
					}
				}
			}

			return n;
		}

		static bool IsInvalidId(char[] s, int split)
		{
			var len = s.Length;
			if (len % split != 0)
				return false;
			var seqLen = len / split;

			for (var part = seqLen; part < len; part += seqLen)
			{
				for (var j = 0; j < seqLen; j++)
				{
					if (s[j] != s[part + j])
						return false;
				}
			}
			return true;
		}
	}
}
