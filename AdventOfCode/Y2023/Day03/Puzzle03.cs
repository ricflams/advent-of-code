using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day03
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Gear Ratios";
		public override int Year => 2023;
		public override int Day => 3;

		public override void Run()
		{
			Run("test1").Part1(4361).Part2(467835);
			Run("input").Part1(533775).Part2(78236071);
			Run("test9").Part1(529618).Part2(77509019);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input);

			// Find all unique positions where there's a digit next to a symbol
			var digits = map.AllPoints(ch => ch != '.' && !char.IsDigit(ch))
				.SelectMany(p => p.LookDiagonallyAround())
				.Where(p => char.IsDigit(map[p]))
				.Distinct();

			// Sum up and make sure to only count each number once
			var sum = 0;
			var seen = new HashSet<Point>();
			foreach (var p in digits)
			{
				var start = StartOfValue(map, p);
				if (seen.Contains(start))
					continue;
				seen.Add(start);
				sum += GetValueAt(map, start);
			}

			return sum;
		}

		protected override long Part2(string[] input)
		{
			var map = CharMap.FromArray(input);

			// We're only interested in numbers adjacent to '*'
			var sum = map
				.AllPoints(ch => ch == '*')
				.Sum(dp =>
				{
					var gearDigits = new List<Point>();
					bool AddIfDigit(Point p)
					{
						var isDigit = char.IsDigit(map[p]);
						if (isDigit)
							gearDigits.Add(p);
						return isDigit;
					}

					// Find all adjacent numbers.
					// There are possibly numbers left and right.
					// There's possibly a number right at the top/bottom and if not
					// then there are possibly numbers there to the left or right.
					AddIfDigit(dp.Left);
					AddIfDigit(dp.Right);
					if (!AddIfDigit(dp.Up))
					{
						AddIfDigit(dp.DiagonalUpLeft);
						AddIfDigit(dp.DiagonalUpRight);
					}
					if (!AddIfDigit(dp.Down))
					{
						AddIfDigit(dp.DiagonalDownLeft);
						AddIfDigit(dp.DiagonalDownRight);
					}				

					// Only the gear-ratio if exactly two numbers are adjacent
					if (gearDigits.Count != 2)
						return 0;
					var v1 = GetValueAt(map, StartOfValue(map, gearDigits[0]));
					var v2 = GetValueAt(map, StartOfValue(map, gearDigits[1]));
					return v1*v2;
				});

			return sum;
		}

		private static Point StartOfValue(CharMap map, Point p)
		{
			while (char.IsDigit(map[p.Left]))
				p = p.Left;
			return p;
		}		

		private static int GetValueAt(CharMap map, Point p)
		{
			var v = 0;
			while (char.IsDigit(map[p]))
			{
				v = v*10 + map[p] - '0';
				p = p.Right;
			}
			return v;
		}
		
	}
}
