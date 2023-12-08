using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;

namespace AdventOfCode.Y2023.Day08
{
	internal class Puzzle : Puzzle<long, long>
	{
		public static Puzzle Instance = new();
		public override string Name => "Haunted Wasteland";
		public override int Year => 2023;
		public override int Day => 8;

		public override void Run()
		{
			Run("test1").Part1(2);
			Run("test2").Part2(6);
			Run("input").Part1(19241).Part2(9606140307013);
			Run("extra").Part1(18113).Part2(12315788159977);
		}

		protected override long Part1(string[] input)
		{
			var (directions, network) = ReadNetwork(input);

			var step = 0;
			var node = "AAA";
			while (node != "ZZZ")
			{
				var dir = directions[step++ % directions.Length];
				node = dir == 'L' ? network[node].Left : network[node].Right;
			}

			return step;
		}

		protected override long Part2(string[] input)
		{
			var (directions, network) = ReadNetwork(input);
 
			// The initial step from any A to a Z turns out to be the same number
			// as the following cycle from that Z to the next Z. That must be by design
			// in the puzzle. So we can find the cycles by just finding the number of
			// steps from each A to their Z.
			var cycles = network.Keys
				.Where(x => x.Last() == 'A')
				.Select(n =>
				{
					var step = 0;
					while (n.Last() != 'Z')
					{
						var dir = directions[step++ % directions.Length];
						n = dir == 'L' ? network[n].Left : network[n].Right;
					}
					return (long)step;
				})
				.ToArray();

			var steps = MathHelper.LeastCommonMultiple(cycles);

			return steps;
		}

		private static (string, Dictionary<string, (string Left, string Right)>) ReadNetwork(string[] input) =>
			(input[0], input
				.Skip(2)
				.Select(s =>
				{
					var (name, left, right) = s.RxMatch("%s = (%s, %s)").Get<string, string, string>();
					return (name, left, right);
				})
				.ToDictionary(x => x.name, x => (x.left, x.right)));
	}
}
