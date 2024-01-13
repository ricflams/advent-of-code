using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day11
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Chronal Charge";
		public override int Year => 2018;
		public override int Day => 11;

		public override void Run()
		{
			Run("test1").Part1("33,45").Part2("90,269,16");
			Run("test2").Part1("21,61").Part2("232,251,12");
			Run("input").Part1("21,77").Part2("224,222,27");
			Run("extra").Part1("34,72").Part2("233,187,13");
		}

		private const int N = 300;

		protected override string Part1(string[] input)
		{
			var serial = int.Parse(input[0]);
			var grid = ReadGrid(serial);

			// Just do the sums manually by exploring and summing up every 3x3 square
			var maxsum = 0;
			var result = "not found";
			for (var x = 1; x <= N-3; x++)
			{
				for (var y = 1; y <= N-3; y++)
				{
					var sum = 0;
					for (var dx = 0; dx < 3; dx++)
					{
						for (var dy = 0; dy < 3; dy++)
						{
							sum += grid[x+dx, y+dy];
						}
					}
					if (sum > maxsum)
					{
						maxsum = sum;
						result = $"{x},{y}";
					}
				}
			}

			return result;
		}

		protected override string Part2(string[] input)
		{
			var serial = int.Parse(input[0]);
			var grid = ReadGrid(serial);

			// This two-step can be simplified, see xysum below
			// // var xsum = new int[N+1,N+1];
			// // for (var x = 1; x <= N; x++)
			// // {
			// // 	var sum = 0;
			// // 	for (var y = 1; y <= N; y++)
			// // 	{
			// // 		sum += grid[x, y];
			// // 		xsum[x, y] = sum;
			// // 	}
			// // }
			// // var xysum = new int[N+1,N+1];
			// // for (var y = 1; y <= N; y++)
			// // {
			// // 	var sum = 0;
			// // 	for (var x = 1; x <= N; x++)
			// // 	{
			// // 		sum += xsum[x, y];
			// // 		xysum[x, y] = sum;
			// // 	}
			// // }

			// Create a super-total lookup table, that holds the total of the sum of
			// all elements from top-left to bottom-right of a square. See deleted code
			// above for a more elaborate version.
			var xysum = new int[N+1,N+1];
			for (var x = 1; x <= N; x++)
			{
				for (var y = 1; y <= N; y++)
				{
					//   V1,1  V2,1
					//   V1,2  V2,2 = value + V1,2 + V2,1 - V1,1, because it's counted twice by the other two
					xysum[x, y] = grid[x,y] + xysum[x-1, y] + xysum[x, y-1] - xysum[x-1, y-1];
				}
			}

			// This time, count from 0 and not 1, because the xysum-table works by doing
			// subtractions with the sum "below" the x,y coordinate
			var maxsum = 0;
			var result = "not found";
			for (var x = 0; x < N; x++)
			{
				for (var y = 0; y < N; y++)
				{
					// Just start with size 2; the largest square is not going to be 1x1
					for (var size = 2; x+size < N && y+size < N; size++)
					{
						// In this rectangle:
						//   A1 A2 A3 A4 A5
						//    .  .  .  .  .
						//    .  .  .  .  .
						//    .  .  .  .  .
						//   B1 B2 B3 B4 B5
						// where B1-A1 is the sum of squares from A1...B1, we could calculate the
						// total sum as
						//    B1-A1 + B2-A2 ... B5-A5
						// == B1+B2+B3+B4+B5 - (A1+A2+A3+A4+A5)
						// But we have already calculated the sum of any Bn-Bm; that's what's in
						// the xysum-table. So finding the total is a matter of finding that total
						// in the last line minus the total of the first line:
						var sum =
							(xysum[x+size, y+size] - xysum[x, y+size]) -
							(xysum[x+size, y] - xysum[x, y]);
						if (sum > maxsum)
						{
							maxsum = sum;
							result = $"{x+1},{y+1},{size}";
						}
					}
				}
			}
			return result;
		}

		private static int[,] ReadGrid(int serial)
		{
			// The sum-calculations become much easier if there's an empty row/column
			// with 0 in it before the actual numbers. So stick with 1-300, not 0-based.
			var grid = new int[N+1, N+1];
			for (var x = 1; x <= N; x++)
			{
				for (var y = 1; y <= N; y++)
				{
					var v1 = ((x + 10)*y + serial)*(x + 10);
					var v2 = (v1 / 100) % 10 - 5;
					grid[x, y] = v2;
				}
			}
			return grid;
		}
	}
}
