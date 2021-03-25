using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day03
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "No Matter How You Slice It";
		public override int Year => 2018;
		public override int Day => 3;

		public void Run()
		{
			Run("test1").Part1(4).Part2(3);
			Run("input").Part1(116489).Part2(1260);
		}

		protected override int Part1(string[] input)
		{
			var claims = ReadClaims(input);

			// Fastest way seem to be to simple hash memo-tables that register
			// every square and therefore can count the first time a square is
			// seen. It's faster than counting up all the overlappings and then
			// searching for those with >1 overlap.
			var once = new SimpleMemo<int>();
			var twice = new SimpleMemo<int>();
			var overlaps = 0;

			foreach (var pos in claims.SelectMany(c => c.Squares()))
			{
				if (once.IsSeenBefore(pos) && !twice.IsSeenBefore(pos))
				{
					overlaps++;
				}
			}

			return overlaps;
		}

		protected override int Part2(string[] input)
		{
			var claims = ReadClaims(input);

			// Make a map of the number of claims overlapping anywhere
			var map = new SafeDictionary<int, int>();
			foreach (var pos in claims.SelectMany(c => c.Squares()))
			{
				map[pos]++;
			}

			// The sought claim is the one that's not overlapped by any other,
			// ie where the map only has registered 1 claim for the area taken
			// up by this claim.
			var id = claims.First(c => c.Squares().All(x => map[x] == 1)).Id;
			return id;
		}

		private static Claim[] ReadClaims(string[] input)
		{
			return input.Select(line => new Claim(line)).ToArray();
		}

		internal class Claim
		{
			private int _x, _y, _w, _h;
		
			public Claim(string s)
			{
				// #1 @ 1,3: 4x4
				(Id, _x, _y, _w, _h) = s.RxMatch("#%d @ %d,%d: %dx%d").Get<int, int, int, int, int>();
			}

			public int Id { get; }

			public IEnumerable<int> Squares()
			{
				for (var x = _x; x < _x + _w; x++)
				{
					for (var y = _y; y < _y + _h; y++)
					{
						yield return x*10000 + y;
					}
				}
			}
		}
	}
}
