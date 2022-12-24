using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day24
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Blizzard Basin";
		public override int Year => 2022;
		public override int Day => 24;

		public void Run() 
		{
			Run("test1").Part1(18).Part2(54);
			Run("input").Part1(262).Part2(785);
		}

		protected override long Part1(string[] input)
		{
			var map = new Map(input);

			return map.TimeAtShortestPath(0, map.Start, map.End);
		}

		protected override long Part2(string[] input)
		{
			var map = new Map(input);

			var t = map.TimeAtShortestPath(0, map.Start, map.End);
			t = map.TimeAtShortestPath(t, map.End, map.Start);
			t = map.TimeAtShortestPath(t, map.Start, map.End);
			return t;
		}

		internal class Map
		{
			public Point Start;
			public Point End;

			private readonly Wind[] _winds;
			private readonly int _width;
			private readonly int _height;

			public Map(string[] input)
			{
				_winds = new []
				{
					new Wind(input, '^'),
					new Wind(input, 'v'),
					new Wind(input, '<'),
					new Wind(input, '>')
				};
				(_width, _height) = (input[0].Length - 2, input.Length - 2);
				Start = Point.From(input[0].IndexOf('.')-1, -1);
				End = Point.From(input[^1].IndexOf('.')-1, _height);
			}

			private class Wind
			{
				public readonly bool[,] On;
				public readonly Func<Point, int, bool> IsOn;
				public Wind(string[] input, char blizzard)
				{
					var (w, h) = (input[0].Length - 2, input.Length - 2);
					On = new bool[w, h];
					IsOn = blizzard switch
					{
						'^' => (p, t) => On[p.X, (p.Y + t) % h],
						'v' => (p, t) => On[p.X, (p.Y - t%h + h) % h],
						'<' => (p, t) => On[(p.X + t) % w, p.Y],
						'>' => (p, t) => On[(p.X - t%w + w) % w, p.Y],
						_ => throw new Exception()
					};
					for (var y = 0; y < h; y++)
						for (var x = 0; x < w; x++)
							On[x, y] = input[y+1][x+1] == blizzard;
				}
			}

			//public bool IsVacant(Point p, int time) => p==Start || p==End || p.Within(Width, Height) && Winds.All(w => !w.IsOn(p, time));
			public bool IsVacant(Point p, int time) =>
				p==Start ||
				p==End ||
				p.Within(_width, _height) && !_winds[0].IsOn(p, time) && !_winds[1].IsOn(p, time) && !_winds[2].IsOn(p, time) && !_winds[3].IsOn(p, time);

			public int TimeAtShortestPath(int time0, Point start, Point end)
			{
				var cycle = (int)MathHelper.LeastCommonMultiple(_width, _height);

				var seen = new Dictionary<int, int>();
				var queue = new PriorityQueue<(Point,int), int>();
				queue.Enqueue((start, time0), 0);
				while (queue.TryDequeue(out var item, out var _))
				{
					var (pos, time) = item;

					var id = (pos.X << 20) + (pos.Y << 10) + (time % cycle);
					if (seen.TryGetValue(id, out var last) && time >= last)
						continue;
					seen[id] = time;

					if (pos == end)
						return time;

					if (IsVacant(pos, time+1))
					{
						queue.Enqueue((pos, time+1), time+1 + pos.ManhattanDistanceTo(end));
					}
					foreach (var p in pos.LookAround().Where(p => IsVacant(p, time+1)))
					{
						queue.Enqueue((p, time+1), time+1 + p.ManhattanDistanceTo(end));
					}
				}
				throw new Exception("No path found");
			}
		}
	}
}
