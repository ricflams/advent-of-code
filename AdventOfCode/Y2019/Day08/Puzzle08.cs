using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2019.Day08
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Space Image Format";
		public override int Year => 2019;
		public override int Day => 8;

		public override void Run()
		{
			Run("input").Part1(2356).Part2("PZEKB");
		}

		const int Width = 25;
		const int Height = 6;
		const int Size = Width * Height;

		protected override int Part1(string[] input)
		{
			var layers = GetLayers(input[0]);

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

			return sum;
		}

		protected override string Part2(string[] input)
		{
			var layers = GetLayers(input[0]);

			// Render all "pixels" by looping through each layer's similar positions and
			// pick the first non-transparent value, turning '1' into black and '2' into blank.
			var rendering = Enumerable.Range(0, Size)
				.Select(pos => layers.Select(x => x[pos]))
				.Select(x =>
				{
					var value = x.First(pixel => pixel != '2');
					return value == '1' ? '#' : ' ';
				})
				.ToArray();
			var image = new string(rendering);

			// Split rendering into <height> individual lines and scan them
			var lines = Enumerable.Range(0, Height)
				.Select(x => image.Substring(x * Width, Width));
			var message = LetterScanner.Scan(lines);

			return message;
		}

		private static string[] GetLayers(string imagedata)
		{
			// Divide raw imagedata into the individual layers
			var layers = Enumerable.Range(0, imagedata.Length / Size)
				.Select(i => imagedata.Substring(i * Size, Size))
				.ToArray();
			return layers;
		}		
	}
}