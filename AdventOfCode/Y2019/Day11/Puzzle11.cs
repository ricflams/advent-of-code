using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using AdventOfCode.Y2019.Intcode;
using System.Linq;

namespace AdventOfCode.Y2019.Day11
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Space Police";
		public override int Year => 2019;
		public override int Day => 11;

		public void Run()
		{
			Run("input").Part1(2343).Part2("JFBERBUH");
		}

		const int ColorBlack = 0;
		const int ColorWhite = 1;

		protected override int Part1(string[] input)
		{
			var map = PaintHull(input[0], ColorBlack);
			var numberOfPoints = map.AllPoints().Count();
			return numberOfPoints;
		}

		protected override string Part2(string[] input)
		{
			var map = PaintHull(input[0], ColorWhite);
			var letters = map.Render((_, val) => val == 'W' ? '#' : ' ');
			var identifier = LetterScanner.Scan(letters);
			return identifier;
		}


		private CharMap PaintHull(string intcode, int color)
		{
			var map = new CharMap('B');
			var pos = Point.From(0, 0);
			var dir = 0;
			var step = 0;
			new Engine()
				.WithMemory(intcode)
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
