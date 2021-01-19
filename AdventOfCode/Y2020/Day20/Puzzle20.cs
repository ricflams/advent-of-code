using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2020.Day20
{
	internal class Puzzle : ComboParts<long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Jurassic Jigsaw";
		public override int Year => 2020;
		public override int Day => 20;

		public void Run()
		{
			RunFor("test1", 20899048083289, 273);
			RunFor("input", 18262194216271, 2023);
		}

		internal class Tile
		{
			public int Id { get; private set; }
			public int Dim { get; private set; }
			public HashSet<uint> AllPossibleSides { get; private set; }
			public Variant[] Variants { get; private set; }
			public Variant Chosen { get; set; }

			private readonly string[] _map;

			public Tile(string[] line)
			{
				var id = line[0].RxMatch("Tile %d:").Get<int>();
				Id = id;
				_map = line[1..];

				uint a = 0, b = 0, c = 0, d = 0;
				Dim = _map[0].Length;
				for (var x = 0; x < Dim; x++)
				{
					a = (a << 1) + (_map[0][Dim - 1 - x] == '#' ? 1U : 0);
					c = (c << 1) + (_map[Dim - 1][Dim - 1 - x] == '#' ? 1U : 0);
				}
				for (var y = 0; y < Dim; y++)
				{
					b = (b << 1) + (_map[y][0] == '#' ? 1U : 0);
					d = (d << 1) + (_map[y][Dim - 1] == '#' ? 1U : 0);
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
					var map = _map.ToCharMatrix();
					if (Chosen.Rotations > 0)
					{
						map = map.RotateClockwise(Chosen.Rotations * 90);
					}
					if (Chosen.Flip)
					{
						map = map.FlipH();
					}
					return map;
				}
			}

			public class Variant
			{
				public uint[] Sides { get; set; }
				public uint Top => Sides[0];
				public uint Left => Sides[1];
				public uint Bot => Sides[2];
				public uint Right => Sides[3];

				public int Rotations { get; set; }
				public bool Flip { get; set; }
			}
		}

		protected override (long, long) Part1And2(string[] input)
		{
			var rawtiles = input.GroupByEmptyLine();
			var tiles = rawtiles.Select(x => new Tile(x)).ToList();

			var sides = tiles.SelectMany(x => x.AllPossibleSides).GroupBy(x => x);
			var border = new HashSet<uint>(sides.Where(g => g.Count() == 1).Select(g => g.Key));
			var corners = tiles.Where(t => t.Variants.Any(v => v.Sides.Count(x => border.Contains(x)) == 2)).ToArray();

			// Find product of the four corners
			var result1 = corners.Aggregate(1L, (sum, t) => sum * t.Id);

			// Assemple the map
			// (This will remove the tiles from the collection, one by one)
			var N = (int)Math.Sqrt(tiles.Count());
			var tilemap = new Tile[N, N];
			for (var x = 0; x < N; x++)
			{
				if (x == 0)
				{
					var tile = corners[0];
					var tilesides = new HashSet<uint>(tile.AllPossibleSides.Where(side => border.Contains(side)));
					tile.Chosen = tile.Variants.First(v => tilesides.Contains(v.Top) && tilesides.Contains(v.Left));
					tilemap[x, 0] = tile;
					tiles.Remove(tile);
				}
				else
				{
					var left = tilemap[x - 1, 0].Chosen.Right;
					var tile = tiles.First(t => t.AllPossibleSides.Contains(left));
					tile.Chosen = tile.Variants.Where(v => border.Contains(v.Top)).First(v => v.Left == left);
					tilemap[x, 0] = tile;
					tiles.Remove(tile);
				}
				for (var y = 1; y < N; y++)
				{
					var top = tilemap[x, y - 1].Chosen.Bot;
					var tile = tiles.First(t => t.AllPossibleSides.Contains(top));
					if (x == 0)
					{
						tile.Chosen = tile.Variants.Where(v => border.Contains(v.Left)).First(v => v.Top == top);
					}
					else
					{
						var left = tilemap[x - 1, y].Chosen.Right;
						tile.Chosen = tile.Variants.First(v => v.Left == left && v.Top == top);
					}
					tilemap[x, y] = tile;
					tiles.Remove(tile);
				}
			}

			// Build one big map from tiles with borders removed
			var dim = tilemap[0, 0].Dim - 2;
			var bigmap = new char[N * dim, N * dim];
			for (var x = 0; x < N; x++)
			{
				for (var y = 0; y < N; y++)
				{
					var map = tilemap[x, y].TransformedMap;
					for (var mapx = 0; mapx < dim; mapx++)
					{
						for (var mapy = 0; mapy < dim; mapy++)
						{
							bigmap[x * dim + mapx, y * dim + mapy] = map[mapx + 1, mapy + 1];
						}
					}
				}
			}

			// Find monsters by rotating/flipping until some are found
			// Rotate the monster instead of the map, as it's cheaper
			static bool FindWaterRoughness(char[,] map, char[,] monster, ref int roughness)
			{
				var mapw = map.GetLength(0);
				var maph = map.GetLength(1);
				var monw = monster.GetLength(0);
				var monh = monster.GetLength(1);
				var xscan = mapw - monw;
				var yscan = maph - monh;

				var monsterspots = monster.PositionsOf('#');
				var monsters = 0;
				// Scan map for occurrences of the monster (don't care about overlaps)
				for (var x = 0; x < xscan; x++)
				{
					for (var y = 0; y < yscan; y++)
					{
						if (monsterspots.All(p => map[x + p.X, y + p.Y] == '#'))
						{
							monsters++;
						}
					}
				}
				if (monsters == 0)
				{
					return false;
				}
				roughness = map.CountChar('#') - monsters * monsterspots.Count();
				return true;
			}

			var monster = new string[]
			{
				"                  # ",
				"#    ##    ##    ###",
				" #  #  #  #  #  #   "
			}.ToCharMatrix();
			var result2 = 0;
			for (var angle = 0; angle < 360; angle += 90)
			{
				var m = monster.RotateClockwise(angle);
				if (FindWaterRoughness(bigmap, m, ref result2) || FindWaterRoughness(bigmap, m.FlipV(), ref result2))
				{
					break;
				}
			}

			return (result1, result2);
		}
	}
}
