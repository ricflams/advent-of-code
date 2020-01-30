using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace AdventOfCode.Y2015.Day02
{
	internal class Puzzle02
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var presents = File.ReadAllLines("Y2015/Day02/input.txt")
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

			Console.WriteLine($"Day  2 Puzzle 1: {totalarea}");
			Debug.Assert(totalarea == 1588178);
		}

		private static void Puzzle2()
		{
			var presents = File.ReadAllLines("Y2015/Day02/input.txt")
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

			Console.WriteLine($"Day  2 Puzzle 2: {totalribbon}");
			Debug.Assert(totalribbon == 3783758);
		}
	}
}