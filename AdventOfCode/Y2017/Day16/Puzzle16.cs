using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

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
			//RunFor("test1", "", 0);
			//RunFor("test2", 0, 0);
			RunFor("input", "namdgkbhifpceloj", "");
		}

		protected override string Part1(string[] input)
		{
			var s = "abcdefghijklmnop".ToCharArray();
			return new string(Dance(s, input[0]));

			// foreach (var move in input[0].Split(','))
			// {
			// 	switch (move[0])
			// 	{
			// 		case 's':
			// 			// Spin, written sX, makes X programs move from the end to the front
			// 			var shiftby = int.Parse(move[1..]);
			// 			s = s[^shiftby..].Concat(s[..^shiftby]).ToArray();
			// 			break;

			// 		case 'x':
			// 			// Exchange, written xA/B, makes the programs at positions A and B swap places.
			// 			var (xa, xb) = move.RxMatch("x%d/%d").Get<int, int>();
			// 			(s[xa], s[xb]) = (s[xb], s[xa]);
			// 			break;

			// 		case 'p':
			// 			// Partner, written pA/B, makes the programs named A and B swap places
			// 			var (pa, pb) = (move[1], move[3]);
			// 			for (var i = 0; i < s.Length; i++)
			// 			{
			// 				// Swap every letter seen
			// 				if (s[i] == pa)
			// 					s[i] = pb;
			// 				else if (s[i] == pb)
			// 					s[i] = pa;
			// 			}
			// 			break;
					
			// 		default:
			// 			throw new Exception($"Unknown move {move}");
			// 	}
			// }

			// return new string(s);
		}

		private char[] Dance(char[] s, string moves)
		{
			foreach (var move in moves.Split(','))
			{
				switch (move[0])
				{
					case 's':
						// Spin, written sX, makes X programs move from the end to the front
						var shiftby = int.Parse(move[1..]);
						s = s[^shiftby..].Concat(s[..^shiftby]).ToArray();
						break;

					case 'x':
						// Exchange, written xA/B, makes the programs at positions A and B swap places.
						var (xa, xb) = move.RxMatch("x%d/%d").Get<int, int>();
						(s[xa], s[xb]) = (s[xb], s[xa]);
						break;

					case 'p':
						// Partner, written pA/B, makes the programs named A and B swap places
						var (pa, pb) = (move[1], move[3]);
						for (var i = 0; i < s.Length; i++)
						{
							// Swap every letter seen
							if (s[i] == pa)
								s[i] = pb;
							else if (s[i] == pb)
								s[i] = pa;
						}
						break;
					
					default:
						throw new Exception($"Unknown move {move}");
				}
			}
			return s;
		}

		protected override string Part2(string[] input)
		{
			var s0 = "abcdefghijklmnop".ToCharArray();


			// for (var i = 0; i < 20; i++)
			// {
			// 	Console.WriteLine($"{i}: {new string(s0)}");
			// 	s0 = Dance(s0, input[0]);
			// }


			var s1 = new string(Dance(s0, input[0]));

			var offsets = s0
				.Select((c, i) => (s1.IndexOf(c) - i + s0.Length) %  s0.Length)
				.ToArray();


			var loops = 0;
			var pos0 = 1;
			var pos = pos0;
			do
			{
				loops++;
				pos = (pos + offsets[pos]) % s0.Length;
			}
			while (pos != pos0);

			var N = 1_000_000_000;
			var s2 = new char[s0.Length];
			for (var i = 0; i < s0.Length; i++)
			{
				var offset = (offsets[i] * N) % s2.Length;
				Console.WriteLine($"offset {i}: {offset}");
				s2[offset] = s0[i];
			}

			return new string(s2);
		}
	}
}
