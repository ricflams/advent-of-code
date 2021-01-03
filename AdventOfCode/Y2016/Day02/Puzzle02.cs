using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2016.Day02
{
	internal class Puzzle : SoloParts<string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Bathroom Security";
		public override int Year => 2016;
		public override int Day => 2;

		public void Run()
		{
			RunFor("test1", "1985", "5DB3");
			RunFor("input", "65556", "CB779");
		}

		protected override string Part1(string[] input)
		{
			var pad = new []
			{
				"123",
				"456",
				"789",
			}.ToMultiDim().ExpandBy(1, ' ');
			return PressKeypad(pad, input);
		}

		protected override string Part2(string[] input)
		{
			var pad = new []
			{
				"  1  ",
				" 234 ",
				"56789",
				" ABC ",
				"  D  "
			}.ToMultiDim().ExpandBy(1, ' ');
			return PressKeypad(pad, input);
		}		

		private static string PressKeypad(char [,] pad, string[] moves)
		{
			var pos = pad.PositionsOf('5').First();
			var pincode = moves.
				Select(line =>
				{
					foreach (var ch in line)
					{
						var newpos = ch switch
						{
							'U' => pos.Up,
							'D' => pos.Down,
							'R' => pos.Right,
							'L' => pos.Left,
							_ => throw new Exception($"Unknown move {ch}")
						};
						if (pad.CharAt(newpos) != ' ')
						{
							pos = newpos;
						};
					}
					return pad.CharAt(pos);
				})
				.ToArray();
			return new string(pincode);			
		}
	}
}
