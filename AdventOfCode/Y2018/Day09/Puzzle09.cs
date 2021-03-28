using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Y2018.Day09
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "";
		public override int Year => 2018;
		public override int Day => 9;

		public void Run()
		{
			Run("test1").Part1(8317);
			Run("test2").Part1(146373);
			Run("test3").Part1(2764);
			Run("test4").Part1(54718);
			Run("test5").Part1(37305);
			Run("input").Part1(371284).Part2(3038972494);
		}

		protected override long Part1(string[] input)
		{
			// 10 players; last marble is worth 1618 points
			var (N, worth) = input[0].RxMatch("%d players; last marble is worth %d").Get<int, int>();

			return CalcScore(N, worth);
			//return CalcScore(10, 25);
		}

		protected override long Part2(string[] input)
		{
			var (N, worth) = input[0].RxMatch("%d players; last marble is worth %d").Get<int, int>();
			return CalcScore(N, worth * 100);
		}

		private static long CalcScore(int N, int worth)
		{
			// var marbles = new List<int>();
			// var players = new long[N];
			var players2 = new long[N];

			var next = new int[worth+2];
			var prev = new int[worth+2];

			var marble2pos = 0;

			// marbles.Add(0);

			for (var m = 1; m <= worth+1; m++)
			{
				// for (var i = 0; i < marbles.Count(); i++)
				// {
				// 	if (i == currentMarble)
				// 		Console.Write($"({marbles[i]})");
				// 	else
				// 		Console.Write($" {marbles[i]} ");
				// }
				// Console.WriteLine();

				// var mpos = 0;
				// do
				// {
				// 	if (mpos == marble2pos)
				// 		Console.Write($"({mpos})");
				// 	else
				// 		Console.Write($" {mpos} ");
				// 	var xx = mpos;
				// 	mpos = next[mpos];
				// 	if (prev[mpos] != xx)
				// 		throw new Exception();
				// }
				// while (mpos != 0);
				// Console.WriteLine();


				if (m % 23 == 0)
				{
					var player = m % N;
					// // players[player] += m;
					// // var pos = (currentMarble - 7 + marbles.Count()) % marbles.Count();
					// // var marbleval = marbles[pos];
					// // players[player] += marbleval;
					// // //Console.WriteLine($"Add {m}  {marbles[pos]}");
					// // marbles.RemoveAt(pos);
					// // currentMarble = pos;

					players2[player] += m;
					for (var i = 0; i < 7; i++)
					{
						marble2pos = prev[marble2pos];
					}
					// // if (marble2pos != marbleval)
					// // 	throw new Exception();

					players2[player] += marble2pos;

					var pr = prev[marble2pos];
					var ne = next[marble2pos];
					next[pr] = ne;
					prev[ne] = pr;
					marble2pos = ne;

				}
				else
				{
					// // var newpos = ((currentMarble + 1) % marbles.Count()) + 1;
					// // marbles.Insert(newpos, m);
					// // currentMarble = newpos;

					var p1 = next[marble2pos];
					var p2 = next[p1];

					marble2pos = m;

					next[marble2pos] = p2;
					next[p1] = marble2pos;

					prev[marble2pos] = p1;
					prev[p2] = marble2pos;
				}
			}

			// // var maxpoints = players.Max();
			var maxpoints2 = players2.Max();
			// // if (maxpoints != maxpoints2)
			// // 	throw new Exception();

			return maxpoints2;			
		}
	}
}
