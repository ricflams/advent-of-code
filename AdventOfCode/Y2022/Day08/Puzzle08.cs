using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day08
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Treetop Tree House";
		public override int Year => 2022;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(21).Part2(8);
			Run("input").Part1(1711).Part2(301392);
			Run("extra").Part1(1787).Part2(440640);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var (w, h) = map.Dim();
			var vis = new bool[w, h];

			MarkVisibleRowsFromLeft();
			for (var i = 0; i < 3; i++)
			{
				map = map.RotateClockwise(90);
				vis = vis.RotateClockwise(90);
				MarkVisibleRowsFromLeft();
			}

			// edges - 4 duplicate corners + all visible trees
			var n = w*2 + h*2 - 4 + vis.AllValues().Count(x => x);
			return n;

			void MarkVisibleRowsFromLeft()
			{
				var (w, h) = map.Dim();

				for (var y = 1; y < h-1; y++)
				{
					var max = map[0, y];
					for (var x = 1; x < w-1; x++)
					{
						var t = map[x, y];
						if (t > max)
						{
							max = t;
							vis[x, y] = true;
						}
					}
				}
			}			
		}

		protected override long Part2(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var (w, h) = map.Dim();
			var score = CharMatrix.Create<int>(w, h, 1);

			FindScenicScore();
			for (var i = 0; i < 3; i++)
			{
				map = map.RotateClockwise(90);
				score = score.RotateClockwise(90);
				FindScenicScore();
			}

			var maxscore = score.AllValues().Max();
			return maxscore;

			void FindScenicScore()
			{
				var (w, h) = map.Dim();

				for (var y = 0; y < h; y++)
				{
					for (var x = 0; x < w; x++)
					{
						var t = map[x, y];
						var n = 0;
						for (var x2 = x + 1; x2 < w; x2++)
						{
							n++;
							if (map[x2, y] >= t)
								break;
						}						
						score[x, y] *= n;
					}
				}
			}
		}
	}
}
