using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2017.Day14
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Disk Defragmentation";
		public override int Year => 2017;
		public override int Day => 14;

		public void Run()
		{
			RunFor("test1", 8108, 1242);
			RunFor("input", 8226, 1128);
		}

		protected override int Part1(string[] input)
		{
			var key = input[0];

			// Hash all messages and simply count the bits
			var squares = 0;
			for (var i = 0; i < 128; i++)
			{
				var message = $"{key}-{i}";
				var hash = Common.KnotHash.Hash(message);
				squares += hash.Sum(b => b.NumberOfSetBits());
			}

			return squares;
		}

		protected override int Part2(string[] input)
		{
			var key = input[0];

			// Geenrate al hashes and transform them into a sparsemap, which is
			// really easy (and fast) to walk around
			var hashes = Enumerable.Range(0, 128)
				.Select(i => Common.KnotHash.Hash($"{key}-{i}"))
				.ToArray();
			var map = new SparseMap<bool>();
			for (var y = 0; y < hashes.Length; y++)
			{
				var hash = hashes[y];
				for (var x = 0; x < hash.Length; x++)
				{
					for (var b = 0; b < 8; b++)
					{
						if ((hash[x] & 1U<<(7-b)) != 0)
						{
							map[x*8+b][y] = true;
						}
					}
				}
			}

			// Find the next set bit, clear the entire region, and count it
			// until all set bits has been cleared
			var n = 0;
			while (true)
			{
				var set = map.FirstOrDefault(b => b);
				if (set == null)
					break;
				ClearRegion(set);
				n++;
			}
			return n;

			void ClearRegion(Point pos)
			{
				map[pos] = false;
				foreach (var p in pos.LookAround().Where(p => map[p]))
				{
					ClearRegion(p);
				}
			}
		}
	}
}
