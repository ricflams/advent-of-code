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
			RunFor("test1", 20899048083289, 273);
			RunFor("input", 18262194216271, 2023);
		}

		internal class Tile
		{
			public int Id { get; private set; }
			public int Dim { get; private set; }
			public string[] Map { get; private set; }
			public Variant[] Variants { get; private set; }
			public HashSet<uint> AllPossibleSides { get; private set; }
			public Tile(string[] line)
			{
				line[0].RegexCapture("Tile %d:").Get(out int id);
				Id = id;
				Map = line[1..];

				uint a = 0, b = 0, c = 0, d = 0;
				Dim = Map[0].Length;
				for (var x = 0; x < Dim; x++)
				{
					a = (a << 1) + (Map[0][Dim - 1 - x] == '#' ? 1U : 0);
					c = (c << 1) + (Map[Dim - 1][Dim - 1 - x] == '#' ? 1U : 0);
				}
				for (var y = 0; y < Dim; y++)
				{
					b = (b << 1) + (Map[y][0] == '#' ? 1U : 0);
					d = (d << 1) + (Map[y][Dim - 1] == '#' ? 1U : 0);
				}
				uint inv(ulong x) => (uint)MathHelper.ReverseBits(x, Dim);
				uint ai = inv(a), bi = inv(b), ci = inv(c), di = inv(d);
				// Only include Rotated and Flipped; that's enough to do everything
				Variants = new[]
				{
					new Variant { Sides = new uint[] { a, b, c, d     }, Rotations = 0 },
					new Variant { Sides = new uint[] { b, ci, d, ai   }, Rotations = 1 },
					new Variant { Sides = new uint[] { ci, di, ai, bi }, Rotations = 2 },
					new Variant { Sides = new uint[] { di, a, bi, c   }, Rotations = 3 },
					new Variant { Sides = new uint[] { c, bi, a, di   }, Rotations = 0, Flip = true },
					new Variant { Sides = new uint[] { d, c, b, a     }, Rotations = 1, Flip = true },
					new Variant { Sides = new uint[] { ai, d, ci, b   }, Rotations = 2, Flip = true },
					new Variant { Sides = new uint[] { bi, ai, di, ci }, Rotations = 3, Flip = true }
				};
				AllPossibleSides = new HashSet<uint>(new []
				{
					a, b, c, d, ai, bi, ci, di
				});
			}

			public char[,] TransformedMap
			{
				get
				{
					var map = Map.ToMultiDim();
					if (Bounds.Rotations > 0)
					{
						map = map.RotateClockwise(Bounds.Rotations * 90);
					}
					if (Bounds.Flip)
					{
						map = map.FlipH();
					}
					return map;
				}
			}

			public Variant Bounds { get; set; }

			[System.Diagnostics.DebuggerDisplay("{ToString()}")]
			public class Variant
			{
				public uint[] Sides { get; set; }
				public int Rotations { get; set; }
				public bool Flip { get; set; }
				public override string ToString()
				{
					return $"[sides:{string.Join(",", Sides.Select(x=>x.ToString()))} rot={Rotations} flip={Flip}]";
				}
				public uint Top => Sides[0];
				public uint Left => Sides[1];
				public uint Bot => Sides[2];
				public uint Right => Sides[3];
			}
		}

		protected override long Part1(string[] input)
		{
			var rawtiles = input.GroupByEmptyLine();
			var alltiles = rawtiles.Select(x => new Tile(x)).ToList();

			var allpossiblesides = alltiles.SelectMany(x => x.AllPossibleSides).GroupBy(x => x);
			var outerIds = new HashSet<uint>(allpossiblesides.Where(g => g.Count() == 1).Select(g => g.Key));
			var corners = alltiles.Where(t => t.Variants.Any(v => v.Sides.Count(x => outerIds.Contains(x)) == 2)).ToArray();

			var result = corners.Aggregate(1L, (sum, t) => sum * t.Id);
			return result;
		}

		protected override long Part2(string[] input)
		{
			var rawtiles = input.GroupByEmptyLine();
			var alltiles = rawtiles.Select(x => new Tile(x)).ToList();

			var allpossiblesides = alltiles.SelectMany(x => x.AllPossibleSides).GroupBy(x => x);
			var outerIds = new HashSet<uint>(allpossiblesides.Where(g => g.Count() == 1).Select(g => g.Key));
			var corners = alltiles.Where(t => t.Variants.Any(v => v.Sides.Count(x => outerIds.Contains(x)) == 2)).ToArray();

			var N = (int)Math.Sqrt(alltiles.Count());
			var tilemap = new Tile[N, N];

			for (var x = 0; x < N; x++)
			{
				if (x == 0)
				{
					var corner0 = corners[0];
					var cornersides = new HashSet<uint>(corner0.AllPossibleSides.Where(x => outerIds.Contains(x)));
					corner0.Bounds = corner0.Variants.First(v => cornersides.Contains(v.Top) && cornersides.Contains(v.Left));
					tilemap[x, 0] = corner0;
					alltiles.Remove(corner0);
				}
				else
				{
					var left = tilemap[x - 1, 0].Bounds.Right;
					var tile = alltiles.First(t => t.AllPossibleSides.Contains(left));
					tile.Bounds = tile.Variants.Where(v => outerIds.Contains(v.Top)).First(v => v.Left == left);
					tilemap[x, 0] = tile;
					alltiles.Remove(tile);
				}
				for (var y = 1; y < N; y++)
				{
					var top = tilemap[x, y - 1].Bounds.Bot;
					var tile = alltiles.First(t => t.AllPossibleSides.Contains(top));

					if (x == 0)
					{
						tile.Bounds = tile.Variants.Where(v => outerIds.Contains(v.Left)).First(v => v.Top == top);
					}
					else
					{
						var left = tilemap[x - 1, y].Bounds.Right;
						tile.Bounds = tile.Variants.First(v => v.Left == left && v.Top == top);
					}

					tilemap[x, y] = tile;
					alltiles.Remove(tile);
				}
			}

			//Console.WriteLine();
			//for (var y = 0; y < N; y++)
			//{
			//	for (var x = 0; x < N; x++)
			//	{
			//		var tile = map[x, y];
			//		Console.Write($"{tile.Id} ({tile.Bounds.Rotations} {tile.Bounds.Flip})    ");
			//	}
			//	Console.WriteLine();
			//}
			//Console.WriteLine();


			var dim = corners.First().Dim - 2;
			var map = new char[N * dim, N * dim];
			for (var x = 0; x < N; x++)
			{
				for (var y = 0; y < N; y++)
				{
					// Console.Write($"{map[x, y].Id}  ");
					var newmap = tilemap[x, y].TransformedMap;
					for (var x1 = 0; x1 < dim; x1++)
					{
						for (var y1 = 0; y1 < dim; y1++)
						{
							map[x * dim + x1, y * dim + y1] = newmap[x1 + 1, y1 + 1];
						}
					}
				}
			}

			//for (var y = 0; y < map2.GetLength(1); y++)
			//{
			//	for (var x = 0; x < map2.GetLength(0); x++)
			//	{
			//		Console.Write(map2[x, y]);
			//	}
			//	Console.WriteLine();
			//}


			var monster = new string[]
			{
				"                  # ",
				"#    ##    ##    ###",
				" #  #  #  #  #  #   ",

			}.ToMultiDim();

			var rough = 0;
			for (var angle = 0; angle < 360; angle += 90)
			{
				var m = monster.RotateClockwise(angle);
				if (FindMonsterRoughSea(map, m, out rough) || FindMonsterRoughSea(map, m.FlipV(), out rough))
				{
					break;
				}
			}


			static bool FindMonsterRoughSea(char[,] map, char[,] monster, out int rough)
			{
				var mapw = map.GetLength(0);
				var maph = map.GetLength(1);
				var monw = monster.GetLength(0);
				var monh = monster.GetLength(1);
				var xscan = mapw - monw;
				var yscan = maph - monh;

				rough = 0;
				var monsterDots = monster.CountChar('#');
				var monstersfound = 0;
				for (var x = 0; x < xscan; x++)
				{
					for (var y = 0; y < yscan; y++)
					{
						var match = 0;
						for (var mx = 0; mx < monw; mx++)
						{
							for (var my = 0; my < monh; my++)
							{
								if (monster[mx, my] == '#' && map[x + mx, y + my] == '#')
								{
									match++;
								}
							}
						}
						if (match == monsterDots)
						{
							monstersfound++;
						}
					}
				}
				if (monstersfound > 0)
				{
					var mapdots = map.CountChar('#');
					rough = mapdots - monstersfound * monsterDots;
					return true;
				}
				return false;
			}

			return rough;
		}
	}

}
