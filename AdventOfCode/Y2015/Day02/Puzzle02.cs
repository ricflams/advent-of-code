using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2015.Day02
{
	internal class Puzzle : Puzzle<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "I Was Told There Would Be No Math";
		public override int Year => 2015;
		public override int Day => 2;

		public void Run()
		{
			RunFor("input", 1588178, 3783758);
		}

		protected override int Part1(string[] input)
		{
			var presents = input
				.Select(x => x.Split('x').Select(int.Parse).ToArray());

			// surface area of the box, which is 2*l*w + 2*w*h + 2*h*l
			var totalarea = presents
				.Select(x =>
				{
					var (l, w, h) = (x[0], x[1], x[2]);
					var (a1, a2, a3) = (l * w, w * h, h * l);
					var spare = Math.Min(Math.Min(a1, a2), a3);
					var area = 2 * (a1 + a2 + a3) + spare;
					return area;
				})
				.Sum();

			return totalarea;
		}

		protected override int Part2(string[] input)
		{
			var presents = input
				.Select(x => x.Split('x').Select(int.Parse).ToArray());

			// Ribbon: shortest distance around + bow equal to the cubic feet of volume
			var totalribbon = presents
				.Select(x =>
				{
					var shortest = x.OrderBy(s => s).Take(2);
					var bow = x[0] * x[1] * x[2];
					var ribbon = 2 * shortest.Sum() + bow;
					return ribbon;
				})
				.Sum();

			return totalribbon;
		}
	}
}
