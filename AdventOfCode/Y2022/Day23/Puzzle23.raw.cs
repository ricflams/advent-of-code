using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day23.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 23";
		public override int Year => 2022;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(110).Part2(20);
			Run("input").Part1(4138).Part2(1010);
		}

		protected override long Part1(string[] input)
		{
			var map = CharMap.FromArray(input, '.');

			for (var j = 0; j < 10; j++)
			{
				var propose = new Dictionary<Point, Point>();
				var moves = new Dictionary<Point, int>();
				foreach (var elf in map.AllPoints(ch => ch == '#'))
				{
					Point move = null;
					if (elf.LookDiagonallyAround().Count(x => map[x] == '#') == 0)
						continue;
					switch (j % 4)
					{
						case 0:
							move =
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								null;
						break;
						case 1:
							move =
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								null;
						break;
						case 2:
							move =
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								null;
						break;
						case 3:
							move =
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								null;
						break;
					}
					if (move != null)
					{
						propose[elf] = move;
						if (!moves.ContainsKey(move))
							moves[move] = 0;
						moves[move]++;
					}
				}
				foreach (var x in propose.Where(p => moves[p.Value] == 1))
				{
					map[x.Key] = '.';
					map[x.Value] = '#';
				}


			}

				map.ConsoleWrite();
				Console.WriteLine();

			var (minx, maxx, miny, maxy) = (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);
			var elfs = 0;
			foreach (var p in map.AllPoints(ch => ch == '#'))
			{
				minx = Math.Min(minx, p.X);
				maxx = Math.Max(maxx, p.X);
				miny = Math.Min(miny, p.Y);
				maxy = Math.Max(maxy, p.Y);
				elfs++;
			}
			var area = (maxx - minx + 1) * (maxy - miny + 1);

			var empty = area - elfs;

			return empty;
		}



		protected override long Part2(string[] input)
		{

			var map = CharMap.FromArray(input, '.');

			for (var j = 0;; j++)
			{
				var propose = new Dictionary<Point, Point>();
				var moves = new Dictionary<Point, int>();
				var hasmoved = false;
				foreach (var elf in map.AllPoints(ch => ch == '#'))
				{
					Point move = null;
					if (elf.LookDiagonallyAround().Count(x => map[x] == '#') == 0)
						continue;
					switch (j % 4)
					{
						case 0:
							move =
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								null;
						break;
						case 1:
							move =
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								null;
						break;
						case 2:
							move =
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								null;
						break;
						case 3:
							move =
								map[elf.E]=='.' && map[elf.NE]=='.' && map[elf.SE]=='.' ? elf.E :
								map[elf.N]=='.' && map[elf.NE]=='.' && map[elf.NW]=='.' ? elf.N :
								map[elf.S]=='.' && map[elf.SE]=='.' && map[elf.SW]=='.' ? elf.S :
								map[elf.W]=='.' && map[elf.NW]=='.' && map[elf.SW]=='.' ? elf.W :
								null;
						break;
					}
					if (move != null)
					{
						propose[elf] = move;
						if (!moves.ContainsKey(move))
							moves[move] = 0;
						moves[move]++;
					}
				}
				foreach (var x in propose.Where(p => moves[p.Value] == 1))
				{
					map[x.Key] = '.';
					map[x.Value] = '#';
					hasmoved = true;
				}

				if (!hasmoved)
				{
					map.ConsoleWrite();
					Console.WriteLine();					
					return j+1;
				}
			}

		}

	}
}
