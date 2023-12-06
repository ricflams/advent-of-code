using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2022.Day10
{
	internal class Puzzle : Puzzle<long, string>
	{
		public static Puzzle Instance = new();
		public override string Name => "Cathode-Ray Tube";
		public override int Year => 2022;
		public override int Day => 10;

		public override void Run()
		{
			Run("test1").Part1(13140);
			Run("input").Part1(13220).Part2("RUAKHBEK");
			Run("extra").Part1(14320).Part2("PCPBKAPJ");
		}

		protected override long Part1(string[] input)
		{
			var cycle = 1;
			var regx = 1;

			var sum = 0;
			void Tick()
			{
				var c = cycle++;
				if (c == 20 || (c - 20) % 40 == 0)
				{
					sum += regx*c;
				}
			}

			foreach (var s in input)
			{
				Tick();
				if (s.StartsWith("addx"))
				{
					Tick();
					regx += int.Parse(s[4..]);
				}
			}

			return sum;
		}

		protected override string Part2(string[] input)
		{
			var regx = 1;

			var crt = new char[40,6];
			var x = 0;
			var y = 0;

			void Tick()
			{
				var islit = (x == regx || x == regx-1 || x == regx+1);
				crt[x, y] = islit ? '#' : '.';
				if (++x == 40)
				{
					x = 0;
					y++;
				}
			}

			foreach (var s in input)
			{
				Tick();
				if (s.StartsWith("addx"))
				{
					Tick();
					regx += int.Parse(s[4..]);
				}
			}

			var msg = LetterScanner.Scan(crt);
			return msg;
		}
	}
}
