using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2025.Day01
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Secret Entrance";
		public override int Year => 2025;
		public override int Day => 1;

		public override void Run()
		{
			Run("test1").Part1(3).Part2(6);
			Run("input").Part1(980).Part2(5961);
		}

		protected override long Part1(string[] input)
		{
			var dial = 50;
			var password = 0;

			foreach (var p in input)
			{
				var dir = p[0] == 'R' ? 1 : -1;
				var moves = int.Parse(p[1..]);

				dial += moves * dir;
				dial %= 100;

				if (dial == 0)
					password++;
			}

			return password;
		}

		protected override long Part2(string[] input)
		{
			var dial = 50;
			var password = 0;

			foreach (var p in input)
			{
				var dir = p[0] == 'R' ? 1 : -1;
				var moves = int.Parse(p[1..]);
				while (moves-- > 0)
				{
					dial += dir;
					dial %= 100;
					if (dial == 0)
						password++;
				}
			}

			return password;
		}
	}
}
