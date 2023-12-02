using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.Day02
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Cube Conundrum";
		public override int Year => 2023;
		public override int Day => 2;

		public void Run()
		{
			Run("test1").Part1(8).Part2(2286);
			Run("input").Part1(2348).Part2(76008);
		}

		protected override long Part1(string[] input)
		{
			var games = ReadGames(input);

			var possibleGames = games
				.Sum(g => g.Cubes.All(c => c.R <= 12 && c.G <= 13 && c.B <= 14) ? g.Id : 0);

			return possibleGames;
		}

		protected override long Part2(string[] input)
		{
			var games = ReadGames(input);

			var powerSum = games
				.Sum(game =>
				{
					var r = game.Cubes.Max(x => x.R);
					var g = game.Cubes.Max(x => x.G);
					var b = game.Cubes.Max(x => x.B);
					return r * g * b;
				});

			return powerSum;
		}

		private record Game(int Id, (int R, int G, int B)[] Cubes);

		private static Game[] ReadGames(string[] input)
		{
			// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
			return input
				.Select((s,i) => {
					var cubes = s.Split(':')[1]
						.Split(';')
						.Select(set =>
						(
							R: CubeCount(set, "red"),
							G: CubeCount(set, "green"),
							B: CubeCount(set, "blue")
						))
						.ToArray();
					return new Game(i + 1, cubes);
				})
				.ToArray();

			static int CubeCount(string set, string color)
			{
				var m = Regex.Match(set, $"(\\d+) {color}");
				return m.Success ? int.Parse(m.Groups[1].Value) : 0;
			}
		}

	}
}
