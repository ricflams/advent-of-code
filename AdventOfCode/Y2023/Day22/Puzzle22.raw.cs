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

namespace AdventOfCode.Y2023.Day22.Raw
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
			//		Run("input").Part1(477).Part2(61555);
			Run("input").Part2(61555);
			//	Run("extra").Part1(0).Part2(0);
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

		protected override long Part1(string[] input)
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
}
