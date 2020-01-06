using AdventOfCode2019.Helpers;
using System;
using System.Linq;
using System.Diagnostics;

namespace AdventOfCode2019.Day11
{
    internal class Puzzle11
    {
		const int ColorBlack = 0;
		const int ColorWhite = 1;

		public static void Run()
		{
			Puzzle1();
			Puzzle2();
		}

		private static void Puzzle1()
		{
			var map = PaintHull(ColorBlack);
			var numberOfPoints = map.AllPoints().Count();
			Console.WriteLine($"Day 11 Puzzle 1: {numberOfPoints}");
			Debug.Assert(numberOfPoints == 2343);
		}

		private static void Puzzle2()
		{
			var map = PaintHull(ColorWhite);
			foreach (var line in map.Render((_,val) => val == 'W' ? Graphics.FullBlock : ' '))
			{
				Console.WriteLine($"Day 11 Puzzle 2: {line}");
			}
		}

		private static CharMap PaintHull(int color)
		{
			var map = new CharMap('B');
			var pos = Point.From(0, 0);
			var dir = 0;
			var step = 0;
			new Intcode.Engine()
				.WithMemoryFromFile("Day11/input.txt")
				.WithInput(color)
				.OnOutput(engine =>
				{
					if (step++ % 2 == 0)
					{
						color = (int)engine.Output.Take();
					}
					else
					{
						// Paint and turn left or right; 0 means up, 1 right, etc
						map[pos] = color == ColorBlack ? 'B' : 'W';
						dir = (dir + (engine.Output.Take() == 0 ? -1 : 1) + 4) % 4;
						switch (dir)
						{
							case 0: pos = pos.Up; break;
							case 1: pos = pos.Right; break;
							case 2: pos = pos.Down; break;
							case 3: pos = pos.Left; break;
						}
						var panelColor = map[pos] == 'W' ? ColorWhite : ColorBlack;
						engine.Input.Add(panelColor);
					}
				})
				.Execute();
			return map;
		}
	}
}
