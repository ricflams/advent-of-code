using AdventOfCode.Helpers;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode.Y2019.Day08
{
	internal class PuzzleXX
	{
		public static void Run()
		{
			Puzzle1And2();
		}

		private static void Puzzle1And2()
		{
			var imagedata = File.ReadAllText("Y2019/Day08/input.txt");
			const int width = 25;
			const int height = 6;

			const int size = width * height;

			// Divide raw imagedata into the individual layers
			var layers = Enumerable.Range(0, imagedata.Length / size)
				.Select(i => imagedata.Substring(i * size, size))
				.ToList();

			// FInd the layer with most 0's and multiply its 1's and 2's
			var layerWithMostZeros = layers
				.Select(l => new
				{
					Count0 = l.Count(x => x == '0'),
					Layer = l
				})
				.OrderBy(x => x.Count0)
				.Select(x => x.Layer)
				.First();
			var sum = layerWithMostZeros.Count(x => x == '1') * layerWithMostZeros.Count(x => x == '2');
			Console.WriteLine($"Day  8 Puzzle 1: {sum}");
			Debug.Assert(sum == 2356);

			// Render all "pixels" by looping through each layer's similar positions and
			// pick the first non-transparent value, turning '1' into black and '2' into blank.
			var rendering = Enumerable.Range(0, size)
				.Select(pos => layers.Select(x => x[pos]))
				.Select(x =>
				{
					var value = x.First(pixel => pixel != '2');
					return value == '1' ? Graphics.FullBlock : ' ';
				})
				.ToArray();
			var image = new string(rendering);

			// Split rendering into <height> individual lines and print them
			var lines = Enumerable.Range(0, height)
				.Select(x => image.Substring(x * width, width));
			foreach (var line in lines)
			{
				Console.WriteLine($"Day  8 Puzzle 2: {line}");
			}
		}
	}
}