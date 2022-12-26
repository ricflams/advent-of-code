using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day02
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Rock Paper Scissors";
		public override int Year => 2022;
		public override int Day => 2;

		public void Run()
		{
			Run("test1").Part1(15).Part2(12);
			Run("test9").Part1(11150).Part2(8295);
			Run("input").Part1(10816).Part2(11657);
		}

		protected override long Part1(string[] input)
		{
			var score = input
				.Select(x => 
				{
					var you = ParseShape(x[0]);
					var me = ParseShape(x[2]);
					return ScoreInCombat(me, you);
				})
				.Sum();
			return score;
		}

		protected override long Part2(string[] input)
		{
			var score = input
				.Select(x => 
				{
					var you = ParseShape(x[0]);
					var strategy = x[2];
					var me = strategy switch
					{
						'X' => CanBeat[you], // lose
						'Y' => you, // tie
						'Z' => IsBeatBy[you], // win
						_ => throw new Exception($"Unhandled {strategy}")
					};
					return ScoreInCombat(me, you);
				})
				.Sum();
			return score;
		}

		private enum Shape { Rock = 1, Paper = 2, Scissor = 3 };

		private static Dictionary<Shape, Shape> CanBeat = new ()
		{
			{ Shape.Rock, Shape.Scissor },
			{ Shape.Paper, Shape.Rock },
			{ Shape.Scissor, Shape.Paper }
		};

		private static Dictionary<Shape, Shape> IsBeatBy = CanBeat
			.ToDictionary(x => x.Value, x => x.Key);

		private static Shape ParseShape(char ch) =>
			ch switch
			{
				'A' or 'X' => Shape.Rock,
				'B' or 'Y' => Shape.Paper,
				'C' or 'Z' => Shape.Scissor,
				_ => throw new Exception($"Unhandled {ch}")
			};

		private static int ScoreInCombat(Shape me, Shape you) =>
			(int)me + (CanBeat[me] == you ? 6 : me == you ? 3 : 0);
	}
}
