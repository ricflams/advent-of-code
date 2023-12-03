using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2021.Day02
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new();
		public override string Name => "Dive!";
		public override int Year => 2021;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(150).Part2(900);
			Run("input").Part1(1648020).Part2(1759818555);
		}

		protected override int Part1(string[] input)
		{
			var p = Point.Origin;

			foreach (var line in input)
			{
				var (dir, len) = line.RxMatch("%s %d").Get<string, int>();
				switch (dir)
				{
					case "forward": p = p.MoveRight(len); break;
					case "backward": p = p.MoveLeft(len); break;
					case "up": p = p.MoveUp(len); break;
					case "down": p = p.MoveDown(len); break;
				}
			}

			return p.X * p.Y;
		}

		protected override int Part2(string[] input)
		{
			var p = Point.Origin;
			var aim = 0;

			foreach (var line in input)
			{
				var (dir, len) = line.RxMatch("%s %d").Get<string, int>();
				switch (dir)
				{
					case "forward": p = p.MoveRight(len); p = p.MoveDown(aim * len); break;
					case "backward": p = p.MoveLeft(len); p = p.MoveUp(aim * len);  break;
					case "up": aim -= len;  break;
					case "down": aim += len;  break;
				}
			}

			return p.X * p.Y;
		}
	}
}
