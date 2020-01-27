using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2015.Day06
{
	internal class Puzzle06
	{
		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var commands = File.ReadAllLines("Y2015/Day06/input.txt");
			var N = 1000;
			var lights = new bool[N, N];

			SetLights<bool>(lights, commands, _ => true, _ => false, x => !x);

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

			Console.WriteLine($"Day  6 Puzzle 1: {lightsOn}");
			Debug.Assert(lightsOn == 543903);
		}

		private static void Puzzle2()
		{
			var commands = File.ReadAllLines("Y2015/Day06/input.txt");
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

			Console.WriteLine($"Day  6 Puzzle 2: {brightness}");
			Debug.Assert(brightness == 14687245);
		}

		private static void SetLights<T>(T[,] lights, IEnumerable<string> commands, Func<T, T> turnOn, Func<T, T> turnOff, Func<T, T> toggle)
		{
			foreach (var command in commands)
			{
				// Examples:
				// toggle 461,550 through 564,900
				// turn off 370,39 through 425,839
				// turn on 599,989 through 806,993
				var match = Regex.Match(command, @"(.*)\s(\d+),(\d+)\sthrough\s(\d+),(\d+)");
				if (!match.Success)
					throw new Exception($"Unexpected input in line {command}");

				var cmd = match.Groups.Skip(1).First().Value;
				var num = match.Groups.Skip(2).Select(c => int.Parse(c.Value)).Take(4).ToArray();
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
