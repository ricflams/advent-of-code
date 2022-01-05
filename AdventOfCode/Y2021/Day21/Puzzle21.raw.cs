using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2021.Day21.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Day 21";
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
				//Console.WriteLine($"P1: pos={p1pos} score={p1score}");
				//Console.WriteLine($"P2: pos={p2pos} score={p2score}");
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

		internal class World
		{
			public static World Copy(World other)
			{
				return new World
				{
					ActivePlayer = other.ActivePlayer,
					Players = other.Players.Select(x => new Player { Pos = x.Pos, Score = x.Score, Wins = 0 }).ToArray()
				};
			}
			public int ActivePlayer;
			public Player[] Players;
			public Player Current => Players[ActivePlayer];
			public string Key => $"{Players[0].Key}-{Players[1].Key}-{ActivePlayer}";
			public string ReverseKey => $"{Players[1].Key}-{Players[0].Key}-{1-ActivePlayer}";

			//public int Seen;
			public override string ToString() => $"[active={ActivePlayer + 1} {Players[0]} {Players[1]}]";
			public List<World> NextWorlds = new List<World>();

			public class Player
			{
				public int Pos;
				public int Score;
				public long Wins;
				public string Key => $"{Pos}-{Score}";
				public override string ToString() => $"[pos={Pos} score={Score} wins={Wins}]";
			}
		}

		protected override long Part2(string[] input)
		{
			var p1startPos = int.Parse(input[0].Split(':').Last());
			var p2startPos = int.Parse(input[1].Split(':').Last());


			//var wincache = new Dictionary<string, (long, long)>();
			var worlds = new Dictionary<string, (long, long)>();

			var state0 = new World
			{
				ActivePlayer = 0,
				Players = new World.Player[]
				{
					new World.Player { Pos = p1startPos, Score = 0 },
					new World.Player { Pos = p2startPos, Score = 0 }
				}
			};

			//worlds[state0.Key] = state0;
			RollDiracDie(state0);

			//        while (true)
			//        {
			//RollDiracDie(state0);
			//state0 = worlds.Values.FirstOrDefault(w => w.Cur.Wins == 0);
			//            if (state0 == null)
			//                break;
			//        }

			//var p1wins = worlds.Values.Where(x => x.ActivePlayer == 0).Select(x => x.Seen * x.Wi).Sum();
			//var p2wins = worlds.Values.Where(x => x.ActivePlayer == 1).Select(x => x.Seen * x.ActivePlayerWinnings).Sum();
			//         var p1wins = worlds.Values.Select(x => (long)(x.Seen * x.Players[0].Wins)).Sum();
			//var p2wins = worlds.Values.Select(x => (long)(x.Seen * x.Players[1].Wins)).Sum();

			return Math.Max(state0.Players[0].Wins, state0.Players[1].Wins);

			//var maxwins = Math.Max(p1wins, p2wins);
			//return maxwins;


			(long, long) RollDiracDie(World world)
			{
				//Console.WriteLine($"Player {(isPlayer1 ? '1' : '2')} p1score={p1score} p2score={p2score}");
				//if (!worlds.TryGetValue(state.Key, out var cur))
				//{
				//	cur = worlds[state.Key] = state;
				//	cur.Seen = 1;
				//}


				var player = world.Current;
				var nextstates = new List<World>();
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
								player.Wins++;
							}
							else
							{
								var rolled = World.Copy(world);
								rolled.Current.Pos = pos;
								rolled.Current.Score = score;
								rolled.ActivePlayer = 1 - rolled.ActivePlayer;
								if (worlds.TryGetValue(rolled.Key, out var x)) // or vice versa!!
								{
									world.Players[0].Wins += x.Item1;
									world.Players[1].Wins += x.Item2;
								}
								else if (worlds.TryGetValue(rolled.ReverseKey, out var x2)) // or vice versa!!
								{
									world.Players[0].Wins += x2.Item2;
									world.Players[1].Wins += x2.Item1;
								}
								else
								{
									var add = RollDiracDie(rolled);
									world.Players[0].Wins += add.Item1;
									world.Players[1].Wins += add.Item2;
								}
							}
						}
					}
				}
				worlds[world.Key] = (world.Players[0].Wins, world.Players[1].Wins);
				return worlds[world.Key];

			}

			//(long, long) RollDiracDie(int p1pos, int p2pos, int roll, long p1score, long p2score)
			//{
			//	//var sum = (0L, 0L);
			//	//for (var i = 0; i < 3; i++)
			//	//{
			//		var isPlayer1 = (roll / 3) % 2 == 0;
			//		var die = (roll % 3) + 1;
			//		Console.WriteLine($"Roll {roll}: p1={isPlayer1} p1score={p1score} p2score={p2score}");
			//		roll++;

			//		if (isPlayer1)
			//		{
			//			p1pos += die;
			//			if (p1pos > 10)
			//				p1pos -= 10;
			//			p1score += p1pos;
			//		}
			//		else
			//		{
			//			p2pos += die;
			//			if (p2pos > 10)
			//				p2pos -= 10;
			//			p2score += p2pos;
			//		}
			//		var key0 = $"{p1pos}-{p2pos}-{p1score}-{p2score}-{isPlayer1}-{die}";
			//		if (!wincache.TryGetValue(key0, out var value))
			//		{
			//			value =
			//				p1score >= 21 ? (1, 0) :
			//				p2score >= 21 ? (0, 1) :
			//				RollDiracDie(p1pos, p2pos, roll, p1score, p2score);
			//			wincache[key0] = value;
			//		}
			//		return value;
			//	//	sum = (sum.Item1 + value.Item1, sum.Item2 + value.Item2);
			//	//}
			//	//return sum;
			//}

			//(long, long) RollDiracDieSimple(int p1pos, int p2pos, bool isPlayer1, long p1score, long p2score)
			//{
			//	Console.WriteLine($"Player {(isPlayer1 ? '1' : '2')} p1score={p1score} p2score={p2score}");

			//	var wins = (0L, 0L);
			//	if (isPlayer1)
			//	{
			//		var (p1wins1, p2wins1) = RollDie(1, ref p1pos, ref p1score);
			//		var (p1wins2, p2wins2) = RollDie(2, ref p1pos, ref p1score);
			//		var (p1wins3, p2wins3) = RollDie(3, ref p1pos, ref p1score);
			//		wins = (p1wins1 + p1wins2 + p1wins3, p2wins1 + p2wins2 + p2wins3);
			//	}
			//	else
			//	{
			//		var (p1wins1, p2wins1) = RollDie(1, ref p2pos, ref p2score);
			//		var (p1wins2, p2wins2) = RollDie(2, ref p2pos, ref p2score);
			//		var (p1wins3, p2wins3) = RollDie(3, ref p2pos, ref p2score);
			//		wins = (p1wins1 + p1wins2 + p1wins3, p2wins1 + p2wins2 + p2wins3);
			//	}

			//		//var key0 = $"{p1pos}-{p2pos}-{p1score}-{p2score}-{isPlayer1}-{die}";
			//		//if (!wincache.TryGetValue(key0, out var value))
			//		//{
			//		//	value =
			//		//		p1score >= 21 ? (1, 0) :
			//		//		p2score >= 21 ? (0, 1) :
			//		//		RollDiracDieSimple(p1pos, p2pos, isPlayer1, p1score, p2score);
			//		//	wincache[key0] = value;
			//		//}
			//		//wins = (wins.Item1 + value.Item1, wins.Item2 + value.Item2);
			//	return wins;

			//	(long, long) RollDie(int die, ref int pos, ref long score)
			//	{
			//		pos += die;
			//		if (pos > 10)
			//			pos -= 10;
			//		score += pos;
			//		if (score >= 21)
			//		{
			//			return score == p1score ? (1, 0) : (0, 1);
			//		}
			//		else
			//		{
			//			return RollDiracDieSimple(p1pos, p2pos, isPlayer1, p1score, p2score);
			//		}
			//	}
			//}
		}

		//protected override long Part2(string[] input)
		//{
		//	var p1startPos = int.Parse(input[0].Split(':').Last());
		//	var p2startPos = int.Parse(input[1].Split(':').Last());


		//	var wins = new Dictionary<string, (long, long)>();

		//	var (p1wins, p2wins) = RollDiracDie(p1startPos, p2startPos, 0, 0, 0);

		//	var maxwins = Math.Max(p1wins, p2wins);
		//	return maxwins;

		//	(long, long) RollDiracDie(int p1pos, int p2pos, int roll, long p1score, long p2score)
		//	{
		//		var isPlayer1 = (roll / 3) % 2 == 0;
		//		var remainingRollsForPlayer = 3;// - (roll % 3);
		//		var key = $"{p1pos}-{p2pos}-{p1score}-{p2score}-{isPlayer1}-{remainingRollsForPlayer}";
		//		if (wins.TryGetValue(key, out var value))
		//			return value;

		//		Console.WriteLine($"Roll {roll}: p1={isPlayer1} p1score={p1score} p2score={p2score}");


		//		if (isPlayer1)
		//		{
		//			var sum = (0L, 0L);
		//			for (var i = 0; i < remainingRollsForPlayer; i++)
		//			{
		//				var die = (roll++ % 3) + 1;
		//				var (p1wins, p2wins) = RollP1Die(die);
		//				sum = (sum.Item1 + p1wins, sum.Item2 + p2wins);
		//			}
		//			wins[key] = sum;
		//		}
		//		else
		//		{
		//			var sum = (0L, 0L);
		//			for (var i = 0; i < remainingRollsForPlayer; i++)
		//			{
		//				var die = (roll++ % 3) + 1;
		//				var (p1wins, p2wins) = RollP2Die(die);
		//				sum = (sum.Item1 + p1wins, sum.Item2 + p2wins);
		//			}
		//			wins[key] = sum;

		//			//var (p1wins1, p2wins1) = RollP2Die();
		//			//var (p1wins2, p2wins2) = RollP2Die();
		//			//var (p1wins3, p2wins3) = RollP2Die();
		//			//wins[key] = (p1wins1 + p1wins2 + p1wins3, p2wins1 + p2wins2 + p2wins3);
		//		}
		//		if (p1score < 21 || p2score < 21)
		//			RollDiracDie(p1pos, p2pos, roll, p1score, p2score);

		//		return wins[key];

		//		(long, long) RollP1Die(int die)
		//		{
		//			//var die = (roll++ % 3) + 1;
		//			p1pos += die;
		//			if (p1pos > 10)
		//				p1pos -= 10;
		//			p1score += p1pos;
		//			if (p1score >= 21)
		//			{
		//				var key = $"{p1pos}-{p2pos}-{p1score}-{p2score}-{isPlayer1}-{remainingRollsForPlayer}";
		//				wins[key] = (1, 0);
		//				return (1, 0);
		//			}
		//			else
		//			{
		//				return RollDiracDie(p1pos, p2pos, roll, p1score, p2score);
		//			}
		//		}

		//		(long, long) RollP2Die(int die)
		//		{
		//			//var die = (roll++ % 3) + 1;
		//			p2pos += die;
		//			if (p2pos > 10)
		//				p2pos -= 10;
		//			p2score += p2pos;
		//			if (p2score >= 21)
		//			{
		//				var key = $"{p1pos}-{p2pos}-{p1score}-{p2score}-{isPlayer1}-{remainingRollsForPlayer}";
		//				wins[key] = (0, 1);
		//				return (0, 1);
		//			}
		//			else
		//			{
		//				return RollDiracDie(p1pos, p2pos, roll, p1score, p2score);
		//			}
		//		}

		//	}
		//}

	}
}
