using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day23
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Unstable Diffusion";
		public override int Year => 2022;
		public override int Day => 23;

		public override void Run()
		{
			Run("test1").Part1(110).Part2(20);
			Run("input").Part1(4138).Part2(1010);
			Run("extra").Part1(4052).Part2(978);
		}

		protected override long Part1(string[] input)
		{
			return EmptySpaces(input, 10);
		}

		protected override long Part2(string[] input)
		{
			return EmptySpaces(input);
		}

		private static int EmptySpaces(string[] input, int rounds = int.MaxValue)
		{
			// Fast lookup of coordinates in one int-hashtable; works for x,y of +/-5000
			int Encode(Point p) => (p.X+5000)*10000 + (p.Y+5000);
			(int X, int Y) Decode(int v) => (v/10000-5000, (v%10000)-5000);
			var p0 = Point.Origin;
			var v0 = Encode(p0);
			var neighbourhood = Point.Origin.LookDiagonallyAround().Select(p => Encode(p) - v0).ToArray();

			var map = CharMap.FromArray(input);
			var elfs = new HashSet<int>(map.AllPointsWhere(ch => ch == '#').Select(Encode));

			// Directions to look in are just numbers that can be added to coordinate
			var directions = new[]
			{
				new[] { Encode(p0.N)-v0, Encode(p0.NE)-v0, Encode(p0.NW)-v0 },
				new[] { Encode(p0.S)-v0, Encode(p0.SE)-v0, Encode(p0.SW)-v0 },
				new[] { Encode(p0.W)-v0, Encode(p0.NW)-v0, Encode(p0.SW)-v0 },
				new[] { Encode(p0.E)-v0, Encode(p0.NE)-v0, Encode(p0.SE)-v0 }
			};

			// Faster than "!neighbourhood.Any(n => elfs.Contains(e + n)))
			bool HasNoNeighbors(int elf)
			{
				foreach (var n in neighbourhood)
					if (elfs.Contains(elf + n))
						return false;
				return true;
			}

			var proposals = new Dictionary<int, int>();
			var collisions = new HashSet<int>();
			for (var j = 0; j < rounds; j++)
			{
				proposals.Clear();
				collisions.Clear();
				foreach (var e in elfs)
				{
					if (HasNoNeighbors(e))
						continue;
					for (var i = 0; i < 4; i++)
					{
						var move = directions[(i + j%4) % 4];
						if (!elfs.Contains(e + move[0]) && !elfs.Contains(e + move[1]) && !elfs.Contains(e + move[2]))
						{
							var moveto = e + move[0];
							if (proposals.ContainsKey(moveto))
								collisions.Add(moveto);
							else
								proposals[moveto] = e;
							break;
						}
					}
				}

				if (!proposals.Any()) // For part 2
					return j+1;
					
				var moves = proposals.Where(p => !collisions.Contains(p.Key));
				foreach (var x in moves)
				{
					elfs.Remove(x.Value);
					elfs.Add(x.Key);
				}				
			}

			var (minx, maxx, miny, maxy) = (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);
			foreach (var e in elfs)
			{
				var (x, y) = Decode(e);
				minx = Math.Min(minx, x);
				maxx = Math.Max(maxx, x);
				miny = Math.Min(miny, y);
				maxy = Math.Max(maxy, y);
			}
			var area = (maxx - minx + 1) * (maxy - miny + 1);
			var empty = area - elfs.Count;

			return empty; // For part 1
		}
	}
}