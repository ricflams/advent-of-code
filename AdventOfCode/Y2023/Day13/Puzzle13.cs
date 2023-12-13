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
			return input.GroupByEmptyLine().Sum(ReflectionValue);

			static int ReflectionValue(string[] map)
			{
				var yreflex = FindReflections(map).FirstOrDefault();
				if (yreflex > 0)
					return 100*yreflex;

				map = CharMatrix.FromArray(map).RotateClockwise(90).ToStringArray();
				return FindReflections(map).First();
			}

		}

		protected override long Part2(string[] input)
		{
			return input.GroupByEmptyLine().Sum(ReflectionSmudge);

			static int ReflectionSmudge(string[] map)
			{
				var reflection = FindReflectionSmudge(map).FirstOrDefault();
				if (reflection > 0)
					return 100*reflection;

				map = CharMatrix.FromArray(map).RotateClockwise(90).ToStringArray();
				return FindReflectionSmudge(map).First();

				static IEnumerable<int> FindReflectionSmudge(string[] map)
				{
					var reflection = FindReflections(map).FirstOrDefault();

					for (var i = 0; i < map.Length; i++)
					{
						var line = map[i].ToCharArray();
						for (var j = 0; j < line.Length; j++)
						{
							map[i] = Flip(line, j);
							foreach (var r in FindReflections(map).Where(r => r != reflection))
								yield return r;
							map[i] = Flip(line, j);
						}
					}

					static string Flip(char[] line, int pos)
					{
						line[pos] = line[pos] == '.' ? '#' : '.';
						return new string(line);
					}
				}			
			}
		}

		private static IEnumerable<int> FindReflections(string[] map)
		{
			// Test all horizontal lines for possible reflection, starting
			// at 1 since line 0 can't reflect "upwards". For each horizontal
			// candidate check if all lines up and down are equal; if they are
			// then we've found a reflection-line at hor.
			// Just use two pointers, up and down, instead of being clever and
			// using just one plus some math; this is easier to grasp.
			var N = map.Length;
			for (var hor = 1; hor < N; hor++)
			{
				var up = hor-1;
				var down = hor;
				while (true)
				{
					if (up < 0 || down == N)
					{
						yield return hor;
						break;
					}
					if (map[up] != map[down])
						break;
					up--;
					down++;
				}
			}
		}
	}
}
