using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day04
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Giant Squid";
		public override int Year => 2021;
		public override int Day => 4;

		public void Run()
		{
			Run("test1").Part1(4512).Part2(1924);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(50008).Part2(17408);
		}

		protected override int Part1(string[] input)
		{
			var numbers = input.First().ToIntArray();

			var boards = input
				.Skip(1)
				.GroupByEmptyLine()
				.Select(lines => lines.Select(x => x.ToIntArray()).ToArray())
				.ToArray();

			foreach (var n in numbers)
			{
				foreach (var b in boards)
				{
					for (var x = 0; x < 5; x++)
					{
						for (var y = 0; y < 5; y++)
						{
							if (b[x][y] == n)
							{
								b[x][y] = -1;

								var coln = 0;
								for (var yy = 0; yy < 5; yy++)
								{
									if (b[x][yy] == -1)
										coln++;
								}

								if (coln == 5 || b[x].All(v => v == -1))
								{
									// found board!
									var unmarked = b.Sum(line => line.Where(x => x != -1).Sum());
									var score = n * unmarked;

									return score;
								}

								x = 10;
								y = 10;
								break;
							}
						}
					}

				}
			}


			return 0;
		}

		protected override int Part2(string[] input)
		{
			var numbers = input.First().ToIntArray();

			var raw = input.Skip(2).GroupByEmptyLine().ToArray();
			var boards = raw
				.Select(lines => lines.Select(line => {
					var parts = line.Split().Where(x => x.Length > 0).ToArray();
					var ss = parts.Select(int.Parse).ToArray();
					return ss;
				}).ToArray())
				.ToArray();


			foreach (var n in numbers)
			{
				//foreach (var b in boards)
				for (var bi = 0; bi < boards.Length; bi++)
				{
					var b = boards[bi];
					if (b == null)
						continue;
					for (var x = 0; x < 5; x++)
					{
						for (var y = 0; y < 5; y++)
						{
							if (b[x][y] == n)
							{
								b[x][y] = -1;

					//			Console.WriteLine($"Board {bi}: mark {n}");

								var coln = 0;
								for (var xx = 0; xx < 5; xx++)
								{
									if (b[xx][y] == -1)
										coln++;
								}

								if (coln == 5 || b[x].All(v => v == -1))
								{
	//								Console.WriteLine($"Board {bi}: DONE #########");
									if (boards.Count(x => x != null) == 1)
									{
										// found last board!
										var unmarked = b.Sum(line => line.Where(x => x != -1).Sum());
										var score = n * unmarked;
										return score;
									}
									boards[bi] = null;

									x = 10;
									y = 10;

									//bi = boards.Length;
								}
							}
						}
					}

				}
			}




			return 0;
		}
	}
}
