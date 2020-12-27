using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2019.Day08
{
	internal class Puzzle : ComboParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2019;
		protected override int Day => 8;

		public void Run()
		{
			RunFor("input", "2356", "PZEKB");
		}

		protected override (string, string) Part1And2(string[] input)
		{
			var imagedata = input[0];
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

			// Split rendering into <height> individual lines and scan them
			var lines = Enumerable.Range(0, height)
				.Select(x => image.Substring(x * width, width));
			var message = LetterScanner.Scan(lines);

			return (sum.ToString(), message);
		}
	}
}