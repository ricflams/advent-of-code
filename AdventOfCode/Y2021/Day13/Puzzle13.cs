using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day13
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Transparent Origami";
		public override int Year => 2021;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(17);
			Run("input").Part1(716).Part2("RPCKFBLR");
			Run("extra").Part1(847).Part2("BCZRCEAB");
		}

		protected override int Part1(string[] input)
		{
			var (paper, w, h, folds) = ReadInput(input);

			// Fold just once
			FoldPaper(paper, ref w, ref h, folds.First());

			// Count the dots
			var dots = 0;
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					if (paper[x, y])
						dots++;
				}
			}

			return dots;
		}


		protected override string Part2(string[] input)
		{
			var (paper, w, h, folds) = ReadInput(input);

			// Perform all folds
			foreach (var fold in folds)
			{
				FoldPaper(paper, ref w, ref h, fold);
			}

			// Convert to printable, easiest done by stuffing into a matrix
			var dotmatrix = CharMatrix.Create(w, h, ' ');
			for (var x = 0; x < w; x++)
			{
				for (var y = 0; y < h; y++)
				{
					if (paper[x, y])
					{
						dotmatrix[x, y] = '#';
					}
				}
			}
			//dotmatrix.ConsoleWrite();

			var code = LetterScanner.Scan(dotmatrix.ToStringArray());
			return code;
		}

		private static (bool[,], int, int, char[]) ReadInput(string[] input)
		{
			var parts = input
				.GroupByEmptyLine()
				.ToArray();

			// The origin isn't at (0,0) but shift it there for convenience
			var points = parts[0]
				.Select(Point.Parse)
				.ToArray();

			// The folding is ALWAYS exactly in the middle in the puzzle (to make it easy)
			// so no need to remember the folding position at all, just the axis.
			// We do need to create a big enough paper though so find the 
			var folds = parts[1]
				.Select(s => ((char Axis, char Fold))s.RxMatch("fold along %c=%d").Get<char,int>())
				.ToArray();
			var axis = folds.Select(x => x.Axis).ToArray();
			var maxx = folds.First(x => x.Axis == 'x').Fold;
			var maxy = folds.First(x => x.Axis == 'y').Fold;
			var w = maxx * 2 + 1;
			var h = maxy * 2 + 1;

			// Now fill the paper with dots
			var paper = new bool[w, h];
			foreach (var p in points)
			{
				paper[p.X, p.Y] = true;
			}


			return (paper, w, h, axis);
		}

		private static void FoldPaper(bool[,] paper, ref int width, ref int height, char fold)
		{
			if (fold == 'x')
			{
				var w = (width - 1) / 2;
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < height; y++)
					{
						paper[x, y] |= paper[width - x - 1, y];
					}
				}
				width = w;
			}
			else
			{
				var h = (height - 1) / 2;
				for (var x = 0; x < width; x++)
				{
					for (var y = 0; y < height; y++)
					{
						paper[x, y] |= paper[x, height - y - 1];
					}
				}
				height = h;
			}
		}
	}
}
