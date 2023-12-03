using System.Diagnostics;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day20
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Trench Map";
		public override int Year => 2021;
		public override int Day => 20;

		public override void Run()
		{
			Run("test1").Part1(35).Part2(3351);
			Run("input").Part1(5229).Part2(17009);
		}

		protected override long Part1(string[] input)
		{
			return LitPixelsAfterEnhancement(input, 2);
		}

		protected override long Part2(string[] input)
		{
			return LitPixelsAfterEnhancement(input, 50);
		}

		private int LitPixelsAfterEnhancement(string[] input, int n)
		{
			var algorithm = input[0].ToCharArray();
			var image = CharMatrix.FromArray(input.Skip(2).ToArray());
			return LitPixelsAfterEnhancement(algorithm, image, n);
		}

		private static int LitPixelsAfterEnhancement(char[] algorithm, char[,] image, int n)
		{
			// Create two scratchpad images to do the transformations on.
			// Init them to the final size plus 1, so even the last transformation
			// has a border to work on.
			// Take pixel-reversal into account by looking at algorithm 0.
			// The approach only works for even transformations.
			Debug.Assert(n % 2 == 0);
			var padding = n + 1;
			var images = new[]
			{
				image.ExpandBy(padding, '.'),
				image.ExpandBy(padding, algorithm[0] == '.' ? '.' : '#')
			};
			var (w, h) = images[0].Dim();

			// Transform the inner parts repeatedly, alternating between images
			// for doing read/write and increasing the area at every loop (by
			// decreasing the padding)
			for (var i = 0; i < n; i++)
			{
				var src = images[i % 2];
				var dst = images[(i + 1) % 2];
				padding--;

				for (var x = padding; x < w - padding; x++)
				{
					for (var y = padding; y < h - padding; y++)
					{
						var idx = 0;
						if (src[x + 1, y + 1] == '#') idx += 1;
						if (src[x, y + 1] == '#') idx += 2;
						if (src[x - 1, y + 1] == '#') idx += 4;
						if (src[x + 1, y] == '#') idx += 8;
						if (src[x, y] == '#') idx += 16;
						if (src[x - 1, y] == '#') idx += 32;
						if (src[x + 1, y - 1] == '#') idx += 64;
						if (src[x, y - 1] == '#') idx += 128;
						if (src[x - 1, y - 1] == '#') idx += 256;
						dst[x, y] = algorithm[idx];
					}
				}
			}

			// The resulting image always ends up in position 0
			return images[0].CountChar('#');
		}
	}
}
