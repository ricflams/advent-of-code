using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System;
using System.Linq;

namespace AdventOfCode.Y2016.Day21
{
	internal class Puzzle : Puzzle<string, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Scrambled Letters and Hash";
		public override int Year => 2016;
		public override int Day => 21;

		public void Run()
		{
			RunPart1For("test1", "decab");
			RunFor("input", "hcdefbag", "fbhaegdc");
		}

		protected override string Part1(string[] input)
		{
			var password = input[0];
			var operations = input[2..];
			var scrambler = new Scrambler(operations);
			var scrambled = scrambler.Scramble(password);
			return scrambled;
		}

		protected override string Part2(string[] input)
		{
			var scrambled = input[1];
			var operations = input[2..];
			var scrambler = new Scrambler(operations);
			var password = scrambler.Scramble(scrambled, true);
			return password;
		}

		private class Scrambler
		{
			private readonly int[] ReverseRotateIndexes;
			private readonly string[] _operations;

			public Scrambler(string[] operations)
			{
				_operations = operations;

				// For "reverse rotation by letter" we need to know the original positions
				// thhat will the letter to end up at the current position, so the reverse
				// operation can rotate it that many times. We simply fill out an array of
				// the formular for rotate-by-letter moves for later lookup. E.g.: at index
				// 2, the letter would end up at 2 (original pos) + 2 (its index, the same)
				// plus 1 (always plus 1) = 5, so we put 2 in slot 5; when reverse-rotating
				// a latter that's at position 5 we then know it muct have been rotated from
				// position 2 so it must be rotated 3 steps to the left.
				var N = 8; // Reverse-rotate only works for strings with length 8
				ReverseRotateIndexes = new int[N];
				for (var i = 0; i < N; i++)
				{
					ReverseRotateIndexes[(i+i+1+(i>=4?1:0)) % N] = i;
				}
			}

			public string Scramble(string password, bool reverse = false)
			{
				var s = password.ToArray();
				var N = s.Length;

				var operations = reverse ? _operations.Reverse() : _operations;

				foreach (var op in operations)
				{
					if (op.IsRxMatch("swap position %d with position %d", out var captures))
					{
						var (x, y) = captures.Get<int, int>();
						// Swap letters at the positions
						(s[x], s[y]) = (s[y], s[x]);
					}
					else if (op.IsRxMatch("swap letter %c with letter %c", out captures))
					{
						var (x, y) = captures.Get<char, char>();
						for (var i = 0; i < N; i++)
						{
							// Swap every letter seen
							if (s[i] == y)
								s[i] = x;
							else if (s[i] == x)
								s[i] = y;
						}
					}
					else if (op.IsRxMatch("rotate %s %d step", out captures))
					{
						var (dir, n) = captures.Get<string, int>();
						// Always turn left into right, unless also reversed
						if (dir == "left" ^ reverse)
						{
							n = N - n;
						}
						s = s.RotateRight(n);
					}
					else if (op.IsRxMatch("rotate based on position of letter %c", out captures))
					{
						var c = captures.Get<char>();
						// Find position and apply formula. For reverse, find "reversed" origin position
						var position = Array.IndexOf(s, c);
						var n = position + 1 + (position >= 4 ? 1 : 0);
						if (reverse)
						{
							var origin = ReverseRotateIndexes[position];
							n = (origin - position + N) % N;
						}
						s = s.RotateRight(n);
					}
					else if (op.IsRxMatch("reverse positions %d through %d", out captures))
					{
						var (x, y) = captures.Get<int, int>();
						// Reverse letters at [x..y] one pair at a time moving "inwards"
						var n = (y - x) / 2;
						for (var i = 0; i <= n; i++)
						{
							(s[x+i], s[y-i]) = (s[y-i], s[x+i]);
						}
					}
					else if (op.IsRxMatch("move position %d to position %d", out captures))
					{
						var (x, y) = captures.Get<int, int>();
						if (reverse)
						{
							(x, y) = (y, x);
						}
						// Simply do array-operations; I don't bother doing it top-efficient
						var c = s[x];
						var s2 = s[0..x].Concat(s[(x+1)..]).ToArray();
						s = s2[0..y].Append(c).Concat(s2[y..]).ToArray();
					}
					else
					{
						throw new Exception($"Unhandled operation: {op}");
					}
				}

				var result = new string(s);
				return result;
			}
		}
	}
}
