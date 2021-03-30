using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day10
{
	internal class Puzzle : PuzzleWithParam<int, string, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "The Stars Align";
		public override int Year => 2018;
		public override int Day => 10;

		public void Run()
		{
			Run("test1").Part1("HI");
			Run("input").Part1("PHLGRNFK").Part2(10407);
		}

		protected override string Part1(string[] input)
		{
			var (message, _) = GetMessageAndSeconds(input);
			return message;
		}

		protected override int Part2(string[] input)
		{
			var (_, seconds) = GetMessageAndSeconds(input);
			return seconds;
		}

		private (string, int) GetMessageAndSeconds(string[] input)
		{
			const int MaxHeight = 10;

			// Read all positions and velocities
			var points = input
				.Select(line =>
				{
					var (px, py, vx, vy) = line.RxMatch("position=<%D,%D> velocity=<%D,%D>").Get<int, int, int, int>();
					return new
					{
						P = Point.From(px, py),
						V = Point.From(vx, vy)
					};
				})
				.ToArray();
			var N = points.Length;

			// Instead of looping through all seconds be smarter at finding the time that
			// all the points align. Just pick the first point that at some time will appear
			// at the same y-value as point[0] - there's sure to be SOME other points that
			// end up at that y-value. Do so by solving this for Y:
			//       P0 + n*V0 == Pn + n*Vn
			//  <==> n == (P0 - Pn) / (Vn - V0), for Vn != V0
			var p0 = points[0];
			int dP(int second) => p0.P.Y - points[second].P.Y;
			int dV(int second) => points[second].V.Y - p0.V.Y;
			var index = Enumerable.Range(1, N)
				.Where(i => points[i].V.Y != p0.V.Y)
				.First(i => dP(i) % dV(i) == 0);
			var alignedAt = dP(index) / dV(index);

			// We now know the second that two points will align. Explore the vicinity of that
			// time to find the exact second where the message is precisely 9 chars tall; that's
			// the time of the message.
			for (var sec = alignedAt - MaxHeight; sec < alignedAt + MaxHeight; sec++)
			{
				var image = Enumerable.Range(0, N).Select(i => points[i].P + points[i].V * sec).ToArray();
				var miny = image.Min(p => p.Y);
				var maxy = image.Max(p => p.Y);
				var height = maxy - miny + 1;
				if (height <= MaxHeight && TryParseMessage(image, out var message))
				{
					return (message, sec);
				}				
			}			
			throw new Exception("Message not found");
		}

		private static bool TryParseMessage(Point[] points, out string message)
		{
			// Don't "parse" as such, but just do a crude check for well-known
			// message patterns.
			var (minx, maxx) = (points.Min(p => p.X), points.Max(p => p.X));
			var (miny, maxy) = (points.Min(p => p.Y), points.Max(p => p.Y));
			var (width, height) = (maxx - minx + 1, maxy - miny + 1);
			var map = CharMatrix.Create(width, height, ' ');
			foreach (var p in points)
			{
				map[p.X - minx, p.Y - miny] = '#';
			}
			//map.ConsoleWrite();
			var parseable = new Dictionary<string, char[,]>
			{
				{
					"HI",
					new string[]
					{
						"#   #  ###",
						"#   #   # ",
						"#   #   # ",
						"#####   # ",
						"#   #   # ",
						"#   #   # ",
						"#   #   # ",
						"#   #  ###",
					}.ToCharMatrix()
				},
				{
					"PHLGRNFK",
					new string[]
					{
						"#####   #    #  #        ####   #####   #    #  ######  #    #",
						"#    #  #    #  #       #    #  #    #  ##   #  #       #   # ",
						"#    #  #    #  #       #       #    #  ##   #  #       #  #  ",
						"#    #  #    #  #       #       #    #  # #  #  #       # #   ",
						"#####   ######  #       #       #####   # #  #  #####   ##    ",
						"#       #    #  #       #  ###  #  #    #  # #  #       ##    ",
						"#       #    #  #       #    #  #   #   #  # #  #       # #   ",
						"#       #    #  #       #    #  #   #   #   ##  #       #  #  ",
						"#       #    #  #       #   ##  #    #  #   ##  #       #   # ",
						"#       #    #  ######   ### #  #    #  #    #  #       #    #"
					}.ToCharMatrix()
				}
			};
			message = parseable
				.Where(p => map.Match(p.Value))
				.Select(x => x.Key)
				.FirstOrDefault();
			return message != null;
		}
	}
}
