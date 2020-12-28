using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2015.Day06
{
	internal class Puzzle : SoloParts<int>
	{
		public static Puzzle Instance = new Puzzle();
		protected override string Name => "Probably a Fire Hazard";
		protected override int Year => 2015;
		protected override int Day => 6;

		public void Run()
		{
			RunFor("input", 543903, 14687245);
		}

		protected override int Part1(string[] input)
		{
			var commands = input;
			var N = 1000;
			var lights = new bool[N, N];

			SetLights(lights, commands, _ => true, _ => false, x => !x);

			var lightsOn = 0;
			for (var x = 0; x < N; x++)
			{
				for (var y = 0; y < N; y++)
				{
					if (lights[x, y])
					{
						lightsOn++;
					}
				}
			}

			return lightsOn;
		}

		protected override int Part2(string[] input)
		{
			var commands = input;
			var N = 1000;
			var lights = new int[N, N];

			// The phrase turn on actually means that you should increase the brightness of those lights by 1.
			// The phrase turn off actually means that you should decrease the brightness of those lights by 1, to a minimum of zero.
			// The phrase toggle actually means that you should increase the brightness of those lights by 2.
			SetLights(lights, commands, x => x + 1, x => Math.Max(0, x - 1), x => x + 2);

			var brightness = 0;
			for (var x = 0; x < N; x++)
			{
				for (var y = 0; y < N; y++)
				{
					brightness += lights[x, y];
				}
			}

			return brightness;
		}

		private static void SetLights<T>(T[,] lights, IEnumerable<string> commands, Func<T, T> turnOn, Func<T, T> turnOff, Func<T, T> toggle)
		{
			foreach (var command in commands)
			{
				// Examples:
				// toggle 461,550 through 564,900
				// turn off 370,39 through 425,839
				// turn on 599,989 through 806,993
				var val = SimpleRegex.Match(command, @"%* %d,%d through %d,%d");
				var cmd = val[0];
				var num = val.Skip(1).Select(c => int.Parse(c)).ToArray();
				var (xmin, ymin, xmax, ymax) = (num[0], num[1], num[2], num[3]);

				switch (cmd)
				{
					case "turn on": SetAllLigths(turnOn); break;
					case "turn off": SetAllLigths(turnOff); break;
					case "toggle": SetAllLigths(toggle); break;
				}

				void SetAllLigths(Func<T, T> func)
				{
					for (var x = xmin; x <= xmax; x++)
					{
						for (var y = ymin; y <= ymax; y++)
						{
							lights[x, y] = func(lights[x, y]);
						}
					}
				}
			}
		}
	}
}
