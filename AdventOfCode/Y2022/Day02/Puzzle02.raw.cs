using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day02.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Rock Paper Scissors";
		public override int Year => 2022;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(15).Part2(12);
			Run("input").Part1(10816).Part2(11657);
		}


		private enum Shape { Rock, Paper, Scissor };
		private class Hand
		{
			public Hand(char ch)
			{
				Shape = ch switch
				{
					'X' => Shape.Rock,
					'Y' => Shape.Paper,
					'Z' => Shape.Scissor,
					'A' => Shape.Rock,
					'B' => Shape.Paper,
					'C' => Shape.Scissor,
					_ => throw new Exception()
				};
			}
			public Shape Shape;
			public bool CanBeat(Hand other)
			{
				return
				 	Shape == Shape.Rock && other.Shape == Shape.Scissor ||
					Shape == Shape.Paper && other.Shape == Shape.Rock ||
					Shape == Shape.Scissor && other.Shape == Shape.Paper;
			}
			public int Value {
				get {
					return Shape switch
								{
									Shape.Rock => 1,
									Shape.Paper => 2,
									Shape.Scissor => 3,
									_ => throw new Exception()
								};
				}

			} 
		}

		protected override long Part1(string[] input)
		{
			var sg = input
				.Select(x => (new Hand(x[0]), new Hand(x[2])))
				.ToArray();

			var score = 0;
			foreach (var (opp, me) in sg)
			{
				score += me.Value;
				if (opp.Shape == me.Shape)
					score += 3;
				else if (me.CanBeat(opp))
					score += 6;
			}

			return score;
		}

		protected override long Part2(string[] input)
		{
			var sg = input
				.Select(x => (new Hand(x[0]), new Hand(x[2])))
				.ToArray();

			var score = 0;
			foreach (var (opp, me) in sg)
			{
				switch (me.Shape)
				{
					case Shape.Rock:
						// must loose
						me.Shape = opp.Shape switch
						{
							Shape.Paper => Shape.Rock,
							Shape.Rock => Shape.Scissor,
							Shape.Scissor => Shape.Paper,
							_ => throw new Exception()
						};
						break;
					case Shape.Paper:
						// must tie
						me.Shape = opp.Shape;
						break;
					case Shape.Scissor:
						// must win
						me.Shape = opp.Shape switch
						{
							Shape.Paper => Shape.Scissor,
							Shape.Rock => Shape.Paper,
							Shape.Scissor => Shape.Rock,
							_ => throw new Exception()
						};
						break;
				}
				score += me.Value;
				if (opp.Shape == me.Shape)
					score += 3;
				else if (me.CanBeat(opp))
					score += 6;
			}

			return score;
		}

	}
}
