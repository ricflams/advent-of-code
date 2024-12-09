using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2020.Day17
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Conway Cubes";
		public override int Year => 2020;
		public override int Day => 17;

		public override void Run()
		{
			Run("test1").Part1(112).Part2(848);
			Run("input").Part1(240).Part2(1180);
			Run("extra").Part1(368).Part2(2696);
		}

		protected override int Part1(string[] input)
		{
			return CountActivePoints(input, 3, 6);
		}

		protected override int Part2(string[] input)
		{
			return CountActivePoints(input, 4, 6);
		}


		private int CountActivePoints(string[] input, int dimensions, int cycles)
		{
			var points = CharMap
				.FromArray(input, '.')
				.AllPointsWhere(ch => ch == '#');

			var space = new Space(dimensions);
			foreach (var p in points)
			{
				space.Set(new[] { (sbyte)p.X, (sbyte)p.Y, (sbyte)0, (sbyte)0 });
			}

			for (var cycle = 0; cycle < cycles; cycle++)
			{
				// We need only investigate the active points and their neighbours
				var pointsToInvestigate = new Space(dimensions);
				foreach (var p in space.Active)
				{
					var neighbours = space.NeighboursOf(p);
					pointsToInvestigate.MergeWith(neighbours);
				}

				// Build the next cycle's space
				var nextSpace = new Space(dimensions);
				foreach (var p in pointsToInvestigate.Active)
				{
					var neighbours = space.NeighboursOf(p);
					var activeNeighbours = neighbours.Active.Count(x => space.IsSet(x));

					var isCellActive = space.IsSet(p);
					if (isCellActive)
					{
						activeNeighbours--; // don't count the active cell itself
					}
					if (isCellActive && (activeNeighbours == 2 || activeNeighbours == 3) || !isCellActive && activeNeighbours == 3)
					{
						nextSpace.Set(p);
					}
				}

				// Update space after the cycle
				space = nextSpace;
			}

			return space.Active.Count();
		}
	}
}
