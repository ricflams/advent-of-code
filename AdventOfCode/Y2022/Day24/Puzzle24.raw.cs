using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day24.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 24";
		public override int Year => 2022;
		public override int Day => 24;

		public void Run() 
		{
			//Run("test1").Part1(0).Part2(0);
			Run("test2").Part1(18).Part2(54);
			Run("input").Part1(262).Part2(785);
			// 277 too high
			// 197 too low
		}

		internal class Wind
		{
			public readonly bool[,] On;
			public readonly Func<Point, int, bool> IsOn;
			public readonly char Blizzard;
			public Wind(string[] input, char blizzard)
			{
				Blizzard = blizzard;
				var w = input[0].Length - 2;
				var h = input.Length - 2;
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
				{
					for (var x = 0; x < w; x++)
					{
						On[x, y] = input[y+1][x+1] == blizzard;
					}
				}
			}
		}

		internal class Map
		{
			public readonly Wind[] Winds;
			public readonly int Width;
			public readonly int Height;
			public int Time;
			public Point Start;
			public Point End;

			public Map(string[] input)
			{
				Winds = new []
				{
					new Wind(input, '^'),
					new Wind(input, 'v'),
					new Wind(input, '<'),
					new Wind(input, '>')
				};
				(Width, Height) = (input[0].Length - 2, input.Length - 2);
				Time = 0;
				Start = Point.Origin;
				End = Point.From(Width-1, Height-1);
			}

			public bool IsVacant(Point p, int time) => Winds.All(w => !w.IsOn(p, time));

			public void Draw(int time)
			{
				Console.WriteLine($"Time={time}");
				for (var y = 0; y < Height; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						var p = Point.From(x, y);
						var blizzards = Winds.Where(w => w.IsOn(p, time)).Select(w => w.Blizzard).ToArray();
						var ch =
							blizzards.Length == 0 ? '.' :
							blizzards.Length == 1 ? blizzards.First() :
							(char)('0' + blizzards.Length);
						Console.Write(ch);
					}
					Console.WriteLine();
				}
				Console.WriteLine();
			}
		}

		protected override long Part1(string[] input)
		{
			var map = new Map(input);
			var (w, h) = (map.Width, map.Height);

			//var queue = new PriorityQueue<(Point,int), int>();
			var queue = new Queue<(Point,int)>();
			var cycle = MathHelper.LeastCommonMultiple(map.Width, map.Height);
			var seen = new Dictionary<string, int>();

			// var time0 = 1;
			// while (!map.IsVacant(map.Start, time0))
			// 	time0++;

			//queue.Enqueue((map.Start, time0), 0);
			queue.Enqueue((map.Start.Up, 0));
			//while (queue.TryDequeue(out var item, out var _))
			while (queue.TryDequeue(out var item))
			{
				var (pos, time) = item;

				// map.Draw(time);
				// map.Draw(time+1);

				if (pos == map.End)
				{
					return time+1;
				}

				var id = $"{pos}{time % cycle}";
				if (seen.TryGetValue(id, out var last))
				{
					if (time >= last)
						continue;
				}
				seen[id] = time;

				if (pos == map.Start.Up || map.IsVacant(pos, time + 1))
				{
					queue.Enqueue((pos, time + 1));//, pos.ManhattanDistanceTo(map.End));
				}
				foreach (var p in pos.LookAround().Where(p => p.Within(w, h) && map.IsVacant(p, time + 1)))
				{
					queue.Enqueue((p, time + 1));//, p.ManhattanDistanceTo(map.End));
				}
			}
			throw new Exception("No path found");
		}

		protected override long Part2(string[] input)
		{

			var map = new Map(input);
			var (w, h) = (map.Width, map.Height);

			//var queue = new PriorityQueue<(Point,int), int>();
			var queue = new Queue<(Point, int, int)>();
			var cycle = MathHelper.LeastCommonMultiple(map.Width, map.Height);
			var seen = new Dictionary<string, int>();

			// var time0 = 1;
			// while (!map.IsVacant(map.Start, time0))
			// 	time0++;

			//queue.Enqueue((map.Start, time0), 0);

			queue.Enqueue((map.Start.Up, 0, 0));
			//while (queue.TryDequeue(out var item, out var _))
			while (queue.TryDequeue(out var item))
			{
				var (pos, trip, time) = item;

				// map.Draw(time);
				// map.Draw(time+1);

				var id = $"{pos}{trip}-{time % cycle}";
				if (seen.TryGetValue(id, out var last))
				{
					if (time >= last)
						continue;
				}
				seen[id] = time;


				if (pos == map.Start.Up)
				{
					queue.Enqueue((pos, trip, time + 1));//, pos.ManhattanDistanceTo(map.End));
					if (map.IsVacant(pos.Down, time + 1))
						queue.Enqueue((pos.Down, trip+1, time + 1));
					continue;
				}
				if (pos == map.Start && trip == 2)
				{
					queue.Enqueue((pos.Up, trip, time + 1));
					// if (map.IsVacant(pos, time + 1))
					// 	queue.Enqueue((pos, trip, time + 1));
				}

				if (pos == map.End)
				{
					queue.Enqueue((pos.Down, trip, time + 1));
					// if (map.IsVacant(pos, time + 1))
					// 	queue.Enqueue((pos, trip, time + 1));
				}
				if (pos == map.End.Down)
				{
					if (trip == 3)
						return time; // huzzah
					queue.Enqueue((pos, trip, time + 1));
					if (map.IsVacant(pos.Up, time + 1))
						queue.Enqueue((pos.Up, 2, time + 1));
					continue;
				}


				if (map.IsVacant(pos, time + 1))
				{
					queue.Enqueue((pos, trip, time + 1));//, pos.ManhattanDistanceTo(map.End));
				}
				foreach (var p in pos.LookAround().Where(p => p.Within(w, h) && map.IsVacant(p, time + 1)))
				{
					queue.Enqueue((p, trip, time + 1));//, p.ManhattanDistanceTo(map.End));
				}
			}
			throw new Exception("No path found");
		}

	}
}
