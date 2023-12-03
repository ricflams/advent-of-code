using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day22
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Mode Maze";
		public override int Year => 2018;
		public override int Day => 22;

		public override void Run()
		{
			Run("test1").Part1(114).Part2(45);
			Run("input").Part1(8090).Part2(992);
		}

		protected override long Part1(string[] input)
		{
			var map = new ErosionMap(input);

			var risk = 0;
			for (var x = 0; x <= map.Target.X; x++)
			{
				for (var y = 0; y <= map.Target.Y; y++)
				{
					risk += map[Point.From(x, y)] % 3;
				}
			}
			return risk;
		}

		protected override long Part2(string[] input)
		{
			var map = new ErosionMap(input);
			return map.ShortestPath();
		}

		private class ErosionMap
		{
			private IDictionary<int, int> _map = new Dictionary<int, int>();
			private int _depth;
			private int Key(Point p) => p.X*10000 + p.Y;

			public ErosionMap(string[] input)
			{
				_depth = input[0].RxMatch("depth: %d").Get<int>();
				var (w, h) = input[1].RxMatch("target: %d,%d").Get<int, int>();
				Target = Point.From(w, h);
			}

			public Point Target { get; private set; }

			public int this[Point p]
			{
				get
				{
					var key = Key(p);
					if (!_map.TryGetValue(key, out var val))
						val = _map[key] = Erosion();
					return val;

					int Erosion()
					{
						if (p.Y == 0)
							return (16807 * p.X + _depth) % 20183;
						if (p.X == 0)
							return (48271 * p.Y + _depth) % 20183;
						if (p == Target)
							return _depth;
						return (this[p.Left] * this[p.Up] + _depth) % 20183;
					}					
				}
			}

			enum Use
			{
				Neither = 0,
				Torch = 1,
				Gear = 2
			}

			enum Terrain
			{
				Rocky = 0,
				Wet = 1,
				Narrow = 2
			}

			public int ShortestPath()
			{
				// Fast dictonary-key
				int Key(Point p) => p.X * 10000 + p.Y;

				var queue = new PriorityQueue<(Point,Use,int), int>();
				var times = Enumerable.Range(0, 3).Select(_ => new Dictionary<int, int>()).ToArray();

				queue.Enqueue((Point.Origin, Use.Torch, 0), 0);

				while (queue.TryDequeue(out var item, out var _))
				{
					var (pos, use, time) = item;

					// Skip if we've visited this place w/ equip before in at least as short time
					var key = Key(pos);
					if (times[(int)use].TryGetValue(key, out var fastest))
					{
						if (fastest <= time)
							continue;
						
					}
					times[(int)use][key] = time;

					// If target is found then ensure torch is being used
					if (pos == Target)
					{
						if (use == Use.Torch)
						{
							return time;
						}
						queue.Enqueue((pos, Use.Torch, time + 7), time + 7);
						continue;
					}

					// Explore
					foreach (var p in pos.LookAround().Where(p => p.X >= 0 && p.Y >= 0))
					{
						// Optimization: skip those that have already been visited quicker
						if (times[(int)use].TryGetValue(Key(p), out fastest))
						{
							if (fastest <= time + 1) // +1 because it takes at least one step to go there
								continue;
						}

						var terrain = (Terrain)(this[p] % 3);
						var alternative = (terrain, use) switch
						{
							(Terrain.Rocky, Use.Gear) => Use.Torch,
							(Terrain.Rocky, Use.Torch) => Use.Gear,
							(Terrain.Wet, Use.Gear) => Use.Neither,
							(Terrain.Wet, Use.Neither) => Use.Gear,
							(Terrain.Narrow, Use.Torch) => Use.Neither,
							(Terrain.Narrow, Use.Neither) => Use.Torch,
							_ => (Use?)null
						};
						if (alternative is Use alt)
						{
							// For A* we use the dist as heuristics, as the real cost will never be lower than that
							var dist = p.ManhattanDistanceTo(Target);
							queue.Enqueue((p, use, time + 1), time + 1 + dist); // Move takes 1 steps
							queue.Enqueue((p, alt, time + 8), time + 8 + dist); // Switch+move takes 7 + 1 steps
						}

					}
				}
				throw new Exception("No path found");
			}
		}
	}
}
