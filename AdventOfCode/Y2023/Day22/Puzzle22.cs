using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Net;

namespace AdventOfCode.Y2023.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2023;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(5).Part2(7);
			//	Run("test2").Part1(0).Part2(0);
			//Run("input").Part1(477);//.Part2(61555);
			Run("input").Part2(61555);

			Run("extra").Part1(451).Part2(66530);
			// 214 too low
		}

		[DebuggerDisplay("{ToString()}")]
		internal record Brick
		{
			private static readonly Point3D Down = new(0, 0, -1);
			//private static readonly Point3D Extend = new(1, 1, 1);

			public Brick(int id, Point3D p1, Point3D p2)
			{
				Id = id;
				(P1, P2) = (p1, p2);
				X = new Interval(p1.X, p2.X+1);
				Y = new Interval(p1.Y, p2.Y+1);
				Z = new Interval(p1.Z, p2.Z+1);
				Debug.Assert(P2.X >= P1.X);
				Debug.Assert(P2.Y >= P1.Y);
				Debug.Assert(P2.Z >= P1.Z);
			}
			public int Id;
			public readonly Point3D P1;
			public readonly Point3D P2;
			public readonly Interval X;
			public readonly Interval Y;
			public readonly Interval Z;

			public bool Overlaps(Brick o) => X.Overlaps(o.X) && Y.Overlaps(o.Y) && Z.Overlaps(o.Z);

			public bool IsAboveGround => Z.Start > 1;

			public Brick MovedDown => new(Id, P1 + Down, P2 + Down);

			public override string ToString() => $"{Id} {P1}~{P2}";
		}

		internal class SandStack
		{
			private readonly Block[] _blocks;
			private readonly List<Block>[] _layers;
			protected static uint HashKey(int x, int y) => ((uint)x << 16) + (uint)y;

			private class Block
			{
				public Block(string def)
				{
					var parts = def.Split('~').ToArray();
					var (p1, p2) = (Point3D.Parse(parts[0]), Point3D.Parse(parts[1]));
					Debug.Assert(p2.X >= p1.X);
					Debug.Assert(p2.Y >= p1.Y);
					Debug.Assert(p2.Z >= p1.Z);

					Area = new HashSet<uint>();
					for (var x = p1.X; x <= p2.X; x++)
					{
						for (var y = p1.Y; y <= p2.Y; y++)
						{
							Area.Add(HashKey(x, y));
						}
					}
					Level = p1.Z - 1; // Easier if levels are 0-aligned
					Height = p2.Z - p1.Z + 1;
				}
				public int Level;
				public int Height;
				public HashSet<uint> Area;

				//public bool AtLevel(int z) => z >= Level && z < Level+Height;

				public bool IsAboveGround => Level > 0;
			}

			public SandStack(string[] input)
			{
				//var bricks = input
				//	.Select((s, idx) =>
				//	{
				//		var bricks = s.Split('~').ToArray();
				//		return new Brick(idx + 1, Point3D.Parse(bricks[0]), Point3D.Parse(bricks[1]));
				//	})
				//	.ToArray();

				_blocks = input.Select(s => new Block(s)).ToArray();

				var zMax = _blocks.Max(b => b.Level + b.Height);
				_layers = Enumerable.Range(0, zMax).Select(_ => new List<Block>()).ToArray();

				foreach (var b in _blocks)
				{
					_layers[b.Level].Add(b);
				}
			}

			public void DropAll()
			{
				foreach (var b in _blocks)
				{
					while (b.IsAboveGround)
					{
						if (_layers[b.Level - 1].Any(lb => lb.Area.Intersect(b.Area).Any()))
							break;
						_layers[b.Level+b.Height-1].Remove(b);
						_layers[b.Level-1].Add(b);
						b.Level--;
					}
				}
			}

			public int CountUnsupportingBlocks()
			{
				return _blocks
					.Count(b => 
					{
						//if (b.Level + b.Height == _layers.Length)
						//	return false; // nothing above
						var needSupport = _layers[b.Level + b.Height];
						if (!needSupport.Any()) // nothing above
							return false;

						var removedSupport = _layers[b.Level + b.Height - 1].Where(x => x != b).Select(b => b.Area).UnionAll();
						return !needSupport.Any(nb => !nb.Area.Intersect(removedSupport).Any());
					});
			}
		}

		protected override long Part1(string[] input)
		{
			var stack = new SandStack(input);

			stack.DropAll();

			//return stack.CountUnsupportingBlocks();

			var bricks = input
				.Select((s, idx) =>
				{
					var bricks = s.Split('~').ToArray();
					return new Brick(idx + 1, Point3D.Parse(bricks[0]), Point3D.Parse(bricks[1]));
				})
				.ToArray();

			bricks = bricks.OrderBy(b => b.P2.Z).ToArray();
			Console.WriteLine($"Sizes: " + string.Join(' ', bricks.Select(b => b.X.Length * b.Y.Length * b.Z.Length).OrderDescending()));
			while (true)
			{
				Console.Write('.');
				var anyFalling = false;
				for (var i = 0; i < bricks.Length; i++)
				{
					var brick = bricks[i];
					while (true)
					{
						if (!brick.IsAboveGround)
							break;
						var down = brick.MovedDown;
						var overlaps = bricks.Where(b => b != brick).Where(b => b.Overlaps(down)).ToArray();
						if (overlaps.Any())
							break;
						brick = bricks[i] = down;
						anyFalling = true;
					}
				}
				if (!anyFalling)
					break;
			}

			var canDisintegrate = 0;
			foreach (var killbrick in bricks)
			{
				var gone = bricks.Where(b => b != killbrick).ToArray();
				var moved = false;
				foreach (var brick in gone)
				{
					if (!brick.IsAboveGround)
						continue;
					var down = brick.MovedDown;
					var overlaps = gone.Where(b => b != brick).Where(b => b.Overlaps(down)).ToArray();
					if (!overlaps.Any())
					{
						moved = true;
						break;
					}
				}
				if (!moved)
					canDisintegrate++;
			}

			return canDisintegrate;
		}



		protected override long Part2(string[] input)
		{
			var bricks = input
				.Select((s, idx) =>
				{
					var bricks = s.Split('~').ToArray();
					return new Brick(idx + 1, Point3D.Parse(bricks[0]), Point3D.Parse(bricks[1]));
				})
				.ToArray();

			;
			while (true)
			{
				var anyFalling = false;
				//bricks = bricks.OrderBy(b => b.P2.Z).ToArray();
				for (var i = 0; i < bricks.Length; i++)
				{
					var brick = bricks[i];
					while (true)
					{
						if (!brick.IsAboveGround)
							break;
						var down = brick.MovedDown;
						var overlaps = bricks.Where(b => b != brick).Where(b => b.Overlaps(down)).ToArray();
						if (overlaps.Any())
							break;
						brick = bricks[i] = down;
						anyFalling = true;
					}
				}
				if (!anyFalling)
					break;
			}

			var disintegrateSum = 0;
			foreach (var killbrick in bricks)
			{
				var gone = bricks.Where(b => b != killbrick).ToArray();

				var disintegrate = new HashSet<int>();

				while (true)
				{
					var anyFalling = false;
					//bricks = bricks.OrderBy(b => b.P2.Z).ToArray();
					for (var i = 0; i < gone.Length; i++)
					{
						var brick = gone[i];
						while (true)
						{
							if (!brick.IsAboveGround)
								break;
							var down = brick.MovedDown;
							var overlaps = gone.Where(b => b != brick).Where(b => b.Overlaps(down)).ToArray();
							if (overlaps.Any())
								break;
							brick = gone[i] = down;
							anyFalling = true;
							disintegrate.Add(i);
						}
					}
					if (!anyFalling)
						break;
				}

				disintegrateSum += disintegrate.Count;

			}

			return disintegrateSum;
		}
	}

	static class HashSetExtensions
	{
		public static HashSet<T> UnionAll<T>(this IEnumerable<HashSet<T>> sets)
		{
			if (!sets.Any())
				return [];
			var union = new HashSet<T>(sets.First());
			foreach (var set in sets.Skip(1))
				union.UnionWith(set);
			return union;
		}
	}
}
