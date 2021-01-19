using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2016.Day18
{
	internal class Puzzle : Puzzle<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Like a Rogue";
		public override int Year => 2016;
		public override int Day => 18;

		public void Run()
		{
			RunPart1For("test1", 38);
			RunFor("input", 1926, 19986699);
		}

		protected override int Part1(string[] input)
		{
			var tiles = input[0];
			var rows = int.Parse(input[1]);
			return CountSafeTiles(tiles, rows);
		}

		protected override int Part2(string[] input)
		{
			var tiles = input[0];
			return CountSafeTiles(tiles, 400000);
		}

		private static int CountSafeTiles(string tiles, int rows)
		{
			// Surround tiles with empty spaces for easy/fast checks against
			// tiles near the edge without having to check for x>0 and x<len.
			// Use two pre-alloated arrays, switching between them.
			var t0 = ("." + tiles + ".").ToCharArray();
			var t1 = new char[t0.Length];
			t1[0] = t1[t1.Length - 1] = '.';

			var safetiles = 0;
			for (var row = 0; row < rows; row++)
			{
				for (var i = 1; i < t0.Length - 1; i++)
				{
					if (t0[i] == '.')
					{
						safetiles++;
					}
					// Criteria for a trap is:
					//   p == "^.."  \ same as "^?."  \
					//   p == "^^."  /                 \ same as "X?Y" where X != Y
					//   p == "..^"  \ same as ".?^"   /
					//   p == ".^^"  /
					t1[i] = t0[i-1] != t0[i+1] ? '^' : '.';
				}
				// Swap t1 in as the next t0
				var tmp = t0;
				t0 = t1;
				t1 = tmp;
			}

			return safetiles;			
		}
	}
}
