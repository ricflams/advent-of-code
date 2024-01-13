using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day13
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Point of Incidence";
		public override int Year => 2023;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(405).Part2(400);
			Run("input").Part1(31877).Part2(42996);
			Run("extra").Part1(30802).Part2(37876);
		}

		protected override long Part1(string[] input)
		{
			// Result is the sum of all the reflection-patterns
			return input.GroupByEmptyLine().Sum(ReflectionValue);

			static int ReflectionValue(string[] pattern)
			{
				var map = CharMatrix.FromArray(pattern);

				// First try to find a horizontal reflection
				var reflectionRow = FindReflectionRow(map).FirstOrDefault();
				if (reflectionRow > 0)
					return 100 * reflectionRow;

				// Rotate the map and find the reflection which must exist by now
				map = map.RotateClockwise(90);
				return FindReflectionRow(map).Single();
			}
		}

		protected override long Part2(string[] input)
		{
			// Result is the sum of all the smudged reflection-patterns
			return input.GroupByEmptyLine().Sum(ReflectionSmudge);

			static int ReflectionSmudge(string[] pattern)
			{
				var map = CharMatrix.FromArray(pattern);

				// First try to find a horizontal reflection
				var reflectionRow = FindReflectionSmudge(map);
				if (reflectionRow > 0)
					return 100 * reflectionRow;

				// Rotate the map and find the reflection which must exist by now
				map = map.RotateClockwise(90);
				return FindReflectionSmudge(map);

				static int FindReflectionSmudge(char[,] map)
				{
					// Find the reflection that exists without any smudge; we want to
					// skip that one when finding the smudged reflection
					var reflection = FindReflectionRow(map).FirstOrDefault();

					// Loop through the entire map, flip each pattern, and see if it
					// produces a reflection that isn't the original unsmudged reflection.
					// If not then just return 0, which isn't a valid reflection-line.
					var (w, h) = map.Dim();
					for (var x = 0; x < w; x++)
					{
						for (var y  = 0; y < h; y++)
						{
							var ch = map[x, y];
							map[x,y] = ch == '.' ? '#' : '.';
							var r = FindReflectionRow(map).Where(r => r != reflection).FirstOrDefault();
							if (r != 0)
								return r;
							map[x, y] = ch;
						}
					}
					return 0;
				}
			}
		}

		private static IEnumerable<int> FindReflectionRow(char[,] map)
		{
			// Test all horizontal lines for possible reflection, starting
			// at 1 since line 0 can't reflect "upwards". For each horizontal
			// candidate check if all lines up and down are equal; if they are
			// then we've found a reflection-line at hor.
			// Just use two pointers, up and down, instead of being clever and
			// using just one plus some math; this is easier to grasp.
			var (w, N) = map.Dim();
			for (var hor = 1; hor < N; hor++)
			{
				var up = hor - 1;
				var down = hor;
				while (true)
				{
					if (up < 0 || down == N)
					{
						yield return hor;
						break;
					}
					if (!LineMatch(up, down))
						break;
					up--;
					down++;
				}
			}

			bool LineMatch(int a, int b)
			{
				for (var x = 0; x < w; x++)
					if (map[x, a] != map[x, b])
						return false;
				return true;
			}
		}
	}
}
