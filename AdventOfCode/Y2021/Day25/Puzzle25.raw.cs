using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day25.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 25";
		public override int Year => 2021;
		public override int Day => 25;

		public override void Run()
		{
			Run("test1").Part1(58).Part2(0);

			//Run("test2").Part1(0).Part2(0);

			Run("input").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMatrix.FromArray(input);
			var (w, h) = map.Dim();
			var rounds = 0;
			while (true)
			{
				var map2 = map.CopyPart(0, 0, w, h);
				var moves = false;
				rounds++;
				for (var x = 0; x < w; x++)
                {
					for (var y = 0; y < h; y++)
					{
						switch (map[x, y])
                        {
							case '>':
								if (map[(x + 1) % w, y] == '.')
                                {
									map2[x, y] = '.';
									map2[(x + 1) % w, y] = '>';
									moves = true;
								}
								break;
						}
					}
				}
				var map3 = map2.CopyPart(0, 0, w, h);
				for (var x = 0; x < w; x++)
				{
					for (var y = 0; y < h; y++)
					{
						switch (map2[x, y])
						{
							case 'v':
								if (map2[x, (y + 1) % h] == '.')
								{
									map3[x, y] = '.';
									map3[x, (y + 1) % h] = 'v';
									moves = true;
								}
								break;
						}
					}
				}
				if (!moves)
					break;
				map = map3;
				//map.ConsoleWrite();
				//Console.WriteLine();
			}

			return rounds;
		}

		protected override long Part2(string[] input)
		{


			return 0;
		}
	}
}
