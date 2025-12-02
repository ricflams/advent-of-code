using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2025.Day02.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2025;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(1227775554).Part2(4174379265);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(28844599675).Part2(0);
			//Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var ranges= string.Join("", input).SplitByComma();

			var invalids = 0L;
			foreach (var r in ranges)
			{
				var x = r.SplitByAny("-").Select(long.Parse).ToArray();
				var (a, b) = (x[0], x[1]);
				for (var n = a; n <= b; n++)
				{
					var len = n.ToString().Length;
					if (len % 2 != 0)
						continue;
					var f = (long)Math.Pow(10, len / 2);
					var id1 = n / f;
					var id2 = n % f;
					if (id1 == id2)
						invalids += n;
				}
			}

			return invalids;
		}

		protected override long Part2(string[] input)
		{
			var ranges = string.Join("", input).SplitByComma();

			var invalids = 0L;
			foreach (var r in ranges)
			{
				var x = r.SplitByAny("-").Select(long.Parse).ToArray();
				var (a, b) = (x[0], x[1]);
				for (var n = a; n <= b; n++)
				{
					var s = n.ToString();
					var len = s.Length;
					//var sqrt = Math.Sqrt(len);
					for (var parts = 2; parts <= len; parts++)
					{
						if (len % parts != 0)
							continue;
						var plen = len / parts;
						var seq = s[..plen];
						var isRep = Enumerable.Range(1, parts-1).All(i => s.Substring(i * plen, plen) == seq);
						if (isRep)
						{
							invalids += n;
							break;
						}
					}
				}
			}

			return invalids;
		}
	}
}
