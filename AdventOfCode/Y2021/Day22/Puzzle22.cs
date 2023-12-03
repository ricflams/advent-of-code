using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Reactor Reboot";
		public override int Year => 2021;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(39).Part2(39);
			Run("test2").Part1(590784).Part2(590784);
			Run("test3").Part2(2758514936282235);
			Run("input").Part1(596989).Part2(1160011199157381);
		}

		protected override long Part1(string[] input)
		{
			var space = new bool[101, 101, 101];
			foreach (var s in input)
			{
				// on x=10..12,y=10..12,z=10..12
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var xx1 = Math.Max(x1, -50);
				var xx2 = Math.Min(x2, 50);
				var yy1 = Math.Max(y1, -50);
				var yy2 = Math.Min(y2, 50);
				var zz1 = Math.Max(z1, -50);
				var zz2 = Math.Min(z2, 50);
				var on = set == "on";
				for (var x = xx1; x <= xx2; x++)
				{
					for (var y = yy1; y <= yy2; y++)
					{
						for (var z = zz1; z <= zz2; z++)
						{
							space[x + 50, y + 50, z + 50] = on;
						}
					}
				}
			}

			var n = 0;
			foreach (var b in space)
			{
				if (b) n++;
			}

			return n;
		}

		protected override long Part2(string[] input)
		{
			var cubes = new List<Cube>();
			foreach (var s in input)
			{
				var (set, x1, x2, y1, y2, z1, z2) = s.RxMatch("%s x=%d..%d,y=%d..%d,z=%d..%d").Get<string, int, int, int, int, int, int>();
				var cube = new Cube(set == "on", x1, x2, y1, y2, z1, z2);
				cubes.AddRange(cubes
					.Where(cube.Intersects)
					.Select(cube.Overlap)
					.ToArray());
				if (cube.On)
				{
					cubes.Add(cube);
				}
			}

			var on = cubes.Select(x => x.On ? x.Size : -x.Size).Sum();
			return on;
		}


		internal class Cube
		{
			private readonly Point3D _bot;
			private readonly Point3D _top;

			public Cube(bool on, int x1, int x2, int y1, int y2, int z1, int z2)
			{
				On = on;
				_bot = new Point3D(x1, y1, z1);
				_top = new Point3D(x2, y2, z2);
			}

			public readonly bool On;

			public long Size => (long)(_top.X - _bot.X + 1) * (_top.Y - _bot.Y + 1) * (_top.Z - _bot.Z + 1);

			public override string ToString() => $"[{(On ? "ON" : "of")} x={_bot.X}..{_top.X},y={_bot.Y}..{_top.Y},z={_bot.Z}..{_top.Z}:{Size}]";

			public bool Intersects(Cube c) =>
				_bot.X <= c._top.X && _top.X >= c._bot.X &&
				_bot.Y <= c._top.Y && _top.Y >= c._bot.Y &&
				_bot.Z <= c._top.Z && _top.Z >= c._bot.Z;

			public Cube Overlap(Cube other)
			{
				var x1 = Math.Max(_bot.X, other._bot.X);
				var x2 = Math.Min(_top.X, other._top.X);
				var y1 = Math.Max(_bot.Y, other._bot.Y);
				var y2 = Math.Min(_top.Y, other._top.Y);
				var z1 = Math.Max(_bot.Z, other._bot.Z);
				var z2 = Math.Min(_top.Z, other._top.Z);
				return new Cube(!other.On, x1, x2, y1, y2, z1, z2);
			}
		}
	}
}
