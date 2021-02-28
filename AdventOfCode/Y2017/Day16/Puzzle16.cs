using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2017.Day16
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Permutation Promenade";
		public override int Year => 2017;
		public override int Day => 16;

		public void Run()
		{
			RunFor("input", "namdgkbhifpceloj", "ibmchklnofjpdeag");
		}

		protected override string Part1(string[] input)
		{
			var moves = ReadMoves(input[0]);
			var s = "abcdefghijklmnop";
			return Dance(s, moves);
		}

		protected override string Part2(string[] input)
		{
			var moves = ReadMoves(input[0]);
			var s0 = "abcdefghijklmnop";

			// Dance until we've seen a complete cycle. It won't be long,
			// just 60 dances for this specific puzzle input.
			var dances = new List<string>();
			dances.Add(s0);
			while (true)
			{
				var s = Dance(dances.Last(), moves);
				if (s == s0)
					break;
				dances.Add(s);
			}

			// Find last dance's index
			var N = 1_000_000_000;
			var lastdance = dances[N % dances.Count()];
			return lastdance;
		}

		private static string Dance(string s0, IMove[] moves)
		{
			var s = s0.ToCharArray();
			foreach (var move in moves)
			{
				move.Step(s);
			}
			return new string(s);
		}

		private static IMove[] ReadMoves(string moves)
		{
			return moves.Split(',').Select(move =>
			{
				IMove m = move[0] switch
				{
					's' => new Spin(move),
					'x' => new Exchange(move),
					'p' => new Partner(move),
					_ => throw new Exception($"Unknown move {move}")
				};
				return m;
			} )
			.ToArray();
		}

		internal interface IMove
		{
			void Step(char[] s);
		}
		internal class Spin : IMove
		{
			private readonly int _x;
			public Spin(string move) => _x = int.Parse(move[1..]);
			public void Step(char[] s)
			{
				// Spin, written sX, makes X programs move from the end to the front
				var tmp = s[^_x..].Concat(s[..^_x]).ToArray();
				Array.Copy(tmp, s, s.Length);
			}
		}
		internal class Exchange : IMove
		{
			private readonly int _a, _b;
			public Exchange(string move) => (_a, _b) = move.RxMatch("x%d/%d").Get<int, int>();
			public void Step(char[] s)
			{
				// Exchange, written xA/B, makes the programs at positions A and B swap places.
				(s[_a], s[_b]) = (s[_b], s[_a]);
			}
		}
		internal class Partner : IMove
		{
			private readonly char _a, _b;
			public Partner(string move) => (_a, _b) = (move[1], move[3]);
			public void Step(char[] s)
			{
				// Partner, written pA/B, makes the programs named A and B swap places
				for (var i = 0; i < s.Length; i++)
				{
					// Swap every letter seen
					if (s[i] == _a)
						s[i] = _b;
					else if (s[i] == _b)
						s[i] = _a;
				}
			}
		}
	}
}
