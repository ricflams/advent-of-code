using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day25
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Sea Cucumber";
		public override int Year => 2021;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1(58);
			Run("input").Part1(386);
		}

		protected override int Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var (w, h) = map.Dim();

			var rounds = 0;
			while (true)
			{
				rounds++;
				var moves = false;

				var east = map.Copy();
				for (var x = 0; x < w; x++)
                {
					var xnext = (x + 1) % w;
					for (var y = 0; y < h; y++)
					{
						if (map[x, y] == '>' && map[xnext, y] == '.')
                        {
							east[x, y] = '.';
							east[xnext, y] = '>';
							moves = true;
						}
					}
				}

				var down = east.Copy();
				for (var y = 0; y < h; y++)
				{
					var ynext = (y + 1) % h;
					for (var x = 0; x < w; x++)
					{
						if (east[x, y] == 'v' && east[x, ynext] == '.')
						{
							down[x, y] = '.';
							down[x, ynext] = 'v';
							moves = true;
						}
					}
				}

				map = down;
				if (!moves)
					break;
			}

			return rounds;
		}

		protected override int Part2(string[] _) => 0;
	}
}
