using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Helpers.String;

namespace AdventOfCode.Y2022.Day10.Raw
{
	internal class Puzzle : Puzzle<long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Cathode-Ray Tube";
		public override int Year => 2022;
		public override int Day => 10;

		public void Run()
		{
			Run("test1").Part1(13140);
			//Run("test2").Part1(0).Part2(0);
			Run("input").Part1(0).Part2("");
		}

		protected override long Part1(string[] input)
		{
			var sum = 0;

			var cycle = 1;
			var x = 1;
			foreach (var s in input)
			{
				if (s.StartsWith("addx"))
				{
					Tick();
					Tick();;
					var num = s[4..];
					var v = int.Parse(num);
				//	Console.WriteLine(num);
					x += v;
				}
				else
					Tick();
			}

			return sum;

			void Tick()
			{
				var c = cycle++;
				if (c == 20 || (c - 20)%40 == 0)
				{
					sum += x*c;
				}
			}
		}

		protected override string Part2(string[] input)
		{
			var sum = 0;

			var crt = new char[40,6];

			var x = 0;
			var y = 0;


			var cycle = 1;
			var regx = 1;
			foreach (var s in input)
			{
				if (s.StartsWith("addx"))
				{
					Tick();
					Tick();;
					var num = s[4..];
					var v = int.Parse(num);
				//	Console.WriteLine(num);
					regx += v;
				}
				else
					Tick();
			}

			var msg = LetterScanner.Scan(crt);
			Console.WriteLine(msg);

			crt.ConsoleWrite();

			return msg;

			void Tick()
			{
				var c = cycle++;

				var islit = (x == regx || x == regx-1 || x == regx+1);
				crt[x, y] = islit ? '#' : '.';

				if (++x == 40)
				{
					x = 0;
					y++;
				}
				if (y > 5)
					return;


				if (c == 20 || (c - 20)%40 == 0)
				{
					sum += regx*c;
				}
			}

		}
	}
}
