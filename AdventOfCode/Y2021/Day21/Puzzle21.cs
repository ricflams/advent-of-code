using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers;

namespace AdventOfCode.Y2021.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Dirac Dice";
		public override int Year => 2021;
		public override int Day => 21;

		public void Run()
		{
			Run("test1").Part1(739785).Part2(444356092776315);
			Run("input").Part1(742257).Part2(93726416205179);
		}

		protected override long Part1(string[] input)
		{
			var p1pos = int.Parse(input[0].Split(':').Last());
			var p2pos = int.Parse(input[1].Split(':').Last());
			var diesize = 100;
			var die = 0;

			var p1score = 0;
			var p2score = 0;

			var turn = 0;
			while (p1score < 1000 && p2score < 1000)
			{
				if (turn++ % 2 == 0)
				{
					p1pos = ((p1pos + RollDie() + RollDie() + RollDie() - 1) % 10) + 1;
					p1score += p1pos;
				}
				else
				{
					p2pos = ((p2pos + RollDie() + RollDie() + RollDie() - 1) % 10) + 1;
					p2score += p2pos;
				}
			}

			var result = Math.Min(p1score, p2score) * (turn * 3);

			int RollDie()
			{
				die++;
				if (die > diesize)
					die = 1;
				return die;
			}

			return result;
		}

		protected override long Part2(string[] input)
		{
			var p1startPos = int.Parse(input[0].Split(':').Last());
			var p2startPos = int.Parse(input[1].Split(':').Last());

			var worlds = new Dictionary<int, (long, long)>();

			var reality = new World
			{
				Active = 0,
				Players = new World.Player[]
				{
					new World.Player { Pos = p1startPos, Score = 0 },
					new World.Player { Pos = p2startPos, Score = 0 }
				}
			};

			var wins = RollDiracDie(reality);
			var maxWins = Math.Max(wins.Item1, wins.Item2);

			return maxWins;


			(long, long) RollDiracDie(World world)
			{
				var player = world.ActivePlayer;
				var player0 = world.Players[0];
				var player1 = world.Players[1];

				var wins = (0L, 0L);

				for (var die1 = 1; die1 <= 3; die1++)
				{
					for (var die2 = 1; die2 <= 3; die2++)
					{
						for (var die3 = 1; die3 <= 3; die3++)
						{
							var move = die1 + die2 + die3;
							var pos = player.Pos + move;
							if (pos > 10)
								pos -= 10;
							var score = player.Score + pos;

							if (score >= 21)
							{
								wins = wins.Plus(world.Active == 0 ? (1, 0) : (0, 1));
							}
							else
							{
								// Create a new world that reflects the rolled situation
								var rolled = world with
                                {
									Players = new World.Player[]
									{
										world.Players[0] with { },
										world.Players[1] with { },
									}
								};
								rolled.ActivePlayer.Pos = pos;
								rolled.ActivePlayer.Score = score;
								rolled.Active = 1 - rolled.Active;

								// If this combination has already been rolled then use that cached
								// result. Also check the reverse situation; it results in a runtime-
								// reduction of ~30%.
								if (worlds.TryGetValue(rolled.Key, out var cached))
								{
									wins = wins.Plus(cached);
								}
								else if (worlds.TryGetValue(rolled.ReverseKey, out var reversed))
								{
									// Note that win-values should be reversed
									wins = wins.Plus((reversed.Item2, reversed.Item1));
								}
								else
								{
									// Situation hasn't been encountered before so roll the die
									var roll = RollDiracDie(rolled);
									wins = wins.Plus(roll);
								}
							}
						}
					}
				}

				worlds[world.Key] = wins;
				return wins;
			}
		}


		internal record World
		{
			public int Active;
			public Player[] Players;
			public Player ActivePlayer => Players[Active];

			public int Key => Active * 1000000 + Players[0].Key * 1000 + Players[1].Key;
			public int ReverseKey => (1 - Active) * 1000000 + Players[1].Key * 1000 + Players[0].Key;

			public override string ToString() => $"[active={Active + 1} {Players[0]} {Players[1]}]";

			public record Player
			{
				public int Pos;
				public int Score;
				public int Key => Score * 11 + Pos;
				public override string ToString() => $"[pos={Pos} score={Score}]";
			}
		}

	}
}
