using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.Runtime.Intrinsics.Arm;

namespace AdventOfCode.Y2023.Day02.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "TODAY";
		public override int Year => 2023;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(8).Part2(2286);
			Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2(0);
		}

		protected override long Part1(string[] input)
		{

			// Game 8: 4 green, 2 red, 14 blue; 9 green, 1 red, 15 blue; 2 green, 9 red, 8 blue; 11 green, 7 red, 8 blue; 9 red, 7 green, 6 blue
			var games = input
				.Select((s,i) => {
					var x1 = s.Split(':');
					var game = i+1;
					var rgbs = x1[1].Split(';').Select(rgb => {
						int r=0, g=0, b=0;
						foreach (var x in rgb.Split(','))
						{
							var (v, color) = x.RxMatch("%d %s").Get<int, string>();
							if (color == "red")
								r = v;
							else if (color == "green")
								g = v;
							else if (color == "blue")
								b = v;
							else
								throw new Exception();
						}
						
						// var (r,g,b) = rgb.RxMatch("%d red, %d red, %d blue").Get<int, int, int>();
						return (r, g, b);
					})
					.ToArray();
					return rgbs;
				})
				.ToArray();



				var possible = games
					.Select((g,i) => {
						var rgbs = g;
						return rgbs.All(x => x.r <= 12 && x.g <= 13 && x.b <= 14) ? i+1 : 0;
					})
					.Sum();

			return possible;
		}

		protected override long Part2(string[] input)
		{
			// Game 8: 4 green, 2 red, 14 blue; 9 green, 1 red, 15 blue; 2 green, 9 red, 8 blue; 11 green, 7 red, 8 blue; 9 red, 7 green, 6 blue
			var games = input
				.Select((s,i) => {
					var x1 = s.Split(':');
					var game = i+1;
					var rgbs = x1[1].Split(';').Select(rgb => {
						int r=0, g=0, b=0;
						foreach (var x in rgb.Split(','))
						{
							var (v, color) = x.RxMatch("%d %s").Get<int, string>();
							if (color == "red")
								r = v;
							else if (color == "green")
								g = v;
							else if (color == "blue")
								b = v;
							else
								throw new Exception();
						}
						
						// var (r,g,b) = rgb.RxMatch("%d red, %d red, %d blue").Get<int, int, int>();
						return (r, g, b);
					})
					.ToArray();
					return rgbs;
				})
				.ToArray();



				var possible = games
					.Select((g,i) => {
						var rgbs = g;
						// if (rgbs.All(x => x.r <= 12 && x.g <= 13 && x.b <= 14)) {

						// }
						var minr = rgbs.Max(x => x.r);
						var ming = rgbs.Max(x => x.g);
						var minb = rgbs.Max(x => x.b);
						return minr * ming * minb;
					})
					.Sum();

			return possible;
		}
	}
}
