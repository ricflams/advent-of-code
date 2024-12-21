using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;
using System.ComponentModel.DataAnnotations;

namespace AdventOfCode.Y2024.Day21.Raw
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "";
		public override int Year => 2024;
		public override int Day => 21;

		public override void Run()
		{
			Run("test1").Part1(126384);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(231564).Part2(281212077733592);

			// 272808441783776 too low
			// 281212077733592

			Run("extra").Part1(157908).Part2(196910339808654);
		}

		private static readonly char[,] NumericKeypad = new char[4, 3]
		{
			{'7', '8', '9'},
			{'4', '5', '6'},
			{'1', '2', '3'},
			{' ', '0', 'A'}
		};

		private static readonly char[,] DirectionalKeypad = new char[2, 3]
		{
			{ ' ', '^', 'A' },
			{ '<', 'v', '>' }
		};

		private class Movements : Dictionary<char, Dictionary<char, string[]>> { }
		private static Movements InitMoves(char[,] pad)
		{
			var (w, h) = pad.Dim();

			var mov = new Movements();
			var forbiddenSpot = FindForbiddenSpot();
			(int, int) FindForbiddenSpot()
			{
				for (var x = 0; x < w; x++)
					for (var y = 0; y < h; y++)
						if (pad[x, y] == ' ') return (x, y);
				throw new Exception();
			}
			bool IsForbiddenSpot(int x, int y) => (x, y) == forbiddenSpot;

			for (var x1 = 0; x1 < w; x1++)
			{
				for (var y1 = 0; y1 < h; y1++)
				{
					if (IsForbiddenSpot(x1, y1))
						continue;
					var ch1 = pad[x1, y1];
					mov[ch1] = [];
					for (var x2 = 0; x2 < w; x2++)
					{
						for (var y2 = 0; y2 < h; y2++)
						{
							//if (x1 == x2 && y1 == y2 || IsForbiddenSpot(x2, y2))
							if (IsForbiddenSpot(x2, y2))
								continue;
							var ch2 = pad[x2, y2];
							mov[ch1][ch2] = AllMovements(x1, y1, x2, y2).Where(x => x != null).ToArray();
						}
					}
				}
			}

			IEnumerable<string> AllMovements(int x1, int y1, int x2, int y2)
			{
				if (IsForbiddenSpot(x1, y1))
				{
					yield return null;
					yield break;
				}

				if (x1 == x2 && y1 == y2)
				{
					yield return "";
					yield break;
				}

				var ch1 = pad[x1, y1];
				var ch2 = pad[x2, y2];
				if (mov.TryGetValue(ch1, out var my) && my.TryGetValue(ch2, out var movs))
				{
					foreach (var m in movs)
						yield return m;
					yield break;
				}

				if (x1 < x2)
				{
					foreach (var m in AllMovements(x1 + 1, y1, x2, y2).Where(x => x != null))
						yield return '>' + m;
				}
				if (x1 > x2)
				{
					foreach (var m in AllMovements(x1 - 1, y1, x2, y2).Where(x => x != null))
						yield return '<' + m;
				}
				if (y1 < y2)
				{
					foreach (var m in AllMovements(x1, y1 + 1, x2, y2).Where(x => x != null))
						yield return 'v' + m;
				}
				if (y1 > y2)
				{
					foreach (var m in AllMovements(x1, y1 - 1, x2, y2).Where(x => x != null))
						yield return '^' + m;
				}
			}

			// Console.WriteLine("Pad moves:");
			// foreach (var from in mov)
			// {
			// 	foreach (var dest in from.Value)
			// 	{
			// 		Console.WriteLine($"From {from.Key} to {dest.Key}: {string.Join(' ', dest.Value)}");
			// 	}
			// }
			return mov;
		}



		protected override long Part1(string[] input)
		{
			var codes = input;

			var numKeypadMoves = InitMoves(NumericKeypad.RotateClockwise(90).FlipV());
			var dirKeypadMoves = InitMoves(DirectionalKeypad.RotateClockwise(90).FlipV());

			var complexity = codes.Sum(code =>
			{
				var seqs = NumPadSequenceLength(code);
				var seq = seqs.Length;
				var num = int.Parse(code[..3]);
				Console.WriteLine($"seq={seq} num={num}: {seqs}");
				return seq * num;
			});


			string NumPadSequenceLength(string code)
			{
				var move0 = FirstDirPadSequenceMinLength(numKeypadMoves['A'][code[0]]);
				var move1 = FirstDirPadSequenceMinLength(numKeypadMoves[code[0]][code[1]]);
				var move2 = FirstDirPadSequenceMinLength(numKeypadMoves[code[1]][code[2]]);
				var move3 = FirstDirPadSequenceMinLength(numKeypadMoves[code[2]][code[3]]);
				return move0 + move1 + move2 + move3;
			}

			string FirstDirPadSequenceMinLength(string[] moves)
			{
				//Console.WriteLine($"  First robot: find min of {string.Join(' ', moves)}");
				return moves.Select(move => (move + "A").ToCharArray()).Select(FirstDirPadSequenceLength).OrderBy(x => x.Length).First();
			}

			string FirstDirPadSequenceLength(char[] keys)
			{
				//Console.WriteLine($"    First robot: pressing {new string(keys)}:");
				var key = 'A';
				var len = "";
				foreach (var next in keys)
				{
					var moves = dirKeypadMoves[key][next];
					len += SecondDirPadSequenceMinLength(moves);
					//len += 'A';
					key = next;
				}
				//Console.WriteLine($"    First robot: pressing {new string(keys)} gave {len}");
				return len;
			}

			string SecondDirPadSequenceMinLength(string[] moves)
			{
				//Console.WriteLine($"        Second robot: find min of {string.Join(' ', moves)}");
				return moves.Select(move => (move + "A").ToCharArray()).Select(SecondDirPadSequenceLength).OrderBy(x => x.Length).First();
			}

			string SecondDirPadSequenceLength(char[] keys)
			{
				//Console.WriteLine($"        Second robot: pressing {new string(keys)}:");
				var key = 'A';
				var len = "";
				foreach (var next in keys)
				{
					var moves = dirKeypadMoves[key][next].First() + "A";
					len += moves;
					key = next;
				}
				//Console.WriteLine($"        Second robot: pressing {new string(keys)} gave {len}");
				return len;
			}

			// string YouDirPadSequenceMinLength(string[] moves)
			// {
			// 	Console.WriteLine($"            You: find min of {string.Join(' ', moves)}");
			// 	return moves.Select(move => (move + "A").ToCharArray()).Select(YouDirPadSequenceLength).OrderBy(x => x.Length).First();
			// }

			// string YouDirPadSequenceLength(char[] keys)
			// {
			// 	var move = "";
			// 	foreach (var key in keys)
			// 	{
			// 		move += dirKeypadMoves['A'][key].First();
			// 		move += 'A';
			// 		move += dirKeypadMoves[key]['A'].First();
			// 		move += 'A';
			// 	}
			// 	Console.WriteLine($"                You: press {new string(keys)} => {move}");
			// 	return move;
			// }

			return complexity;
		}




		protected override long Part2(string[] input)
		{
			var codes = input;

			var numKeypadMoves = InitMoves(NumericKeypad.RotateClockwise(90).FlipV());
			var dirKeypadMoves = InitMoves(DirectionalKeypad.RotateClockwise(90).FlipV());

			var memo = new Dictionary<string, long>();

			var complexity = codes.Sum(code =>
			{
				var seq = NumPadSequenceLength(code);
				var num = int.Parse(code[..3]);
				Console.WriteLine($"seq={seq} num={num}");
				return seq * num;
			});


			long NumPadSequenceLength(string code)
			{
				var move0 = FirstDirPadSequenceMinLength(numKeypadMoves['A'][code[0]]);
				var move1 = FirstDirPadSequenceMinLength(numKeypadMoves[code[0]][code[1]]);
				var move2 = FirstDirPadSequenceMinLength(numKeypadMoves[code[1]][code[2]]);
				var move3 = FirstDirPadSequenceMinLength(numKeypadMoves[code[2]][code[3]]);
				return move0 + move1 + move2 + move3;
			}

			long FirstDirPadSequenceMinLength(string[] moves)
			{
				//Console.WriteLine($"  First robot: find min of {string.Join(' ', moves)}");
				return moves.Select(move => (move + "A").ToCharArray()).Select(x => FirstDirPadSequenceLength(x, 23)).OrderBy(x => x).First();
			}

			long FirstDirPadSequenceLength(char[] keys, int loop)
			{
				var memokey = $"{new string(keys)}-{loop}";
				if (memo.TryGetValue(memokey, out var s))
					return s;

				//Console.WriteLine($"    First robot: pressing {new string(keys)}:");
				var key = 'A';
				var len = 0L;
				foreach (var next in keys)
				{
					var moves = dirKeypadMoves[key][next];
					len += SecondDirPadSequenceMinLength(moves, loop);
					//len += 'A';
					key = next;
				}
				//Console.WriteLine($"    First robot: pressing {new string(keys)} gave {len}");
				memo[memokey] = len;
				return len;
			}

			long SecondDirPadSequenceMinLength(string[] moves, int loop)
			{
				//Console.WriteLine($"        Second robot: find min of {string.Join(' ', moves)}");
				return moves.Select(move => (move + "A").ToCharArray()).Select(x => loop > 0 ? FirstDirPadSequenceLength(x, loop - 1) : SecondDirPadSequenceLength(x)).OrderBy(x => x).First();
				//return moves.Select(move => (move + "A").ToCharArray()).Select(SecondDirPadSequenceLength).OrderBy(x => x).First();
			}

			long SecondDirPadSequenceLength(char[] keys)
			{
				//Console.WriteLine($"        Second robot: pressing {new string(keys)}:");
				var key = 'A';
				var len = 0L;
				foreach (var next in keys)
				{
					var moves = dirKeypadMoves[key][next].First().Length + 1; // for 'A'
					len += moves;
					key = next;
				}
				//Console.WriteLine($"        Second robot: pressing {new string(keys)} gave {len}");
				return len;
			}

			// string YouDirPadSequenceMinLength(string[] moves)
			// {
			// 	Console.WriteLine($"            You: find min of {string.Join(' ', moves)}");
			// 	return moves.Select(move => (move + "A").ToCharArray()).Select(YouDirPadSequenceLength).OrderBy(x => x.Length).First();
			// }

			// string YouDirPadSequenceLength(char[] keys)
			// {
			// 	var move = "";
			// 	foreach (var key in keys)
			// 	{
			// 		move += dirKeypadMoves['A'][key].First();
			// 		move += 'A';
			// 		move += dirKeypadMoves[key]['A'].First();
			// 		move += 'A';
			// 	}
			// 	Console.WriteLine($"                You: press {new string(keys)} => {move}");
			// 	return move;
			// }

			return complexity;
		}
	}
}
