using AdventOfCode.Helpers.Puzzles;
using System.Linq;

namespace AdventOfCode.Y2018.Day05
{
	internal class Puzzle : Puzzle<int, int>
	{
		public static Puzzle Instance = new Puzzle();
		public override string Name => "Alchemical Reduction";
		public override int Year => 2018;
		public override int Day => 5;

		public void Run()
		{
			Run("test1").Part1(10).Part2(4);
			Run("input").Part1(11298).Part2(5148);
		}

		protected override int Part1(string[] input)
		{
			var polymer = input[0].ToCharArray();
			return LengthAfterReduction(polymer);
		}

		protected override int Part2(string[] input)
		{
			var polymer = input[0];

			// The polymer should have all its pairs removed, one pair at a
			// time. For fast construction of a pair-less polymer we first count
			// how many times each unit appear so when it comes time to build the
			// modified polymer we know how much shorter it should be.
			// Simply look the polymer and count the occurrences of each character.
			// In ASCII the highest character is 'z' so make room for 'z'+1 counts.
			var counts = new int[128]; // int['z'+1] would do, but just say 128
			foreach (var ch in polymer)
			{
				counts[ch]++;
			}

			// Find the shortest reduced polymer by removing pairs a-z, one by one
			var minlength = Enumerable.Range(0, 26) // 26 chars a-z
				.Select(pairNo =>
				{
					// Allocate space for the new modified polymer, which will be as
					// long as the original minus the number of pair-letters. Then copy
					// the original polymer into it, minus any chars matching the pair.
					// Finally, return the length of that reduced polymer.
					var upper = 'A' + pairNo;
					var lower = 'a' + pairNo;
					var removals = counts[upper] + counts[lower];
					var modifiedPolymer = new char[polymer.Length - removals];
					var i = 0;
					foreach (var ch in polymer)
					{
						if (ch != lower && ch != upper)
						{
							modifiedPolymer[i++] = ch;
						}
					}
					return LengthAfterReduction(modifiedPolymer);
				})
				.Min();

			return minlength;
		}


		private static int LengthAfterReduction(char[] polymer)
		{
			// Convenient shorthand
			var N = polymer.Length;

			// For fast calculation do two things:
			// 1. Turn string into a form which is really fast to do comparisons on and
			//    that is padded at the ends so we don't need any boundary-checks or
			//    special cases for the going beyond the first or last character.
			// 2. Don't modify the string at all; simply maintain a list of prev-pointers
			//    to make it possible to go/skip backwards when a match is found, to keep
			//    unrolling that match.

			// Create an array of values to inspect that replaces a-z with positive values
			// and A-Z with the corresponding negative value; finding aA is then a matter
			// of checking whether the two values are the same but differ in sign.
			// The end will simply be value 0, which isn't part of the input and therefore
			// always will compare false and stop the loops etc without need for an explicit
			// boundary-check, like "if (i>0 & ...)"
			var s = new int[N + 2];
			for (var i = 0; i < N; i++)
			{
				var ch = polymer[i];
				s[i+1] = char.IsLower(ch) ? ch+1 - 'a' : 'A' - (ch+1);
			}

			// We only ever need to skip characters when going back, never when going forward.
			// Therefore only a backwards list is needed.
			// prev[0] will be -1 but that doens't matter since it will never be tested against.
			var prev = new int[N + 2];
			for (var i = 0; i < N + 2; i++)
			{
				prev[i] = i-1;
			}

			// Look the transformed string and count the reductions
			var reductions = 0;
			for (var i = 1; i <= N; i++)
			{
				// Compare the i'th char with the previous char. If they form
				// a pair then move i one step forward and update that next i's
				// prev twice so the pair will be "skipped"
				while (s[i] == -s[prev[i]])
				{
					i++;
					prev[i] = prev[prev[prev[i]]];
					reductions++;
				}
			}

			// Final length is twice the reductions less than the original
			return N - 2*reductions;			
		}
	}
}
