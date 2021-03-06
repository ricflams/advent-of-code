using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2017.Day21
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2017;
		public override int Day => 21;

		public void Run()
		{
			//RunFor("test1", 0, 0);
			//RunFor("test2", 0, 0);
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
			var pat = ".#./..#/###".Split('/').ToCharMatrix();

			for (var loop = 0; loop < steps; loop++)
			{
				var w = pat.Width();
				var dim = w % 2 == 0 ? 2 : 3;
				var newdim = dim + 1;
				var n = w / dim; // n is number of new squares across or down (same number)
				var pat2 = new char[n*newdim, n*newdim];
				for (var i = 0; i < n; i++)
				{
					for (var j = 0; j < n; j++)
					{
						var part = pat.CopyPart(i*dim, j*dim, dim, dim);
						var replacement = mappings.Enhance(part);
						pat2.PastePart(i*newdim, j*newdim, replacement);
					}
				}
				pat = pat2;
				var on0 = pat.CountChar('#');
				Console.WriteLine($"on={on0}");
			}

			var on = pat.CountChar('#');


			return on;
		}


		internal class Enhancements
		{
			private readonly Dictionary<string, Enhancement> _rules;

			public Enhancements(string[] input)
			{
				_rules = input
					.Select(x => new Enhancement(x))
					.ToDictionary(x => x.From, x => x);
			}

			public char[,] Enhance(char[,] from)
			{
				for (var angle = 0; angle < 360; angle += 90)
				{
					var pat = from.RotateClockwise(angle);
					var enhance = TryEnhance(pat) ?? TryEnhance(pat.FlipV());
					if (enhance != null)
						return enhance;
				}
				throw new Exception($"No enhancement");

				char[,] TryEnhance(char[,] from)
				{
					var flat = string.Join('/', from.ToStringArray());
					return _rules.TryGetValue(flat, out var e) ? e.To : null;
				}
			}
		}

		internal class Enhancement
		{
			public Enhancement(string def)
			{
				// ##/## => .##/#../##.
				// .../.../... => .#.#/###./##.#/###.
				var parts = def.Split(" => ");
				From = parts[0];
				To = parts[1].Split('/').ToCharMatrix();
				Dim = From.Length == 5 ? 2 : 3;
			}
			public int Dim { get; }
			public string From { get; }
			public char[,] To { get; }
		}

	}
}
