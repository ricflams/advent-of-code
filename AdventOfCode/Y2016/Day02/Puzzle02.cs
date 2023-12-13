using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;
using System.Linq;

namespace AdventOfCode.Y2016.Day02
{
	internal class Puzzle : Puzzle<int, string>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Bathroom Security";
		public override int Year => 2016;
		public override int Day => 2;

		public override void Run()
		{
			Run("test1").Part1(1985).Part2("5DB3");
			Run("input").Part1(65556).Part2("CB779");
			Run("extra").Part1(61529).Part2("C2C28");
		}

		protected override int Part1(string[] input)
		{
			var pad = new []
			{
				"123",
				"456",
				"789",
			}.ToCharMatrix().ExpandBy(1, ' ');
			var code = int.Parse(PressKeypad(pad, input));
			return code;
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
			}.ToCharMatrix().ExpandBy(1, ' ');
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
