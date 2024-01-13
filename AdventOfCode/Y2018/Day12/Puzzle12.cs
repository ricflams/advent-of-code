using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2018.Day12
{
	internal class Puzzle : Puzzle<int, long>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Subterranean Sustainability";
		public override int Year => 2018;
		public override int Day => 12;

		public override void Run()
		{
			Run("test1").Part1(325);
			Run("input").Part1(3738).Part2(3900000002467);
			Run("extra").Part1(3230).Part2(4400000000304);
		}

		protected override int Part1(string[] input)
		{
			var tunnel = new Tunnel(input);
			while (tunnel.Iterations < 20)
			{
				tunnel.Grow();
			}
			return tunnel.SumOfPots;
		}

		protected override long Part2(string[] input)
		{
			var tunnel = new Tunnel(input);

			// Grow until we've reached a "crawler", ie a pattern that shifts one
			// position to the right for every step.
			var last = "";
			while (tunnel.Pots.TrimEnd('.') != "." + last.TrimEnd('.'))
			{
				last = tunnel.Pots;
				tunnel.Grow();
			}

			// After this many iterations the sum was x.
			// For every next iteration the sum will grow by the number of pots,
			// as all pots are shifted to the right, ie every pot's value will
			// be incremented by one.
			var sumNow = tunnel.SumOfPots;
			var sumRest = (50_000_000_000 - tunnel.Iterations) * tunnel.NumberOfPots;
			var sumFinal = sumNow + sumRest; 
			return sumFinal;
		}

		internal class Tunnel
		{
			private readonly Dictionary<string, char> _rules;

			public Tunnel(string[] input)
			{
				Pots = input[0].RxMatch("initial state: %*").Get<string>();
				_rules = input[2..]
					.Select(line => line.Split(" => ").ToArray())
					.ToDictionary(x => x[0], x => x[1][0]);				
			}

			public string Pots { get; private set; }
			public int Padding { get; private set; }
			public int Iterations { get; private set; }
			public int SumOfPots => Pots.Select((c, i) => c == '#' ? i - Padding : 0).Sum();
			public int NumberOfPots => Pots.Count(c => c == '#');

			public void Grow()
			{
				// Ensure that there are always 4 "empty pots" at the beginning
				// and at the end of the pots.
				if (Pots[0..4] != "....")
				{
					Pots = "...." + Pots;
					Padding += 4;
				}
				if (Pots[^4..] != "....")
				{
					Pots = Pots + "....";
				}

				// Grow the next pots, by checking for matches to any rule-patterns.
				var next = Pots.ToCharArray();
				for (var i = 0; i < Pots.Length - 4; i++)
				{
					var pattern = Pots[i..(i+5)];
					next[i+2] = _rules.TryGetValue(pattern, out var ch) ? ch : '.';
				}
				Pots = new string(next);
				Iterations++;
			}
		}
	}
}
