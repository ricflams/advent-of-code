using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2017.Day21
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Fractal Art";
		public override int Year => 2017;
		public override int Day => 21;

		public void Run()
		{
			RunFor("input", 147, 1936582);
		}

		protected override int Part1(string[] input)
		{
			return FindPixelsOnAfter(input, 5);
		}

		protected override int Part2(string[] input)
		{
			return FindPixelsOnAfter(input, 18);
		}

		private static int FindPixelsOnAfter(string[] input, int steps)
		{
			var mappings = new Enhancements(input);
			var pattern = ".#./..#/###".Split('/').ToCharMatrix();

			for (var loop = 0; loop < steps; loop++)
			{
				var w = pattern.Width();
				var dim = w % 2 == 0 ? 2 : 3;
				var newdim = dim + 1;
				var n = w / dim; // n is number of new squares across or down (same number)
				var nextPattern = new char[n*newdim, n*newdim];
				for (var i = 0; i < n; i++)
				{
					for (var j = 0; j < n; j++)
					{
						var part = pattern.CopyPart(i*dim, j*dim, dim, dim);
						var replacement = mappings.Enhance(part);
						nextPattern.PastePart(i*newdim, j*newdim, replacement);
					}
				}
				pattern = nextPattern;
			}

			var pixelsOn = pattern.CountChar('#');
			return pixelsOn;
		}


		internal class Enhancements
		{
			private readonly Dictionary<string, char[,]> _rules;

			public Enhancements(string[] input)
			{
				_rules = new Dictionary<string, char[,]>();
				foreach (var e in input.Select(x => new Enhancement(x)))
				{
					foreach (var from in e.Froms)
					{
						_rules[from] = e.To;
					}
				}
			}

			public char[,] Enhance(char[,] from)
			{
				var flat = string.Join('/', from.ToStringArray());
				return _rules[flat];
			}
		}

		internal class Enhancement
		{
			public Enhancement(string def)
			{
				// ##/## => .##/#../##.
				// .../.../... => .#.#/###./##.#/###.
				var parts = def.Split(" => ");
				Dim = parts[0].Length == 5 ? 2 : 3;
				var from = parts[0].Split('/').ToCharMatrix();
				To = parts[1].Split('/').ToCharMatrix();
				var froms = new List<string>();
				for (var angle = 0; angle < 360; angle += 90)
				{
					var pat = from.RotateClockwise(angle);
					froms.Add(string.Join('/', pat.ToStringArray()));
					froms.Add(string.Join('/', pat.FlipV().ToStringArray()));
				}
				Froms = froms.ToArray();
			}
			public int Dim { get; }
			public string[] Froms { get; }
			public char[,] To { get; }
		}
	}
}
