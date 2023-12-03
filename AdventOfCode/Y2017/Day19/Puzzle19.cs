using AdventOfCode.Helpers;
using AdventOfCode.Helpers.Puzzles;
using System;

namespace AdventOfCode.Y2017.Day19
{
	internal class Puzzle : Puzzle<string, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "A Series of Tubes";
		public override int Year => 2017;
		public override int Day => 19;

		public override void Run()
		{
			Run("test1").Part1("ABCDEF").Part2(38);
			Run("input").Part1("AYRPVMEGQ").Part2(16408);
		}

		protected override string Part1(string[] input)
		{
			var (letters, _) = FindLettersAndSteps(input);
			return letters;
		}

		protected override int Part2(string[] input)
		{
			var (_, steps) = FindLettersAndSteps(input);
			return steps;
		}

		private (string, int) FindLettersAndSteps(string[] input)
		{
			var map = input.ToCharMatrix();
			var x0 = input[0].IndexOf('|');

			// Move along the path, pick up letters and count the steps
			var pose = Pose.From(x0, 0, Direction.Down);
			var letters = "";
			var steps = 0;
			while (true)
			{
				pose.Move(1);
				steps++;
				var ch = map.CharAt(pose.Point);
				if (ch == ' ') // Stepping into the void means we're done
					break;
				else if (char.IsLetter(ch)) // Found a letter
					letters += ch;
				else if (ch == '|' || ch == '-') // Nothing to do, just continue down the path
					{}
				else if (map.CharAt(pose.PeekLeft) != ' ') // At a turning point with something to the left
					pose.TurnLeft();
				else if (map.CharAt(pose.PeekRight) != ' ') // At a turning point with something to the right
					pose.TurnRight();
				else
					throw new Exception($"Unexpected situation at {pose}");
			}

			return (letters, steps);			
		}
	}
}
