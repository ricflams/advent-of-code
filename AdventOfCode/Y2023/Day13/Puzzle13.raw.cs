using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Net;
using MathNet.Numerics.Optimization.LineSearch;

namespace AdventOfCode.Y2023.Day13.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 13;

		public override void Run()
		{
			Run("test1").Part1(405).Part2(400);
//			Run("test2").Part1(0).Part2(0);
			Run("input").Part1(31877).Part2(42996);
			// 27418 too low

//			Run("extra").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var maps = input
				.GroupByEmptyLine()
				.Select(x => x.ToArray())
				//.Select(lines => CharMap.FromArray(lines))
				.ToArray();
			
			return maps.Sum(ReflectionValue);
		}

		protected override long Part2(string[] input)
		{
			var maps = input
				.GroupByEmptyLine()
				.ToArray();
			
			return maps.Sum(ReflectionSmudge);
		}

		private static int ReflectionSmudge(string[] map)
		{
			var orgReflection = ReflectionValue2(map).First();
			for (var i = 0; i < map.Length; i++)
			{
				var org = map[i];
				for (var j = 0; j < org.Length; j++)
				{
					var line = org.ToCharArray();
					line[j] = line[j] == '.' ? '#' : '.';
					map[i] = new string(line);
					var reflections = ReflectionValue2(map).ToArray();
					var newReflection = reflections.Where(x => x != orgReflection).FirstOrDefault();
					if (newReflection > 0)
					//if (reflection > 0 && reflection != orgReflection)
						return newReflection;
					map[i] = org;
				}
			}
			Console.WriteLine("bad");
			CharMap.FromArray(map).ConsoleWrite();
			return 0;
			throw new Exception();
		}



		private static IEnumerable<int> ReflectionValue2(string[] map)
		{
			foreach (var yreflex in ReflectionAt2(map))
			// if (yreflex >= 0)
			{
				yield return 100*yreflex;
			}


			var mm = CharMatrix.FromArray(map);
			mm = mm.RotateClockwise(90);
			var map2 = mm.ToStringArray();

			foreach (var xreflex in ReflectionAt2(map2))
			//if (xreflex >= 0)
			{
				yield return xreflex;
			}

			// return -1;
		}

		private static IEnumerable<int> ReflectionAt2(string[] map)
		{
			var h = map.Length;

			for (var top = 1; top < h; top++)
			{
				var y1 = top-1;
				var y2 = top;
				while (true)
				{
					if (y1 < 0 || y2 == h)
					{
						yield return top;
						break;
					}
					if (map[y1] != map[y2])
						break;
					y1--;
					y2++;
				}
			}
		}

		private static int ReflectionValue(string[] map)
		{
			var yreflex = ReflectionAt(map);
			if (yreflex >= 0)
			{
				return 100*yreflex;
			}


			var mm = CharMatrix.FromArray(map);
			mm = mm.RotateClockwise(90);
			var map2 = mm.ToStringArray();

			var xreflex = ReflectionAt(map2);
			if (xreflex >= 0)
			{
				return xreflex;
			}

			return -1;
		}

		private static int ReflectionAt(string[] map)
		{
			var h = map.Length;

			for (var top = 1; top < h; top++)
			{
				var y1 = top-1;
				var y2 = top;
				while (true)
				{
					if (y1 < 0 || y2 == h)
						return top;
					if (map[y1] != map[y2])
						break;
					y1--;
					y2++;
				}
			}
			return -1;

		}


	}
}
