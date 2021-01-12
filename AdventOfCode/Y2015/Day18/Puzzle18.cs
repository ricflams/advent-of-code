using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2015.Day18
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Like a GIF For Your Yard";
		public override int Year => 2015;
		public override int Day => 18;

		public void Run()
		{
			RunFor("input", 768, 781);
		}

		protected override int Part1(string[] input)
		{
			var lights = GetLights(input);
			var n = CountLightsOnAfter(lights, 100, false);
			return n;
		}

		protected override int Part2(string[] input)
		{
			var lights = GetLights(input);
			var n = CountLightsOnAfter(lights, 100, true);
			return n;
		}

		private static SparseMap<bool> GetLights(string[] input)
		{
			var map = CharMap.FromArray(input);
			var lights = new SparseMap<bool>();
			foreach (var p in map.AllPoints())
			{
				lights[p] = map[p] == '#';
			}
			return lights;
		}

		private static int CountLightsOnAfter(SparseMap<bool> lights, int rounds, bool cornersAreStuck)
		{
			var corners = lights.Span().ToList();

			// A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
			// A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.
			for (var i = 0; i < rounds; i++)
			{
				var nextlights = new SparseMap<bool>();
				foreach (var (p, on) in lights.AllValues())
				{
					if (cornersAreStuck && corners.Any(corner => p == corner))
					{
						nextlights[p] = true;
					}
					else
					{
						var n = p.LookDiagonallyAround().Count(x => lights[x]);
						nextlights[p] = on && (n == 2 || n == 3) || !on && (n == 3);
					}
				}
				lights = nextlights;
			}
			var lightsOn = lights.AllPoints(c => c).Count();
			return lightsOn;
		}
	}
}
