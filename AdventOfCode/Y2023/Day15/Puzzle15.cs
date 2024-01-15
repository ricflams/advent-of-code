using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day15
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Lens Library";
		public override int Year => 2023;
		public override int Day => 15;

		public override void Run()
		{
			Run("test1").Part1(1320).Part2(145);
			Run("input").Part1(516070).Part2(244981);
			Run("extra").Part1(516469).Part2(221627);
		}

		protected override long Part1(string[] input)
		{
			var seq = input[0].Split(',');

			var sum = seq.Sum(Hash);

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var seq = input[0].Split(',');

			var boxes = Enumerable.Repeat(0, 256)
				.Select(_ => new List<(string Label, int Focal)>())
				.ToArray();

			foreach (var s in seq)
			{
				if (s[^1] == '-')
				{
					var label = s[..^1];
					boxes[Hash(label)].RemoveAll(x => x.Label == label);
				}
				else
				{
					var parts = s.Split('=').ToArray();
					var (label, focal) = (parts[0], int.Parse(parts[1]));
					var box = boxes[Hash(label)];
					var li = box.IndexOf(x => x.Label == label);
					if (li >= 0)
						box[li] = (label, focal);
					else
						box.Add((label, focal));
				}
			}

			var focusingPower = boxes
				.SelectMany((box, bi) => box.Select((lens, li) => (1+bi) * (1+li) * lens.Focal))
				.Sum();

			return focusingPower;
		}

		private static int Hash(string s) =>
			(int)(s.ToCharArray().Aggregate(0u, (sum, c) => (sum + c) * 17) % 256);
	}
}
