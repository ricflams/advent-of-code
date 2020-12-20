using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2020.Day20
{
	internal class Puzzle : SoloParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		protected override int Year => 2020;
		protected override int Day => 20;

		public void Run()
		{
			RunFor("test1", 20899048083289, 0);
			//RunFor("test2", null, null);
			RunFor("input", 0, 0);
		}

		internal class Tile
		{
			public int Id { get; private set; }
			public uint[][] Variants { get; private set; }
			public Tile(string[] line)
			{
				line[0].RegexCapture("Tile %d:").Get(out int id);
				Id = id;
				var map = line[1..];

				uint a = 0, b = 0, c = 0, d = 0;
				int w = map[0].Length;
				int h = map.Length;
				for (var x = 0; x < w; x++)
				{
					a = (a << 1) + (map[0][x] == '#' ? 1U : 0);
					c = (c << 1) + (map[h - 1][x] == '#' ? 1U : 0);
				}
				for (var y = 0; y < h; y++)
				{
					b = (b << 1) + (map[y][0] == '#' ? 1U : 0);
					d = (d << 1) + (map[y][w-1] == '#' ? 1U : 0);
				}
				uint inv(uint x) => MathHelper.ReverseBits(x, w);
				uint ai = inv(a), bi = inv(b), ci = inv(c), di = inv(d);
				Variants = new[]
				{
					new uint[] { ai, b, ci, d },
					new uint[] { d, a, b, c },
					new uint[] { ci, d, ai, b },
					new uint[] { b, ci, d, ai },
					new uint[] { a, b, c, d },
					new uint[] { di, a, bi, c },
					new uint[] { c, d, a, b },
					new uint[] { bi, ci, di, ai }
				};
			}
		}

		protected override long Part1(string[] input)
		{
			var rawtiles = input.GroupByEmptyLine();

			var tiles = rawtiles.Select(x => new Tile(x)).ToArray();




			return 0;
		}

		protected override long Part2(string[] input)
		{





			return 0;
		}
	}




	//  internal class Puzzle : ComboPart<int>
	//  {
	//  	public static Puzzle Instance = new Puzzle();
	//  	protected override int Year => 2020;
	//  	protected override int Day => 20;
	//  
	//  	public void Run()
	//  	{
	//  		RunFor("test1", null, null);
	//  		//RunFor("test2", null, null);
	//  		//RunFor("input", null, null);
	//  	}
	//  
	//  	protected override (int, int) Part1And2(string[] input)
	//  	{
	//  
	//  
	//  
	//  
	//  
	//  		return (0, 0);
	//  	}
	//  }

}
