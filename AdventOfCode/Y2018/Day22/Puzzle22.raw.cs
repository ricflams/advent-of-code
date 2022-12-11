using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2018.Day22.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Mode Maze";
		public override int Year => 2018;
		public override int Day => 22;

		public void Run()
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

		enum Use {
			Neither,
			Torch,
			Gear
		}

		protected override long Part2(string[] input)
		{
			// var (ero, target) = ReadErosionMap(input, 3);

			var map = new ErosionMap(input);
			var queue = new PriorityQueue<(Point,Use), int>();

			var shortesttime = 0;

			var shortest = new Dictionary<string, int>();

			queue.Enqueue((Point.Origin, Use.Torch), 0);
//			shortest[Point.Origin] = 0;

			string Key(Point p, Use use) => $"{p}-{use}";

			while (queue.TryDequeue(out var item, out var time))
			{
				var (pos, use) = item;
			//	Console.WriteLine($"{pos} at {time}");

				// if (seen.Contains(state))
				// {
				// 	continue;
				// }
				// seen.Add(state);

				// if (state == FinalState)
				// {
				// 	return energy;
				// }
				// State = state;

				if (shortest.TryGetValue(Key(pos, Use.Neither), out var t2) && t2 < time - 7)
					continue;
				if (shortest.TryGetValue(Key(pos, Use.Torch), out var t3) && t3 < time - 7)
					continue;
				if (shortest.TryGetValue(Key(pos, Use.Gear), out var t4) && t4 < time - 7)
					continue;


				if (shortest.TryGetValue(Key(pos,use), out var t))
				{
					if (t <= time)
						continue;
					
				}
				shortest[Key(pos,use)] = time;
				
				


				if (pos == map.Target)
				{
					if (use == Use.Torch)
					{
						shortesttime = time;
						Console.WriteLine(time);
						break;
					}
					queue.Enqueue((pos, Use.Torch), time + 7);
					continue;
				}

				foreach (var p in pos.LookAround().Where(p => p.X >= 0 && p.Y >= 0))
				{
					// if (shortest.TryGetValue(Key(p,use), out var t2))
					// {
					// 	if (t2 <= time)
					// 		continue;
					// }

					var switchcost = 7+1;
					var reg = map[p] % 3;
					switch (reg)
					{
						case 0: // rocky
							switch (use)
							{
								case Use.Gear:
									queue.Enqueue((p, use), time + 1);
									queue.Enqueue((p, Use.Torch), time + switchcost);
									break;
								case Use.Torch:
									queue.Enqueue((p, use), time + 1);
									queue.Enqueue((p, Use.Gear), time  + switchcost);
									break;
								//default:
									//throw new Exception("Invalid state");
							}
							break;
						case 1: // wet
							switch (use)
							{
								case Use.Gear:
									queue.Enqueue((p, use), time + 1);
									queue.Enqueue((p, Use.Neither), time  + switchcost);
									break;
								case Use.Neither:
									queue.Enqueue((p, use), time + 1);
									queue.Enqueue((p, Use.Gear), time + switchcost);
									break;
								//default:
									//throw new Exception("Invalid state");
							}
							break;
						case 2: // narrow
							switch (use)
							{
								case Use.Torch:
									queue.Enqueue((p, use), time + 1);
									queue.Enqueue((p, Use.Neither), time + switchcost);
									break;
								case Use.Neither:
									queue.Enqueue((p, use), time + 1);
									queue.Enqueue((p, Use.Torch), time + switchcost);
									break;
								//default:
									//throw new Exception("Invalid state");
							}
							break;
						default:
							throw new Exception();
					}
				}

			}

			// 986 too low (right for someone else)

			return shortesttime;
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
						val = _map[key] = Erosion(p);
					return val;
				}
			}

			private int Erosion(Point p)
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
}
