using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day12
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Passage Pathing";
		public override int Year => 2021;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(10).Part2(36);
			Run("test2").Part1(19).Part2(103);
			Run("test3").Part1(226).Part2(3509);
			Run("input").Part1(3713).Part2(91292);
			Run("extra").Part1(4304).Part2(118242);
		}

		protected override long Part1(string[] input)
		{
			return CountPaths(input, false);
		}

		protected override long Part2(string[] input)
		{
			return CountPaths(input, true);
		}

		private static int CountPaths(string[] input, bool allowOneSmallCaveRevisit)
		{
			var terrain = new Terrain(input);
			var start = terrain.Nodes.First(n => n.Data.Name == "start");
			var end = terrain.Nodes.First(n => n.Data.Name == "end");

			// Keep track of the paths seen by performing a continuous hashing
			// of all the visited nodes; the hash at each individual path taken
			// will be unique for sure
			var seen = new HashSet<ulong>();
			var count = 0;
			Visit(start, 0, allowOneSmallCaveRevisit, 3074457345618258791UL);
			return count;

			void Visit(Terrain.Node cave, uint visited, bool allowRevisit, ulong path)
			{
				if (seen.Contains(path))
					return;
				seen.Add(path);

				// Count the number of full paths found
				if (cave == end)
				{
					count++;
					return;
				}

				foreach (var (c, _) in cave.Neighbors)
				{
					if (c == start)
						continue;

					var path2 = path * 3074457345618258799UL + (ulong)c.Index;
					if (c.Data.IsSmall)
					{
						// Maybe allow one small cave to be revisited
						var revisit = (visited & c.Data.Bit) != 0;
						if (revisit && !allowRevisit)
							continue;
						Visit(c, visited | c.Data.Bit, allowRevisit && !revisit, path2);
					}
					else
					{
						Visit(c, visited, allowRevisit, path2);
					}
				}
			}
		}


		private class Terrain : Graph<string, Terrain.Cave>
		{
			internal record Cave
			{
				public Cave(string name) => (Name, IsSmall) = (name, char.IsLower(name.First()));
				public string Name;
				public bool IsSmall;
				public uint Bit;
				public override string ToString() => $"{Name}: IsSmall={IsSmall} Bit={Bit}";
			}

			public Terrain(string[] input)
			{
				foreach (var line in input)
				{
					var (from, to) = line.RxMatch("%s-%s").Get<string, string>();
					AddNodes(from, new Cave(from), to, new Cave(to), 1);
				}

				// Assign a bit to each node for more efficient sets 
				var bit = 1u;
				foreach (var n in Nodes)
				{
					n.Data.Bit = bit;
					bit <<= 1;
				}
			}
		}
	}
}
