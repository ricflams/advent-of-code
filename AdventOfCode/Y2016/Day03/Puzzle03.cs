using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2016.Day03
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Squares With Three Sides";
		public override int Year => 2016;
		public override int Day => 3;

		public override void Run()
		{
			Run("input").Part1(993).Part2(1849);
		}

		protected override int Part1(string[] input)
		{
			var valid = input
				.Where(line => 
				{
					var side = line.ToIntArray()
						.OrderBy(x => x)
						.ToArray();
					return side[0] + side[1] > side[2];
				})
				.Count();

			return valid;
		}

		protected override int Part2(string[] input)
		{
			var tv1 = new TriangleValidator();
			var tv2 = new TriangleValidator();
			var tv3 = new TriangleValidator();
			foreach (var line in input)
			{
				var side = line.ToIntArray();
				tv1.AddSide(side[0]);
				tv2.AddSide(side[1]);
				tv3.AddSide(side[2]);
			}
			var valid = tv1.Count + tv2.Count + tv3.Count;
			return valid;
		}

		private class TriangleValidator
		{
			private readonly List<int> _sides = new List<int>();
			public int Count { get; set; }
			public void AddSide(int side)
			{
				_sides.Add(side);
				if (_sides.Count() == 3)
				{
					var s = _sides.OrderBy(x => x).ToArray();
					if (s[0] + s[1] > s[2])
					{
						Count++;
					}
					_sides.Clear();
				}
			}
		}		
	}
}
