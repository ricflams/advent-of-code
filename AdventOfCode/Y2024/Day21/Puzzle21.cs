using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2024.Day21
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Keypad Conundrum";
		public override int Year => 2024;
		public override int Day => 21;

		public override void Run()
		{
			Run("test1").Part1(126384);
			Run("input").Part1(231564).Part2(281212077733592);
			Run("extra").Part1(157908).Part2(196910339808654);
		}

		protected override long Part1(string[] input)
		{
			return ComplexitySum(input, 2);
		}

		protected override long Part2(string[] input)
		{
			return ComplexitySum(input, 25);
		}

		private static long ComplexitySum(string[] input, int robots)
		{
			var codes = input;
			var numPadMoves = FindPadMoves(NumericKeypad);
			var dirPadMoves = FindPadMoves(DirectionalKeypad);
			var memo = new Dictionary<string, long>();

			var complexity = codes.Sum(code =>
			{
				var seq = NumPadPresses(code);
				var num = int.Parse(code[..3]);
				return seq * num;
			});

			return complexity;


			long NumPadPresses(string code)
			{
				// The robot must pick the smallest set of clicks to perform the desired
				// movement for all keys in the code, one by one starting at 'A'.
				// This first step is performed by a robot and each RobotKeyPresses is
				// also performed by a robot, so the parameter moreRobots should exclude
				// these two robots in the final chain of robots needed.
				var moreRobots = robots - 2;
				var from = 'A';
				var length = 0L;
				foreach (var key in code)
				{
					var moves = numPadMoves[from][key];
					length += moves.Min(x => RobotKeyPresses(x + "A", moreRobots));
					from = key;
				}
				return length;
			}

			long RobotKeyPresses(string keys, int moreRobots)
			{
				// Without memo we won't ever finish
				var memokey = $"{keys}-{moreRobots}";
				if (memo.TryGetValue(memokey, out var length))
					return length;

				// The robot must pick the smallest set of clicks to perform the desired
				// movement for all keys in the code, one by one starting at 'A', by
				// asking either another robot or finally the human.
				var key = 'A';
				foreach (var next in keys)
				{
					var moves = dirPadMoves[key][next];
					length += moreRobots > 0
						? moves.Min(x => RobotKeyPresses(x + "A", moreRobots - 1))
						: moves.Min(x => HumanKeyPresses(x + "A"));
					key = next;
				}

				memo[memokey] = length;
				return length;
			}

			long HumanKeyPresses(string keys)
			{
				// This is finally the human input. It's just as many keypresses as there exist
				// in the desired sequence (eg ^<< is 3 keypresses) plus the final press on 'A'
				var key = 'A';
				var length = 0L;
				foreach (var next in keys)
				{
					// All moves from a to b are equally long so just pick length of the first move
					var moves = dirPadMoves[key][next].First().Length + 1;
					length += moves;
					key = next;
				}
				return length;
			}
		}

		private static readonly char[,] NumericKeypad = new char[,]
		{
			{ '7', '8', '9' },
			{ '4', '5', '6' },
			{ '1', '2', '3' },
			{ ' ', '0', 'A' }
		};

		private static readonly char[,] DirectionalKeypad = new char[,]
		{
			{ ' ', '^', 'A' },
			{ '<', 'v', '>' }
		};

		private class Movements : Dictionary<char, Dictionary<char, string[]>> { }

		private static Movements FindPadMoves(char[,] pad)
		{
			// Rotate the pad for sanity so x,y works as expected
			pad = pad.FlipXY();
			var (w, h) = pad.Dim();

			// Find the blind spot, marked by ' ' space
			var blindSpot = FindBlindSpot();
			(int, int) FindBlindSpot()
			{
				for (var x = 0; x < w; x++)
					for (var y = 0; y < h; y++)
						if (pad[x, y] == ' ') return (x, y);
				throw new Exception("No blind spot");
			}
			bool IsForbiddenSpot(int x, int y) => (x, y) == blindSpot;

			// Find all moves from all keys to any other key.
			// Include moves from a key to itself because it makes later steps easier.
			// Avoid the blind spot.
			// Don't bother reusing sub-results (caching) as it makes no difference
			var movements = new Movements();
			for (var x1 = 0; x1 < w; x1++)
			{
				for (var y1 = 0; y1 < h; y1++)
				{
					if (IsForbiddenSpot(x1, y1))
						continue;
					var from = pad[x1, y1];
					movements[from] = [];
					for (var x2 = 0; x2 < w; x2++)
					{
						for (var y2 = 0; y2 < h; y2++)
						{
							if (IsForbiddenSpot(x2, y2))
								continue;
							var to = pad[x2, y2];
							movements[from][to] = FindMovements(x1, y1, x2, y2).ToArray();
						}
					}
				}
			}
			return movements;

			IEnumerable<string> FindMovements(int x1, int y1, int x2, int y2)
			{
				if (IsForbiddenSpot(x1, y1))
					yield break;

				// No more moves when we've finally arrived
				if (x1 == x2 && y1 == y2)
				{
					yield return "";
					yield break;
				}

				// Move a step in all legit directions and append the movements from there
				if (x1 < x2)
					foreach (var m in FindMovements(x1 + 1, y1, x2, y2))
						yield return '>' + m;
				else if (x1 > x2)
					foreach (var m in FindMovements(x1 - 1, y1, x2, y2))
						yield return '<' + m;
				if (y1 < y2)
					foreach (var m in FindMovements(x1, y1 + 1, x2, y2))
						yield return 'v' + m;
				else if (y1 > y2)
					foreach (var m in FindMovements(x1, y1 - 1, x2, y2))
						yield return '^' + m;
			}

			// Console.WriteLine("Pad moves:");
			// foreach (var from in mov)
			// 	foreach (var dest in from.Value)
			// 		Console.WriteLine($"From {from.Key} to {dest.Key}: {string.Join(' ', dest.Value)}");
		}
	}
}
